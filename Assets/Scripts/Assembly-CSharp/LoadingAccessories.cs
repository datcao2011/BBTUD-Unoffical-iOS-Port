using UnityEngine;
using UnityEngine.SceneManagement;
using Platforms;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.IO;
using TMPro;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using Microsoft.Win32;
#endif

public class LoadingAccessories : MonoBehaviour
{
    public TMP_Text loadingText;
    public GameObject loadingIcon;
    public GameObject loadingThing;
    public string GET_UPDATE_URL;
    private bool dialog_shown;
    public Animator dialog;
    public Animator updateDialog;
    public Animator noticeDialog;
    public TMP_Text dialogTitle;
    public TMP_Text dialogText;
    public TMP_Text updateText;
    public TMP_Text updateTitle;

    private void Awake() // Instantly load to warning
    {
        InstantLoad = Settings.GetBool("SkipUpdatesAndGaps");
        if (InstantLoad && (Application.internetReachability == NetworkReachability.NotReachable || !Settings.HasKey("AccountSession")))
        {
            AccountScript.LOAD_TO_SCENE = Settings.GetBool("Skip_Warning") ? "MainMenu" : "Warning";
            loadingIcon.SetActive(false); // So it doesn't show an icon & text on a single frame
            loadingText.gameObject.SetActive(false);
            SceneManager.LoadScene(AccountScript.LOAD_TO_SCENE);
        }
    }

    private void Start()
    {
        loadingText.text = "LoadingScreen()";
        if (!(InstantLoad && (Application.internetReachability == NetworkReachability.NotReachable || !Settings.HasKey("AccountSession"))))
        {
            StartCoroutine(LoadingScreen());
        }
    }

    private float InfiniteTimer;
    private float Progress = Mathf.Infinity;
    private bool INF_Forward = true;
    public UnityEngine.UI.Slider slider;

    private void SetProgress(float progress)
    {
        this.Progress = progress;
        slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
        slider.normalizedValue = Progress;
    }

    private void Update()
    {
        if (float.IsInfinity(Progress))
        {
            slider.direction = INF_Forward ? UnityEngine.UI.Slider.Direction.LeftToRight : UnityEngine.UI.Slider.Direction.RightToLeft;
            if (INF_Forward)
            {
                InfiniteTimer += Time.unscaledDeltaTime;
                if (InfiniteTimer >= 1f)
                {
                    INF_Forward = false;
                }
            }
            else
            {
                InfiniteTimer -= Time.unscaledDeltaTime;
                if (InfiniteTimer <= 0f)
                {
                    INF_Forward = true;
                }
            }
            slider.value = InfiniteTimer;
        }
    }

    private IEnumerator LoadingScreen()
    {
        // ORIGNAL GAME TO RETRIEVE SAVED DATA FROM THE ORIGINAL MOD
        if (!PlayerPrefsPlus.GetBool("ORIGINGAMESAVECHECK"))
        {
            PlayerPrefsPlus.SetBool("ORIGINGAMESAVECHECK", true);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            loadingText.text = "Copying save data from Original Game";
            yield return new WaitForSecondsRealtime(1f);
            try
            {
#if UNITY_EDITOR_WIN
                const string PATH = @"Software\Unity\UnityEditor\SeenWonderAlex\Baldi's Basics Ultra Decompile";
#else
                const string PATH = @"Software\SeenWonderAlex\Baldi's Basics Ultra Decompile";
#endif
#if UNITY_EDITOR_WIN
                const string CURRENT_PATH = @"Software\Unity\UnityEditor\SeenWonderAlex\Baldi's Basics Ultra Decompile OS";
#else
                const string CURRENT_PATH = @"Software\SeenWonderAlex\Baldi's Basics Ultra Decompile OS";
#endif
                RegistryKey OldGame = Registry.CurrentUser.OpenSubKey(PATH);
                RegistryKey Game = Registry.CurrentUser.OpenSubKey(CURRENT_PATH, true);

                foreach (string name in OldGame.GetValueNames())
                {
                    RegistryValueKind kind = OldGame.GetValueKind(name);
                    object value;
                    if ((value = OldGame.GetValue(name, null)) != null)
                    {
                        if (kind == RegistryValueKind.DWord && ((value.GetType() == typeof(int) && (int)value > int.MaxValue) || (value.GetType() == typeof(Int64) && (Int64)value > int.MaxValue)))
                        {
                            continue;
                        }
                        Game.SetValue(name, value, kind);
                    }
                }

                OldGame.Close();
                Game.Close();
                PlayerPrefsPlus.SetBool("ORIGINGAMESAVECHECK", true);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                loadingText.text = "Failed to copy save data";
            }
            yield return new WaitForSecondsRealtime(1f);
#endif
        }
        #region ONLINE CHECK
        loadingText.text = "Checking for online connection";
        AccountScript.LOAD_TO_SCENE = Settings.GetBool("Skip_Warning") ? "MainMenu" : "Warning";
        #endregion
        if (!InstantLoad) yield return CheckForUpdates();

        #region RESET DATA CHECK
        float td = 0f;
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
#if UNITY_STANDALONE
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
#endif
            td = 3f;
        }
        while (td > 0f)
        {
            td -= Time.unscaledDeltaTime;
            loadingText.text = "Data reset (" + td.ToString("0") + ")";
            yield return null;
        }
        #endregion

        #region INPUT SYSTEM CHECK
        loadingText.text = "Starting input system";
        if (!InstantLoad) yield return StartCoroutine(DeviceChecker());
        #endregion

        #region CONSENT CHECK

        if (!Settings.HasKey("GameFirstUse3"))
        {
            Settings.SetString("GameFirstUse3", "_");
            while (!Application.isFocused) // Waiting for focus
            {
                loadingText.text = "Waiting for focus";
                yield return null;
            }
            if (Settings.HasKey("UsedVolume"))
            {
                ShowDialog("Message from DEV", "Wow! It looks like you've been playing BBTUD for a while (since V1.2.2 at least). I want to say thank you for playing. I really appericate you playing for this long. Now go along and play the big V3 update!");
            }
            else if (Settings.HasKey("GameFirstUse"))
            {
                ShowDialog("Message", "Hi there! It looks like you're been playing since late-V1 or V2 Beta. I appericate you've been playing for that long. V3 has more content than V2, so go ahead and explore them all!");
            }
            else if (Settings.HasKey("GameFirstUse2"))
            {
                ShowDialog("WELCOME TO V3!", "You'll experience a bunch of content here. I worked hard on this for the final update. Now go play it.");
            }
            while (dialog_shown)
            {
                yield return null;
            }
            AccountScript.LOAD_TO_SCENE = "Warning";
            Settings.DeleteKey("NoYCTP"); // Yeah, I'm resetting that setting because I want everyone to experience the new subjects :)
            // Resetting the high scores due to the online scoreboard system.
            Settings.DeleteKey("FarmBestTime");
            Settings.DeleteKey("FarmBestScore");
            Settings.DeleteKey("HighBooks");
            Settings.DeleteKey("HighCampScore");
            // Also delete the stored user since all sessions before this update will be deleted.
            if (Settings.HasKey("Account"))
            {
                dialog_shown = true;
                noticeDialog.SetBool("Shown", true);
            }
            Settings.DeleteKey("Account");
            Settings.DeleteKey("AccountSession");
        }
        while (dialog_shown)
        {
            yield return null;
        }
        #endregion

        #region Resources loading
        loadingText.text = "Checking for available optimization";
        if (!InstantLoad) yield return new WaitForSeconds(0.1f);
        #endregion

        #region Account login check
        loadingText.text = "Checking account";
        if ((Application.internetReachability != NetworkReachability.NotReachable && Settings.HasKey("AccountSession")) || !InstantLoad) yield return StartCoroutine(AccountChecking());
        #endregion

        loadingText.text = "Preparing to load level assets";
        if (!InstantLoad) yield return new WaitForSeconds(0.3f);
        AsyncOperation sceneOperation = SceneManager.LoadSceneAsync(AccountScript.LOAD_TO_SCENE);
        sceneOperation.allowSceneActivation = false;
        float progress = 0f;
        while (progress < 1f)
        {
            progress = Mathf.Clamp01(sceneOperation.progress / .9f);
            SetProgress(progress);
            loadingText.text = "Loading level assets (" + Mathf.CeilToInt(progress * 100f) + "%)";
            yield return null;
        }
        SetProgress(float.PositiveInfinity);
        loadingText.text = SceneLoader.GetRandomText();
        if (!InstantLoad) yield return new WaitForSeconds(0.8f);
        sceneOperation.allowSceneActivation = true;
        yield break;
    }

    private IEnumerator CheckForUpdates()
    {
#if UNITY_WEBGL
        // SKIP UPDATE
        yield break;
#else
        loadingText.text = "Checking for the latest update";
        if (Application.internetReachability == NetworkReachability.NotReachable) yield break;
        UnityWebRequest request = UnityWebRequest.Get(GET_UPDATE_URL);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            console.error("Connection to update URL failed: " + request.error);
            if (request.error.Contains("SSL Connection", StringComparison.InvariantCultureIgnoreCase))
            {
                ShowDialog("Update Connection Failed", "We're unable to connect to the update server due to certificate verification problems. See bit.ly/SSLConnection for details.");
            }
            else
            {
                ShowDialog("Update Connection Failed", "Update Check Failed (" + request.result + "): " + request.error);
            }
            while (dialog_shown)
            {
                yield return null;
            }
        }
        else
        {
            try
            {
                string latestVersionStr = request.downloadHandler.text;
                Version latestVersion;
                if (!Version.TryParse(latestVersionStr, out latestVersion))
                {
                    console.error("Failed to parse version text: " + request.error);
                    ShowDialog("Update Connection Failed", "Update Check Failed: Failed to parse response from the server");
                }
                else
                {
                    Version currentVersion = VersionInput.currentVersion;
                    if (latestVersion.Major > currentVersion.Major)
                    {
                        dialog_shown = true;
                        updateDialog.SetBool("Shown", true);
                        updateTitle.text = "V3 is finally available!";
                        updateText.text = "The long awaited update is here! You have version " + currentVersion + ". GO DOWNLOAD THE NEW UPDATE!";
                    }
                    else if (latestVersion.Minor > currentVersion.Minor && latestVersion.Major == currentVersion.Major)
                    {
                        dialog_shown = true;
                        updateDialog.SetBool("Shown", true);
                        updateTitle.text = "A minor update is available!";
                        updateText.text = "A new minor update (V" + latestVersion + ") is available with minor features and bugfixes. You have version " + currentVersion + ". Download the new update with minor features.";
                    }
                    else if (latestVersion.Build > currentVersion.Build && latestVersion.Minor == currentVersion.Minor && latestVersion.Major == currentVersion.Major)
                    {
                        dialog_shown = true;
                        updateDialog.SetBool("Shown", true);
                        updateTitle.text = "A update is available!";
                        updateText.text = "A new patch is now available (V" + latestVersion + "). You have version " + currentVersion + ". If you experienced any bugs, download the new patch now!";
                    }
                    else if ((latestVersion.Major < currentVersion.Major || latestVersion.Minor < currentVersion.Minor || latestVersion.Build < currentVersion.Build) && currentVersion != new Version(3, 0, 5))
                    {
                        ShowDialog("Beta Testing V" + currentVersion, "You are currently playing the FINAL beta version of Baldi's Basics The Ultra Decompile V3! Please report any bugs and broken features. The latest release version was v" + latestVersion);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                ShowDialog("Update Connection Failed", "Update Check Failed: " + ex.Message);
            }
            while (dialog_shown)
            {
                yield return null;
            }
        }
        yield break;
#endif
    }

    public GameObject _eventScreenGameObject;

    public EventMessage DisplayEvent(string message, float timer = 3f, Action<EventMessage> ButtonAction = null, string ButtonText = "OK")
    {
        GameObject gameObject = UnityEngine.Object.Instantiate(_eventScreenGameObject);
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


    private class StatusResponse
    {
        public string message;
        public bool available;
    }

    public class RequestResponse3
    {
        public bool valid;
        public string error;
        public string username;
        public string gamejolt;
    }

    private class VerificationResponse
    {
        public bool success;
        public string error = "FAILED TO RETRIEVE ERROR";
        public bool Recording;
    }

    private int AccountChecking_Attempts;

    private IEnumerator AccountChecking()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable || !Settings.HasKey("AccountSession"))
        {
            yield break;
        }
        if (Settings.HasKey("Account") & Settings.HasKey("AccountSession"))
        {
        Retry:
            loadingText.text = "Connecting to your account";
            string sessionId = Settings.GetString("AccountSession");
            string previousUser = Settings.GetString("Account");
            Settings.DeleteKey("Account");
            Settings.DeleteKey("AccountSession");

            UnityWebRequest webRequest = UnityWebRequest.Get(InputManager.siteRequestUrl + "validateSession?id=" + sessionId + "&gjinfo=1");
            yield return webRequest.SendWebRequest();
            RequestResponse3 data;
            try
            {
                data = JsonUtility.FromJson<RequestResponse3>(webRequest.downloadHandler.text);
                if (data == null) throw new System.Exception("Non-type JSON response received: " + webRequest.downloadHandler.text.Substring(0, 30));
            }
            catch (System.Exception)
            {
                data = new RequestResponse3();
                data.valid = false;
                data.error = "Server connection failed";
            }
            if (data.valid == false)
            {
                Debug.LogError(data.error);
                if (data.error == "Servers are too busy!")
                {
                    // No more! Sorry!
                    if (AccountChecking_Attempts >= 2)
                    {
                        float t_l = 3f;
                        while (t_l > 0f)
                        {
                            t_l -= Time.unscaledDeltaTime;
                            loadingText.text = "Servers too busy, login later (" + t_l.ToString("0") + ")";
                            yield return null;
                        }
                        yield break;
                    }
                    // Try to re-attempt
                    float t_3 = 3f;
                    string message = (AccountChecking_Attempts == 1) ? "Re-attempting one more time" : "Re-attempting to connect";
                    while (t_3 > 0f)
                    {
                        t_3 -= Time.unscaledDeltaTime;
                        loadingText.text = message + " (" + t_3.ToString("0") + ")";
                        yield return null;
                    }
                    AccountChecking_Attempts++;
                    Settings.SetString("Account", previousUser);
                    Settings.SetString("AccountSession", sessionId);
                    goto Retry;
                }
                // If it wasn't the servers being busy...
                float t_2 = 3f;
                while (t_2 > 0f)
                {
                    t_2 -= Time.unscaledDeltaTime;
                    loadingText.text = "Login failed (" + t_2.ToString("0") + ")";
                    yield return null;
                }
                yield break;
            }
            else if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                float t_2 = 3f;
                while (t_2 > 0f)
                {
                    t_2 -= Time.unscaledDeltaTime;
                    loadingText.text = "Connection failed (" + t_2.ToString("0") + ")";
                    yield return null;
                }
                yield break;
            }
            else
            {
                Settings.SetString("Account", data.username);
                Settings.SetString("AccountSession", sessionId);
                if (!string.IsNullOrEmpty(data.gamejolt)) GameJolt.BBTUD.GameJoltManager.Create(data.gamejolt);
                AccountManager.SetData_LoadingManager(data.username, sessionId);
            }
        }
        loadingText.text = "Downloading profile";
        yield return AccountManager.WaitGetProfile();
        yield break;
    }

    public bool InstantLoad;

    private IEnumerator DeviceChecker()
    {
        Platforms.Console.Logger.ConsoleLogger.Log("Starting input system on " + CurrentDevice.currentPlatform);
        if (Application.isEditor) { if (!InstantLoad) yield return new WaitForSecondsRealtime(0.09f); yield break; }
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                loadingText.text = "Device: Windows";
                break;
            case RuntimePlatform.OSXPlayer:
                loadingText.text = "Device: MacOSX";
                break;
            case RuntimePlatform.LinuxPlayer:
                loadingText.text = "Device: Linux";
                break;
            case RuntimePlatform.WebGLPlayer:
                loadingText.text = "Device: WebGL";
                break;
            case RuntimePlatform.Android:
                loadingText.text = "Device: Android";
                break;
            default:
                loadingText.text = "Unsupported Device";
                ShowDialog("Unsupported Device", "You are currently on a unsupported device (" + Application.platform + "). Supported devices of BBTUD are Windows, Mac OSX, Android, and WebGL. You can not play this game on a unsupported device.");
                while (dialog_shown)
                {
                    yield return null;
                }
                ApplicationConfigure.closeApplication();
                throw new NotSupportedException("Current Device is not supported. Cannot continue.");
        }
        if (!InstantLoad) yield return new WaitForSecondsRealtime(0.09f);
        yield break;
    }

    public void ShowDialog(string title, string text)
    {
        dialog.SetBool("Shown", true);
        dialog_shown = true;
        dialogTitle.text = title;
        dialogText.text = text;
    }

    public void OK_Button()
    {
        dialog.SetBool("Shown", false);
        dialog_shown = false;
    }

    public void GoToUpdate()
    {
        UpdateOK_Button();
        Application.OpenURL("https://seenwalex.wixsite.com/site/tud-download?rtm=update");
    }

    public void NoticeOK_Button()
    {
        noticeDialog.SetBool("Shown", false);
        dialog_shown = false;
    }

    public void NoticeYes_Button()
    {
        UpdateOK_Button();
        AccountScript.LOAD_TO_SCENE = "Logon";
    }

    public void UpdateOK_Button()
    {
        updateDialog.SetBool("Shown", false);
        dialog_shown = false;
    }
}