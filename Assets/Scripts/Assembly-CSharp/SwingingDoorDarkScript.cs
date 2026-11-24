// SwingingDoorScript
using UnityEngine;
using System.Collections;

public class SwingingDoorDarkScript : MonoBehaviour
{
    public GameControllerScript gc;

    public BaldiScript baldi;

    public MeshCollider barrier;

    public GameObject obstacle;

    public MeshCollider trigger;

    public MeshRenderer inside;

    public MeshRenderer outside;

    public Material closedDark;

    public Material openDark;

    public Material open;

    public Material closed;

    public Material lockedDark;

    public Material locked;

    public Material buyDark;

    public Material buy;

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
            inside.sharedMaterial = closedDark;
            outside.sharedMaterial = closed;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!bDoorLocked)
        {
            bDoorOpen = true;
            inside.sharedMaterial = openDark;
            outside.sharedMaterial = open;
            openTime = 2f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!bDoorLocked && !bDoorOpen) // V3.0.5 Update: Swinging Doors no longer trigger a noise when it's already open
        {
            myAudio.PlayOneShot(doorOpen, 1f);
            if (other.tag == "Player" && baldi.isActiveAndEnabled & Vector3.Distance(baldi.transform.position, other.transform.position) <= 122f)
            {
                baldi.Hear(base.transform.position, 1f);
                return;
            }
            if (baldi.gameController.DoctorReflex != null && baldi.gameController.DoctorReflex.Angry && !baldi.gameController.DoctorReflex.WantsTheHammer)
            {
                baldi.gameController.DoctorReflex.Hear(transform.position); // Hear like Baldi
            }
        }
    }

    public void LockDoor(float time, bool SendEvent = true)
    {
        barrier.enabled = true;
        obstacle.SetActive(true);
        bDoorLocked = true;
        lockTime = time;
        inside.sharedMaterial = lockedDark;
        outside.sharedMaterial = locked;
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi && SendEvent)
        {
            Multiplayer.ClientSend.SendActionToOtherPlayer("lock_dark:" + Multiplayer.ClientHandle.GetIdFromPosition<SwingingDoorDarkScript>(transform.position));
        }
    }

    private void UnlockDoor()
    {
        barrier.enabled = false;
        obstacle.SetActive(false);
        bDoorLocked = false;
        inside.sharedMaterial = closedDark;
        outside.sharedMaterial = closed;
    }

    public void BuyDoor()
    {
        if (gc.notebooks < 2)
        {
            myAudio.PlayOneShot(baldiDoor, 1f);
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
        inside.sharedMaterial = buyDark;
        outside.sharedMaterial = buy;
        lockTime = float.PositiveInfinity;
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