// using System.Collections;
// using System.Collections.Generic;
// using Photon;
// using Photon.Realtime;
// using Photon.Pun;
using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using Multiplayer;
// using Platforms;
using Cinemachine;

public class SpectatingCamera : MonoBehaviour
{
    private Camera MainCamera;
    private SpectatingHud spectatingHud;
    private GameObject MainObject;
    public GameControllerScript gc;
    private PlayerScript ps;
    public CinemachineFreeLook Cam;
    public CinemachineBrain MainCameraMovement;

    // Start is called before the first frame update
    private void Start()
    {
        spectatingHud = GetComponent<SpectatingHud>();
        ps = FindObjectOfType<PlayerScript>();
        MainCamera = FindObjectOfType<CameraScript>().GetComponent<Camera>();
        FindObjectOfType<CameraScript>().IgnoreCameraInputs = true;
        MainObject = new GameObject("Spectating Object");
        MainCameraMovement.enabled = true;
        Cam.gameObject.SetActive(true);
        Cam.Follow = MainObject.transform;
        Cam.LookAt = MainObject.transform;
        MobileTouchScript.XAxis = 0f;
        MobileTouchScript.YAxis = 0f;
    }

    private void OnDisable()
    {
        if (MainCameraMovement == null || Cam == null) return;
        MainCameraMovement.enabled = false;
        Cam.gameObject.SetActive(false);
        MainCamera.fieldOfView = 60f;
        ps.cameraScript.IgnoreCameraInputs = false;
    }

    private void Disable3DCamera()
    {
        MainCameraMovement.enabled = false;
        MainCamera.fieldOfView = 60f;
        Cam.gameObject.SetActive(false);
    }

    private void Enable3DCamera()
    {
        MainCameraMovement.enabled = true;
        Cam.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (gc.mouseLocked)
        {
            gc.mouseLocked = false;
            gc.UnlockMouse();
        }
        if (!spectatingHud.FirstPerson)
        {
            spectatingHud.SetAngle(spectatingHud.SpectatingAngle + (MobileTouchScript.XAxis * ps.mouseSensitivity));
            // Don't like this
            // if (Settings.GetBool("3d movement"))
            // {
            //     float rotationY = spectatingHud.SpectatingAngleY;
            //     rotationY -= InputManager.TurnY(true) * Settings.GetFloat("TurnSensitivity", 250f) * Time.unscaledDeltaTime;
            //     rotationY = Mathf.Clamp(rotationY, -90f, 90f);
            //     spectatingHud.SpectatingAngleY = rotationY;
            // }
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (spectatingHud.FocusedGameObject != null)
        {
            if (spectatingHud.FirstPerson)
            {
                MainCamera.nearClipPlane = 0.3f;
                Vector3 AddVector = new Vector3(0f, 0.8f, 0f);
                if (spectatingHud.FocusedGameObject.name == "Baldi")
                {
                    AddVector.y += (gc.baldiScrpt.FlattenDuration > 0f) ? -0.2f : 4f;
                }
                if (Cam.isActiveAndEnabled)
                {
                    Disable3DCamera();
                }
                MainCamera.transform.position = spectatingHud.FocusedGameObject.transform.position + AddVector;
                MainCamera.transform.rotation = spectatingHud.FocusedGameObject.transform.rotation;
            }
            else
            {
                MainCamera.nearClipPlane = 1f;
                float PositionY = 1f;
                if (spectatingHud.FocusedGameObject.name == "Baldi")
                {
                    PositionY += (gc.baldiScrpt.FlattenDuration > 0f) ? -0.2f : 4f;
                }
                if (!Cam.isActiveAndEnabled)
                {
                    Enable3DCamera();
                }
                MainObject.transform.position = spectatingHud.FocusedGameObject.transform.position + new Vector3(0f, PositionY, 0f);
                Cam.m_XAxis.Value = spectatingHud.SpectatingAngle;
            }
        }
    }
}
