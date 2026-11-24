using System.Collections;
using UnityEngine;
using Platforms;
using UnityEngine.UI;

public class BaldiCameraScript : MonoBehaviour
{

    private Transform cameraRotation;

    private float mouseSensitivity;
    private float rotationY;

    public PlayerScript ps;

    public Transform playerCamera;

    private PlatPlayer player;

    public Camera BaldiCam;

    private void Start()
    {
        playerCamera = Camera.main.transform;
        player = PlatTools.GetPlatformPlayer();
        cameraRotation = base.transform.parent;
        playerCamera.GetComponent<AudioListener>().enabled = false;
        gameObject.AddComponent<AudioListener>();

        OnDistanceChanged("Distance", Settings.GetInt("Distance", GameOptimizer.CameraOptimizationDefault));
        OnDistanceChanged("OcclusionCulling", Settings.GetBool("OcclusionCulling", GameOptimizer.OptimizationDefault));
        Settings.OnSettingChanged += OnDistanceChanged;
    }

    private void OnDistanceChanged(string name, object val)
    {
        if (name == "Distance")
        {
            int[] Distances = new int[] { 100, 200, 500, 10000 };
            BaldiCam.farClipPlane = Distances[(int)val];
            BaldiCam.nearClipPlane = (int)val >= 2 ? 0.1f : 1f;
        }
        if (name == "OcclusionCulling")
        {
            BaldiCam.useOcclusionCulling = (bool)val;
            if (Settings.GetString("ModeType", "Normal") == "BB")
            {
                BaldiCam.useOcclusionCulling = false;
            }
        }
    }
    
    private void OnDestroy()
    {
        Settings.OnSettingChanged -= OnDistanceChanged;
    }

    private void Update()
    {
        MouseMove();
        if (ps.gameOver)
        {
            base.transform.position = playerCamera.transform.position;
            base.transform.rotation = playerCamera.transform.rotation;
        }
        else
        {
            playerCamera.transform.position = base.transform.position;
            playerCamera.transform.rotation = base.transform.rotation;
        }
    }

    private void MouseMove()
    {
        if (MultiplayerManager.isActive && MultiplayerManager.instance.PauseMovement)
        {
            return;
        }

        if (Settings.GetBool("3d movement"))
        {
            this.rotationY -= InputManager.TurnY(true) * Settings.GetFloat("TurnSensitivity", 1f);
            rotationY = Mathf.Clamp(this.rotationY, -90f, 90f);
        }
        else
        {
            rotationY = 0f;
        }

        mouseSensitivity = Settings.GetFloat("TurnSensitivity", 1f);
        cameraRotation.eulerAngles += new Vector3(0f, InputManager.TurnX(false) * mouseSensitivity * Time.timeScale);

        transform.localEulerAngles = new Vector3(rotationY, (player.GetButton("LookBack") || ps.cameraScript.lookBehindButton) ? 180f : 0f);
    }
}