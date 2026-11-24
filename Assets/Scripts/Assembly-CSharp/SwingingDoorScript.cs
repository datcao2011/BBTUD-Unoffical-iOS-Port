// SwingingDoorScript
using UnityEngine;
using System.Collections;

public class SwingingDoorScript : MonoBehaviour
{
    public GameControllerScript gc;

    public BaldiScript baldi;

    public MeshCollider barrier;

    public GameObject obstacle;

    public MeshCollider trigger;

    public MeshRenderer inside;

    public MeshRenderer outside;

    public Material closed;

    public Material open;

    public Material buy;

    public Material locked;

    public AudioClip doorOpen;

    public AudioClip baldiDoor;

    private float openTime;

    private float lockTime;

    private bool bDoorOpen;

    private bool bDoorLocked;

    private bool requirementMet;

    public string mode;

    public bool coinDoor;

    private AudioSource myAudio;

    public bool SkipRequirement;

    private void Start()
    {
        myAudio = base.GetComponent<AudioSource>();
        bDoorLocked = true;
        if (coinDoor)
        {
            DoorOnSale();
        }
    }

    private void Update()
    {
        if (!requirementMet & ((gc.notebooks >= 2 & !coinDoor) || SkipRequirement))
        {
            requirementMet = true;
            UnlockDoor();
        }
        if (gc.notebooks < 2 & requirementMet & !coinDoor & !SkipRequirement)
        {
            requirementMet = false;
            barrier.enabled = true;
            obstacle.SetActive(true);
            inside.sharedMaterial = closed;
            outside.sharedMaterial = closed;
            bDoorLocked = true;
        }
        if (openTime > 0f)
        {
            openTime -= 1f * Time.deltaTime;
        }
        if (lockTime > 0f)
        {
            lockTime -= Time.deltaTime;
        }
        else if (bDoorLocked & requirementMet)
        {
            UnlockDoor();
        }
        if (openTime <= 0f & bDoorOpen & !bDoorLocked)
        {
            bDoorOpen = false;
            inside.sharedMaterial = closed;
            outside.sharedMaterial = closed;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!bDoorLocked)
        {
            bDoorOpen = true;
            inside.sharedMaterial = open;
            outside.sharedMaterial = open;
            openTime = 2f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!bDoorLocked && !bDoorOpen) // V3.0.5 Update: Swinging Doors no longer trigger a noise when it's already open
        {
            myAudio.PlayOneShotWC(doorOpen, 1f);
            if (other.tag == "Player" && baldi.isActiveAndEnabled & Vector3.Distance(baldi.transform.position, other.transform.position) <= 200f)
            {
                baldi.Hear(base.transform.position, 1f);
                return;
            }
            if (other.tag == "Player" && NullManager.Instance != null)
            {
                NullManager.Instance.NULL.Hear(transform.position, 1f);
                return;
            }
            if (other.tag == "Player" && baldi.gameController.DoctorReflex != null && baldi.gameController.DoctorReflex.Angry && !baldi.gameController.DoctorReflex.WantsTheHammer)
            {
                baldi.gameController.DoctorReflex.Hear(transform.position); // Hear like Baldi
            }
            if (gc.HASManager && other.name == "Baldi" & Vector3.Distance(gc.playerTransform.position, other.transform.position) <= 200f)
            {
                if (gc.HASManager.seekTimer > 0f && gc.HASManager.hiderOrSeeker == "Seeker")
                {
                    FindObjectOfType<DoorIncidator>().Show(base.transform.position);
                    return;
                }
            }
            if (other.tag == "Player" && FindObjectOfType<Guy>())
            {
                FindObjectOfType<Guy>().SetTo(transform.position);
                gc.audioDevice.Stop();
                gc.audioDevice.PlayOneShotWC(FindObjectOfType<Guy>().audioDevice.clip);
                return;
            }
        }
    }

    public void LockDoor(float time, bool SendEvent = true)
    {
        if (!coinDoor)
        {
            barrier.enabled = true;
            obstacle.SetActive(true);
            bDoorLocked = true;
            lockTime = time;
            inside.sharedMaterial = locked;
            outside.sharedMaterial = locked;
            if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi && SendEvent)
            {
                Multiplayer.ClientSend.SendActionToOtherPlayer("lock:" + Multiplayer.ClientHandle.GetIdFromPosition<SwingingDoorScript>(transform.position));
            }
        }
    }

    public void UnlockDoor()
    {
        barrier.enabled = false;
        obstacle.SetActive(false);
        bDoorLocked = false;
        inside.sharedMaterial = closed;
        outside.sharedMaterial = closed;
    }

    public void BuyDoor()
    {
        if (gc.notebooks < 2)
        {
            myAudio.PlayOneShotWC(baldiDoor, 1f);
            StartCoroutine(ReturnItem(5));
            return;
        }
        if (!coinDoor)
        {
            StartCoroutine(ReturnItem(5));
            return;
        }
        coinDoor = false;
        UnlockDoor();
    }

    public void DoorOnSale()
    {
        barrier.enabled = true;
        obstacle.SetActive(true);
        coinDoor = true;
        bDoorLocked = true;
        inside.sharedMaterial = buy;
        outside.sharedMaterial = buy;
        lockTime = float.PositiveInfinity;
        transform.parent.SetParent(null, true); // Fixes the bug with the Swinging Door not working properly (when optimization is enabled)
    }

    private IEnumerator ReturnItem(int num)
    {
        float waitTime = 0.2f;
        while (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        gc.GetItem(gc.itemSelected, num);
        yield break;
    }
}