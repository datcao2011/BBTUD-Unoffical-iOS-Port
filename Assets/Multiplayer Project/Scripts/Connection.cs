using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Multiplayer;
using UnityEngine.UI;
using SimpleJSON;
using System.Globalization;
using UnityEngine.Networking;
using Photon.Pun;
using System;
using System.Text.RegularExpressions;

public class Connection : MonoBehaviour
{
    public TMP_Text Status;
    public TMP_Text GameStatistics;
    public GameObject ContinuousSlider;
    public TMP_Text Percentage;
    public Slider normalSlider;
    public GameObject LoadingScreen;

    public float SliderUpdatedValue;
    public Slider LoadingSlider;
    public TMP_Text LoadingStatus;

    public bool SliderUpdateValue = true;

    private bool NicknameChangeActive;
    public TMP_InputField NicknameField;
    public Button NicknameSetButton;
    public TMP_Text NicknameText;
    public Animator NicknameDialog;
    AsyncOperation operation;

    public GameObject PublicPlay1;
    public GameObject PublicPlay2;
    public GameObject LoginRequired;

    public Animator CameraAnimation;
    public GameObject Buttons;
    public GameObject AvatarUI;

    public RectTransform buttonRect;

    public PlayerAvatarApplier Avatar;

    public bool PreloadLevels;

    private void Awake()
    {
        // Initialize Photon so the firewall dialog appears.
        PhotonHandler.InitializeHandler = true;
        PhotonNetwork.KeepAliveInBackground = PhotonNetwork.KeepAliveInBackground;
    }

    public void GoBack()
    {
        // Hide the dialog to make sure
        NicknameDialog.transform.localScale = new Vector3(0.017483f, 0.017483f, 0.017483f);
        NicknameDialog.GetComponent<CanvasGroup>().alpha = 0f;
        AvatarUI.SetActive(false);
        CameraAnimation.SetBool("t", false);
        StartCoroutine(CA_2());
    }

    public void GoToAvatar()
    {
        // Turn off nickname dialog if it's still visible
        if (NicknameChangeActive)
        {
            NicknameChangeActive = false;
            NicknameSetButton.interactable = false;
            NicknameDialog.SetBool("Shown", false);
        }
        Buttons.SetActive(false);
        CameraAnimation.SetBool("t", true);
        StartCoroutine(CA_1());
    }

    private IEnumerator CA_1()
    {
        yield return new WaitForSeconds(0.6f);
        AvatarUI.SetActive(true);
        yield break;
    }

    private IEnumerator CA_2()
    {
        yield return new WaitForSeconds(0.6f);
        Buttons.SetActive(true);
        yield break;
    }

    private void Start()
    {
        ServerRegion.SetValueWithoutNotify(PlayerPrefs.GetInt("Region", 1));
        PreloadLevels = Settings.GetBool("MP-PreloadLevels", true);
        StartCoroutine(E01459());
        NicknameField.onValueChanged.AddListener(OnNicknameCheck);
        Time.timeScale = 1f;
    }

    private void OnNicknameCheck(string Str)
    {
        bool Interact;
        if (Str.Length >= 3 && Str.Length <= 20)
        {
            Interact = true;
        }
        else
        {
            Interact = false;
        }
        if (System.Text.RegularExpressions.Regex.Match(Str, @"^[a-zA-Z0-9_]+( [a-zA-Z0-9_]+)*$").Length <= 0)
        {
            Interact = false;
        }
        NicknameSetButton.interactable = Interact;
    }

    private WordsFilter TheTextFilter;

    public void SetNickname()
    {
        if (!NicknameChangeActive) return;
        string Str = NicknameField.text;
        if (TheTextFilter == null) TheTextFilter = new WordsFilter();
        Str = TheTextFilter.Filter(Str).TrimEnd(); // This is due to the extra space at the end.
        NicknameField.text = Str;
        if (System.Text.RegularExpressions.Regex.Match(Str, @"^[a-zA-Z0-9_]+( [a-zA-Z0-9_]+)*$").Length <= 0)
        {
            return;
        }
        if (!(Str.Length >= 3 && Str.Length <= 20))
        {
            return;
        }
        Settings.SetString("MP_DefUser", Str);
        NicknameChangeActive = false;
        NicknameSetButton.interactable = false;
        NicknameDialog.SetBool("Shown", false);
        if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
        {
            PhotonNetwork.NickName = GetUsername();
        }
    }

    public void GoToAccountScreen()
    {
        AccountScript.LOAD_TO_SCENE = "MM_Connect";
        SceneManager.LoadScene("Logon");
    }

    public void ShowNicknameDialog()
    {
        NicknameChangeActive = true;
        NicknameSetButton.interactable = false;
        NicknameField.text = Settings.GetString("MP_DefUser");
        NicknameDialog.SetBool("Shown", true);
    }

    private void Update()
    {
        if (SliderUpdateValue)
        {
            float UpdateVal = Time.deltaTime * 1.25f;
            if (Mathf.Abs(SliderUpdatedValue - LoadingSlider.value) <= UpdateVal)
            {
                LoadingSlider.value = SliderUpdatedValue;
            }
            else if (LoadingSlider.value > SliderUpdatedValue)
            {
                LoadingSlider.value -= UpdateVal;
            }
            else if (LoadingSlider.value < SliderUpdatedValue)
            {
                LoadingSlider.value += UpdateVal;
            }
        }
        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom)
        {
            GameStatistics.text = $"== ==\n<color=#5DABFF>{PhotonNetwork.CountOfPlayers} {(PhotonNetwork.CountOfPlayers == 1 ? "Player" : "Players")} Online</color>\n<color=yellow>Running {PhotonNetwork.CountOfRooms} Games</color>";
            NicknameText.text = GetTColor() + PhotonNetwork.NickName;
            // AFK timer
            AFKTimer += Time.unscaledDeltaTime;
            if (AFKTimer >= 160f && !AFKWarning)
            {
                AFKWarning = true;
                msg = InputManager.DisplayEvent("Are you still there?", 20f, (EventMessage ms) => { ms.Disappear(); AFKWarning = false; AFKTimer = 0f; msg = null; }, "Yes");
            }
            if (AFKTimer >= 180f)
            {
                SceneManager.LoadScene("Starter");
            }
        }
        else if (msg != null)
        {
            msg.Disappear();
            msg = null;
            AFKTimer = 0f;
            AFKWarning = false;
        }
    }

    private void OnDisable()
    {
        if (msg != null)
        {
            msg.Disappear();
            msg = null;
        }
    }

    private EventMessage msg;

    [SerializeField]
    private float AFKTimer;
    private bool AFKWarning;

    private string GetTColor()
    {
        if (string.IsNullOrEmpty(AccountManager.getSessionId()))
        {
            return string.Empty;
        }
        string color = "";
        string UserTag = AccountManager.hasInstance() ? AccountManager.UserTag : "";
        if (string.IsNullOrEmpty(UserTag)) return string.Empty;
        switch (UserTag.ToLower())
        {
            case "creator":
                color = "#2fad42";
                break;
            case "helper":
                color = "#2f98ad";
                break;
            case "yt":
                color = "#FF4040";
                break;
            case "supporter":
                color = "yellow";
                break;
            case "dev":
            case "verified":
                color = "blue";
                break;
        }
        return string.IsNullOrEmpty(UserTag) ? string.Empty : ("<color=" + color + ">");
    }

    public static string region;

    public void HasConnected()
    {
        if (GameObject.Find("LOADING_TEXT") != null) GameObject.Find("LOADING_TEXT").GetComponent<TMPro.TMP_Text>().text = "Connection successful";
        LoadingStatus.text = "Connected to lobby!";
        SliderUpdatedValue = 1f;
        StartCoroutine(ConnectionAnimationDone());
    }

    private IEnumerator ConnectionAnimationDone()
    {
        while (LoadingSlider.value < 1f) yield return null;
        if (GameObject.Find("LoadingMultiplayer") != null) GameObject.Find("LoadingMultiplayer").SetActive(false);
        SliderUpdateValue = false;
        yield break;
    }

    public void ViewList(bool Cooperative)
    {
        StartCoroutine(CheckForServers(Cooperative));
    }

    public void SetBestRegion(string region)
    {
        if (!RegionTexts.ContainsKey(region)) return;
        ServerRegion.options[0].text = "Best (" + RegionTexts[region] + ")";
        ServerRegion.RefreshShownValue();
    }

    private IEnumerator PreloadSchoolScene()
    {
        PhotonNetwork.PreloadLevel("School");
        Status.text = "Pre-loading level assets";
        Percentage.text = "0%";
        ContinuousSlider.SetActive(false);
        normalSlider.value = 0f;
        float progress = 0f;
        while (progress < 1f)
        {
            progress = Mathf.Clamp01(PhotonNetwork.LevelPreLoadingProgress / .9f);
            normalSlider.value = progress;
            float progressText = progress * 100f;
            Percentage.text = Mathf.CeilToInt(progressText) + "%";
            yield return null;
        }
    }

    private IEnumerator CheckForServers(bool Cooperative)
    {
        Status.text = "Preparing for connection";
        Percentage.text = "...";
        ContinuousSlider.SetActive(true);
        if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(1f);
        if (PreloadLevels) yield return StartCoroutine(PreloadSchoolScene());

        Status.text = "Searching for players";
        Percentage.text = "...";
        ContinuousSlider.SetActive(true);
        Client client = MultiplayerManager.instance.client;
        client.StartPublicPlay(Cooperative);

        while (!client.Started)
        {
            yield return null;
        }
        Status.text = "Loading level assets";

        // 100% level loading Fix
        while (PhotonNetwork.LevelLoadingProgress == 1f && !PreloadLevels)
        {
            yield return null;
        }

        Percentage.text = "0%";
        ContinuousSlider.SetActive(false);
        normalSlider.value = 0f;
        float progress = 0f;
        while (progress < 1f && !PreloadLevels)
        {
            progress = Mathf.Clamp01(PhotonNetwork.LevelLoadingProgress / .9f);
            normalSlider.value = progress;
            float progressText = progress * 100f;
            Percentage.text = Mathf.CeilToInt(progressText) + "%";
            yield return null;
        }
        normalSlider.value = 1f;
        Percentage.text = "100%";

        Status.fontStyle = TMPro.FontStyles.Normal;
        Status.text = GetLoadingText();
        yield break;
    }

    public static void SetStatus(string text)
    {
        FindObjectOfType<Connection>().Status.text = text;
    }

    private class E01460
    {
        public bool available;
        public int MultiplayerAvailability = 1;
        public string message;
        public bool Delay;
        public bool CooperativeAvailable = false;
        public string Override = "";
    }

#if false // Modifying the code to work with the Open-Source project
/*
    private IEnumerator E01459()
    {
#if UNITY_EDITOR
        if (!AccountManager.hasInstance())
        {
            if (Settings.HasKey("Account"))
            {
                AccountScript.LOAD_TO_SCENE = "MM_Connect";
                SceneManager.LoadScene("Logon");
                InputManager.DisplayEvent("AccountManager instance wasn't created. Please wait while we fix the problem.", Color.red, 5f);
                yield break;
            }
            new GameObject("Account Manager").AddComponent<AccountManager>();
            yield return null;
        }
#else
        if (!AccountManager.hasInstance())
        {
            AccountScript.LOAD_TO_SCENE = "MM_Connect";
            SceneManager.LoadScene("Logon");
            InputManager.DisplayEvent("AccountManager instance wasn't created. Please wait while we fix the problem.", Color.red, 5f);
            yield break;
        }
#endif
        SliderUpdatedValue = 0.04f;
        if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(1f);
        SliderUpdatedValue = 0.1f;
#if UNITY_WEBGL
        LoadingStatus.text = "Checking from BBTUD Community server (Mobile:" + Application.isMobilePlatform + " Browser / V" + VersionInput.currentVersion + ")";
#else
        LoadingStatus.text = "Checking from BBTUD Community server (" + Platforms.CurrentDevice.currentPlatform + " / V" + VersionInput.currentVersion + ")";
#endif
        UnityWebRequest check = UnityWebRequest.Get($"{InputManager.siteRequestUrl}multiplayerCompatible?v={VersionInput.currentVersion}&d={Platforms.CurrentDevice.currentPlatform.ToString().ToLower()}");
        yield return check.SendWebRequest();

        if (check.result == UnityWebRequest.Result.ConnectionError)
        {
            SceneManager.LoadScene("Starter");
            yield break;
        }


        E01460 cls;
        try
        {
            cls = JsonUtility.FromJson<E01460>(check.downloadHandler.text);
            if (cls == null) throw new System.Exception("Non-type JSON response received: " + check.downloadHandler.text.Substring(0, 30));
        }
        catch (System.Exception ex)
        {
            cls = new E01460
            {
                available = false,
                message = ex.Message
            };
        }


        if (!cls.available)
        {
            SceneManager.LoadScene("Starter");
            InputManager.DisplayException(new System.UnauthorizedAccessException(cls.message), 15f);
            Debug.LogError(cls.message);
            yield break;
        }
        M_Availablity = cls.MultiplayerAvailability;
#if DEBUG
        switch (M_Availablity)
        {
            case 0:
                Debug.Log("Multiplayer Availablity: " + M_Availablity + " - Authentication Skipped");
                break;
            case 2:
                Debug.Log("Multiplayer Availablity: " + M_Availablity + " - Authentication Not Required For Private Play");
                break;
            case 3:
                Debug.Log("Multiplayer Availablity: " + M_Availablity + " - Authentication Not Required");
                break;
            default:
                Debug.Log("Multiplayer Availablity: " + M_Availablity + " - Authentication Required");
                break;
        }
#endif
        if (string.IsNullOrEmpty(AccountManager.getSessionId()) && M_Availablity == 1)
        {
            AccountScript.LOAD_TO_SCENE = "MM_Connect";
            SceneManager.LoadScene("Logon");
            InputManager.DisplayEvent("You must have a BBTUD Community Account to play multiplayer.", Color.red, 5f);
            yield break;
        }
        if (globalSettings.AllowCooperativePlay && GameObject.Find("CompetitiveText")) GameObject.Find("CompetitiveText").GetComponent<TMP_Text>().text = "COMPETITIVE (vs. Player)";
        PublicPlay1.SetActive(!(string.IsNullOrEmpty(AccountManager.getSessionId()) && M_Availablity == 2));
        PublicPlay2.SetActive(!(string.IsNullOrEmpty(AccountManager.getSessionId()) && M_Availablity == 2) && globalSettings.AllowCooperativePlay);
        LoginRequired.SetActive(string.IsNullOrEmpty(AccountManager.getSessionId()) && M_Availablity == 2);
        if (GameObject.Find("LOADING_TEXT") != null) GameObject.Find("LOADING_TEXT").GetComponent<TMP_Text>().text = "Checking for available connection";
        if (!Settings.HasKey("MP_DefUser") && string.IsNullOrEmpty(AccountManager.getSessionId()))
        {
            SliderUpdatedValue = 0.07f;
            LoadingStatus.text = "Waiting for player to input nickname";
            yield return WaitForNickname();
        }
        if (!string.IsNullOrEmpty(AccountManager.getSessionId()))
        {
            buttonRect.anchorMin = new Vector2(1f, 0f);
            buttonRect.anchorMax = new Vector2(1f, 0f);
            buttonRect.anchoredPosition = new Vector2(-221.8f, 20.55f);
        }
        if (cls.Delay)
        {
            yield return DelayConnection();
        }
        if (!string.IsNullOrEmpty(cls.Override))
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = cls.Override;
        }
        SliderUpdatedValue = 0.1f;
        LoadingStatus.text = "";
        MultiplayerManager.instance.client.ConnectToMaster(M_Availablity);
        yield break; 
    }
    */
#else
    [Serializable]
    private class GlobalSettings
    {
        [Header("General")]
        public AuthenticationNecessity GameAuthentication = AuthenticationNecessity.NotNeeded;
        public bool AllowCooperativePlay = true;
        [Header("Project Settings")]
        public string OverridePhotonProjectId = "";

        [Header("Connection Delay (I don't suggest using this at all)")]
        public bool DelayConnecting = false;
    }

    [SerializeField]
    private GlobalSettings globalSettings;

    private enum AuthenticationNecessity
    {
        SkipChecks,
        Required,
        RequiredForPublicPlay,
        NotNeeded,
    }

    private IEnumerator E01459() // Modified version for the Open Source Code
    {
#if UNITY_EDITOR
        if (!AccountManager.hasInstance())
        {
            if (Settings.HasKey("Account"))
            {
                AccountScript.LOAD_TO_SCENE = "MM_Connect";
                SceneManager.LoadScene("Logon");
                InputManager.DisplayEvent("AccountManager instance wasn't created. Please wait while we fix the problem.", Color.red, 5f);
                yield break;
            }
            new GameObject("Account Manager").AddComponent<AccountManager>();
            yield return null;
        }
#else
        if (!AccountManager.hasInstance())
        {
            AccountScript.LOAD_TO_SCENE = "MM_Connect";
            SceneManager.LoadScene("Logon");
            InputManager.DisplayEvent("AccountManager instance wasn't created. Please wait while we fix the problem.", Color.red, 5f);
            yield break;
        }
#endif
        SliderUpdatedValue = 0.04f;
        if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(1f);
        SliderUpdatedValue = 0.1f;

        M_Availablity = (int)globalSettings.GameAuthentication;
#if DEBUG
        switch (M_Availablity)
        {
            case 0:
                Debug.Log("Multiplayer Availablity: " + M_Availablity + " - Authentication Skipped");
                break;
            case 2:
                Debug.Log("Multiplayer Availablity: " + M_Availablity + " - Authentication Not Required For Private Play");
                break;
            case 3:
                Debug.Log("Multiplayer Availablity: " + M_Availablity + " - Authentication Not Required");
                break;
            default:
                Debug.Log("Multiplayer Availablity: " + M_Availablity + " - Authentication Required");
                break;
        }
#endif
        if (string.IsNullOrEmpty(AccountManager.getSessionId()) && M_Availablity == 1)
        {
            AccountScript.LOAD_TO_SCENE = "MM_Connect";
            SceneManager.LoadScene("Logon");
            InputManager.DisplayEvent("You must have a BBTUD Community Account to play multiplayer.", Color.red, 5f);
            yield break;
        }
        if (globalSettings.AllowCooperativePlay && GameObject.Find("CompetitiveText")) GameObject.Find("CompetitiveText").GetComponent<TMP_Text>().text = "COMPETITIVE (vs. Player)";
        PublicPlay1.SetActive(!(string.IsNullOrEmpty(AccountManager.getSessionId()) && M_Availablity == 2));
        PublicPlay2.SetActive(!(string.IsNullOrEmpty(AccountManager.getSessionId()) && M_Availablity == 2) && globalSettings.AllowCooperativePlay);
        LoginRequired.SetActive(string.IsNullOrEmpty(AccountManager.getSessionId()) && M_Availablity == 2);
        if (GameObject.Find("LOADING_TEXT") != null) GameObject.Find("LOADING_TEXT").GetComponent<TMP_Text>().text = "Checking for available connection";
        if (!Settings.HasKey("MP_DefUser") && string.IsNullOrEmpty(AccountManager.getSessionId()))
        {
            SliderUpdatedValue = 0.07f;
            LoadingStatus.text = "Waiting for player to input nickname";
            yield return WaitForNickname();
        }
        if (!string.IsNullOrEmpty(AccountManager.getSessionId()))
        {
            buttonRect.anchorMin = new Vector2(1f, 0f);
            buttonRect.anchorMax = new Vector2(1f, 0f);
            buttonRect.anchoredPosition = new Vector2(-221.8f, 20.55f);
        }
        if (globalSettings.DelayConnecting)
        {
            yield return DelayConnection();
        }
        if (!string.IsNullOrEmpty(globalSettings.OverridePhotonProjectId))
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = globalSettings.OverridePhotonProjectId;
        }
#if UNITY_EDITOR
        if (PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime == "c56b67f3-80dc-48d4-94fc-35d77b747fd7")
        {
            InputManager.DisplayEvent("It's recommend to create your own Photon Project and use that App ID instead. This warning only appears in the editor.", Color.yellow, 10f);
        }
#endif
        SliderUpdatedValue = 0.1f;
        LoadingStatus.text = "";
        MultiplayerManager.instance.client.ConnectToMaster(M_Availablity);
        yield break;
    }
#endif

    private bool HasAlreadyUpdated;

    public void UpdateTexts(int Default, int Cooperative)
    {
#if DEBUG
        Debug.Log("Competitive:" + Default + " Cooperative:" + Cooperative);
#endif
        if (Default <= 0 && Cooperative <= 0 && !HasAlreadyUpdated) return;
        HasAlreadyUpdated = true;
        if (PublicPlay2.activeSelf)
        {
            PublicPlay1.GetComponentInChildren<TMP_Text>().text = $"COMPETITIVE (vs. Player) [{Default}]";
            PublicPlay2.GetComponentInChildren<TMP_Text>().text = $"COOPERATIVE (vs. AI) [{Cooperative}]";
        }
        else
        {
            string a = (Default == 1) ? "PLAYER" : "PLAYERS";
            PublicPlay1.GetComponentInChildren<TMP_Text>().text = $"PLAY NOW WITH {Default} {a}";
        }
    }

    private double Duration = double.PositiveInfinity;

    private IEnumerator DelayConnection()
    {
        if (GameObject.Find("LOADING_TEXT") != null) GameObject.Find("LOADING_TEXT").GetComponent<TMP_Text>().text = "Please wait";
        string Tag = string.IsNullOrEmpty(AccountManager.getSessionId()) ? "Normal" : AccountManager.UserTag;
        if (!string.IsNullOrEmpty(AccountManager.getSessionId()) && string.IsNullOrEmpty(Tag))
        {
            Tag = "Member";
        }
        LoadingStatus.text = "We will connect you soon, stay in this screen";
        UnityWebRequest req = UnityWebRequest.Get(InputManager.siteRequestUrl + "MPDelayDuration?t=" + Tag);
        yield return req.SendWebRequest();
        try
        {
            if (!string.IsNullOrEmpty(req.error))
            {
                throw new Exception(req.error);
            }
            Duration = (float)JSON.Parse(req.downloadHandler.text)["Duration"].AsDouble;
        }
        catch (Exception)
        {
            SceneManager.LoadScene("Starter");
            InputManager.DisplayEvent("MP Servers are currently unavailable as of now. Try again later!", Color.red, 5f);
            throw;
        }
        float MaxValue = (float)Duration;
        while (Duration > 0f)
        {
            Duration -= Time.unscaledDeltaTime;
            int minutes = Mathf.FloorToInt((float)Duration / 60F);
            int seconds = Mathf.FloorToInt((float)Duration - minutes * 60);

            string Timer = string.Format("{0:0}:{1:00}", minutes, seconds);
            SliderUpdatedValue = 1f - Mathf.InverseLerp(0f, MaxValue, (float)Duration);

            if (GameObject.Find("LOADING_TEXT") != null) GameObject.Find("LOADING_TEXT").GetComponent<TMP_Text>().text = "Connecting in " + Timer;
            LoadingStatus.text = $"Priority({Tag}) - To limit traffic, we'll connect you in {Mathf.FloorToInt((float)Duration)}s";
            yield return null;
        }
        if (GameObject.Find("LOADING_TEXT") != null) GameObject.Find("LOADING_TEXT").GetComponent<TMP_Text>().text = "Checking for available connection";
        LoadingStatus.text = "Ready for connection";
        yield break;
    }

    private IEnumerator WaitForNickname()
    {
        ShowNicknameDialog();
        while (NicknameChangeActive)
            yield return null;
        yield break;
    }

    public string GetUsername(bool WithHyphen = true)
    {
        if (!string.IsNullOrEmpty(AccountManager.getSessionId()))
        {
            if (GameObject.Find("Rename Button"))
            {
                GameObject.Find("Rename Button").SetActive(false);
            }
            return AccountManager.getUsername();
        }
        return (WithHyphen ? "<color=yellow>~</color>" : "") + Settings.GetString("MP_DefUser");
    }

    public int M_Availablity;

    public void Connect()
    {
        if (NicknameChangeActive)
        {
            NicknameChangeActive = false;
            NicknameSetButton.interactable = false;
            NicknameDialog.SetBool("Shown", false);
        }
        LoadingScreen.SetActive(true);
        reconnectAttempts = 0;
        StartCoroutine(ServerConnection());
    }

    public void Create()
    {
        if (NicknameChangeActive)
        {
            NicknameChangeActive = false;
            NicknameSetButton.interactable = false;
            NicknameDialog.SetBool("Shown", false);
        }
        LoadingScreen.SetActive(true);
        reconnectAttempts = 0;
        StartCoroutine(PrivateConnection());
    }

    public void Cancel()
    {
        if (MultiplayerManager.instance.client.Started) return;
        StopAllCoroutines();
        // if (MultiplayerManager.instance.client.isConnected)
        // {
        //     MultiplayerManager.instance.client.Stop();
        // }
        LoadingScreen.SetActive(false);
        MultiplayerManager.instance.client.CancelConnection();
    }

    public void DisableScreen()
    {
        StopAllCoroutines();
        // if (MultiplayerManager.instance.client.isConnected)
        // {
        //     MultiplayerManager.instance.client.Stop();
        // }
        LoadingScreen.SetActive(false);
    }

    public void ActivateScene()
    {
        //Client.DisconnectAction = Client.DefaultDisconnectAction;
        operation.allowSceneActivation = true;
    }

    public static int reconnectAttempts = 0;
    public TMP_InputField code;
    public TMP_Dropdown ServerRegion;

    public readonly static string[] Regions = new string[] {
        "",
        "us",
        "usw",
        "eu",
        "asia",
        "sa",
        "za",
        "in",
        "au",
        "cae",
        "ru",
        "rue",
        "jp",
        "tr"
    };

    public readonly static Dictionary<string, string> RegionTexts = new Dictionary<string, string>() {
        {"", "Unknown"},
        {"us", "USA East"},
        {"usw", "USA West"},
        {"ussc", "USA South Central"},
        {"eu", "Europe"},
        {"asia", "Asia"},
        {"sa", "South America" },
        {"za", "South Africa"},
        {"in", "India"},
        {"au", "Australia"},
        {"cae", "Canada"},
        {"ru", "Russia"},
        {"rue", "Russia East"},
        {"jp", "Japan"},
        {"tr", "Turkey"}
    };

    public void ChangeRegionVal(int value)
    {
        PlayerPrefs.SetInt("Region", value);
        if (PhotonNetwork.IsConnected) InputManager.DisplayEvent("Connecting to another region requires you to reload.", 10f, (eventmsg) => { eventmsg.Disappear(); AccountScript.LOAD_TO_SCENE = "MM_Connect"; SceneManager.LoadScene(string.IsNullOrEmpty(AccountManager.getSessionId()) ? "Starter" : "Logon"); }, "Reload");
    }

    private IEnumerator PrivateConnection()
    {
        Status.text = "Preparing to connect";
        Percentage.text = "...";
        ContinuousSlider.SetActive(true);
        if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(1f);
        if (PreloadLevels) yield return StartCoroutine(PreloadSchoolScene());

        Status.text = "Creating private room";
        Percentage.text = "...";
        ContinuousSlider.SetActive(true);

        Client client = MultiplayerManager.instance.client;
        client.CreateRoom();
        while (!client.Started)
        {
            yield return null;
        }
        Status.text = "Loading level assets";

        // 100% level loading Fix
        while (PhotonNetwork.LevelLoadingProgress == 1f && !PreloadLevels)
        {
            yield return null;
        }

        Percentage.text = "0%";
        ContinuousSlider.SetActive(false);
        normalSlider.value = 0f;
        float progress = 0f;
        while (progress < 1f && !PreloadLevels)
        {
            progress = Mathf.Clamp01(Photon.Pun.PhotonNetwork.LevelLoadingProgress / .9f);
            normalSlider.value = progress;
            float progressText = progress * 100f;
            Percentage.text = Mathf.CeilToInt(progressText) + "%";
            yield return null;
        }
        normalSlider.value = 1f;
        Percentage.text = "100%";

        Status.fontStyle = TMPro.FontStyles.Normal;
        Status.text = GetLoadingText();
        yield break;
    }

    private IEnumerator ServerConnection()
    {
        Status.text = "Preparing to connect";
        Percentage.text = "...";
        ContinuousSlider.SetActive(true);
        if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(1f);
        if (code.text == string.Empty)
        {
            DisableScreen();
            yield break;
        }
        if (PreloadLevels) yield return StartCoroutine(PreloadSchoolScene());

        Status.text = "Searching for the room";
        Percentage.text = "...";
        ContinuousSlider.SetActive(true);

        Client client = MultiplayerManager.instance.client;
        client.JoinRoom(code.text);
        while (!client.Started)
        {
            yield return null;
        }
        Status.text = "Loading level assets";

        // 100% level loading Fix
        while (PhotonNetwork.LevelLoadingProgress == 1f && !PreloadLevels)
        {
            yield return null;
        }

        Percentage.text = "0%";
        ContinuousSlider.SetActive(false);
        normalSlider.value = 0f;
        float progress = 0f;
        while (progress < 1f && !PreloadLevels)
        {
            progress = Mathf.Clamp01(PhotonNetwork.LevelLoadingProgress / .9f);
            normalSlider.value = progress;
            float progressText = progress * 100f;
            Percentage.text = Mathf.CeilToInt(progressText) + "%";
            yield return null;
        }
        normalSlider.value = 1f;
        Percentage.text = "100%";

        Status.fontStyle = TMPro.FontStyles.Normal;
        Status.text = (SceneLoader.GetRandomText());
        while (MultiplayerManager.instance.PlayerList[0] == null || string.IsNullOrEmpty(MultiplayerManager.instance.PlayerList[0].username))
            yield return null;
        Status.text = GetLoadingText();
        yield break;
    }

    private string GetLoadingText()
    {
        bool IsCooperative = PhotonNetwork.CurrentRoom.IsVisible ? (MultiplayerManager.instance.client.GetLobby().Name == "Cooperative") : (
            PhotonNetwork.IsMasterClient ? (FindObjectOfType<LobbyPlayerListing>().Settings.PickedBaldi == -1) : (MultiplayerManager.instance.CurrentSettings.PickedBaldi == -1)
        );
        return IsCooperative ? "<color=#00ffff>COOPERATIVE</color> <color=yellow>MODE</color>" : "<color=red>COMPETITIVE</color> <color=yellow>MODE</color>";
    }
}
