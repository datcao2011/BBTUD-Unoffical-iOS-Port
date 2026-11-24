using UnityEngine;
using UnityEngine.UI;
using Platforms;
using System;
using System.Globalization;
using System.Collections;
using TMPro;


public class StarterScript : MonoBehaviour
{
    public Button mainMenuButton;

    public Button newGameMode;

    public Button playButton;

    public GameObject NULLBG;

    private bool chooseMode;

    private bool isInteractable;

    private bool goingToPlay;

    private bool introPlayed;

    private float animationTime;

    private AudioSource audioDevice;

    public AudioClip aud_loop;

    public AudioClip aud_open;

    public AudioClip aud_close;

    public AudioClip aud_buzz;

    public Animator animator;

    public Animator buttonAnim;

    public GameObject mainModes;

    public GameObject eventModes;

    public GameObject SHHModes;

    public GameObject SHHReal;

    [SerializeField]
    private GameObject SHHAbort;

    private bool elevatorPlayed;
    public SceneLoadAwake sceneLoadAwake;
    public Button multiplayerButton;
    public Button SaveSystem;
    public HoverText hoverText;
    //private string multiplayerDesc;

    private void DeleteKey(string key)
    {
        Settings.DeleteKey(key);
    }

    public void OpenSaves()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("SaveSystem", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        //ChooseStoryMode("Normal");
    }

    public void OnPressButton(Button button)
    {
        button.onClick.Invoke();
    }

    private static bool HasKnowledged = false;

    private void Start()
    {
        if (SaveSystem != null) this.SaveSystem.onClick.AddListener(OpenSaves);
        // if (GameObject.Find("STARTER_UNAVAILABLEMODES"))
        // {
        //     System.Collections.Generic.List<string> UnavailableModes = new System.Collections.Generic.List<string>();
        //     foreach (Transform transfor in GameObject.Find("STARTER_UNAVAILABLEMODES").transform)
        //     {
        //         UnavailableModes.Add(transfor.name);
        //     }
        //     foreach (string UnavailableMode in UnavailableModes.ToArray())
        //     {
        //         if (Settings.GetString("CurrentMode", "") == UnavailableMode)
        //         {
        //             chooseMode = true;
        //             break;
        //         }
        //     }
        // }

        //multiplayerButton.interactable = false;
        //hoverText.texts[5] = "Currently checking compatibility";

        if (Settings.HasKey("ModeSelected"))
        {
            sceneLoadAwake.sceneName = Settings.GetString("ModeSelected");
        }
        DeleteKey("HAS_Rounds");
        DeleteKey("HAS_MRounds");
        DeleteKey("HAS_BWins");
        DeleteKey("HAS_PWins");
        DeleteKey("HAS_KNH");
        DeleteKey("HAS_HOS");
        DeleteKey("Save");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainMenuButton.interactable = false;
        SaveSystem.interactable = false;
        SaveSystem.gameObject.SetActive(HasSaves());
        newGameMode.interactable = false;
        if (playButton != null) playButton.interactable = false;

        audioDevice = transform.parent.gameObject.AddComponent<AudioSource>();
        audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.AMusic : null;
        audioDevice.gameObject.AddComponent<AudioSourceSpeed>();
        animationTime = 1.2f;
        // DateTime eventDate = new DateTime(2020, 10, 31, 20, 0, 0); // UTC Time format (EST: 4:00 PM)
        // TimeSpan span = eventDate.Subtract(System.DateTime.UtcNow); // Subtract the UTC date.
        // if (span.TotalHours <= 1 & span.TotalMinutes > -10)
        // {
        //     chooseMode = true;
        //     mainModes.SetActive(false);
        //     eventModes.SetActive(true);
        // }
        // #if HAS_EVENT_PLUGGED
        //         CountDownTimer.e007 = Settings.GetInt("CC_SStage", 0);
        //         //CountDownTimer.e007 = 1;
        //         if (CountDownTimer.e007 != 0)
        //         {
        //             chooseMode = true;
        //             mainModes.SetActive(false);
        //             eventModes.SetActive(true);
        //             if (GameObject.Find("SavesButton"))
        //             {
        //                 GameObject.Find("SavesButton").SetActive(false);
        //             }
        //             CanvasGroup cg = GameObject.Find("CollectedItems").AddComponent<CanvasGroup>();
        //             cg.alpha = 0f;
        //             cg.interactable = false;
        //             cg.blocksRaycasts = false;
        //         }
        //         if (CountDownTimer.e007 != 2)
        //         {
        //             audioDevice.clip = aud_loop;
        //             audioDevice.loop = true;
        //             audioDevice.Play();
        //         }
        //         else
        //         {
        //             GameObject.Find("PLAY_BUTTON_TEXT").GetComponent<TMPro.TMP_Text>().text = "...";
        //             GameObject.Find("MainMenu").GetComponentInChildren<TMPro.TMP_Text>().text = "...";
        //             GameObject.Find("NewMode").GetComponentInChildren<TMPro.TMP_Text>().text = "...";
        //             _BaldiSpin.color = Color.black;
        //             _LoadingText.text = "...";
        //             Gradient grad = new Gradient();
        //             GradientColorKey[] colorKey;
        //             GradientAlphaKey[] alphaKey;
        //             colorKey = new GradientColorKey[2];
        //             colorKey[0].color = Color.black;
        //             colorKey[0].time = 0.0f;
        //             colorKey[1].color = Color.black;
        //             colorKey[1].time = 1.0f;

        //             // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        //             alphaKey = new GradientAlphaKey[2];
        //             alphaKey[0].alpha = 1.0f;
        //             alphaKey[0].time = 0.0f;
        //             alphaKey[1].alpha = 1.0f;
        //             alphaKey[1].time = 1.0f;
        //             grad.SetKeys(colorKey, alphaKey);
        //             FindObjectOfType<LoadingScreen>().backgroundColors = grad;
        //         }
        // #else
        if (Settings.GetInt("NULL_FirstTime") > 0)
        {
            if (Settings.GetString("ModeType") != "NULL")
            {
                chooseMode = true;
            }
            mainModes.SetActive(false);
            SHHModes.SetActive(true);
            if (NullScript.Attempts < 3)
            {
                CanvasGroup cg = slider.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
            if (Settings.GetBool("NULL_AbortAvailable") || NullScript.Attempts >= 5)
            {
                if (!HasKnowledged)
                {
                    chooseMode = true;
                    HasKnowledged = true;
                }
                Settings.SetBool("NULL_AbortAvailable", true);
                SHHModes.SetActive(false);
                SHHAbort.SetActive(true);
                AttemptsText.text = NullScript.Attempts + " attempts today";
            }
        }
        audioDevice.clip = aud_loop;
        audioDevice.loop = true;
        audioDevice.Play();
        // #endif
        if (!Settings.GetBool("ModeChoosen"))
        {
            chooseMode = true;
        }

        StartCoroutine(E01459());
        if (AchievementManager.Instance != null && AchievementManager.Instance.HasEarned((int)Achievements.undefined))
        {
            SHHReal.SetActive(true);
        }
    }

    private class E01460
    {
        public bool available;
        public string message;
    }

    [SerializeField]
    private TMP_Text AttemptsText;

    public void AbortNullStyle()
    {
        Settings.SetInt("NULL_FirstTime", 0);
        Settings.DeleteKey("NULL_AbortAvailable");
        InputManager.DisplayEvent("NULL Style has been aborted", Color.red, 5f);
    }

    private System.Collections.IEnumerator E01459()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            hoverText.texts[5] = "You are currently offline. Please check and connect to a WiFi or ethernet.";
            multiplayerButton.interactable = false;
            yield break;
        }
        // UNUSED IN OPEN SOURCE CODE

        //         UnityEngine.Networking.UnityWebRequest check = UnityEngine.Networking.UnityWebRequest.Get($"{InputManager.siteRequestUrl}multiplayerCompatible?v={VersionInput.currentVersion}&d={Platforms.CurrentDevice.currentPlatform.ToString().ToLower()}");
        //         check.SetRequestHeader("Accept", "text/plain");
        //         yield return check.SendWebRequest();

        //         if (check.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
        //         {
        //             if (check.error.Contains("SSL connection", StringComparison.InvariantCultureIgnoreCase))
        //             {
        //                 hoverText.texts[5] = "Your device may not support our servers. See bit.ly/SSLConnection for details.";
        //                 yield break;
        //             }
        //             hoverText.texts[5] = check.error;
        //             yield break;
        //         }

        //         if (check.responseCode == 404)
        //         {
        //             hoverText.texts[5] = "Whoa there, something's definitely wrong in our part. Please report this bug in the comments.";
        //             yield break;
        //         }

        //         if (check.responseCode != 200)
        //         {
        // #if DEBUG
        //             Debug.LogError(check.downloadHandler.text);
        // #endif
        //             hoverText.texts[5] = check.downloadHandler.text;
        //             yield break;
        //         }

        //         multiplayerButton.interactable = true;
        //         hoverText.texts[5] = check.downloadHandler.text;
        yield break;
    }

    public GameObject ModeSelectScreen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && introPlayed & chooseMode)
        {
            animator.SetBool("ScreenOpened", false);
            T_Hide();
            animationTime = 0.5f;
            chooseMode = false;
            modeSelected = true;
        }
        if (animationTime > 0f)
        {
            animationTime -= Time.deltaTime;
            if (!elevatorPlayed)
            {
                elevatorPlayed = true;
                audioDevice.PlayOneShot(aud_close, 1f);
                GameObject go = new GameObject("Audio Buzzer");
                ad = go.AddComponent<AudioSource>();
                ad.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
                go.AddComponent<AudioSourceSpeed>();
                ad.clip = aud_buzz;
#if UNITY_WEBGL
                Invoke("AudDelay", 0.9f);
#else
                ad.PlayDelayed(0.9f);
#endif
                Destroy(go, 0.9f + aud_buzz.length);
            }
        }
        else if (!introPlayed)
        {
            introPlayed = true;
            if (slider != null)
            {
                slider.gameObject.SetActive(true);
            }
            if (chooseMode)
            {
                ChooseMode();
            }
            else
            {
                SetInteractableButton(true);
            }
        }
        else if (modeSelected)
        {
            modeSelected = false;
            SetInteractableButton(true);
        }
    }

    private AudioSource ad;

    private void AudDelay()
    {
        ad.Play();
    }

    private bool modeSelected;

    public bool HasSaves()
    {
#if UNITY_WEBGL
        return false;
#else
        if (!System.IO.Directory.Exists(UnityEngine.Application.persistentDataPath + "/Saves"))
        {
            System.IO.Directory.CreateDirectory(UnityEngine.Application.persistentDataPath + "/Saves");
            return false;
        }
        return System.IO.Directory.GetFiles(Application.persistentDataPath + "/Saves").Length > 0;
#endif
    }

    public void ClearSavedSlots()
    {
        Settings.DeleteKey("ItemSaved0");
        Settings.DeleteKey("ItemSaved1");
        Settings.DeleteKey("ItemSaved2");
        Settings.DeleteKey("ItemSaved3");
        Settings.DeleteKey("ItemSaved4");
        Settings.DeleteKey("ItemSaved5");
    }

    public void ChooseMode()
    {
        chooseMode = true;
        Settings.DeleteKey("MazeRandom");
        Settings.DeleteKey("ModeSelected");
        Settings.SetBool("ModeChoosen", false);
        animator.SetBool("ScreenOpened", true);
        T_Show();
        SetInteractableButton(false);
    }

    public void T_Hide()
    {
        StartCoroutine(TT_Hide());
    }

    private System.Collections.IEnumerator TT_Hide()
    {
        float a = 1f;
        while (a > 0f)
        {
            a -= Time.deltaTime * 2.5f;
            if (a < 0f) a = 0f;
            ModeSelectScreen.GetComponent<CanvasGroup>().alpha = a;
            yield return null;
        }
        ModeSelectScreen.SetActive(false);
        yield break;
    }

    public void T_Show()
    {
        StartCoroutine(TT_Show());
    }

    private bool ShownFirst;

    private System.Collections.IEnumerator TT_Show()
    {
        yield return new WaitForSeconds(ShownFirst ? 1f : 1.75f);
        ShownFirst = true;
        ModeSelectScreen.SetActive(true);
        float a = 0f;
        while (a < 1f)
        {
            a += Time.deltaTime * 2.5f;
            if (a >= 1f) a = 1f;
            ModeSelectScreen.GetComponent<CanvasGroup>().alpha = a;
            yield return null;
        }
        yield break;
    }

    public Animator slider;

    public void SetInteractableButton(bool interactable)
    {
        mainMenuButton.interactable = interactable;
        SaveSystem.interactable = interactable;
        newGameMode.interactable = interactable;
        isInteractable = interactable;
        if (Settings.GetString("ModeSelected") == string.Empty & interactable) return;
        buttonAnim.SetBool("up", interactable);
    }

    public void Play()
    {
        if (!isInteractable)
        {
            return;
        }
        if (slider == null) slider = GameObject.Find("CollectedItems").GetComponent<Animator>();
        if (slider.GetComponent<RectTransform>().anchoredPosition.x >= 0)
        {
            slider.SetTrigger("s2");
        }
        else
        {
            slider.SetTrigger("s1");
        }
        buttonAnim.SetTrigger("push");
        audioDevice.PlayOneShot(aud_open, 1f);
        mainMenuButton.interactable = false;
        SaveSystem.interactable = false;
        newGameMode.interactable = false;
        isInteractable = false;
        animator.SetTrigger("start");
        if (Settings.GetString("ModeType") == "NULL")
        {
#if UNITY_ANDROID
            if (!Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
            {
                Settings.SetBool("Optimize", true);
                InputManager.DisplayEvent("We turned on Game Optimization to prevent crashing on this mode.", 5f);
            }
#endif
            if (Settings.GetInt("NULL_FirstTime") == 1)
            {
                Invoke("StartDelay", 0.7f);
            }
        }
    }

    private void StartDelay()
    {
        audioDevice.Stop();
        bool B = Settings.GetBool("NULL_Discovery") ? Settings.GetBool("SkipUpdatesAndGaps") : false;
        Settings.SetBool("NULL_Discovery", true);
#if !UNITY_WEBGL
        System.Threading.Thread.Sleep(B ? 1000 : UnityEngine.Random.Range(10000, 15000));
#endif
        Settings.SetInt("NULL_FirstTime", 2);
        animator.SetBool("instant", true);
        NULLBG.SetActive(true);
    }

    public void ChooseModeToPlay(string modeScene)
    {
        Settings.SetBool("ModeChoosen", true);
        Settings.SetString("ModeSelected", modeScene);
        sceneLoadAwake.sceneName = modeScene;
        animator.SetBool("ScreenOpened", false);
        T_Hide();
        animationTime = 0.5f;
        chooseMode = false;
        modeSelected = true;
    }

    public void SetRandom(bool value)
    {
        Settings.SetBool("MazeRandom", value);
    }

    public void SelectFarmingMode(string text)
    {
        Settings.SetString("FarmMode", text);
    }

    public void SelectSeed(TMPro.TMP_InputField field)
    {
        // No longer used due to mystman12 banned level generators.
        // string seed = field.text;
        // if (seed == string.Empty)
        // {
        //     seed = UnityEngine.Random.Range(-2147483648, 2147483647).ToString();
        // }
        // if (seed == "-")
        // {
        //     seed = UnityEngine.Random.Range(-2147483648, 2147483647).ToString();
        // }
        // int intseed = int.Parse(seed);
        // if (Convert.ToInt64(intseed) > 2147483615L)
        // {
        //     intseed = Convert.ToInt32(2147483615L);
        // }
        // else if (Convert.ToInt64(intseed) < -2147483615L)
        // {
        //     intseed = Convert.ToInt32(-2147483615L);
        // }
        // Settings.SetInt("MazeSeed", intseed);
    }

    public void ChooseEndlessModeToPlay(string Scene = "School")
    {
        Settings.SetBool("ModeChoosen", true);
        Settings.SetString("ModeSelected", Scene);
        sceneLoadAwake.sceneName = Scene;
        Settings.SetString("CurrentMode", "endless");
        Settings.SetString("ModeType", "Normal");
        animator.SetBool("ScreenOpened", false);
        T_Hide();
        animationTime = 0.5f;
        chooseMode = false;
        modeSelected = true;
    }

    public void ChooseEventMode()
    {
        // #if HAS_EVENT_PLUGGED
        //         Settings.SetBool("ModeChoosen", true);
        //         Settings.SetString("ModeSelected", InputManager.IsSchoolScene());
        //         sceneLoadAwake.sceneName = InputManager.IsSchoolScene();
        //         Settings.SetString("CurrentMode", "event");
        //         Settings.SetString("ModeType", "Normal");
        //         animator.SetBool("ScreenOpened", false);
        //         T_Hide();
        //         animationTime = 0.5f;
        //         chooseMode = false;
        //         modeSelected = true;
        // #endif
    }

    public void ChooseStoryMode(string type)
    {
        Settings.SetBool("ModeChoosen", true);
        Settings.SetString("ModeSelected", "School");
        sceneLoadAwake.sceneName = "School";
        if (type != "HAS")
        {
            Settings.SetString("CurrentMode", "story");
            Settings.SetString("ModeType", type);
        }
        else
        {
            Settings.SetString("CurrentMode", "hideAndSeekGame");
            Settings.SetString("ModeType", "Normal");
        }
        if (type == "Multiplayer")
        {
            sceneLoadAwake.sceneName = "MM_Connect";
            Settings.SetString("ModeSelected", "MM_Connect");
        }
        animator.SetBool("ScreenOpened", false);
        T_Hide();
        animationTime = 0.5f;
        chooseMode = false;
        modeSelected = true;
    }

    public void ChooseStoryModeNEW(string type)
    {
        Settings.SetBool("ModeChoosen", true);
        Settings.SetString("ModeSelected", "School_2");
        sceneLoadAwake.sceneName = "School_2";
        if (type != "HAS")
        {
            Settings.SetString("CurrentMode", "story");
            Settings.SetString("ModeType", type);
        }
        else
        {
            Settings.SetString("CurrentMode", "hideAndSeekGame");
            Settings.SetString("ModeType", "Normal");
        }
        animator.SetBool("ScreenOpened", false);
        T_Hide();
        animationTime = 0.5f;
        chooseMode = false;
        modeSelected = true;
    }
}
