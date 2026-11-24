using System.Collections;
using System.IO;
using UnityEngine;
using Platforms;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SchoolSaves;
using Photon.Pun;
using Multiplayer;

public class GameControllerScript : MonoBehaviour
{
    public GameObject map;

    public SystemTimeScript systemTime;

    public GameObject fifthPrize;

    public Gradient colored;

    public DangerousTeleporter teleporter;

    public PlayerScript player;

    public CameraScript cameraScript;

    public Transform playerTransform;
    //[HideInInspector]
    public float time;

    public float TestDuration;

    public Transform baldiTransform;

    public GameObject playerObject;

    public GameObject notebook;

    public GameObject clickCursorObject;

    public GameObject characterTime;

    public TMP_Text characterTimeText;

    public GameObject exitsR;

    private bool didIt;

    public TMP_Text exitCount;

    public GameObject baldiScoopModeScreen;

    public TMP_Text exitText;

    public TMP_Text fourtyTwoText;

    public TMP_Text noClipText;

    public GameObject cheatConsole;

    public GameObject crazyText;

    public Transform cameraTransform;

    public EntranceScript entrance_0;

    public EntranceScript entrance_1;

    public EntranceScript entrance_2;

    public EntranceScript entrance_3;

    public GameObject baldiTutor;

    public GameObject baldi;

    public BaldiScript baldiScrpt;

    public AudioClip aud_Prize;

    public AudioClip aud_NewPrize;

    public AudioClip aud_RON;

    public Color redColor;

    public AudioClip aud_AllNotebooks;

    public GameObject principal;

    public GameObject crafters;

    public GameObject playtime;

    public CheatScript cheatingConsole;

    public PlaytimeScript playtimeScript;

    public MathGameScript mathgame;

    public GameObject gottaSweep;

    public GameObject bully;

    public GameObject firstPrize;

    private bool played;

    public NotebookScript[] notebooksActive;

    public FirstPrizeScript firstPrizeScript;

    public DrReflex DoctorReflex;

    public Test LookAtGuy;

    public GameObject quarter;

    public AudioSource tutorBaldi;

    public string mode;

    public int notebooks;

    public int failedNotebooks;

    public int maxNotebooks = 7;

    public bool spoopMode;

    public bool finaleMode;

    public bool debugMode;

    public bool infiniteItems;

    public bool mouseLocked;

    public int exitsReached;

    public int itemSelected;

    public int[] item = new int[6];

    public RawImage[] itemSlot = new RawImage[6];

    private string[] itemNames = new string[26]
    {
        "Nothing",
        "Energy flavored Zesty Bar",
        "Swinging Door Lock",
        "Principal's Keys",
        "BSODA",
        "Quarter",
        "Baldi Anti Hearing and Disorienting Tape",
        "Alarm Clock",
        "WD-NoSquee (Door Type)",
        "Safety Scissors",
        "Big Ol' Boots",
        "An Apple for Baldi",
        "Legendary Apple",
        "Teleportation Teleporter",
        "Swinging Door Sale",
        "Pencil",
        "Emergency Health Kit",
        "Principal's Whistle",
        "Class Pass",
        "Secret Formula",
        "Grappling Hook",
        "Portal Poster",
        "Faculty Nametag",
        "Dirty Chalk Eraser",
        "Reflex Hammer",
        "'Nana Peel"
    };

    public TMP_Text itemText;

    public Texture[] itemTextures = new Texture[26];

    public Sprite[] itemSprites = new Sprite[26];

    public GameObject bsodaSpray;

    public GameObject alarmClock;

    public GameObject bananaPeel;

    public TMP_Text notebookCount;

    public TMP_Text[] enableTexts;

    public GameObject RestartLoad;

    public GameObject pauseText;

    public GameObject highScoreText;

    public GameObject[] itemsActive;

    public TMP_Text warning;

    public GameObject reticle;

    public RectTransform itemSelect;

    public AudioQueueScript AudMan;

    public AudioClip aud_prizeMouse;

    private int[] itemSelectOffset = new int[6]
    {
        -202,
        -164,
        -126,
        -92,
        -54,
        -17
    };

    public bool gamePaused;

    public bool cheatConsoleActive;

    public bool learningActive;

    // Removed Golden Baldi as of 5/20/2021 due to request by xTPSx
    // public GameObject goldenBaldi;

    private float gameOverDelay = 1.5f;

    [HideInInspector]
    public AudioSource audioDevice;

    private AudioSource ExitNoise;
    public void StopNoise()
    {
        if (ExitNoise != null)
        {
            CanNotPlayFinaleSounds = true;
            ExitNoise.Stop();
            Destroy(ExitNoise);
        }
    }

    public AudioClip aud_Soda;

    public AudioClip aud_Spray;

    public AudioClip aud_buzz;
    public AudioClip[] aud_buzzes;

    public AudioClip aud_Hang;

    public AudioClip aud_MachineQuiet;

    public AudioClip aud_MachineStart;

    public AudioClip aud_MachineRev;

    public AudioClip aud_MachineLoop;

    public AudioClip aud_Switch;

    public AudioSource schoolMusic;

    public AudioSource learnMusic;

    public AudioClip baldiHAS;
    public AudioClip baldiYGLKP;
    public AudioClip baldiWFLKP;
    public AudioClip baldiIGH;
    public AudioClip baldiIGS;
    public AudioClip baldiCountdown;

    public AudioClip CoinDrop;

    public AudioClip Crunch;

    public GameObject hud;

    public GameObject networkConsole;

    public GameObject mrsPomp;

    public GameObject beans;

    public GameObject badEnding;
    public GameObject goodEnding;
    public GameObject challengeEnding;
    public GameObject cheatedEnding;
    public GameObject gEnding;
    public GameObject bEnding;

    public HideAndSeekGame HASManager;

    public AudioClip[] kickstarterMusic;

    private string enableHud;

    public string scene;

    [HideInInspector]
    public string HudVersion = "1";

    private bool done1;

    private PlatPlayer inputPlayer;

    private AudioClip LoserBuzz;

    public AudioClip HammerSound_TempVariable;

    [HideInInspector]
    public CaptionsManager CM;

    #region Dr. Reflex Hammer Detector
    public int CurrentHammerSlot = -1;
    #endregion

    public LayerMask CharacterMask;

    public Material WhiteWall;

    [SerializeField]
    private GameObject MobileControls;

    public BaldiTV TV;

    //public static int CurrentMPBananaID;

    private bool HadYCTP;

    [HideInInspector]
    public bool HasUsedCollectedItems = false;

    private void Start()
    {
        try
        {
            LoserBuzz = GetRandomBuzz();
            inputPlayer = PlatTools.GetPlatformPlayer();
            HudVersion = (Settings.GetInt("TypeHud", 1) + 1).ToString();
            CM = FindObjectOfType<CaptionsManager>();
            if (Captions == null && CM != null)
            {
                Captions = CM.gameObject;
            }
            if (Captions != null) Captions.SetActive(Settings.GetBool("Captions"));
            if (HudVersion == "2")
            {
                TransformSlider(3);
                TransformItemSlotsV2();
            }
            GameObject obj = GameObject.Find("Characters");
            mode = Settings.GetString("CurrentMode");
            if (!Settings.HasKey("TimeSpeed"))
            {
                Settings.SetFloat("TimeSpeed", 1f);
            }
            audioDevice = base.GetComponent<AudioSource>();
            if (MultiplayerManager.isActive)
            {
                mode = "story";
                MultiplayerManager.instance.Activate();
                itemNames[13] = "Dangerous Teleporter";
            }
            gameObject.AddComponent<InteractScript>();
            LockMouse();
            SaveSystemScript.gameController = this;
            warning.text = "IT\"s LOAD";

            enableHud = Settings.GetString("Hud", "true");
            StaminaPercentage = Settings.GetString("SPHud", "true") == "true";
            if (!StaminaPercentage)
            {
                warning.gameObject.SetActive(false);
            }
            Settings.OnSettingChanged += OnValChange;
            scene = SceneManager.GetActiveScene().name;
            if (mode == "NonSchool" & scene == InputManager.IsSchoolScene() || mode == "" & scene == InputManager.IsSchoolScene())
            {
                Settings.SetString("CurrentMode", "story");
                mode = "story";
            }
            if (mode == "endless")
            {
                baldiScrpt.endless = true;
                HadYCTP = !Settings.GetBool("NoYCTP");
            }
            Settings.SetString("FourtyTwoMode", "false");
            itemSelected = 0;
            if (MultiplayerManager.isActive)
            {
                mode = "story";
                PhotonNetwork.AutomaticallySyncScene = false;
                if (MultiplayerManager.instance.CurrentSettings.SecondFloor) entrance_2.OpenElevate();
                return;
            }
            Destroy(MultiplayerEvents, 0f);
            if (scene == InputManager.IsSchoolScene())
            {
                if (FindObjectOfType<CheatScript>()) FindObjectOfType<CheatScript>().SetOptions(itemNames, itemSprites);
                // Hide and Seek Game Activation
                if (Settings.GetString("ModeType") == "NULL")
                {
                    itemNames[13] = "Dangerous Teleporter";
                }
                if (mode == "hideAndSeekGame")
                {
                    GameObject.Find("NoYCTPNotebook").SetActive(false);
                    foreach (NotebookScript activation in FindObjectsOfType<NotebookScript>())
                    {
                        activation.gameObject.SetActive(false);
                    }
                    if (enableHud == "true")
                    {
                        hud.SetActive(true);
                    }
                    else if (enableHud == "false")
                    {
                        hud.SetActive(false);
                    }
                    entrance_2.OpenElevate();
                    chances = Random.Range(-1f, 3f);
                    player.SetPosition(new Vector3(5f, 4f, 0f));
                    KickstarterSoundtrack kickstarterSoundTrack = new GameObject("Kickstarter Soundtrack").AddComponent<KickstarterSoundtrack>();
                    kickstarterSoundTrack.music = new AudioClip[kickstarterMusic.Length];
                    kickstarterSoundTrack.music = kickstarterMusic;
                    kickstarterSoundTrack.StartMusic();
                    notebooks = 2;
                    notebookCount.gameObject.SetActive(false);
                    HASManager = new GameObject("Hide and Seek Manager").AddComponent<HideAndSeekGame>();
                    HASManager.gc = this;
                    HASManager.baldiHAS = baldiHAS;
                    HASManager.baldiYGLKP = baldiYGLKP;
                    HASManager.baldiWFLKP = baldiWFLKP;
                    HASManager.baldiIGH = baldiIGH;
                    HASManager.baldiIGS = baldiIGS;
                    HASManager.baldiCountdown = baldiCountdown;
                    HASManager.StartGame();
                    FindObjectOfType<RandomEvent>().PauseRandomEvents();
                    crafters.SetActive(false);
                    if (CollectedItems.itemToUse > 0)
                    {
                        HasUsedCollectedItems = true;
                        for (int i = 0; i < CollectedItems.itemToUse; i++)
                        {
                            CollectedItems.CollectedItemToBeUsed item = CollectedItems.itemIdsToUse[i];
                            if (item.id != 0)
                            {
                                GetItem(i, item.id);
                                MysteryScript.RemoveItemCollected(item.id);
                            }
                        }
                        CollectedItems.itemToUse = 0;
                    }
                    // gameObject.AddComponent<DisableSaveSystem>();
                    // if (InputManager.usingTouch && GameObject.Find("SaveButton")) GameObject.Find("SaveButton").SetActive(false);
                    return;
                }


                notebooksActive = FindObjectsOfType<NotebookScript>();
                for (int i = 0; i < itemsActive.Length; i++)
                {
                    itemsActive[i] = FindObjectsOfType<PickupScript>()[i].gameObject;
                }

                if (Settings.HasKey("Save"))
                {
                    LoadPlayer(Settings.GetString("Save"));
                    CheatScript.cheated = true;
                    Settings.DeleteKey("Save");
                }
                else if (CollectedItems.itemToUse > 0)
                {
                    HasUsedCollectedItems = true;
                    for (int i = 0; i < CollectedItems.itemToUse; i++)
                    {
                        CollectedItems.CollectedItemToBeUsed item = CollectedItems.itemIdsToUse[i];
                        if (item.id != 0)
                        {
                            GetItem(i, item.id);
                            MysteryScript.RemoveItemCollected(item.id);
                        }
                    }
                    CollectedItems.itemToUse = 0;
                }
                if (!spoopMode)
                {
                    schoolMusic.Play();
                    UpdateNotebookCount();
                    entrance_1.OpenElevate();
                    entrance_3.OpenElevate();
                }
                if (Settings.GetString("ModeType", "Normal") != "BB") entrance_2.OpenElevate();
                if (Settings.GetString("ModeType", "Normal") == "Stealthy")
                {
                    int itemSlots = 5;
                    for (int i = 0; i < itemSlots; i++)
                    {
                        if (item[i] == 0)
                        {
                            GetItem(i, 19);
                        }
                    }
                    if (item[itemSlots] == 0)
                    {
                        GetItem(itemSlots, 23);
                    }
                }
            }
            if ((!MultiplayerManager.isActive || !MultiplayerManager.instance.isBaldi) && hud != null)
            {
                if (enableHud == "true")
                {
                    hud.SetActive(true);
                }
                else if (enableHud == "false")
                {
                    hud.SetActive(false);
                }
            }
            chances = Random.Range(-1f, 3f);
        }
        catch (System.Exception exception)
        {
            if (SceneManager.GetActiveScene().name == InputManager.IsSchoolScene())
            {
                GameSystem.AbortApplication("An error occurred while loading (" + exception.Message + ")", "SCHOOL_FAILED_TO_LOAD");
                throw;
            }
            else
            {
#if DEBUG
                Debug.LogException(exception);
#endif
            }
        }
    }

    private bool StaminaPercentage;

    public AudioClip GetRandomBuzz(bool IsYCTP = false)
    {
        if (aud_buzzes == null || aud_buzzes.Length <= 0)
        {
            return aud_buzz;
        }
        if (IsYCTP)
        {
            return aud_buzzes[Mathf.RoundToInt(Random.Range(0f, 1f))];
        }
        return aud_buzzes[Mathf.RoundToInt(Random.Range(0f, aud_buzzes.Length - 1f))];
    }

    public GameObject Captions;

    private void OnValChange(string name, object val)
    {
        if (name == "Vibrate")
        {
            if (baldiScrpt != null)
            {
                baldiScrpt.rumble = (bool)val;
            }
        }
        if (name == "Captions")
        {
            if (Captions != null) Captions.SetActive((bool)val);
            return;
        }
        if (name != "Hud" && name != "SPHud") return;
        if (name == "SPHud")
        {
            StaminaPercentage = (string)val == "true";
            warning.gameObject.SetActive(StaminaPercentage);
            return;
        }
        enableHud = (string)val;
        if (enableHud == "true")
        {
            hud.SetActive(true);
        }
        else if (enableHud == "false")
        {
            hud.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Settings.OnSettingChanged -= OnValChange;
    }

    public IEnumerator hideCheatMenu(string reason)
    {
        StartCoroutine(player.CheatingOff(reason));
        yield break;
    }

    private IEnumerator QueuePositionAndRotation(Vector3 Pos, Quaternion Rot)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        player.SetPosition(Pos);
        player.SetRotation(Rot);
        yield break;
    }

    public void LoadPlayer(string name)
    {
        SchoolSaves.PlayerData data = SaveSystem.LoadPlayer(name);

        item = data.Items;
        itemSelected = data.ItemCurrentlySelected;
        if (data.Param_ModeType == "Normal" || data.Param_ModeType == "Grapple")
        {
            player.SetPosition(new Vector3(data.Position[0], data.Position[1], data.Position[2]));
            player.SetRotation(Quaternion.Euler(new Vector3(data.Rotation[0], data.Rotation[1], data.Rotation[2])));
        }
        else StartCoroutine(QueuePositionAndRotation(new Vector3(data.Position[0], data.Position[1], data.Position[2]), Quaternion.Euler(new Vector3(data.Rotation[0], data.Rotation[1], data.Rotation[2]))));

        SaveXML XML;
        if (!string.IsNullOrEmpty(data.SaveBIN))
        {
            XML = Utils.GetXMLContent(Utils.Base64Decode(data.SaveBIN));
        }
        else
        {
            // Using old data
            Debug.LogWarning("[Save System] OLD PLAIN TEXT DATA DETECTED! NEXT TIME THE PLAYER SAVES, IT'LL CONVERT TO BINARY.");
            XML = Utils.GetXMLContent(data.SaveXML);
        }
        if (Settings.GetString("ModeType") != "NULL")
        {
            foreach (Character character in XML.Characters)
            {
                GameObject chara = Utils.FindObjectWithName(character.GB_Name);
                if (character.Active != null) chara.SetActive((bool)character.Active);
                SetWarp(chara.transform, character.Position);
            }
            if (data.BaldiIsAngry)
            {
                ActivateSpoopMode();
            }
        }

        notebooks = data.Notebooks;
        maxNotebooks = data.MaxNotebooks;
        failedNotebooks = data.FailedNotebooks;

        if (data.FinaleModeActivated)
        {
            ActivateFinaleMode();
            RenderSettings.ambientLight = Color.red;
            exitsR.SetActive(true);
        }

        if (data.Exits > 0 && finaleMode)
        {
            EntranceScript[] entrances = FindObjectsOfType<EntranceScript>();
            if (data.ExitsReached != null)
            {
                for (int i = 0; i < entrances.Length; i++)
                {
                    if (entrances[i] == entrance_0)
                    {
                        if (data.ExitsReached[i])
                        {
                            entrances[i].Lower();
                        }
                    }
                    else
                    {
                        if (data.ExitsReached[i])
                        {
                            entrances[i].CloseElevate();
                        }
                    }
                }
            }
            exitsReached = data.Exits - 1;
            ExitReached();
        }

        if (data.EventID != -1 && data.EventTimer != 0f)
        {
            cheatingConsole.schoolEventSystem.InstantChooseEvent(data.EventID, data.EventTimer);
        }

        try
        {
            if (XML.Items == null || XML.Items.Length <= 0) throw new System.Exception("No saved items' position can be handled");
            PickupScript[] items = FindObjectsOfType<PickupScript>(true);
            foreach (Item item in XML.Items)
            {
                if (!item.IsUp)
                {
                    foreach (PickupScript pickup in items)
                    {
                        if (Mathf.Round(pickup.transform.position.x) == Mathf.Round(item.Position[0]) && Mathf.Round(pickup.transform.position.y) == Mathf.Round(item.Position[1]) && Mathf.Round(pickup.transform.position.z) == Mathf.Round(item.Position[2]))
                        {
                            pickup.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

        NotebookScript[] AllNotebooks = FindObjectsOfType<NotebookScript>(true);
        foreach (Notebook notebook in XML.Notebooks)
        {
            foreach (NotebookScript nb in AllNotebooks)
            {
                if (nb.name == notebook.GB_Name)
                {
                    NotebookScript notebooks = nb.GetComponent<NotebookScript>();
                    notebooks.up = notebook.IsUp;
                    if (!notebooks.up) notebooks.SetDown(true);
                }
            }
        }

        if (data.Param_ModeSelected == "endless")
        {
            System.Collections.Generic.Dictionary<string, object> ExtraParameters = new System.Collections.Generic.Dictionary<string, object>();
            if (data.Params_Keys != null && data.Params_Keys.Length > 0)
            {
                for (int i = 0; i < data.Params_Keys.Length; i++)
                {
                    ExtraParameters.Add(data.Params_Keys[i], data.Params_Values[i]);
                }
            }
            foreach (System.Collections.Generic.KeyValuePair<string, object> Pair in ExtraParameters)
            {
                foreach (NotebookScript nb in AllNotebooks)
                {
                    if (nb.name == Pair.Key)
                    {
                        nb.GetComponent<NotebookScript>().respawnTime = (float)Pair.Value;
                    }
                }
            }
        }

        if (data.Param_ModeType == "NULL")
        {
            NullManager.Instance.NULL.baldiAnger = data.Angry;
            NullManager.Instance.NULL.baldiWait = data.BaldiWait;
            NullManager.Instance.NULL.baldiTempAnger = data.TemporaryAngry;
            NullManager.Instance.NULL.position = ((float[])data.Params_Values[0]).ToVector3();
            NullManager.Instance.NULL.transform.eulerAngles = ((float[])data.Params_Values[1]).ToVector3();
        }
        else if (baldiScrpt.isActiveAndEnabled)
        {
            baldiScrpt.baldiAnger = data.Angry;
            baldiScrpt.baldiWait = data.BaldiWait;
            baldiScrpt.baldiTempAnger = data.TemporaryAngry;
        }

        player.health = data.Health;
        player.stamina = data.Stamina;
        RefreshScores();
    }

    public void SetWarp(Transform transform, float[] position)
    {
        if (!transform.GetComponent<NPC>() || !transform.GetComponent<UnityEngine.AI.NavMeshAgent>())
        {
            transform.position = new Vector3(position[0], position[1], position[2]);
            return;
        }
        transform.GetComponent<NPC>().position = new Vector3(position[0], position[1], position[2]);
    }

    private bool onConsole;

    private bool done3;

    private bool vibrated = false;

    private bool FakeBan;

    public void Ban()
    {
        FakeBan = true;
        baldiScrpt.SpoopyJumpscare();
    }

    private float chances;

    public GameObject SpectatingDisplay;

    public GameObject MultiplayerEvents;

    public bool OverrideStaminaDesign;

    public void LoadSaves()
    {
        if (Time.timeScale != 0f & !SaveSystemStopper.Enabled)
        {
            if (scene == InputManager.IsSchoolScene()) SceneManager.LoadSceneAsync("SaveSystem", LoadSceneMode.Additive);
        }
    }

    private void Update()
    {
        if (ConsoleTraceInGame.shown & !onConsole)
        {
            onConsole = true;
            // console.info("Console Window Version: 1.0.1");
            // console.info("Generated by Unity");
            cheatConsoleActive = true;
            if (!MultiplayerManager.isActive) Time.timeScale = 0f;
            UnlockMouse();
        }
        else if (!ConsoleTraceInGame.shown && onConsole)
        {
            onConsole = false;
            cheatConsoleActive = false;
            if (!MultiplayerManager.isActive && Time.timeScale == 0f) Time.timeScale = Settings.GetFloat("TimeSpeed");
            LockMouse();
        }
        if (inputPlayer.GetButtonDown("SaveMenu"))
        {
            LoadSaves();
        }
        if (StaminaPercentage && !OverrideStaminaDesign)
        {
            if (player.stamina < 0f)
            {
                warning.text = HudVersion == "2" ? "REST" : "YOU NEED REST!";
            }
            else
            {
                if (BirthdayBash.Instance != null && BirthdayBash.Instance.SecondGlitchActive)
                {
                    warning.isRightToLeftText = true;
                    warning.characterSpacing = 1.5f;
                    warning.text = Mathf.FloorToInt(player.stamina) + "1!";
                    player.staminaBar.direction = Slider.Direction.RightToLeft;
                    player.staminaBar.fillRect.GetComponent<Image>().color = colored.Evaluate(1f - player.staminaBar.normalizedValue);
                    warning.color = colored.Evaluate(1f - player.staminaBar.normalizedValue);
                }
                else
                {
                    warning.text = Mathf.FloorToInt(player.stamina) + "%";
                    if (scene != "TestRoom")
                    {
                        player.staminaBar.fillRect.GetComponent<Image>().color = colored.Evaluate(player.staminaBar.normalizedValue);
                        warning.color = colored.Evaluate(player.staminaBar.normalizedValue);
                    }
                }
            }
        }
        if (player.gameOver)
        {
            if (!MultiplayerManager.isActive) Time.timeScale = 0f;
            if (Settings.GetBool("Vibrate") == true && !vibrated)
            {
                vibrated = true;

                if (Application.isMobilePlatform)
                {
                    StartCoroutine(gameOverMobileVibration());
                }
            }
            MobileControls.SetActive(false);
            gameOverDelay -= Time.unscaledDeltaTime;
            if (scene == InputManager.IsSchoolScene())
            {
                cameraScript.ChangeCamera(0, 1);
            }
            if (!done1)
            {
                if (CM) CM.DestroyAllCaptions(MultiplayerManager.isActive ? Client.NoCapturedPlayersLeft : true); // Destroys all Captions
                Can_UseItems = false;
                Can_SwitchItemSlots = false;
            }
            if (LoserBuzz != null)
            {
                if (LoserBuzz.name == aud_buzz.name)
                {
                    if (chances < 2f)
                        audioDevice.PlayOneShot(LoserBuzz);
                    else if (!done1)
                    {
                        audioDevice.PlayOneShot(LoserBuzz);
                    }
                }
                else
                {
                    if (!done1)
                    {
                        audioDevice.PlayOneShot(LoserBuzz);
                    }
                }
            }
            done1 = true;
            hud.SetActive(false);
            if (player.jumpRope)
            {
                player.jumpRopeScreen.SetActive(false);
                player.DeactivateJumpRope();
            }
            if (MultiplayerManager.isActive && MultiplayerEvents != null)
            {
                MultiplayerEvents.SetActive(false);
            }
            if (gameOverDelay <= 0f)
            {
#if DEBUG
                if (mode == "endless" && CheatScript.cheated)
                {
                    Debug.Log("Not submitting score due to cheating: " + notebooks);
                }
#endif
                if (mode == "endless")
                {
                    if (!CheatScript.cheated && Settings.HasKey("Account"))
                    {
                        if (!AccountManager.hasInstance())
                        {
                            Debug.LogError($"There was no account instance available to sync scores. If you care about your stats, please screenshot and send this to SWA:\nHIGH={notebooks}\nTIME={Time.timeSinceLevelLoad}\nLAST={Settings.GetInt("HighBooks_" + scene)}\nYCTP={HadYCTP}\nFOR={scene}\nWHEN={System.DateTime.Now}\nUSER={Settings.GetString("Account")}");
                            InputManager.DisplayEvent("Uh oh! Your scores couldn't be submitted. If your scores value enough, open your logs.", Color.red, 10f, (eve) =>
                            {
                                eve.Disappear();
                                FindObjectOfType<ConsoleTraceInGame>().show = true;
                            }, "Open Logs");
                        }
                        else
                        {
                            AccountManager.SubmitEndless(scene, notebooks, Time.timeSinceLevelLoad, HadYCTP, HasUsedCollectedItems);
                        }
                    }
                    if (!CheatScript.cheated && notebooks > Settings.GetInt("HighBooks_" + scene))
                    {
                        if (Settings.GetInt("HighBooks_" + scene) != 0)
                        {
                            AchievementManager.Instance.Earn(Achievements.EndlessHighScore);
                        }
                        Settings.SetInt("HighBooks_" + scene, notebooks);
                        highScoreText.SetActive(true);
                    }
                    Settings.SetInt("CurrentBooks", notebooks);
                }
                if (!MultiplayerManager.isActive) Time.timeScale = Settings.GetFloat("TimeSpeed");
                if (MultiplayerManager.isActive && !Client.NoCapturedPlayersLeft)
                {
                    player.gameOver = false;
                    if (exitsReached <= 0) audioDevice.Stop();
                    cameraScript.gameOverFade.GetComponent<Animator>().Play("GameOver", -1, 0f);
                    cameraScript.gameOverFade.SetActive(false);
                    SpectatingDisplay.SetActive(true);
                    MultiplayerEvents.SetActive(true);
                    gameOverDelay = 1.5f;
                    done1 = false;
                }
                else if (!HASManager)
                {
                    if (FakeBan)
                    {
                        GameSystem.AbortApplication("You are banned from playing Baldi's Basics The Ultra Decompile.\nReason: Cheating, like every time\nIf you like to appeal, please visit JUSTCLICKRESTART.BALD", "BANNED_FROM_BBTUD_BTW_ITSAJOKE");
                        return;
                    }
                    if (!player.dead)
                    {
                        SceneManager.LoadScene("GameOver");
                    }
                    else
                    {
                        SceneManager.LoadScene("GameOver1");
                    }
                }
                else
                {
                    HASManager.EndRound(true);
                }
            }
        }
        if (inputPlayer.GetButtonDown("CheatMenu") & !gamePaused & !MultiplayerManager.isActive)
        {
            if (!cheatConsoleActive)
            {
                ShowCheatConsole();
            }
            else
            {
                HideCheatConsole();
            }
        }
        if (!learningActive)
        {
            if (Input.GetButtonDown("Pause") & !cheatConsoleActive)
            {
                if (!gamePaused)
                {
                    PauseGame();
                }
                else
                {
                    UnpauseGame();
                }
            }
            if (!gamePaused & !cheatConsoleActive & !player.gameOver & !SaveSystemStopper.isOn & Time.timeScale != Settings.GetFloat("TimeSpeed"))
            {
                if (!MultiplayerManager.isActive) Time.timeScale = Settings.GetFloat("TimeSpeed");
            }
            if (scene != InputManager.IsSchoolScene())
            {
                return;
            }
            if (!audioDevice.isPlaying && played && finaleMode)
            {
                exitsR.SetActive(true);
                UpdateExitCount();
                played = false;
            }
            if (inputPlayer.GetButtonDown("Use") & !InputManager.usingTouch)
            {
                UseItem();
            }
            // ITEM SELECTOR
            if (!(MultiplayerManager.isActive && MultiplayerManager.instance.PauseMovement))
            {
                if ((inputPlayer.GetButtonDown("ItemLeft") || inputPlayer.GetAxisRaw("ItemScroll") > 0f) & Time.timeScale != 0f)
                {
                    DecreaseItemSelection();
                }
                else if ((inputPlayer.GetButtonDown("ItemRight") || inputPlayer.GetAxisRaw("ItemScroll") < 0f) & Time.timeScale != 0f)
                {
                    IncreaseItemSelection();
                }

                if ((inputPlayer.GetButtonDown("DropItem")) & Time.timeScale != 0f & item[itemSelected] != 0)
                {
                    if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi)
                    {
                        ClientSend.DropItem(item[itemSelected], playerTransform.position.ToFloat());
                    }
                    if (itemSelected == CurrentHammerSlot)
                    {
                        DropHammer(playerTransform.position);
                    }
                    else DropItem(playerTransform.position, item[itemSelected]);
                    ResetItem();
                }
                if (Can_SwitchItemSlots)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1) & Time.timeScale != 0f)
                    {
                        itemSelected = 0;
                        UpdateItemSelection();
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2) & Time.timeScale != 0f)
                    {
                        itemSelected = 1;
                        UpdateItemSelection();
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3) & Time.timeScale != 0f)
                    {
                        itemSelected = 2;
                        UpdateItemSelection();
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4) & Time.timeScale != 0f)
                    {
                        itemSelected = 3;
                        UpdateItemSelection();
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha5) & Time.timeScale != 0f)
                    {
                        itemSelected = 4;
                        UpdateItemSelection();
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha6) & Time.timeScale != 0f)
                    {
                        itemSelected = 5;
                        UpdateItemSelection();
                    }
                }
            }
        }
        else
        {
            if (Time.timeScale != 0f)
            {
                if (!MultiplayerManager.isActive) Time.timeScale = 0f;
            }
            // if (InputManager.isUsingController == true)
            // {
            //     if (!mouseLocked) LockMouse();
            // }
            // else
            // {
            //     if (mouseLocked) UnlockMouse();
            // }
        }

        if (MultiplayerManager.isActive) time += Time.deltaTime;

        if (gainStamina)
        {
            if (player.stamina < 100f)
            {
                player.stamina += Time.unscaledDeltaTime * 60f * player.staminaRate;
                if (player.stamina > 100f)
                {
                    player.stamina = 100f;
                }
            }
        }
        if (finaleMode && exitsReached == 3 && !CanNotPlayFinaleSounds && !ExitNoise.isPlaying)
        {
            ExitNoise.clip = aud_MachineLoop;
            ExitNoise.loop = true;
            ExitNoise.Play();
        }
        if (player.bigBootsTime <= 0f && !LookAtGuy.blinding && timeBig.activeSelf)
        {
            timeBig.SetActive(false);
        }
        else if (LookAtGuy.blinding)
        {
            timeBoots.color = Color.white;
            timeBig.SetActive(true);
            if (TestDuration > 0f)
            {
                TestDuration -= Time.deltaTime;
            }
            timeBoots.text = "Blinded: " + GetBlindedTimer();
        }
        else if (player.bigBootsTime > 0f)
        {
            if (exitModeCheatActive)
            {
                timeBoots.color = Color.white;
            }
            else
            {
                timeBoots.color = Color.black;
            }
            timeBig.SetActive(true);
            timeBoots.text = "Big Ol' Boots Timer: " + player.bigBootsTime.ToString("0");

        }
        if (SquishGB != null)
        {
            if (player.FlatDuration > 0f)
            {
                if (exitModeCheatActive)
                {
                    SquishTimer.color = Color.white;
                }
                else
                {
                    SquishTimer.color = Color.black;
                }
                SquishTimer.text = "Squished for " + player.FlatDuration.ToString("0") + "s";
                SquishTimer.gameObject.SetActive(true);
            }
            else if (SquishGB.activeSelf)
            {
                SquishGB.SetActive(false);
            }
        }
        // if ((playerTransform.position.x >= 5000000 || playerTransform.position.z >= 5000000) || (playerTransform.position.x <= -5000000 || playerTransform.position.z <= -5000000))
        // {
        //     GameSystem.AbortApplication("Player's position failed parsing multiple times.", "PLAYER_POSITION_PARSE_ERROR");
        // }
    }

    private bool CanNotPlayFinaleSounds;

    private string GetBlindedTimer()
    {
        int minutes = Mathf.FloorToInt(TestDuration / 60F);
        int seconds = Mathf.FloorToInt(TestDuration - minutes * 60);

        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    private IEnumerator gameOverMobileVibration()
    {
        Platforms.HandheldDevice.Vibration.OldVibration();
        yield return new WaitForSecondsRealtime(0.5f);
        Platforms.HandheldDevice.Vibration.OldVibration();
        yield return new WaitForSecondsRealtime(0.5f);
        Platforms.HandheldDevice.Vibration.OldVibration();
        yield break;
    }

    public void UpdateNotebookCount()
    {
        if (notebookCount == null) return;
        if (mode == "story")
        {
            notebookCount.text = notebooks.ToString() + "/" + maxNotebooks + " Notebooks";
        }
        else
        {
            notebookCount.text = notebooks.ToString() + " Notebooks";
        }
    }

    public void CollectNotebook()
    {
        notebooks++;
        if (notebooks == maxNotebooks && MultiplayerManager.isActive)
        {
            entrance_0.Raise();
            entrance_1.OpenElevate();
            entrance_3.OpenElevate();
        }
        UpdateNotebookCount();
    }

    public void SelectMobileItem(int id)
    {
        if (Can_SwitchItemSlots)
        {
            itemSelected = id;
            UpdateItemSelection();
        }

    }

    public void UseMobileItemButton()
    {
        UseItem();
    }

    public void LockMouse()
    {
        if (!learningActive & !cheatConsoleActive & !gamePaused)
        {
            CursorControllerScript.LockCursor();
            mouseLocked = true;
            lostConnection = false;
            reticle.SetActive(true);
        }
    }

    public void UnlockMouse()
    {
        CursorControllerScript.UnlockCursor();
        mouseLocked = false;
        reticle.SetActive(false);
    }

    public void PauseGame()
    {
        // if (!onlineGame.multiplayerActive)
        // {
        if (!MultiplayerManager.isActive)
        {
            Time.timeScale = 0f;
            if (player.gameOver) return;
        }
        else MultiplayerManager.instance.PauseMovement = true;
        // }
        if (NullManager.Instance != null)
        {
            NullManager.Instance.Music.Drums(float.PositiveInfinity);
        }

        gamePaused = true;
        pauseText.SetActive(true);
        UnlockMouse();
    }

    public void UnpauseGame()
    {
        if (!MultiplayerManager.isActive) Time.timeScale = Settings.GetFloat("TimeSpeed"); else MultiplayerManager.instance.PauseMovement = false;
        if (NullManager.Instance != null)
        {
            NullManager.Instance.Music.Drums(0f);
        }
        gamePaused = false;
        pauseText.SetActive(false);
        LockMouse();
    }

    public void ActivateSpoopMode()
    {
        spoopMode = true;
        if (mode != "event")
            entrance_0.Lower();
        if (entrance_1.isOpened)
        {
            entrance_1.CloseElevate();
            entrance_3.CloseElevate();
            if (!MultiplayerManager.isActive && !Settings.GetBool("NoYCTP"))
            {
                if (!systemTime.time)
                {
                    Hang hang = FindObjectOfType<Hang>();
                    if (hang != null && hang.CopyrightedHangSound != null & Settings.GetBool("CopyrightMusic", true))
                    {
                        aud_Hang = hang.CopyrightedHangSound;
                    }
                    audioDevice.PlayOneShot(aud_Hang);
                }
                audioDevice.PlayOneShotWC(aud_RON);
            }
            else if (schoolMusic.isPlaying)
            {
                FadeMusicDown();
            }
        }
        if (mode != "event")
        {
            baldiTutor.SetActive(false);
            if (CM != null)
            {
                CM.DestroyAllCaptions(false);
            }
            cheatingConsole.SelectTMPText(enableTexts[0]);
            cheatingConsole.SetCharacterState(0, true);
        }
        learnMusic.Stop();
        LearnProblem2.Stop();
        LearnProblem3.Stop();
        if (!Settings.GetBool("NoYCTP")) schoolMusic.Stop();
        if (Settings.GetBool("Vibrate"))
            Hang.Play();
    }

    public void SetGameOverDelay(float delay) { gameOverDelay = delay; }
    public void SetBuzzClip(AudioClip clip) { LoserBuzz = clip; }

    public void FadeMusicDown()
    {
        FindObjectOfType<RandomEvent>().FadeMusicDown(schoolMusic);
    }

    public GameObject Cloud;

    public void CreateCloud(Vector3 Pos)
    {
        GameObject TheCloud = Instantiate(Cloud, Pos, Quaternion.identity);
        TheCloud.name = "Cloud Prefab";
        Clouds TCloud = TheCloud.GetComponent<Clouds>();
        TCloud.ExcludeCollision(playerColider);
    }

    public void ShowCheatConsole()
    {
        if (onConsole)
        {
            return;
        }
        // if (!onlineGame.multiplayerActive)
        // {
        if (NullManager.Instance != null && Settings.GetInt("NULL_FirstTime") > 0)
        {
            NullManager.Instance.NULL.transform.position = player.transform.position;
            NullManager.Instance.NULL.targetPlayer = true;
            NullManager.Instance.KillAllowance = true;
            audioDevice.clip = NullManager.Instance.NULL.Null_CheatingHaha;
            audioDevice.PlayDelayed(7f);
            if (NullScript.Attempts > 0)
            {
                NullScript.Attempts--;
            }
            return;
        }
        if (MultiplayerManager.isActive) return;
        UnlockMouse();
        if (!MultiplayerManager.isActive) Time.timeScale = 0f;
        cheatConsole.SetActive(true);
        cheatConsoleActive = true;
        cheatingConsole.OnCheatEnable();
        // }
    }
    public void HideCheatConsole()
    {
        if (onConsole)
        {
            return;
        }
        // if (!onlineGame.multiplayerActive)
        // {
        LockMouse();
        cheatConsole.SetActive(false);
        cheatConsoleActive = false;
        if (!learningActive)
        {
            if (!MultiplayerManager.isActive) Time.timeScale = Settings.GetFloat("TimeSpeed");
        }
        // }
    }

    public bool lostConnection;

    public void ActivateFinaleMode()
    {
        finaleMode = true;
        if (spoopMode && !MultiplayerManager.isActive)
        {
            entrance_0.Raise();
            entrance_1.OpenElevate();
            entrance_3.OpenElevate();
        }
    }

    // public void DisableNotebooks()
    // {
    //     notebooks = -8;
    //     foreach (NotebookScript pickup in FindObjectsOfType<NotebookScript>())
    //     {
    //         pickup.enabled = false;
    //     }
    // }

    public void GetAngry(float value)
    {
        if (!spoopMode)
        {
            ActivateSpoopMode();
        }
        baldiScrpt.GetAngry(value);
    }

    public void Fliparoo()
    {
        player.Fliparoo();
    }

    public void ActivateLearningGame()
    {
        learningActive = true;
        gainStamina = true;
        UnlockMouse();
        tutorBaldi.GetComponent<AudioQueueScript>().ClearQueue();
        tutorBaldi.Stop();
        if (!spoopMode)
        {
            schoolMusic.Stop();
            learnMusic.Play();
            SetLearnMusicStage(1);
            LearnProblem2.Play();
            LearnProblem3.Play();
        }
        if (BirthdayBash.Instance != null && BirthdayBash.Instance.glitchActive)
        {
            learnMusic.clip = BirthdayBash.Instance.aud_LearnLoop;
            learnMusic.Play();
        }
    }

    public void SetLearnMusicStage(int problem)
    {
        if (Settings.GetBool("CopyrightMusic", true)) return;
        LearnProblem2.mute = problem < 2;
        LearnProblem3.mute = problem < 3;
    }

    public AudioSource LearnProblem2;
    public AudioSource LearnProblem3;

    public void DeactivateLearningGame(GameObject subject)
    {
        learningActive = false;
        if (subject != null)
            Object.Destroy(subject);
        UpdateNotebookCount();
        LockMouse();
        gainStamina = false;
        if (player.stamina < 100f && player.staminaRate > 0f)
        {
            player.stamina = 100f;
        }
        if (!spoopMode & !schoolMusic.isPlaying)
        {
            schoolMusic.Play();
            learnMusic.Stop();
            LearnProblem2.Stop();
            LearnProblem3.Stop();
        }
        #region Caption fix
        if (CM != null)
        {
            CM.DestroyAllCaptions(false);
        }
        #endregion
        if (!MultiplayerManager.isActive && notebooks > 1 && !spoopMode)
        {
            DropItem(quarter.transform.position, Mathf.RoundToInt(Random.Range(1f, itemNames.Length - 1)));
            if (notebooks == 2 & BirthdayBash.Instance == null)
            {
                if (tutorBaldi.GetComponent<AudioSource>()) tutorBaldi.GetComponent<AudioSource>().clip = null;
                AudMan.Queue(aud_NewPrize);
                tutorBaldi.GetComponentInChildren<TutorBaldi>().Prize_Notebook2();
            }
        }
        if (notebooks == 1 && !spoopMode)
        {
            quarter.SetActive(true);
            if (BirthdayBash.Instance == null && !MultiplayerManager.isActive)
            {
                if (tutorBaldi.GetComponent<AudioSource>()) tutorBaldi.GetComponent<AudioSource>().clip = null;
                AudMan.Queue(aud_Prize);
                AudMan.Queue(aud_prizeMouse);
                tutorBaldi.GetComponentInChildren<TutorBaldi>().Prize();
            }
        }
        else if (notebooks == maxNotebooks & mode == "story")
        {
            if (!played)
            {
                if (TV.isActiveAndEnabled)
                {
                    TV.DescendTV();
                    TV.PlayAudioBeforeAscend(aud_AllNotebooks, 0.8f);
                }
                else audioDevice.PlayOneShotWC(aud_AllNotebooks, 0.8f);
            }
            if (!finaleMode && (!MultiplayerManager.isActive || !MultiplayerManager.instance.isBaldi))
            {
                ActivateFinaleMode();
            }
            played = true;
        }
    }

    private void IncreaseItemSelection()
    {
        if (!Can_SwitchItemSlots) return;
        itemSelected++;
        if (itemSelected > 5)
        {
            itemSelected = 0;
        }
        itemSelect.anchoredPosition = new Vector3((float)itemSelectOffset[itemSelected], HudVersion != "2" ? 0f : -7f, 0f);
        UpdateItemName();
    }

    public void RefreshScores()
    {
        UpdateItemName();
        UpdateItemSelection();
        UpdateNotebookCount();
        UpdateExitCount();
        for (int i = 0; i < 6; i++)
        {
            itemSlot[i].texture = itemTextures[item[i]];
        }
    }

    private void DecreaseItemSelection()
    {
        if (!Can_SwitchItemSlots) return;
        itemSelected--;
        if (itemSelected < 0)
        {
            itemSelected = 5;
        }
        itemSelect.anchoredPosition = new Vector3((float)itemSelectOffset[itemSelected], HudVersion != "2" ? 0f : -7f, 0f);
        UpdateItemName();
    }

    private void UpdateItemSelection()
    {
        itemSelect.anchoredPosition = new Vector3((float)itemSelectOffset[itemSelected], HudVersion != "2" ? 0f : -7f, 0f);
        UpdateItemName();
    }

    public void CollectItem(int item_ID, Vector3 itemPosition)
    {
        int PutToSlot;
        if (item[0] == 0)
        {
            item[0] = item_ID;
            itemSlot[0].texture = itemTextures[item_ID];
            PutToSlot = 0;
        }
        else if (item[1] == 0)
        {
            item[1] = item_ID;
            itemSlot[1].texture = itemTextures[item_ID];
            PutToSlot = 1;
        }
        else if (item[2] == 0)
        {
            item[2] = item_ID;
            itemSlot[2].texture = itemTextures[item_ID];
            PutToSlot = 2;
        }
        else if (item[3] == 0)
        {
            item[3] = item_ID;
            itemSlot[3].texture = itemTextures[item_ID];
            PutToSlot = 3;
        }
        else if (item[4] == 0)
        {
            item[4] = item_ID;
            itemSlot[4].texture = itemTextures[item_ID];
            PutToSlot = 4;
        }
        else if (item[5] == 0)
        {
            item[5] = item_ID;
            itemSlot[5].texture = itemTextures[item_ID];
            PutToSlot = 5;
        }
        else
        {
            if (itemSelected == CurrentHammerSlot)
            {
                DropHammer(itemPosition);
            }
            else DropItem(itemPosition, item[itemSelected]);

            item[itemSelected] = item_ID;
            itemSlot[itemSelected].texture = itemTextures[item_ID];
            PutToSlot = itemSelected;
            if (CollabItems.Active)
            {
                CollabItems.Instance.ClearCollabItem(PutToSlot);
            }
        }
        if (IsGoingToCarryDRRHammer)
        {
            IsGoingToCarryDRRHammer = false;
            CurrentHammerSlot = PutToSlot;
        }
        UpdateItemName();
    }

    public void GetItem(int id, int itemID)
    {
        if (CurrentHammerSlot == id && itemID != 24)
        {
#if DEBUG
            Debug.Log("Magically teleporting the hammer back because we're replacing the item:" + CurrentHammerSlot);
#endif
            MagicallyTeleportTheHammerBack();
        }
        if (CollabItems.Active)
        {
            CollabItems.Instance.ClearCollabItem(id);
        }
        item[id] = itemID;
        itemSlot[id].texture = itemTextures[itemID];
        UpdateItemName();
    }

    private void TransformSlider(int stage)
    {
        // #if HAS_EVENT_PLUGGED
        //         if (stage == 2)
        //         {
        //             warning.enabled = false;
        //             Slider slider = player.staminaBar;
        //             slider.transform.GetChild(0).gameObject.SetActive(false);
        //             slider.fillRect.GetComponent<Image>().color = Color.blue;
        //             RectTransform rect = slider.GetComponent<RectTransform>();
        //             rect.pivot = Vector2.zero;
        //             rect.anchorMin = Vector2.zero;
        //             rect.anchorMax = new Vector2(1f, 0f);
        //             rect.anchoredPosition3D = new Vector2(0f, -0.7f);
        //             rect.sizeDelta = new Vector2(0f, 3f);
        //         }
        // #endif
        if (stage == 3)
        {
            Slider slider = player.staminaBar;
            RectTransform rect = slider.GetComponent<RectTransform>();
            rect.pivot = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.anchoredPosition3D = new Vector2(250f, 12f);
            rect.sizeDelta = new Vector2(140f, 50f);

            rect = slider.transform.GetChild(0).GetComponent<RectTransform>();
            rect.GetComponent<Image>().color = Color.white;
            Texture2D texture = Resources.Load<Texture2D>("StaminometerSheet_BG");
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            rect.GetComponent<Image>().sprite = sprite;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0f, 0.25f);
            rect.anchorMax = new Vector2(1f, 0.75f);
            rect.anchoredPosition3D = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            rect = GameObject.Instantiate(Resources.Load<GameObject>("StaminometerHandle"), slider.GetComponent<RectTransform>()).GetComponent<RectTransform>();
            slider.handleRect = rect.GetChild(0).GetComponent<RectTransform>();
            slider.fillRect.GetComponent<Image>().enabled = false;

            rect = GameObject.Instantiate(Resources.Load<GameObject>("StaminometerBG"), slider.transform.parent).GetComponent<RectTransform>();
            rect.gameObject.name = "STAMINOMETER_BG";
            rect.SetSiblingIndex(slider.GetComponent<RectTransform>().GetSiblingIndex() + 1);
            MoveSlider(rect.gameObject);
        }
    }

    public void MoveSlider(GameObject STAMINOMETERBG = null)
    {
        if (HudVersion != "2") return;
        if (STAMINOMETERBG == null) STAMINOMETERBG = GameObject.Find("STAMINOMETER_BG");
        if (STAMINOMETERBG == null)
        {
            // Just going to remove the warning.
            // if ((!MultiplayerManager.isActive || !MultiplayerManager.instance.isBaldi) && hud != null && hud.activeSelf && Time.timeSinceLevelLoad < 1f) // If it's Baldi or the hud is invisible, it doesn't matter.
            // {
            //     InputManager.DisplayEvent("Unable to properly setup the remastered staminometer. Your UI may look weirder than normal.", 10f);
            // }
            return;
        }
        bool MobileControlsShown = InputManager.usingTouch;
        if (MobileControlsShown)
        {
            SystemTimeScript timeScript = FindObjectOfType<SystemTimeScript>();
            if (timeScript != null)
            {
                timeScript.timeText.fontSizeMax = 24;
                timeScript.timeText.alignment = TextAlignmentOptions.Left;
                RectTransform TimeRect = timeScript.GetComponent<RectTransform>();

                TimeRect.anchorMin = new Vector2(0f, 0f);
                TimeRect.anchorMax = new Vector2(0f, 0f);
                TimeRect.pivot = new Vector2(0.5f, 0.5f);
                TimeRect.anchoredPosition = new Vector2(100.2f, 24.34357f);
            }

            RectTransform slider = player.staminaBar.GetComponent<RectTransform>();
            RectTransform BG = STAMINOMETERBG.GetComponent<RectTransform>();
            slider.anchoredPosition = new Vector2(250f, 12f);
            BG.anchoredPosition = new Vector2(222f, -10.42755f);

            warning.margin = Vector4.zero;
            warning.fontSize = 16;

            RectTransform rect2 = warning.rectTransform;
            rect2.pivot = Vector2.zero;
            rect2.anchorMin = new Vector2(0.5f, 0f);
            rect2.anchorMax = new Vector2(0.5f, 0f);
            rect2.anchoredPosition3D = new Vector2(-63f, 43f);
            rect2.sizeDelta = new Vector2(125f, 4f);
        }
        else
        {
            RectTransform slider = player.staminaBar.GetComponent<RectTransform>();
            RectTransform BG = STAMINOMETERBG.GetComponent<RectTransform>();
            slider.anchoredPosition = new Vector2(30f, 10f);
            BG.anchoredPosition = new Vector2(2f, -11.5f);

            warning.margin = Vector4.zero;
            warning.fontSize = 16;

            RectTransform rect = warning.rectTransform;
            rect.pivot = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.anchoredPosition3D = new Vector2(37f, 42f);
            rect.sizeDelta = new Vector2(125f, 4f);

            SystemTimeScript timeScript = FindObjectOfType<SystemTimeScript>();
            if (timeScript != null)
            {
                timeScript.timeText.fontSizeMax = 20;
                timeScript.timeText.alignment = TextAlignmentOptions.Center;
                RectTransform TimeRect = timeScript.GetComponent<RectTransform>();

                TimeRect.anchorMin = new Vector2(0.5f, 0f);
                TimeRect.anchorMax = new Vector2(0.5f, 0f);
                TimeRect.pivot = new Vector2(0.5f, 0.5f);
                TimeRect.anchoredPosition = new Vector2(0f, 24.34357f);
            }
        }
    }

    private void TransformItemSlotsV2()
    {
        if (SceneManager.GetActiveScene().name != InputManager.IsSchoolScene() && SceneManager.GetActiveScene().name != "School_2") return;
        RectTransform element;
        #region Item Slots
        element = GameObject.Find("ItemSlot0").GetComponent<RectTransform>();
        element.anchoredPosition = new Vector2(-223, -9);
        element = GameObject.Find("ItemSlot1").GetComponent<RectTransform>();
        element.anchoredPosition = new Vector2(-180, -9);
        element = GameObject.Find("ItemSlot2").GetComponent<RectTransform>();
        element.anchoredPosition = new Vector2(-136, -9);
        element = GameObject.Find("ItemSlot3").GetComponent<RectTransform>();
        element.anchoredPosition = new Vector2(-92, -9);
        element = GameObject.Find("ItemSlot4").GetComponent<RectTransform>();
        element.anchoredPosition = new Vector2(-48, -9);
        element = GameObject.Find("ItemSlot5").GetComponent<RectTransform>();
        element.anchoredPosition = new Vector2(-5, -9);
        #endregion

        #region Item Elements
        element = GameObject.Find("ItemSlots").GetComponent<RectTransform>();
        element.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("ItemSlotsV2");
        element.sizeDelta = new Vector2(150, 67);
        element.anchoredPosition = new Vector2(-130, 0f);
        element = GameObject.Find("ItemSlots2").GetComponent<RectTransform>();
        element.sizeDelta = new Vector2(150, 67);
        element.anchoredPosition = new Vector2(-130, 0f);
        element.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("ItemSlotsV2");

        element = GameObject.Find("ItemBG").GetComponent<RectTransform>();
        element.sizeDelta = new Vector2(254, 35);
        element.anchoredPosition = new Vector2(-3, -7f);

        element = GameObject.Find("Select").GetComponent<RectTransform>();
        element.sizeDelta = new Vector2(36, 35);
        element.anchoredPosition = new Vector2(-221, -7f);

        itemSelectOffset[0] = -221;
        itemSelectOffset[1] = -178;
        itemSelectOffset[2] = -134;
        itemSelectOffset[3] = -90;
        itemSelectOffset[4] = -46;
        itemSelectOffset[5] = -3;

        element = GameObject.Find("ItemText").GetComponent<RectTransform>();
        element.anchoredPosition = new Vector2(-107, -46f);
        element.sizeDelta = new Vector2(200, 70);
        itemText.fontSize = 20;
        itemText.alignment = TextAlignmentOptions.TopRight;
        #endregion
    }

    public void UseApple()
    {
        if (MultiplayerManager.isActive) Multiplayer.ClientSend.SendActionToOtherPlayer("apple");
        AchievementManager.Instance.Earn(Achievements.Apple);
        if (infiniteItems) return;
        if (item[0] == 11)
        {
            LoseItem(0);
        }
        else if (item[1] == 11)
        {
            LoseItem(1);
        }
        else if (item[2] == 11)
        {
            LoseItem(2);
        }
        else if (item[3] == 11)
        {
            LoseItem(3);
        }
        else if (item[4] == 11)
        {
            LoseItem(4);
        }
        else if (item[5] == 11)
        {
            LoseItem(5);
        }
    }

    public void UsePass()
    {
        if (infiniteItems) return;
        if (item[0] == 18)
        {
            LoseItem(0);
        }
        else if (item[1] == 18)
        {
            LoseItem(1);
        }
        else if (item[2] == 18)
        {
            LoseItem(2);
        }
        else if (item[3] == 18)
        {
            LoseItem(3);
        }
        else if (item[4] == 18)
        {
            LoseItem(4);
        }
        else if (item[5] == 18)
        {
            LoseItem(5);
        }
    }

    public bool Can_UseItems = true;
    public bool Can_SwitchItemSlots = true;

    private void UseItem()
    {
        if (!Can_UseItems) return;
        if (item[itemSelected] != 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit raycastHit;
            bool IsRaycasting;
            switch (item[itemSelected])
            {
                case 9:
                case 15:
                case 24:
                    IsRaycasting = Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, CharacterMask);
                    break;
                default:
                    IsRaycasting = Physics.Raycast(ray, out raycastHit);
                    break;
            }
            bool IsHovering = IsRaycasting && Vector3.Distance(playerTransform.position, raycastHit.transform.position) <= 10f;
            if (item[itemSelected] == 1)
            {
                player.stamina = player.stamina <= 200f ? (player.maxStamina * 2f) : (player.stamina + player.maxStamina * 2f);
                if (!player.inCafe)
                    player.ResetGuilt("eat", 1f);
                ResetItem();
                audioDevice.PlayOneShotWC(Crunch);
                if (MultiplayerManager.isActive)
                {
                    ClientSend.PlaySoundClip("CRUNCH", PhotonNetwork.LocalPlayer.UserId);
                }
            }
            else if (item[itemSelected] == 2)
            {
                if (IsHovering && (raycastHit.collider.tag == "SwingingDoor"))
                {
                    if (raycastHit.collider.name == "_DoorDarkOut")
                        raycastHit.collider.gameObject.GetComponent<SwingingDoorDarkScript>().LockDoor(30f);
                    else
                        raycastHit.collider.gameObject.GetComponent<SwingingDoorScript>().LockDoor(30f);
                    ResetItem();
                }
            }
            else if (item[itemSelected] == 3)
            {
                if (IsHovering && raycastHit.collider.tag == "Door" && raycastHit.transform.GetComponent<DoorScript>())
                {
                    if (!raycastHit.collider.GetComponent<DoorScript>().bDoorLocked)
                    {
                        return;
                    }
                    raycastHit.collider.gameObject.GetComponent<DoorScript>().UnlockDoor();
                    ResetItem();
                }
            }
            else if (item[itemSelected] == 4)
            {
                if (MultiplayerManager.isActive)
                {
                    float[] Position = playerTransform.position.ToFloat();
                    float[] Rotation = cameraTransform.rotation.eulerAngles.ToFloat();
                    ClientHandle.SentThatBSODA = true;
                    ClientSend.BSODA(Position, Rotation);
                }
                else
                {
                    Instantiate(bsodaSpray, playerTransform.position, cameraTransform.rotation);
                }
                ResetItem();
                if (!player.inCafe)
                    player.ResetGuilt("drink", 1f);
                audioDevice.PlayOneShotWC(aud_Soda);
            }
            else if (item[itemSelected] == 5)
            {
                if (IsHovering)
                {
                    if (raycastHit.collider.name.EndsWith("Machine"))
                    {
                        if (BirthdayBash.Instance != null)
                        {
                            audioDevice.PlayOneShotWC(CoinDrop);
                            ResetItem();
                            CollectItem(Mathf.RoundToInt(Random.Range(1f, 25f)), playerTransform.position);
                        }
                        else
                        {
                            System.Action Reset = () =>
                            {
                                audioDevice.PlayOneShotWC(CoinDrop);
                                ResetItem();
                            };
                            // Tried to organize this a little bit.
                            switch (raycastHit.collider.name)
                            {
                                case "BSODAMachine":
                                    Reset();
                                    CollectItem(4, playerTransform.position);
                                    break;
                                case "ZestyMachine":
                                    Reset();
                                    CollectItem(1, playerTransform.position);
                                    break;
                                case "BananaMachine":
                                    Reset();
                                    CollectItem(25, playerTransform.position);
                                    break;
                                case "ClockMachine":
                                    Reset();
                                    CollectItem(7, playerTransform.position);
                                    break;
                                case "BootsMachine":
                                    Reset();
                                    CollectItem(10, playerTransform.position);
                                    break;
                                case "AppleMachine":
                                    Reset();
                                    CollectItem(11, playerTransform.position);
                                    break;
                                case "TeleporterMachine":
                                    Reset();
                                    CollectItem(13, playerTransform.position);
                                    break;
                                case "ScissorsMachine":
                                    Reset();
                                    CollectItem(9, playerTransform.position);
                                    break;
                                case "GrapplingMachine":
                                    Reset();
                                    CollectItem(20, playerTransform.position);
                                    break;
                                case "ChalkMachine":
                                    Reset();
                                    CollectItem(23, playerTransform.position);
                                    break;
                                case "PortalMachine":
                                    Reset();
                                    CollectItem(21, playerTransform.position);
                                    break;
                                case "SwingDoorsMachine":
                                    Reset();
                                    CollectItem((Mathf.RoundToInt(Random.Range(0f, MultiplayerManager.isActive ? 0f : 1f)) == 0f) ? 2 : 14, playerTransform.position);
                                    break;
                            }
                        }
                    }
                    // Couldn't put this in...
                    if (raycastHit.collider.name.StartsWith("LegendAppleMachine"))
                    {
                        audioDevice.PlayOneShotWC(CoinDrop);
                        ResetItem();
                        if (raycastHit.collider.name.EndsWith("3Q"))
                        {
                            raycastHit.collider.name = "LegendAppleMachine_2Q";
                        }
                        else if (raycastHit.collider.name.EndsWith("2Q"))
                        {
                            raycastHit.collider.name = "LegendAppleMachine_1Q";
                        }
                        else
                        {
                            CollectItem(12, playerTransform.position);
                            raycastHit.collider.name = "LegendAppleMachine_3Q";
                        }
                    }
                    else if (raycastHit.collider.name == "PayPhone")
                    {
                        raycastHit.collider.gameObject.GetComponent<TapePlayerScript>().Play();
                        ResetItem();
                    }
                    if (raycastHit.collider.tag == "SwingingDoor")
                    {
                        if (raycastHit.collider.name == "_DoorDarkOut")
                            raycastHit.collider.gameObject.GetComponent<SwingingDoorDarkScript>().BuyDoor();
                        else
                            raycastHit.collider.gameObject.GetComponent<SwingingDoorScript>().BuyDoor();
                        ResetItem();
                    }
                }
            }
            else if (item[itemSelected] == 6)
            {
                if (IsHovering && (raycastHit.collider.name == "TapePlayer"))
                {
                    raycastHit.collider.gameObject.GetComponent<TapePlayerScript>().Play();
                    ResetItem();
                }
            }
            else if (item[itemSelected] == 7)
            {
                if (MultiplayerManager.isActive)
                {
                    ClientSend.SetAlarm(new Vector3(playerTransform.position.x, playerTransform.position.y - 0.4f, playerTransform.position.z));
                }
                GameObject gameObject = Object.Instantiate(alarmClock, new Vector3(playerTransform.position.x, playerTransform.position.y - 0.4f, playerTransform.position.z), cameraTransform.rotation);
                gameObject.GetComponent<AlarmClockScript>().baldi = baldiScrpt;
                ResetItem();
            }
            else if (item[itemSelected] == 8)
            {
                if (IsHovering)
                {
                    if (raycastHit.collider.tag == "Door")
                    {
                        raycastHit.collider.gameObject.GetComponent<DoorScript>().SilenceDoor();
                        ResetItem();
                        audioDevice.PlayOneShotWC(aud_Spray);
                    }
                    // else if (raycastHit.transform.name == "_Front" && raycastHit.transform.GetComponent<OpenLockerScript>())
                    // {
                    //     raycastHit.collider.GetComponent<OpenLockerScript>().Silence();
                    // }
                }
            }
            else if (item[itemSelected] == 9)
            {
                if (player.jumpRope)
                {
                    player.DeactivateJumpRope(true);
                    playtimeScript.Disappoint();
                    ResetItem();
                }
                else if (player.gumScreen.activeSelf)
                {
                    player.cutGum = true;
                    ResetItem();
                }
                else if (IsRaycasting && raycastHit.collider.name == "1st Prize")
                {
                    firstPrizeScript.GoCrazy();
                    ResetItem();
                }
                else if (IsRaycasting && raycastHit.collider.name == "5th Prize")
                {
                    raycastHit.transform.GetComponent<FifthPrizeScript>().GoCrazy();
                    ResetItem();
                }
            }
            else if (item[itemSelected] == 10)
            {
                player.ActivateBoots();
                WearBoots();
                ResetItem();
            }
            else if (item[itemSelected] == 12)
            {
                if (MultiplayerManager.isActive)
                {
                    Debug.LogError("Attempt to use an unsupported item: Legendary Apple");
                    ResetItem();
                    FindObjectOfType<HudScreenTextScript>().ShowHudText("This item is not supported in this mode!");
                    return;
                }
                ResetItem();
                player.stamina = 1000;
                GetItem(0, 4);
                GetItem(1, 11);
                GetItem(2, 9);
                GetItem(3, 11);
                GetItem(4, 2);
                GetItem(5, 11);
                SetAllCharacters(false);
                characterTime.SetActive(true);
                characterTimeText.text = "Restoring Characters: 0%";
                AchievementManager.Instance.Earn(Achievements.LegendaryApple);
                StartCoroutine(CharactersTimer());
            }
            else if (item[itemSelected] == 13)
            {
                teleporter.UseTeleporter();
                ResetItem();
            }
            else if (item[itemSelected] == 14)
            {
                if (IsHovering)
                {
                    if (raycastHit.collider.tag == "SwingingDoor")
                    {
                        if (raycastHit.collider.name == "_DoorDarkOut")
                            raycastHit.collider.gameObject.GetComponent<SwingingDoorDarkScript>().DoorOnSale();
                        else
                            raycastHit.collider.gameObject.GetComponent<SwingingDoorScript>().DoorOnSale();
                        ResetItem();
                    }
                }
            }
            else if (item[itemSelected] == 15)
            {
                if (player.jumpRope)
                {
                    player.DeactivateJumpRope(false);
                    playtimeScript.Disappoint();
                    player.ResetGuilt("stabbing", 1f);
                    ResetItem();
                }
                if (IsHovering)
                {
                    if (raycastHit.collider.name == "Playtime" & !player.jumpRope)
                    {
                        playtimeScript.Disappoint();
                        player.ResetGuilt("stabbing", 1f);
                        ResetItem();
                    }
                    else if (raycastHit.collider.name == "1st Prize")
                    {
                        firstPrizeScript.Stabbed();
                        player.ResetGuilt("stabbing", 1f);
                        ResetItem();
                    }
                    else if (raycastHit.collider.name == "5th Prize")
                    {
                        raycastHit.transform.GetComponent<FifthPrizeScript>().Stabbed();
                        player.ResetGuilt("stabbing", 1f);
                        ResetItem();
                    }
                    else if (raycastHit.collider.name == "Principal of the Thing")
                    {
                        principalNPC.Stabbed();
                        player.ResetGuilt("stabbing", 1f);
                        ResetItem();
                    }
                }
            }
            else if (item[itemSelected] == 16)
            {
                player.Heal(Random.Range(0.3f, 3.6f));
            }
            else if (item[itemSelected] == 17) // New item
            {
                audioDevice.PlayOneShotWC(aud_Whistle);
                if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer)
                {
                    Vector3 soundLocation = playerTransform.position;
                    ClientSend.SendActionToOtherPlayer("WOHOO:" + soundLocation.x.ToString("0") + "," + soundLocation.y.ToString("0") + "," + soundLocation.z.ToString("0"));
                }
                else
                {
                    if (principalNPC.isActiveAndEnabled) principalNPC.Hear(playerTransform.position, 12f, true);
                }
                if (NullManager.Instance != null)
                {
                    if (NullManager.Instance.BossFight) NullManager.Instance.NULL.Speed = 99f; else NullManager.Instance.NULL.GetAngry(100f);
                    NullManager.Instance.NULL.timeToMove = 0f;
                    NullManager.Instance.NULL.Hear(playerTransform.position, 99f);
                }
                ResetItem();
            }
            else if (item[itemSelected] == 19)
            {
                if (MultiplayerManager.isActive)
                {
                    Debug.LogError("Attempt to use an unsupported item: Secret Formula");
                    ResetItem();
                    FindObjectOfType<HudScreenTextScript>().ShowHudText("This item is not supported in this mode!");
                    return;
                }
                if (_CurrentTimer != null)
                {
                    StopCoroutine(_CurrentTimer);
                    _CurrentTimer = null;
                }
                _CurrentTimer = StartCoroutine(InvisibleTimer(Settings.GetString("ModeType", "Normal") == "Stealthy" ? 7f : 30f));
                ResetItem();
            }
            else if (item[itemSelected] == 20)
            {
                GameObject Hook = Instantiate(Grapple);
                Hook.SetActive(true);
                Hook.GetComponent<GrapplingHook>().StartGrappling();
                if (Settings.GetString("ModeType", "Normal") != "Grapple")
                {
                    ResetItem();
                }
            }
            else if (item[itemSelected] == 21)
            {
                if (MultiplayerManager.isActive)
                {
                    Debug.LogError("Attempt to use an unsupported item: Portal Poster");
                    ResetItem();
                    if (Resources.Load("ErrorMaybe") != null)
                    {
                        audioDevice.PlayOneShotWC((AudioClip)Resources.Load("ErrorMaybe"));
                    }
                    FindObjectOfType<HudScreenTextScript>().ShowHudText("This item is not supported in this mode!");
                    return;
                }
                if (!IsHovering)
                {
                    audioDevice.PlayOneShotWC((AudioClip)Resources.Load("ErrorMaybe"));
                    return;
                }
                MeshCollider MainCollider = raycastHit.collider.GetComponent<MeshCollider>();
                if (MainCollider != null)
                {
                    if (MainCollider.GetComponent<MeshRenderer>() && !(MainCollider.GetComponent<MeshRenderer>().material.name.Contains("WhiteBrickWall") || MainCollider.GetComponent<MeshRenderer>().material.name.Contains("SaloonWall") || MainCollider.GetComponent<MeshRenderer>().material.name.Contains("WallWithMolding")))
                    {
#if DEBUG
                        Debug.LogError("Attempt to put a poster on a non-verified wall: " + MainCollider.GetComponent<MeshRenderer>().material.name);
#endif
                        audioDevice.PlayOneShotWC((AudioClip)Resources.Load("ErrorMaybe"));
                        return;
                    }
                    if (MainCollider.GetComponent<MeshRenderer>() && MainCollider.tag == "BoundaryWall")
                    {
#if DEBUG
                        Debug.LogError("Attempt to put a poster on a boundary wall: " + MainCollider.GetComponent<MeshRenderer>().material.name);
#endif
                        audioDevice.PlayOneShotWC((AudioClip)Resources.Load("ErrorMaybe"));
                        return;
                    }
                    //                     if (MainCollider.transform)
                    //                     {
                    // #if DEBUG
                    //                         Debug.LogError("Attempt to put a poster on a outside wall: " + MainCollider.GetComponent<MeshRenderer>().material.name);
                    // #endif
                    //                         audioDevice.PlayOneShot((AudioClip)Resources.Load("ErrorMaybe"));
                    //                         return;
                    //                     }
                    if (Resources.Load("Slap") != null)
                    {
                        audioDevice.PlayOneShotWC((AudioClip)Resources.Load("Slap"));
                    }

                    Vector3 Pos = raycastHit.transform.position;
                    if (Resources.Load("Portal") != null && MainCollider.GetComponent<MeshRenderer>()) MainCollider.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Portal");
                    MainCollider.enabled = false;
                    foreach (MeshCollider meshRenderer in FindObjectsOfType<MeshCollider>())
                    {
                        if (meshRenderer.transform.position != Pos) continue;
                        if (Resources.Load("Portal") != null && meshRenderer.GetComponent<MeshRenderer>()) meshRenderer.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Portal");
                        meshRenderer.enabled = false;
                    }
                    ResetItem();
                }
                else
                {
                    if (Resources.Load("ErrorMaybe") != null)
                    {
                        audioDevice.PlayOneShotWC((AudioClip)Resources.Load("ErrorMaybe"));
                    }
                }
            }
            else if (item[itemSelected] == 22)
            {
                player.NoFacultyAlert = 30f;
                ResetItem();
            }
            else if (item[itemSelected] == 23)
            {
                CreateCloud(playerTransform.position);
                if (MultiplayerManager.isActive)
                {
                    float[] Position = playerTransform.position.ToFloat();
                    string Pos = Position[0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Position[1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Position[2].ToString(System.Globalization.CultureInfo.InvariantCulture);
                    ClientSend.SendActionToOtherPlayer("chalkcloud:" + Pos);
                }
                ResetItem();
            }
            else if (item[itemSelected] == 24)
            {
                IsHovering = IsRaycasting && Vector3.Distance(raycastHit.transform.position, playerTransform.position) <= 12f;
                if (IsHovering && raycastHit.collider.GetComponent<NPC>())
                {
                    if (itemSelected == CurrentHammerSlot && raycastHit.collider.name == "Doctor Reflex" && (DoctorReflex.ChasingPlayer || DoctorReflex.Angry))
                    {
                        if (Resources.Load("ErrorMaybe") != null)
                        {
                            audioDevice.PlayOneShotWC((AudioClip)Resources.Load("ErrorMaybe"));
                        }
                        Debug.LogError("[DR. REFLEX'S HAMMER] This hammer cannot squish Dr. Reflex because he's chasing or angry. Sorry, I don't make the rules.");
                        if (GameObject.Find("Hud").GetComponent<HudScreenTextScript>().hudScreen.activeSelf)
                        {
                            GameObject.Find("Hud").GetComponent<HudScreenTextScript>().HideHudText();
                            Invoke("HammerHud_Delay", 0.03f);
                        }
                        else GameObject.Find("Hud").GetComponent<HudScreenTextScript>().ShowHudText(1f, "CANNOT SQUISH HIM RIGHT NOW");
                        return;
                    }
                    audioDevice.PlayOneShotWC(HammerSound_TempVariable);
                    raycastHit.collider.GetComponent<NPC>().FlattenDuration = 30f;
                    if (MultiplayerManager.isActive)
                    {
                        ClientSend.SendActionToOtherPlayer("squish:" + raycastHit.collider.name);
                    }
                    if (itemSelected != CurrentHammerSlot)
                        ResetItem();
                }
                IsRaycasting = Physics.Raycast(ray, out raycastHit);
                IsHovering = IsRaycasting && Vector3.Distance(raycastHit.transform.position, playerTransform.position) <= 12f;
                if (IsHovering && raycastHit.collider.name == "Its a Bully")
                {
                    BullyScript bully = raycastHit.collider.GetComponent<BullyScript>();
                    audioDevice.PlayOneShotWC(HammerSound_TempVariable);
                    bully.SetFlatDuration(30f);
                    if (MultiplayerManager.isActive)
                    {
                        ClientSend.SendActionToOtherPlayer("squish:" + bully.gameObject.name);
                    }
                    if (itemSelected != CurrentHammerSlot)
                        ResetItem();
                }
                if (IsHovering && raycastHit.collider.name == "Window Collider")
                {
                    raycastHit.collider.GetComponent<WindowScript>().Break();
                    if (MultiplayerManager.isActive)
                    {
                        ClientSend.SendActionToOtherPlayer("window:" + ClientHandle.GetIdFromPosition<WindowScript>(raycastHit.transform.position));
                    }
                    if (itemSelected != CurrentHammerSlot)
                        ResetItem();
                }
                if (IsHovering && raycastHit.collider.name == "Wall" && raycastHit.transform.parent != null && raycastHit.transform.parent.name == "Its a Bully") // Its a Bully
                {
                    BullyScript bully = raycastHit.collider.GetComponentInParent<BullyScript>();
                    audioDevice.PlayOneShotWC(HammerSound_TempVariable);
                    bully.SetFlatDuration(30f);
                    if (MultiplayerManager.isActive)
                    {
                        ClientSend.SendActionToOtherPlayer("squish:" + bully.gameObject.name);
                    }
                    if (itemSelected != CurrentHammerSlot)
                        ResetItem();
                }
            }
            else if (item[itemSelected] == 25)
            {
                if (MultiplayerManager.isActive)
                {
                    Debug.LogError("Attempt to use an unsupported item: 'Nana Peel");
                    ResetItem();
                    FindObjectOfType<HudScreenTextScript>().ShowHudText("This item is not supported in this mode!");
                    return;
                }
                Vector3 Pos = playerTransform.position;
                Pos.y = 0f;
                GameObject Banana = Instantiate(bananaPeel, Pos, Quaternion.identity);
                Banana.name = "A Banana Peel";
                ResetItem();
            }
        }
    }

    private void HammerHud_Delay()
    {
        if (CurrentHammerSlot != -1)
            GameObject.Find("Hud").GetComponent<HudScreenTextScript>().ShowHudText(1f, "CANNOT SQUISH HIM RIGHT NOW");
    }

    public GameObject Grapple;

    public GrapplingHook HookManager;

    private Coroutine _CurrentTimer;

    public AudioClip aud_Whistle;

    public PrincipalScript principalNPC;

    public Pomp MrsPompNPC;

    public string[] GetItemNames() => itemNames;

    private IEnumerator CharactersTimer()
    {
        float time = 0f;
        while (time < 100f)
        {
            time += 3f * Time.deltaTime;
            characterTimeText.text = "Restoring Characters: " + time.ToString("0") + "%";
            yield return null;
        }
        StartCoroutine(HideTimer());
        SetAllCharacters(true);
        characterTimeText.text = "Restoring Complete!";
        yield break;
    }

    private IEnumerator HideTimer()
    {
        float time = 7f;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        characterTime.SetActive(false);
        yield break;
    }

    private IEnumerator InvisibleTimer(float time = 30f)
    {
        characterTime.SetActive(true);
        player.isInvisible = true;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            characterTimeText.text = "Invisible for " + time.ToString("0") + " sec";
            yield return null;
        }
        player.isInvisible = false;
        characterTime.SetActive(false);
        yield break;
    }

    public void SetAllCharacters(bool response)
    {
        if (mode == "event") response = false;
        // firstPrize.SetActive(response);
        // fifthPrize.SetActive(response);
        // crafters.SetActive(response);
        // gottaSweep.SetActive(response);
        // bully.SetActive(response);
        // mrsPomp.SetActive(response);
        // beans.SetActive(response);
        // goldenBaldi.SetActive(response);
        if (response == true)
        {
            if (spoopMode)
            {
                cheatingConsole.SelectTMPText(enableTexts[0]);
                cheatingConsole.SetCharacterState(0, true);
            }
        }
        else
        {
            cheatingConsole.SelectTMPText(enableTexts[0]);
            cheatingConsole.SetCharacterState(0, false);
        }
        for (int i = 1; i < enableTexts.Length; i++)
        {
            if (i == 4) continue;
            cheatingConsole.SelectTMPText(enableTexts[i]);
            cheatingConsole.SetCharacterState(i, response);
        }
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        if (!focusStatus)
        {
            if (!learningActive & !cheatConsoleActive & !gamePaused)
            {
                this.PauseGame();
            }
        }
    }


    private void WearBoots()
    {
        bootsAnimation.SetTrigger("WearBoots");
    }

    public void BootsOff()
    {
        bootsAnimation.SetTrigger("NoBoots");
    }
    public void ResetItem()
    {
        if (infiniteItems) return;
        if (CurrentHammerSlot == itemSelected)
        {
#if DEBUG
            Debug.Log("Magically teleporting the hammer back from reseting item, expected slot: " + CurrentHammerSlot);
#endif
            MagicallyTeleportTheHammerBack();
        }
        if (CollabItems.Active)
        {
            CollabItems.Instance.CheckForUseItem(itemSelected, item[itemSelected]);
        }
        item[itemSelected] = 0;
        itemSlot[itemSelected].texture = itemTextures[0];
        UpdateItemName();
    }

    public void LoseItem(int id)
    {
        if (CurrentHammerSlot == id)
        {
#if DEBUG
            Debug.Log("Magically teleporting the hammer back, losing item, expected slot: " + CurrentHammerSlot);
#endif
            MagicallyTeleportTheHammerBack();
        }
        if (item[id] == 20 && Settings.GetString("ModeType", "Normal") == "Grapple")
        {
            // Can't take the grappling hook away 
            return;
        }
        item[id] = 0;
        itemSlot[id].texture = itemTextures[0];
        if (CollabItems.Active)
        {
            CollabItems.Instance.ClearCollabItem(id);
        }
        UpdateItemName();
    }

    public void MagicallyTeleportTheHammerBack()
    {
        if (HammerReflex || GameObject.Find("Pickup_HammerReflex"))
        {
            Destroy(GameObject.Find("Pickup_HammerReflex"), 0f); // Destroy it since it'll be teleported.
            teleporter.audioDevice.PlayOneShotWC(teleporter.teleportSound); // Play the sound
            ClinicHammerTrigger clinicHammer = FindObjectOfType<ClinicHammerTrigger>();
            if (clinicHammer)
            {
                clinicHammer.GiveHammerBack();
            }
        }
        if (CurrentHammerSlot != -1)
        {
            CurrentHammerSlot = -1;
            teleporter.audioDevice.PlayOneShotWC(teleporter.teleportSound); // Play the sound
            ClinicHammerTrigger clinicHammer = FindObjectOfType<ClinicHammerTrigger>();
            if (clinicHammer)
            {
                clinicHammer.GiveHammerBack();
            }
        }
    }

    public void TakeAwayHammer()
    {
        item[CurrentHammerSlot] = 0;
        itemSlot[CurrentHammerSlot].texture = itemTextures[0];
        CurrentHammerSlot = -1;
        UpdateItemName();
        GameObject.Find("Hud").GetComponent<HudScreenTextScript>().HideHudText();
    }

    public bool IsGoingToCarryDRRHammer;

    [HideInInspector]
    public GameObject HammerReflex;

    public bool HasHammer()
    {
        if (CurrentHammerSlot == -1) return false;
        if (item[CurrentHammerSlot] != 24)
        {
            if (!DoctorReflex.HammerManager.Hammer.activeSelf && HammerReflex != null)
            {
                CurrentHammerSlot = -1;
                return false;
            }
#if DEBUG
            Debug.Log("Magically teleporting the hammer back because expected slot: " + CurrentHammerSlot);
#endif
            MagicallyTeleportTheHammerBack();
            return false;
        }
        return item[CurrentHammerSlot] == 24;
    }

    public void DropHammer(Vector3 pos)
    {
        CurrentHammerSlot = -1;
        if (MultiplayerManager.isActive)
        {
            return;
        }
        int ID = 24;
        GameObject gameObject = Object.Instantiate<GameObject>(itemQuarter, pos, playerTransform.rotation);
        gameObject.GetComponent<PickupScript>().player = playerTransform;
        gameObject.GetComponent<PickupScript>().gc = this;
        gameObject.GetComponent<PickupScript>().clickCursor = clickCursorObject;
        gameObject.GetComponent<Collider>().isTrigger = false;
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = this.itemSprites[ID];
        gameObject.transform.name = "Pickup_HammerReflex";
        HammerReflex = gameObject;
        AchievementManager.Instance.Earn(Achievements.DropItem);
    }

    public void DropItem(Vector3 pos, int ID)
    {
        if (ID == 20 && Settings.GetString("ModeType", "Normal") == "Grapple")
        {
            throw new System.Exception("Grappling hook mode, can't drop item."); // This will make sure that the item isn't reset.
        }
        GameObject gameObject = Object.Instantiate<GameObject>(itemQuarter, pos, playerTransform.rotation);
        gameObject.GetComponent<PickupScript>().player = playerTransform;
        gameObject.GetComponent<PickupScript>().gc = this;
        gameObject.GetComponent<PickupScript>().clickCursor = clickCursorObject;
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = this.itemSprites[ID];
        gameObject.transform.name = this.itemObjectNames[ID];
        AchievementManager.Instance.Earn(Achievements.DropItem);
    }

    private void UpdateItemName()
    {
        itemText.text = itemNames[item[itemSelected]];
        if (itemSelected == CurrentHammerSlot)
        {
            itemText.text = "<color=grey>Dr.</color><color=yellow> Reflex's</color> <color=grey>Hammer";
        }
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi)
        {
            Multiplayer.ClientSend.SendActionToOtherPlayer("select:" + item[itemSelected]);
        }
    }

    public void UpdateExitCount()
    {
        if (exitsReached > 0) exitCount.gameObject.SetActive(true);
        exitCount.text = exitsReached.ToString() + "/4 Exits";
    }

    private void OnApplicationPause(bool pause)
    {
        if (!learningActive & !cheatConsoleActive & !gamePaused)
        {
            this.PauseGame();
        }
    }

    public void restartga()
    {
        if (MultiplayerManager.isActive)
        {
            InputManager.DisplayException(new System.Exception("You can not restart the game while in a multiplayer game."), 5f);
            return;
        }
        RestartLoad.SetActive(true);
        RestartLoad.GetComponent<SceneLoader>().LoadSceneAsync(scene);
        switch (scene)
        {
            case "FarmScene":
            case "CampScene":
            case "FarmCamping":
            case "BusCamping":
                if (GameObject.Find("MobileControls"))
                {
                    GameObject.Find("MobileControls").SetActive(false);
                }
                break;
        }
    }

    public void ExitReached()
    {
        // 3.0.5 - Lowered Volume of Exit Noise
        if (ExitNoise == null)
        {
            ExitNoise = gameObject.AddComponent<AudioSource>();
            ExitNoise.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.AAmbience : null;
            ExitNoise.pitch = Settings.GetFloat("TimeSpeed", 1f);
        }
        //
        exitsReached++;
        if (exitsReached == 1 || (exitsReached < 3 && NullManager.Instance != null))
        {
            exitsR.SetActive(true);
            EnableExitMode(true);
            audioDevice.PlayOneShotWC(aud_Switch, 0.8f);
            ExitNoise.clip = aud_MachineQuiet;
            ExitNoise.loop = true;
            ExitNoise.Play();
        }
        else if (exitsReached == 2)
        {
            audioDevice.volume = 0.9f;
            ExitNoise.volume = 0.51f; // 3.0.5 - Lowered Volume of Exit Noise
            ExitNoise.clip = aud_MachineStart;
            ExitNoise.loop = true;
            ExitNoise.Play();
        }
        else if (exitsReached == 3)
        {
            if (NullManager.Instance != null)
            {
                EnableExitMode(false);
                ExitNoise.Stop();
                ExitNoise.volume = 1f;
                audioDevice.volume = 1f;
                audioDevice.PlayOneShotWC(aud_Switch, 0.8f);
                UpdateExitCount();
                NullManager.Instance.NULL.NullRunFrom = 1;
                NullManager.Instance.NULL.OnLastExit();
                NullManager.Instance.NULL.targetPlayer = false;
                NullManager.Instance.KillAllowance = false;
                CanNotPlayFinaleSounds = true;
                return;
            }
            ExitNoise.clip = aud_MachineRev;
            ExitNoise.loop = false;
            ExitNoise.Play();
        }
        UpdateExitCount();
    }

    public void DespawnCrafters()
    {
        crafters.SetActive(false);
    }

    public void SetTextWhite()
    {
        exitModeCheatActive = true;
        notebookCount.color = Color.white;
        exitCount.color = Color.white;
        itemText.color = Color.white;
        if (!systemTime.threeDarkness)
        {
            systemTime.WhiteText();
        }
    }

    public void SetTextBlack()
    {
        notebookCount.color = Color.black;
        exitCount.color = Color.black;
        itemText.color = Color.black;
        exitModeCheatActive = false;
        if (!systemTime.threeDarkness)
        {
            systemTime.BlackText();
        }
    }

    public void EnableExitMode()
    {
        if (!exitModeCheatActive)
        {
            exitText.text = "Disable Red Mode";
            RenderSettings.ambientLight = Color.red;
            notebookCount.color = Color.white;
            exitCount.color = Color.white;
            itemText.color = Color.white;
            exitModeCheatActive = true;
            if (!systemTime.threeDarkness)
            {
                systemTime.WhiteText();
            }
        }
        else
        {
            exitText.text = "Enable Red Mode";
            RenderSettings.ambientLight = Color.white;
            notebookCount.color = Color.black;
            exitCount.color = Color.black;
            itemText.color = Color.black;
            exitModeCheatActive = false;
            if (!systemTime.threeDarkness)
            {
                systemTime.BlackText();
            }
        }
    }

    public void EnableExitMode(bool B)
    {
        exitModeCheatActive = !B;
        EnableExitMode();
    }

    public void mode42()
    {
        CheatScript.cheated = true;
        if (!mode42enabled)
        {
            fourtyTwoText.text = "Disable 42 Mode";
            Settings.SetString("FourtyTwoMode", "true");
            mode42enabled = true;
            console.log("Mode on");
        }
        else
        {
            fourtyTwoText.text = "Enable 42 Mode";
            Settings.SetString("FourtyTwoMode", "false");
            mode42enabled = false;
            console.log("Mode off");
        }
    }

    public void noclip()
    {
        CheatScript.cheated = true;
        if (!noclipEnabled)
        {
            noClipText.text = "Disable Noclip";
            playerColider.isTrigger = true;
            noclipEnabled = true;
            debugMode = true;
        }
        else
        {
            noClipText.text = "Enable Noclip";
            noclipEnabled = false;
            playerColider.isTrigger = false;
            debugMode = false;
        }
    }

    public void SetNoclip(bool noclipenable)
    {
        if (!MultiplayerManager.isActive)
        {
            if (noclipenable)
            {
                noClipText.text = "Disable Noclip";
                playerColider.isTrigger = true;
                noclipEnabled = true;
                debugMode = true;
            }
            else
            {
                noClipText.text = "Enable Noclip";
                noclipEnabled = false;
                playerColider.isTrigger = false;
                debugMode = false;
            }
        }
    }

    public void playerJumpscare()
    {
        baldi.SetActive(true);
        player.transform.position = baldi.transform.position;
    }

    public bool exitModeCheatActive;

    public bool mode42enabled;

    public GameObject boots;

    public GameObject timeBig;
    public TMP_Text SquishTimer;
    public GameObject SquishGB;

    public Collider playerColider;

    public TMP_Text timeBoots;

    public bool gainStamina;

    public bool noclipEnabled;

    public Animator bootsAnimation;

    public GameObject itemQuarter;

    private string[] itemObjectNames = new string[26]
    {
        "Nothing",
        "Pickup_EnergyFlavoredZestyBar",
        "Pickup_YellowDoorLock",
        "Pickup_Key",
        "Pickup_BSODA",
        "Pickup_Quarter",
        "Pickup_Tape",
        "Pickup_AlarmClock",
        "Pickup_WD-3D",
        "Pickup_SafetyScissors",
        "Pickup_Boots",
        "Pickup_Apple",
        "Pickup_AppleGod",
        "Pickup_Teleporter",
        "Pickup_CoinDoor",
        "Pickup_Pencil",
        "Pickup_Health",
        "Pickup_Whistle",
        "Pickup_MathPass",
        "Pickup_InvisibleItem",
        "Pickup_Hook",
        "Pickup_Portal",
        "Pickup_Nametag",
        "Pickup_Chalk",
        "Pickup_Hammer",
        "Pickup_Banana"
    };
}