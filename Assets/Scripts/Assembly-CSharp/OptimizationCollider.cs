using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizationCollider : MonoBehaviour
{
    private bool IsInside;
    public DoorScript Door;

    public GameObject Cameras;
    public Transform Player;

    private bool a;

    [SerializeField]
    private bool AllowThisBeingDestroyed = true;

    private Vector3[] RenderingCameras;

    // Awake, so this gets called before anything else...
    private void Awake()
    {
        Cameras.SetActive(false);
        if (Settings.GetString("ModeType", "Normal") == "BB" && AllowThisBeingDestroyed)
        {
            Destroy(this);
        }
        Camera[] RenderingCameras = Cameras.GetComponentsInChildren<Camera>();
        this.RenderingCameras = new Vector3[RenderingCameras.Length];
        for (int i = 0; i < RenderingCameras.Length; i++)
        {
            this.RenderingCameras[i] = RenderingCameras[i].transform.position;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Player.position.y >= transform.position.y && Vector3.Distance(Player.position, transform.position) <= 75f)
        {
            Cameras.SetActive(IsInside || Door.bDoorOpen);
            a = false;
        }
        else if (!a)
        {
            a = true;
            Cameras.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            IsInside = true;
            if (Settings.GetBool("1.4Shaders"))
            {
                foreach (Camera cam in Cameras.GetComponentsInChildren<Camera>())
                {
                    cam.transform.position = new Vector3(-999f, 7.6f, 999f);
                }
                StartCoroutine(Delay());
            }
        }
    }

    private IEnumerator Delay()
    {
        yield return null;
        Camera[] RenderingCameras = Cameras.GetComponentsInChildren<Camera>();
        for (int i = 0; i < RenderingCameras.Length; i++)
        {
            RenderingCameras[i].transform.position = this.RenderingCameras[i];
        }
        yield break;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            IsInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            IsInside = false;
        }
    }
}
