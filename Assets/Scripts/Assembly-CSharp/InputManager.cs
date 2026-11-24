using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Linq;
using System.Diagnostics;
using Platforms;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;

public class InputManager : MonoBehaviour
{
    public static bool usingTouch
    {
        get
        {
#if UNITY_ANDROID
            return true;
#else
            return TouchControls.Detected || (instance != null && instance.touchTest);
#endif
        }
    }

    public static bool isUsingController
    {
        get
        {
            return CurrentDevice.currentPlatform == PFPlatform.Windows && GamePadController.isAnyGamepadConnected && Controller.isEnabled;
        }
    }

    public static InputManager instance;

    public GameObject errorScreenObject;
    public GameObject eventScreenObject;

    [Header("Audio Mixers")]
    public AudioMixer MainAudioMan;
    public AudioMixerGroup AMusic;
    public AudioMixerGroup ASFX;
    public AudioMixerGroup ACharacters;
    public AudioMixerGroup AAmbience;
    [Header("BACK TO IT!")]
    private static GameObject errorScreenGameObject;
    private static GameObject eventScreenGameObject;

    public Sprite jump;

    public static bool lostControllerSignal;

    //public static int mouseLockPositionX, mouseLockPositionY;

    public Vector2 MousePosition;

    //private bool restartingApp;

    [Tooltip("Makes usingTouch equals true")]
    public bool touchTest;

    //private int secretKeysStage;

    private static bool disableDisplayingErrorMessages;

    private static PlatPlayer platPlayer;
    public bool instanceCreated;

    public static string IsSchoolScene()
    {
        if (!(SceneManager.GetActiveScene().name == "School_2"))
        {
            return "School";
        }
        return "School_2";
    }

    public static EventMessage DisplayEvent(string message, float timer = 3f, Action<EventMessage> ButtonAction = null, string ButtonText = "OK")
    {
        if (disableDisplayingErrorMessages) return null;
        if (eventScreenGameObject == null) return null;
        GameObject gameObject = UnityEngine.Object.Instantiate(eventScreenGameObject);
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
        EventMessage eventMessage = gameObject.GetComponent<EventMessage>();
        eventMessage.Message = message;
        eventMessage.disappearMessageTimer = timer;
        eventMessage.ButtonAction = ButtonAction;
        eventMessage.Button = ButtonText;
        eventMessage.StartLogMessage();
        eventMessage.StartTimer = true;
        return eventMessage;
    }

    public static EventMessage DisplayEvent(string message, Color Background, float timer = 3f, Action<EventMessage> ButtonAction = null, string ButtonText = "OK")
    {
        if (disableDisplayingErrorMessages) return null;
        if (eventScreenGameObject == null) return null;
        GameObject gameObject = UnityEngine.Object.Instantiate(eventScreenGameObject);
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
        EventMessage eventMessage = gameObject.GetComponent<EventMessage>();
        eventMessage.Message = message;
        eventMessage.disappearMessageTimer = timer;
        eventMessage.ButtonAction = ButtonAction;
        eventMessage.Button = ButtonText;
        eventMessage.StartLogMessage();
        eventMessage.StartTimer = true;
        UnityEngine.UI.Image image = eventMessage.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        Background.a = image.color.a;
        image.color = Background;
        return eventMessage;
    }

    public static void DisplayException(Exception e, float timer = 15f)
    {
        if (disableDisplayingErrorMessages) return;
        if (errorScreenGameObject == null) return;
        string errorTrace = "";
        StackTrace st = new StackTrace(1);
        errorTrace += e.GetType().Name + ": " + e.Message + "\n" + st.ToString();
        GameObject gameObject = UnityEngine.Object.Instantiate(errorScreenGameObject);
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
        ErrorMessage errorMessage = gameObject.GetComponent<ErrorMessage>();
        errorMessage.errorMessage = e.Message;
        errorMessage.fullErrorMessageTrace = errorTrace;
        errorMessage.disappearMessageTimer = timer;
        errorMessage.countdownTimer = true;
        errorMessage.StartLogMessage();
    }

    public static void DisplayException(Exception e, float timer, string message)
    {
        if (disableDisplayingErrorMessages) return;
        if (errorScreenGameObject == null) return;
        string errorTrace = "";
        StackTrace st = new StackTrace(1);
        errorTrace += e.GetType().Name + ": " + e.Message + "\n" + st.ToString();
        GameObject gameObject = UnityEngine.Object.Instantiate(errorScreenGameObject);
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
        ErrorMessage errorMessage = gameObject.GetComponent<ErrorMessage>();
        errorMessage.errorMessage = message;
        errorMessage.fullErrorMessageTrace = errorTrace;
        errorMessage.disappearMessageTimer = timer;
        errorMessage.countdownTimer = true;
        errorMessage.StartLogMessage();
    }

    public static string siteRequestUrl
    {
        get
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return "https://seenwalex.wixsite.com/site/_functions-dev/";
#else
            return "https://seenwalex.wixsite.com/site/_functions/";
#endif
        }
    }

    public static void ClearMessages()
    {
        foreach (ErrorMessage errorMsg in UnityEngine.Object.FindObjectsOfType<ErrorMessage>())
        {
            errorMsg.countdownTimer = true;
            errorMsg.disappearMessageTimer = 0f;
        }
        foreach (EventMessage eventMsg in UnityEngine.Object.FindObjectsOfType<EventMessage>())
        {
            Destroy(eventMsg.gameObject, 0f);
        }
    }

    public static void StopDisplayingMessages()
    {
        disableDisplayingErrorMessages = true;
        ClearMessages();
    }

    private void Awake()
    {
        if (!PlayerPrefsPlus.GetBool("ORIGINGAMESAVECHECK"))
        {
            SceneManager.LoadScene("Loading");
            return;
        }
        if (instanceCreated) return;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(base.gameObject);
            return;
        }
        instanceCreated = true;
        currentScene = "School_2";
        disableDisplayingErrorMessages = false; // Forgot that this won't show any new messages if the game restarts.
        platPlayer = PlatTools.GetPlatformPlayer();
        GameObject traceObject = new GameObject("Screen Logging Trace");
        DontDestroyOnLoad(traceObject);
        ConsoleTraceInGame script = traceObject.AddComponent<ConsoleTraceInGame>();
        if (script == null) return;
        // script.shakeAcceleration = 25;
        // script.shakeToOpen = true;
        script.toggleMainKey = KeyCode.F8;

#if UNITY_EDITOR || NETWORK_CONSOLE
        NetworkConsole script2 = traceObject.AddComponent<NetworkConsole>();
        script2.toggleMainKey = KeyCode.F6;
#endif
        errorScreenGameObject = errorScreenObject;
        eventScreenGameObject = eventScreenObject;
#if !UNITY_WEBGL
        SetFrameRate(Settings.GetInt("frames", -1));
#endif
#if !UNITY_ANDROID && !UNITY_WEBGL
        SetVSync(Settings.GetBool("vsync", false));
#endif
    }

    private void Start()
    {
        if (instance == this && MainAudioMan != null)
        {
            string[] AudioVolumes = new string[] { "AmbienceVolume", "CharacterVolume", "SoundVolume", "MasterVolume", "MusicVolume" };
            foreach (string AudioVolumeVal in AudioVolumes)
            {
                float Volume = Settings.GetFloat("Volume_" + AudioVolumeVal, 1f);
                if (AudioVolumeVal == "MasterVolume")
                {
                    AudioListener.volume = Volume;
                }
                else
                {
                    MainAudioMan.SetFloat(AudioVolumeVal, Mathf.Log10(Volume) * 20);
                }
            }
        }
    }

    public static void SetFrameRate(int frames)
    {
        Application.targetFrameRate = frames;
        Settings.SetInt("frames", frames);
    }

#if !UNITY_ANDROID && !UNITY_WEBGL
    public static void SetVSync(bool vsync)
    {
        QualitySettings.vSyncCount = vsync ? 1 : 0;
        Settings.SetBool("vsync", vsync);
    }
#endif

    private void InitControls()
    {

    }

    public static float Forward(bool affectsSpeed)
    {
        return (platPlayer.GetAxisRaw("Forward") + JoystickSetAxis.forward) * (affectsSpeed ? Time.timeScale : 1f);
    }

    public static float Strafe(bool affectsSpeed)
    {
        return (platPlayer.GetAxisRaw("Strafe") + JoystickSetAxis.strafe) * (affectsSpeed ? Time.timeScale : 1f);
    }

    public static float TurnX(bool affectsSpeed)
    {
        float rotateValue;
        if (InputManager.usingTouch)
        {
            rotateValue = MobileTouchScript.XAxis;
        }
        else
        {
            rotateValue = platPlayer.GetAxis("Turn X");
            // console.log("Init mouse on");
        }
        return rotateValue * (affectsSpeed ? Time.timeScale : 1f);
    }

    public static float TurnY(bool affectsSpeed)
    {
        float rotateValue;
        if (InputManager.usingTouch)
        {
            rotateValue = MobileTouchScript.YAxis;
        }
        else
        {
            rotateValue = platPlayer.GetAxis("Turn Y");
            // console.log("Init mouse on");
        }
        return rotateValue * (affectsSpeed ? Time.timeScale : 1f);
    }

    public bool isMouseInteractedInThisFrame
    {
        get
        {
            return Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        }
    }

    private bool isFullScreen;
    private float deltaTime;

    private float fps;

    public static string Stats;

    public void Update()
    {
#if UNITY_STANDALONE_WIN
        if (GamePadController.isAnyGamepadConnected || !Controller.isEnabled)
        {
            if (isMouseInteractedInThisFrame && Controller.isEnabled && !KeyBindsConfigurtion.IsActive)
            {
                Controller.DisablePad();
            }
            else if (Input.GetButtonDown("Click") || Input.GetButtonDown("DropItem") || Input.GetButtonDown("Use") || Input.GetAxisRaw("TurnX") != 0f || Input.GetAxisRaw("TurnY") != 0f || Input.GetAxisRaw("MoveVertical") != 0f || Input.GetAxisRaw("MoveHorizontal") != 0f)
            {
                Controller.EnablePad();
            }
        }
        // if (Controller.usedController & !GamePadController.isAnyGamepadConnected)
        // {
        //     InputManager.lostControllerSignal = true;
        // }
        // else
        // {
        //     InputManager.lostControllerSignal = false;
        // }
#endif
        // if (GamePadController.isAnyGamepadConnected & InputManager.currentScene == "MainMenu")
        // {
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        // }
        // else if (InputManager.currentScene == "MainMenu")
        // {
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;
        //     this.onSelect = false;
        // }
        // if (((Input.touchCount > 0 && Input.touchSupported) || touchTest) & !usingTouch)
        // {
        //     usingTouch = true;
        // }
        // else if ((isUsingController || isMouseInteractedInThisFrame) && usingTouch && !touchTest)
        // {
        //     usingTouch = false;
        // }
#if UNITY_EDITOR
        //if (!Application.isMobilePlatform && !Application.isConsolePlatform) MousePosition = Input.mousePosition;
#endif
#if UNITY_STANDALONE_WIN
        if (CurrentDevice.currentPlatform == PFPlatform.Windows && GamePadController.isAnyGamepadConnected && Cursor.visible)
        {
            cursorposition = platPlayer.GetGlobalCursorPosition(true);
            cursorposition.x += Input.GetAxisRaw("MoveHorizontal") * Settings.GetFloat("TurnSensitivity", 1f) * 0.05f;
            cursorposition.y -= Input.GetAxisRaw("MoveVertical") * Settings.GetFloat("TurnSensitivity", 1f) * 0.05f;
            platPlayer.SetGlobalCursorPosition(cursorposition);
            if (Input.GetButtonDown("MoveSubmit"))
            {
                platPlayer.MouseLeftClick();
            }
        }
#endif
        // if (!Application.isMobilePlatform && !Application.isConsolePlatform && ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
        // Input.GetKeyDown(KeyCode.K)) && SceneManager.GetActiveScene().name != "RestartError")
        // {
        //     GameSystem.AbortApplication("An keybind has occurred manually to this screen.", "KEYBIND_MANUALLY_SCREEN");
        // }
        this.deltaTime += (Time.unscaledDeltaTime - this.deltaTime) * 0.1f;

        float f = 1f / this.deltaTime;

        fps = Mathf.Ceil(f);
        string textNow = "FPS: " + fps;
        if (Photon.Pun.PhotonNetwork.IsConnected)
        {
            textNow += " | Ping: " + Photon.Pun.PhotonNetwork.GetPing();
        }
        Stats = textNow;

        // #if UNITY_WEBGL
        //         if (Screen.fullScreen != isFullScreen)
        //         {
        //             UnityEngine.Debug.Log("Full screen change " + Screen.fullScreen);
        //             if (Screen.fullScreen)
        //             {
        //                 try
        //                 {
        //                     UnityEngine.Debug.Log("Resolution: " + Screen.resolutions.Length);
        //                     UnityEngine.Debug.Log(Screen.currentResolution.width + "xCCx" + Screen.currentResolution.height);
        //                     Screen.SetResolution(Screen.resolutions[0].width, Screen.resolutions[0].height, Screen.fullScreenMode);
        //                 }
        //                 catch (System.Exception ex)
        //                 {
        //                     console.logException(ex);
        //                 }
        //             }
        //             else
        //             {
        //                 Resolution[] resolutions = Screen.resolutions;
        //                 for (int i = 0; i < resolutions.Length; i++)
        //                 {
        //                     UnityEngine.Debug.Log("Resolution: " + resolutions[i].width + "x" + resolutions[i].height);
        //                 }
        //                 Screen.SetResolution(1024, 576, Screen.fullScreenMode);
        //             }
        //         }
        //         isFullScreen = Screen.fullScreen;
        // #endif
    }

    private Vector3 cursorposition;

    private int times;

    private bool onSelect;
    private string sceneNow;

    public static string currentScene;
}