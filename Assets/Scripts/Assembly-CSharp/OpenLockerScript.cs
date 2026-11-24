using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLockerScript : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer lockerFront;

    [SerializeField]
    private Material closed;

    [SerializeField]
    private Material open;

    public GameObject clickCursor;

    public GameObject click;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private BaldiScript baldi;

    [SerializeField]
    private PlayerScript playerScript;

    [SerializeField]
    private BoxCollider invisibleBarrier;

    [SerializeField]
    private BoxCollider trigger;

    public bool bDoorOpen;

    public float openTime;

    private int silentOpens;

    public void Silence()
    {
        silentOpens = 4;
    }

    private void Update()
    {
        if (openTime > 0f)
        {
            openTime -= 1f * Time.deltaTime;
        }
        if (openTime <= 0f & bDoorOpen)
        {
            bDoorOpen = false;
            invisibleBarrier.enabled = true;
            lockerFront.sharedMaterial = closed;
        }
        if (InteractScript.Intracting && Time.timeScale != 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit) & ((Object)raycastHit.collider == (Object)trigger && Vector3.Distance(player.position, base.transform.position) < 10f))
            {
                if (silentOpens <= 0 && baldi.isActiveAndEnabled)
                {
                    baldi.Hear(base.transform.position, 1f);
                }
                if (silentOpens > 0)
                {
                    silentOpens--;
                }
				playerScript.ResetGuilt("lockers", 1f);
                OpenDoor();
            }
        }
        Ray rray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit rraycastHit;
        if (Physics.Raycast(rray, out rraycastHit) & ((Object)rraycastHit.collider == (Object)trigger && Vector3.Distance(player.position, base.transform.position) < 10f))
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

    public void OpenDoor()
    {
        if (silentOpens <= 0 && !bDoorOpen)
        {
            myAudio.PlayOneShot(doorOpen, 1f);
        }
        bDoorOpen = true;
        invisibleBarrier.enabled = false;
        lockerFront.sharedMaterial = open;
        openTime = 3f;
    }

    [SerializeField]
    private AudioClip doorOpen;

    [SerializeField]
    private AudioSource myAudio;
}
