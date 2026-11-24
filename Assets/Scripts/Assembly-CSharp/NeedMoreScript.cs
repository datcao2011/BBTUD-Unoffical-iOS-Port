using UnityEngine;

public class NeedMoreScript : MonoBehaviour
{
    [SerializeField]
    private GameControllerScript gc;

    private SwingingDoorScript swingingDoor;

    private SwingingDoorDarkScript swingingDoorDark;

    private AudioSource audioDevice;

    [SerializeField]
    private AudioClip baldiDoor;

    private bool Optimized;

    private NPC[] characters = new NPC[0];

    private void Start()
    {
        audioDevice = GetComponent<AudioSource>();
        swingingDoor = GetComponent<SwingingDoorScript>();
        swingingDoorDark = GetComponent<SwingingDoorDarkScript>();
        Optimized = Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault);
        if (Optimized && !MultiplayerManager.isActive)
        {
            enabled = false;
        }
        else if (!Optimized)
        {
            characters = FindObjectsOfType<NPC>(true);
        }
    }

    private void Update()
    {
        if (!audioDevice.isPlaying && gc.notebooks < 2 && DoorRequirement())
        {
            if (Optimized && MultiplayerManager.isActive)
            {
                if (Vector3.Distance(gc.baldiTransform.position, transform.position) < 5f)
                {
                    NeedMore();
                }
            }
            else
            {
                foreach (NPC character in characters)
                {
                    if (character.isActiveAndEnabled && Vector3.Distance(character.transform.position, transform.position) < 5f)
                    {
                        NeedMore();
                    }
                }
            }
        }
    }

    private void NeedMore()
    {
        bool isCoined;
        if (swingingDoorDark != null)
        {
            isCoined = swingingDoorDark.coinDoor;
        }
        else if (swingingDoor != null)
        {
            isCoined = swingingDoor.coinDoor;
        }
        else
        {
            isCoined = false;
        }
        if (!isCoined)
        {
            audioDevice.PlayOneShotWC(baldiDoor, 1f);
        }
    }

    private bool DoorRequirement()
    {
        return (swingingDoorDark != null) ? !swingingDoorDark.SkipRequirement : (swingingDoor != null ? !swingingDoor.SkipRequirement : true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && gc.notebooks < 2 && !other.isTrigger && DoorRequirement())
        {
            NeedMore();
        }
    }
}