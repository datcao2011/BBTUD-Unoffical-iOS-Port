// EntranceScript
using UnityEngine;

public class EntranceScript : MonoBehaviour
{
    public GameControllerScript gc;

    public Material map;

    public MeshRenderer wall;

    public Animator elevate;

    public bool isOpened;

    public bool exitTaken;

    public AudioSource elevateSound;

    public AudioClip close;

    public AudioClip open;

    public AudioClip gateClose;
    private bool raised = true;

    public void Lower()
    {
        raised = false;
        base.transform.position = base.transform.position - new Vector3(0f, 10f, 0f);
        if (gc.finaleMode)
        {
            wall.material = map;
            exitTaken = true;
        }
    }

    public void Raise()
    {
        if (raised) return;
        raised = true;
        base.transform.position = base.transform.position + new Vector3(0f, 10f, 0f);
    }

    public void OpenElevate()
    {
        if (!exitTaken)
        {
            isOpened = true;
            elevate.SetTrigger("Open");
            elevateSound.PlayOneShot(open);
        }
    }

    public void CloseElevate()
    {
        if (!exitTaken)
        {
            isOpened = false;
            elevate.SetTrigger("Close");
            elevateSound.PlayOneShot(close);
            if (gc.finaleMode)
            {
                exitTaken = true;
                elevate.SetTrigger("OutOfOrder");
                transform.GetChild(0).GetChild(1).gameObject.AddComponent<AudioQueueScript>().Queue(gateClose);
            }
        }
    }

    public void CloseUpElevate()
    {
        isOpened = false;
        elevate.SetTrigger("Up_Close");
        elevateSound.PlayOneShot(close);
    }

    public void OpenUpElevate()
    {
        isOpened = false;
        elevate.SetTrigger("Up_Open");
        elevateSound.PlayOneShot(open);
    }

    public void CloseDownElevate()
    {
        isOpened = false;
        elevate.SetTrigger("Close");
        elevateSound.PlayOneShot(close);
    }

    public void OpenDownElevate()
    {
        isOpened = false;
        elevate.SetTrigger("Open");
        elevateSound.PlayOneShot(open);
    }
}