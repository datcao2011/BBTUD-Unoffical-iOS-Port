using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEndingActivity1 : MonoBehaviour
{
    [SerializeField]
    private PlayerScript Player;

    private GameObject click;

    [SerializeField]
    private GameObject clickCursor;

    private CameraScript Cam;

    [SerializeField]
    private Transform CamPosition;

    private bool Clicked = false;

    private void Start()
    {
        Cam = Camera.main.GetComponent<CameraScript>();
    }

    private void Update()
    {
        if (Clicked)
        {
            if (click == null)
            {
                click = Instantiate(clickCursor);
                Destroy(click, 10.25f);
            }
            if (InteractScript.Intracting)
            {
                Clicked = false;
                Cam.IgnoreCameraInputs = false;
                Camera.main.fieldOfView = 60f;
                if (click != null)
                {
                    Destroy(click);
                }
            }
        }
        else
        {
            if (Vector3.Distance(Player.transform.position, base.transform.position) < 10f)
            {
                Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
                RaycastHit raycastHit;
                bool isHovering = Physics.Raycast(ray, out raycastHit) && (raycastHit.transform == transform);
                if (isHovering)
                {
                    if (click == null)
                    {
                        click = Instantiate(clickCursor);
                        Destroy(click, 10.25f);
                    }
                    if (InteractScript.Intracting)
                    {
                        Clicked = true;
                        Cam.IgnoreCameraInputs = true;
                        Cam.transform.position = CamPosition.position;
                        Cam.transform.rotation = CamPosition.rotation;
                        Camera.main.fieldOfView = 90f;
                    }
                }
                else if (click != null)
                {
                    Destroy(click);
                }
            }
            else if (click != null)
            {
                Destroy(click);
            }
        }
    }
}
