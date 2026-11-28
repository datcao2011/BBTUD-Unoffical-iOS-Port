using UnityEngine;
using UnityEngine.UI;
using Multiplayer;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviour
{
    public static bool isActive;
    public bool isBaldi;
    public bool isMainPlayer;
    public bool isAIBaldi;
    public static MultiplayerManager instance;
    public bool PauseMovement;
    public AudioClip[] sounds;
    private static AudioClip[] soundStatic;
    public GameControllerScript gc;
    public PlayerScript player;
    public Client client;
    private Platforms.PlatPlayer platform;
    public PlayerListItem[] PlayerList = new PlayerListItem[2];

    public Texture Default;

    //public Dictionary<int, BANANAPEELAAAA> BananaPeels = new Dictionary<int, BANANAPEELAAAA>();

    public bool HasMoreThan2Players { get; set; }
    public GameObject MiniMessage;

    public static Client.InProgressEvent inProgressEvent;

    public class PlayerListItem
    {
        public string username;
        public Texture profilePicture;
        public string tag = "";
        public string device = "browser";
        public int actor = -1;
        public bool isLan;
    }

    public static string LanTag = " <color=blue>(LAN)</color>";

    [System.Serializable]
    public class GameSettings
    {
        public bool RandomEvents = true;
        public bool SecondFloor = true;
        public int Notebooks = -1;
        public int PickedBaldi = -2; // Only used for the master client.
        public int JoinInProgress = 0;
        public bool[] CharacterActivated = new bool[10] { true, true, true, true, true, true, true, true, true, true };
        public int YCTPUsage = 0; // 0 = User Choice, 1 = Yes, 2 = Only Math, 3 = No
    }

    public GameSettings CurrentSettings;

    public void Activate()
    {
        if (platform != null) return;
        platform = Platforms.PlatTools.GetPlatformPlayer();
        gc = FindObjectOfType<GameControllerScript>();
        player = FindObjectOfType<PlayerScript>();
        soundStatic = sounds;
        gc.baldi.SetActive(true);
        gc.baldiScrpt.Partygoer = false;
        gc.baldiScrpt.getAgent().angularSpeed = 0f;
        gc.spoopMode = true;
        gc.baldiTutor.SetActive(false);
        gc.schoolMusic.Stop();
        if (GameObject.Find("Pickup_CoinDoor"))
        {
            GameObject OBJ = GameObject.Find("Pickup_CoinDoor");
            OBJ.name = "Pickup_Hammer";
            OBJ.GetComponentInChildren<SpriteRenderer>().sprite = gc.itemSprites[24];
        }
        if (GameObject.Find("Pickup_Health"))
        {
            GameObject OBJ = GameObject.Find("Pickup_Health");
            OBJ.name = "Pickup_Nametag";
            OBJ.GetComponentInChildren<SpriteRenderer>().sprite = gc.itemSprites[22];
        }
        if (GameObject.Find("Pickup_Pencil"))
        {
            GameObject OBJ = GameObject.Find("Pickup_Pencil");
            OBJ.name = "Pickup_Chalk";
            OBJ.GetComponentInChildren<SpriteRenderer>().sprite = gc.itemSprites[23];
        }
        //#endif
        #region Notebooks Check
        if (CurrentSettings.Notebooks == -1) // Automatic
        {
            NotebookScript[] Notebooks = FindObjectOfType<FogScript>().OrderOfNotebooks;
            int Amount = 7;
            if (CurrentSettings.SecondFloor)
            {
                if (PlayerList.Length >= 7)
                {
                    Amount = 9;
                }
                else if (PlayerList.Length >= 4)
                {
                    Amount = 8;
                }
            }
            gc.maxNotebooks = Amount;
            for (int i = 0; i < Notebooks.Length; i++)
            {
                if (Amount - 1 >= i)
                {
                    Notebooks[i].gameObject.SetActive(true);
                }
                else
                {
                    Notebooks[i].gameObject.SetActive(false);
                }
            }

        }
        else if (CurrentSettings.Notebooks >= 2) // Security pre-caution
        {
            NotebookScript[] Notebooks = FindObjectOfType<FogScript>().OrderOfNotebooks;
            int Amount = CurrentSettings.Notebooks;
            if (Amount > 7 && !CurrentSettings.SecondFloor) Amount = 7; // Impossible to get more than 7 notebooks without the second floor
            gc.maxNotebooks = Amount;
            for (int i = 0; i < Notebooks.Length; i++)
            {
                if (Amount - 1 >= i)
                {
                    Notebooks[i].gameObject.SetActive(true);
                }
                else
                {
                    Notebooks[i].gameObject.SetActive(false);
                }
            }
        }
        #endregion
        GameObject[] Characters = FindObjectOfType<FogScript>().OrderOfCharacters;
        for (int i = 0; i < Characters.Length; i++)
        {
            if (!CurrentSettings.CharacterActivated[i])
            {
                Characters[i].SetActive(false);
                Characters[i].AddComponent<DisableOnLoad>();
            }
        }
        player.gc.MrsPompNPC.timerText.gameObject.SetActive(false);
        InitProgressEvents();
        player.gc.UpdateNotebookCount();
        ElevatorScript.U(!CurrentSettings.SecondFloor);
        GameObject.Find("Floor 2 Elevator").GetComponent<ElevatorScript>().unavailable = true;
        gc.gameObject.AddComponent<DisableSaveSystem>();
        if (!CurrentSettings.RandomEvents) FindObjectOfType<RandomEvent>().PauseRandomEvents();

        if (GameObject.Find("MobileControls"))
        {
            GameObject.Find("MobileControls").transform.GetChild(0).GetChild(0).GetChild(16).gameObject.SetActive(false);
            GameObject.Find("MobileControls").transform.GetChild(0).GetChild(0).GetChild(17).gameObject.SetActive(true);
        }

        if (isAIBaldi && isMainPlayer)
        {
            player.baldi.GetAngry(1f); // No breaking
            gc.baldiScrpt.Partygoer = true;
            gc.baldiScrpt.getAgent().angularSpeed = 2160f;
        }
        foreach (HideableLocker lockers in FindObjectsOfType<HideableLocker>())
        {
            lockers.LimitedTime = true;
            lockers.HidingDuration = 240f;
        }

        if (isBaldi)
        {
            player.GetComponent<Collider>().isTrigger = true;
            player.baldi.transform.GetChild(4).gameObject.SetActive(true);
            player.baldi.GetAngry(1f);
            Camera mainCamera = Camera.main;
            GameObject.Find("Canvas Stuff").transform.GetChild(2).gameObject.SetActive(false);
            GameObject.Find("Canvas Stuff").transform.GetChild(3).gameObject.SetActive(true);
            GameObject.Find("MobileControls").transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            GameObject.Find("MobileControls").transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

            player.gc.notebookCount = GameObject.Find("Canvas Stuff").transform.GetChild(3).GetChild(1).GetComponent<TMPro.TMP_Text>();
            player.gc.exitsR = GameObject.Find("Canvas Stuff").transform.GetChild(3).GetChild(2).gameObject;
            player.gc.exitCount = GameObject.Find("Canvas Stuff").transform.GetChild(3).GetChild(2).GetComponent<TMPro.TMP_Text>();
            player.gc.UpdateNotebookCount();
        }
        if (HasMoreThan2Players)
        {
            gc.cheatingConsole.SelectTMPText(gc.enableTexts[5]);
            gc.cheatingConsole.SetCharacterState(5, false);
        }
    }

    // public void AddBananaItem(int BananaID, BANANAPEELAAAA Peel)
    // {
    //     BananaPeels[BananaID] = Peel; // This is considered adding anyway
    // }

    // public BANANAPEELAAAA GetBananaItem(int ID)
    // {
    //     if (!BananaPeels.ContainsKey(ID)) return null;
    //     return BananaPeels[ID];
    // }

    // public void RemoveBananaItem(int ID)
    // {
    //     if (BananaPeels.ContainsKey(ID)) BananaPeels.Remove(ID);
    // }

    public void InitProgressEvents()
    {
        if (inProgressEvent != null)
        {
            if (gc.notebooks == 0)
            {
                ClientHandle.ReceiveAction("notebook:none:" + inProgressEvent.Notebooks, "", "");
            }
            if (inProgressEvent.Exits > 0 || CurrentSettings.JoinInProgress <= 1)
            {
                CreateMiniMessage(inProgressEvent.Exits > 0 ? "Players are starting to escape, no new players can join in-progress." : "You are spectating this game", Color.red, 5f);
                gc.exitsReached = Mathf.Clamp(inProgressEvent.Exits, 0, 3);
                if (inProgressEvent.Exits > 0)
                {
                    gc.exitsReached--;
                    if (inProgressEvent.Exits > 1) gc.EnableExitMode();
                    gc.ExitReached();
                }
                client.CapturedPeople[PhotonNetwork.LocalPlayer.ActorNumber] = true;
                gc.hud.SetActive(false);
                if (GameObject.Find("MobileControls")) GameObject.Find("MobileControls").SetActive(false);

                if (player.GetComponent<Collider>()) player.GetComponent<Collider>().enabled = false;
                FindObjectOfType<CameraScript>().GetComponent<Camera>().nearClipPlane = 0.1f;
                gc.SpectatingDisplay.SetActive(true);
            }
            NotebookScript[] Notebooks = FindObjectOfType<FogScript>().OrderOfNotebooks;
            for (int i = 0; i < Notebooks.Length; i++)
            {
                if (inProgressEvent.NotebooksCollected[i])
                {
                    if (Notebooks[i].name.Contains("NoYCTPNotebook")) // The Math machine still works, possible exploit
                    {
                        Notebooks[i].gameObject.SetActive(false);
                    }
                    else Notebooks[i].SetDown(true);
                }
            }
            inProgressEvent = null;
        }
    }

    public void CreateMiniMessage(string Message, Color BGColor, float timer = 5f)
    {
        if (Settings.GetString("MPPopups", "true") != "true") return;
        GameObject gameObject = Instantiate(MiniMessage, gc.MultiplayerEvents.transform);
        MiniEventMessage eventMessage = gameObject.GetComponent<MiniEventMessage>();
        BGColor.a = 0.47f;
        if (BGColor != null) eventMessage.GetComponentInChildren<Image>().color = BGColor;
        eventMessage.Message = Message;
        eventMessage.disappearMessageTimer = timer;
        eventMessage.StartLogMessage();
        eventMessage.StartTimer = true;
    }

    public GameObject gb;

    private string Current;

    private void OnActiveSceneChanged(Scene OldScene, Scene Next)
    {
        Current = Next.name;
    }

    private void Update()
    {
        if (Current != "School" && Current != "MM_Connect")
        {
            DestroyImmediate(gameObject);
        }
        if (Current == "School")
        {
            if (platform.GetButtonDown("PList"))
            {
                // #if HAS_EVENT_PLUGGED
                //                 if (CountDownSyncing.e001)
                //                 {
                //                     PlayerList = FindObjectOfType<CountDownSyncing>().GetLatestPlayerList();
                //                     PlayerList[0].profilePicture = AccountManager.getProfilePicture() ?? null;

                //                     GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(7).GetChild(1).GetChild(0).GetComponent<RawImage>().texture = PlayerList[0].profilePicture;
                //                     GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(7).GetChild(1).GetChild(1).GetComponent<TMPro.TMP_Text>().text = HasNameTag(PlayerList[0]);
                //                     GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(7).GetChild(1).GetChild(2).GetComponent<Image>().sprite = GetImage(PlayerList[0].device.ToLower());


                //                     GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(7).GetChild(2).GetChild(0).gameObject.SetActive(false);
                //                     GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(7).GetChild(2).GetChild(2).gameObject.SetActive(false);

                //                     GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(7).GetChild(2).GetChild(1).GetComponent<TMPro.TMP_Text>().fontSizeMax = 8f;
                //                     GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(7).GetChild(2).GetChild(1).GetComponent<TMPro.TMP_Text>().rectTransform.anchoredPosition = new Vector2(100f, -27f);
                //                     Vector2 delta = new Vector2(170f, 55f);
                //                     GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(7).GetChild(2).GetChild(1).GetComponent<TMPro.TMP_Text>().rectTransform.sizeDelta = delta;
                //                     string str = "** OTHER PLAYERS **:\n";
                //                     foreach (PlayerListItem player in PlayerList)
                //                     {
                //                         if (player.username == PlayerList[0].username) continue;
                //                         if (player.username == "SERVER") continue;
                //                         str += HasNameTag(player);
                //                         str += " [" + player.device + "]";
                //                         str += "\n";
                //                     }
                //                     GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(7).GetChild(2).GetChild(1).GetComponent<TMPro.TMP_Text>().text = str;
                //                     if (gb == null) gb = GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(7).gameObject;
                //                     gb.SetActive(platform.GetButton("PList"));
                //                     return;
                //                 }
                // #endif
                if (PlayerList[0].username.StartsWith("<"))
                {
                    PlayerList[0].profilePicture = Default;
                    PlayerList[0].tag = "";
                }
                else if (PlayerList[0].username == AccountManager.getUsername())
                {
                    PlayerList[0].profilePicture = AccountManager.getProfilePicture();
                    PlayerList[0].tag = AccountManager.UserTag;
                }
                else
                {
                    PlayerList[0].profilePicture = AccountManager.LatestGotProfilePicture ?? Default;
                    PlayerList[0].tag = AccountManager.LatestUserTag;
                }
                if (PlayerList[1].username.StartsWith("<"))
                {
                    PlayerList[1].profilePicture = Default;
                    PlayerList[1].tag = "";
                }
                else if (PlayerList[1].username == AccountManager.getUsername())
                {
                    PlayerList[1].profilePicture = AccountManager.getProfilePicture();
                    PlayerList[1].tag = AccountManager.UserTag;
                }
                else
                {
                    PlayerList[1].profilePicture = AccountManager.LatestGotProfilePicture ?? Default;
                    PlayerList[1].tag = AccountManager.LatestUserTag;
                }
                GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(8).GetChild(1).GetChild(0).GetComponent<RawImage>().texture = PlayerList[0].profilePicture;
                PlayerList[0].isLan = client != null && client.HasLanPeerFor(PhotonNetwork.LocalPlayer.ActorNumber);
                GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(8).GetChild(1).GetChild(1).GetComponent<TMPro.TMP_Text>().text = HasNameTag(PlayerList[0]);
                GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(8).GetChild(1).GetChild(2).GetComponent<Image>().sprite = GetImage(PlayerList[0].device.ToLower());
                Player[] PlayerCheck = PhotonNetwork.PlayerListOthers;
                Player player = PlayerCheck.Length > 0 ? PlayerCheck[0] : null;
                string a = "";
                string b = "<color=white>";
                if (player != null)
                {
                    if (PlayerList[1].username != player.NickName)
                    {
                        PlayerList[1].username = player.NickName;
                        if (player.NickName.StartsWith("<"))
                        {
                            PlayerList[1].profilePicture = Default;
                            Debug.Log("Replaced profile picture/tag since the other person left: " + player.NickName);
                        }
                        else
                        {
                            AccountManager.GetProfilePicture(player.NickName);
                            Debug.Log("Fetching new profile picture/tag since the other person left: " + player.NickName);
                        }
                    }
                    else if (client.CapturedPeople.ContainsKey(player.ActorNumber) && client.CapturedPeople[player.ActorNumber])
                    {
                        a = "<color=red>";
                    }
                    if (PlayerList.Length == 3)
                    {
                        if (PlayerList[2].username == PlayerList[1].username)
                        {
                            PlayerList[2].username = "1 Player";
                        }
                        else if (PlayerList[2].username != "1 Player")
                        {
                            Player player2 = PlayerCheck.Length > 1 ? PlayerCheck[1] : null;
                            if (player2 == null)
                            {
                                b = "<color=#383838>";
                            }
                            else if (PlayerList[2].username != player2.NickName)
                            {
                                PlayerList[2].username = player2.NickName;
                            }
                            else if (client.CapturedPeople.ContainsKey(player2.ActorNumber) && client.CapturedPeople[player2.ActorNumber])
                            {
                                b = "<color=red>";
                            }
                        }
                    }
                }
                else
                {
                    a = "<color=#383838>";
                    b = "<color=#383838>";
                }
                GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(8).GetChild(2).GetChild(0).GetComponent<RawImage>().texture = PlayerList[1].profilePicture;
                PlayerList[1].isLan = client != null && player != null && client.HasLanPeerFor(player.ActorNumber);
                GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(8).GetChild(2).GetChild(1).GetComponent<TMPro.TMP_Text>().text = a + HasNameTag(PlayerList[1]);
                GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(8).GetChild(2).GetChild(2).GetComponent<Image>().sprite = GetImage(PlayerList[1].device.ToLower());
                if (PlayerList.Length > 2)
                {
                    GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(8).GetChild(3).GetComponent<TMPro.TMP_Text>().text = PlayerList.Length == 3 ? "(+" + b + PlayerList[2].username + "</color>)" : "(+" + (PlayerList.Length - 2) + " Players)";
                }
            }
            if (gb == null) gb = GameObject.Find("Canvas Stuff").transform.GetChild(0).GetChild(8).gameObject;
            gb.SetActive(platform.GetButton("PList"));
        }


        Time.timeScale = 1f;
    }

    public Sprite[] DeviceSprites;

    public void SquishPlayerAnim(Transform t, bool JustModelPlayer)
    {
        StartCoroutine(ModelTransformSquish(t, JustModelPlayer));
    }

    private IEnumerator ModelTransformSquish(Transform t, bool JustModelPlayer)
    {
        Vector3 LocalPos = t.localPosition;
        Vector3 Scale = t.localScale;
        Vector3 a = t.localPosition;
        Vector3 b = t.localScale;
        if (JustModelPlayer)
        {
            a.y = -0.769f;
            b.y = 208f;
            t.localPosition = a;
            t.localScale = b;
        }
        else
        {
            t.GetComponent<AnimationOnWalk>().YAddition = -2.75f;
            a.y = 1.25f;
            b.y = 1.25f;
            t.localPosition = a;
            t.localScale = b;
        }
        float FlatDuration = 30f;
        while (FlatDuration > 0f)
        {
            FlatDuration -= Time.unscaledDeltaTime;
            yield return null;
        }
        if (t == null) yield break;
        t.localPosition = LocalPos;
        t.localScale = Scale;
        if (!JustModelPlayer) t.GetComponent<AnimationOnWalk>().YAddition = 0f;
        yield break;
    }

    private Sprite GetImage(string device)
    {
        switch (device)
        {
            case "windows":
                return DeviceSprites[0];
            case "macos":
                return DeviceSprites[1];
            case "linux":
                return DeviceSprites[2];
            case "android":
                return DeviceSprites[3];
            case "webgl":
                return DeviceSprites[4];
        }
        return null;
    }

    public string GetOtherUsername()
    {
        if (PlayerList[0].username != AccountManager.getUsername())
        {
            return PlayerList[0].username;
        }
        else
        {
            return PlayerList[1].username;
        }
    }

    private string HasNameTag(PlayerListItem item)
    {
        string username = item.username;
        string lan = item.isLan ? LanTag : "";
        switch (item.tag.ToLower())
        {
            case "creator":
                return "<color=green>" + username + "</color> <color=#2fad42><size=8>CREATOR</size></color>" + lan;
            case "helper":
                return username + " <color=#2f98ad><size=8>HELPER</size></color>" + lan;
            case "dev":
                return username + " <color=blue><size=8>DEV</size></color>" + lan;
            case "yt":
                return username + " <color=red><size=8>YT</size></color>" + lan;
            case "supporter":
                return username + " <color=yellow><size=8>SUPPORTER</size></color>" + lan;
            case "verified":
                return username + " <color=blue><size=8>VERIFIED</size></color>" + lan;
        }
        return username + lan;
    }

    private float LastSpeed = 1f;

    private void Awake()
    {
        isActive = true;
        instance = this;
        client = GetComponent<Client>();
        DontDestroyOnLoad(gameObject);
        LastSpeed = Settings.GetFloat("TimeSpeed", 1f);
        Settings.SetFloat("TimeSpeed", 1f);
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDestroy()
    {
        instance = null;
        isActive = false;
        Settings.SetFloat("TimeSpeed", LastSpeed);
        Time.timeScale = LastSpeed;
        PhotonNetwork.PreloadCleanup();
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    public static AudioClip GetAudio(string name)
    {
        for (int i = 0; i < soundStatic.Length; i++)
        {
            if (soundStatic[i].name.ToLower() == name.ToLower())
            {
                return soundStatic[i];
            }
        }
        return null;
    }
}