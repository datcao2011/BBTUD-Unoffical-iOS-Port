// DoorScript
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public float openingDistance;

    public Transform player;

    public BaldiScript baldi;

    public MeshCollider barrier;

    public MeshCollider trigger;

    public MeshCollider invisibleBarrier;

    public MeshRenderer inside;

    public MeshRenderer outside;

    public AudioClip doorOpen;

    public AudioClip doorClose;

    public AudioClip doorLock;
    public AudioClip doorUnlock;
    public AudioClip doorLocked;

    public Material closed;

    public Material open;

    public Material locked;

    public GameObject clickCursor;

    public GameObject click;

    [HideInInspector]
    public bool bDoorOpen;

    [HideInInspector]
    public bool bDoorLocked;

    public int silentOpens;

    private float openTime;

    public float lockTime;

    private AudioSource myAudio;

    public FloodScript floodScript;

    private bool OpenedbyFlood;

    private bool IsFirstFloor = false;

    private void Start()
    {
        myAudio = base.GetComponent<AudioSource>();
        IsFirstFloor = transform.position.y <= 20f;
    }

    private void Update()
    {
        if (floodScript.isActiveAndEnabled && IsFirstFloor)
        {
            OpenedbyFlood = true;
            OpenDoor();
        }
        if (lockTime > 0f)
        {
            lockTime -= 1f * Time.deltaTime;
        }
        else if (bDoorLocked)
        {
            UnlockDoor();
        }
        if (bDoorLocked && !bDoorOpen && locked != null)
        {
            inside.sharedMaterial = locked;
            outside.sharedMaterial = locked;
        }
        if (openTime > 0f)
        {
            openTime -= 1f * Time.deltaTime;
        }
        if (openTime <= 0f & bDoorOpen)
        {
            barrier.enabled = true;
            invisibleBarrier.enabled = true;
            bDoorOpen = false;
            inside.sharedMaterial = bDoorLocked ? locked : closed;
            outside.sharedMaterial = bDoorLocked ? locked : closed;
            if (silentOpens <= 0)
            {
                if (OpenedbyFlood)
                    myAudio.PlayOneShot(doorClose, 1f);
                else
                    myAudio.PlayOneShotWC(doorClose, 1f);
            }
            OpenedbyFlood = false;
        }
        bool isHovering = false;
        if (Vector3.Distance(player.position, base.transform.position) < openingDistance)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit raycastHit;
            isHovering = Physics.Raycast(ray, out raycastHit) && ((Object)raycastHit.collider == (Object)trigger);
            if (InteractScript.Intracting & Time.timeScale != 0f)
            {
                if (isHovering)
                {
                    if (!bDoorLocked)
                    {
                        if (silentOpens <= 0)
                        {
                            if (NullManager.Instance != null)
                            {
                                NullManager.Instance.NULL.Hear(base.transform.position, 1f);
                            }
                            else
                            {
                                if (baldi.isActiveAndEnabled && Vector3.Distance(baldi.transform.position, player.position) <= 200f)
                                {
                                    baldi.Hear(base.transform.position, 1f);
                                }
                            }
                        }
                        if (silentOpens <= 0 & baldi.gameController.DoctorReflex != null && baldi.gameController.DoctorReflex.Angry && !baldi.gameController.DoctorReflex.WantsTheHammer)
                        {
                            baldi.gameController.DoctorReflex.Hear(transform.position); // Hear like Baldi
                        }
                        if (silentOpens <= 0 & baldi.gameController.principalNPC.currentPriority > 2f)
                        {
                            baldi.gameController.principalNPC.Hear(base.transform.position, 8f);
                        }
                        if (FindObjectOfType<Guy>() & silentOpens <= 0)
                        {
                            FindObjectOfType<Guy>().SetTo(transform.position);
                            FindObjectOfType<GameControllerScript>().audioDevice.Stop();
                            FindObjectOfType<GameControllerScript>().audioDevice.PlayOneShotWC(FindObjectOfType<Guy>().audioDevice.clip);
                        }
                        OpenDoor();
                        if (silentOpens > 0)
                        {
                            silentOpens--;
                        }
                    }
                    else
                    {
                        myAudio.PlayOneShotWC(doorLocked, 1f);
                    }
                }
            }
        }
        // This was moved outside so it won't glitch out
        if (isHovering & !bDoorLocked)
        {
            if (click == null)
                click = Object.Instantiate(clickCursor);
        }
        else
        {
            if (click != null)
                Object.Destroy(click);
        }
    }

    public void CloseDoor()
    {
        openTime = 0f;
    }

    public void OpenDoor()
    {
        if (silentOpens <= 0 && !bDoorOpen)
        {
            if (OpenedbyFlood)
                myAudio.PlayOneShot(doorOpen, 1f);
            else
                myAudio.PlayOneShotWC(doorOpen, 1f);
        }
        barrier.enabled = false;
        invisibleBarrier.enabled = false;
        bDoorOpen = true;
        inside.sharedMaterial = open;
        outside.sharedMaterial = open;
        openTime = 3f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!bDoorLocked && other.CompareTag("NPC"))
        {
            if (baldi.gameController.HASManager && other.name == "Baldi" & Vector3.Distance(player.position, other.transform.position) <= 200f)
            {
                if (baldi.gameController.HASManager.seekTimer > 0f && baldi.gameController.HASManager.hiderOrSeeker == "Seeker")
                {
                    FindObjectOfType<DoorIncidator>().Show(base.transform.position);
                    return;
                }
            }
            OpenDoor();
        }
        if (!bDoorLocked && MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi && !MultiplayerManager.instance.HasMoreThan2Players && other.tag == "Player")
        {
            OpenDoor();
        }
        if (MultiplayerManager.isActive && MultiplayerManager.instance.HasMoreThan2Players && other.tag == "PlayerModel")
        {
            OpenDoor();
        }
    }

    public void LockDoor(float time)
    {
        if (!bDoorLocked) myAudio.PlayOneShotWC(doorLock, 1f);
        bDoorLocked = true;
        lockTime = time;
        inside.sharedMaterial = locked;
        outside.sharedMaterial = locked;
    }

    public void UnlockDoor()
    {
        if (bDoorLocked) myAudio.PlayOneShotWC(doorUnlock, 1f);
        bDoorLocked = false;
        inside.sharedMaterial = closed;
        outside.sharedMaterial = closed;
    }

    public void SilenceDoor()
    {
        silentOpens = 4;
    }
}