#if UNITY_EDITOR || NETWORK_CONSOLE
// NOTE: For an actively-maintained version of this script, see https://github.com/mminer/consolation.
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using System.Linq;
using Multiplayer;
using ExitGames.Client.Photon;

/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>
public class NetworkConsole : MonoBehaviour
{
    public Version version = new Version(1, 0, 0);

    /// <summary>
    /// The hotkey to show and hide the console window.
    /// </summary>

    public KeyCode toggleMainKey;

    /// <summary>
    /// Whether to open the window by shaking the device (mobile-only).
    /// </summary>
    public bool shakeToOpen = false;
    public static bool instanceOpen;

    public float shakeAcceleration = 10f;

    float textFieldWidth;

    public List<Log> rlogs = new List<Log>();
    public List<Log> logs = new List<Log>();
    Vector2 scrollPosition;
    Vector2 scrollPos2;
    public bool show
    {
        get { return shown; }
        set
        {
            shown = value;
        }
    }
    public static bool shown { get; protected set; }
    bool collapse;
    bool noAudioEvents;

    // Visual elements:

    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
    {
        { LogType.Assert, Color.white },
        { LogType.Error, Color.red },
        { LogType.Exception, Color.red },
        { LogType.Log, Color.white },
        { LogType.Warning, Color.yellow },
    };

    const int margin = 20;

    Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
    Rect titleBarRect = new Rect(0, 0, 10000, 20);
    GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
    GUIContent closeLabel = new GUIContent("Close", "Closes the log details.");
    GUIContent sendLabel = new GUIContent("Send", "Send command to the console.");
    string commandText = "";
    bool isWebGL;
    string lastScene;

    void OnEnable()
    {
        if (instanceOpen)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instanceOpen = true;
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        textFieldWidth = Screen.width - (margin * 7);
        isWebGL = Platforms.CurrentDevice.currentPlatform == Platforms.PFPlatform.WebGL;
        if (isWebGL) shakeToOpen = false;
    }

    void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(toggleMainKey))
        {
            if (!isWebGL)
            {
                show = !show;
            }
        }
    }

    void OnGUI()
    {
        if (!show)
        {
            return;
        }
        windowRect = GUILayout.Window(123457, windowRect, ConsoleWindow, "Network Console");
    }

    int Toggle;
    string[] ToolbarStrings = { "Profiles", "Statistics", "Debug Menu", "Events" };

    private GameObject[] CharactersEnabled;

    /// <summary>
    /// A window that displayss the recorded logs.
    /// </summary>
    /// <param name==="windowID">Window ID.</param>
    void ConsoleWindow(int windowID)
    {
        GUILayout.BeginHorizontal();
        Toggle = GUILayout.Toolbar(Toggle, ToolbarStrings);
        GUILayout.EndHorizontal();

        if (Toggle == 0)
        {
            if (!PhotonNetwork.IsConnected)
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("CANNOT DISPLAY CONTENT");
                GUI.contentColor = Color.white;
                GUILayout.Label("PhotonNetwork.IsConnected === false");
            }
            else if (!PhotonNetwork.InRoom)
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("CANNOT DISPLAY CONTENT");
                GUI.contentColor = Color.white;
                GUILayout.Label("PhotonNetwork.IsConnected === true");
                GUILayout.Label("PhotonNetwork.InRoom === false");
            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                Room Current = PhotonNetwork.CurrentRoom;
                GUILayout.Label("=== Room ===");
                GUILayout.Space(0.2f);
                GUILayout.Label("Name/Code: " + Current.Name);
                GUILayout.Label("Public? " + Current.IsVisible);
                GUILayout.Label("Joinable? " + Current.IsOpen);
                GUILayout.Label("Players: " + Current.PlayerCount + "/" + Current.MaxPlayers);
                Player master = Current.GetPlayer(Current.MasterClientId);
                GUILayout.Label("Master Client: " + $"{master.NickName} (A:{master.ActorNumber} {master.UserId})");
                GUILayout.Space(1f);

                GUILayout.Label("=== Players ===");
                GUILayout.Space(0.2f);
                Player[] players = PhotonNetwork.PlayerList;
                foreach (Player player in players)
                {
                    GUILayout.Label(player.NickName + " (" + player.UserId + ") ----");
                    GUILayout.Label("Photon ---");
                    GUILayout.Label("ID: " + player.ActorNumber);
                    GUILayout.Label("Is Me? " + player.IsLocal);
                    GUILayout.Label("Is Inactive? " + player.IsInactive);
                    GUILayout.Label("Has Rejoined? " + player.HasRejoined);
                    ExitGames.Client.Photon.Hashtable Properties = player.CustomProperties;
                    for (int i = 0; i < Properties.Count; i++)
                    {
                        object a = Properties.Keys.ElementAt(i);
                        object b = Properties.Values.ElementAt(i);
                        GUILayout.Label(a.ToString() + ": " + b.ToString());
                    }
                    if (MultiplayerManager.instance.client.Started)
                    {
                        GUILayout.Label("Game ---");
                        GUILayout.Label("Is Captured? " + MultiplayerManager.instance.client.CapturedPeople[player.ActorNumber]);
                        GUILayout.Label("Is Baldi?" + (MultiplayerManager.instance.client.CurrentActor_Baldi == player.ActorNumber));
                        GUILayout.Label("Is Main Priority (player)?" + (MultiplayerManager.instance.client.CurrentActor_HasPingPriority == player.ActorNumber));

                        if (!player.IsLocal && GameObject.Find("EMP_" + player.UserId))
                        {
                            Transform t = GameObject.Find("EMP_" + player.UserId).transform;
                            GUILayout.Label("Actor ---");
                            GUILayout.Label($"Position: {t.position.x}, {t.position.y}, {t.position.z}");
                            GUILayout.Label($"Rotation: {t.eulerAngles.x}, {t.eulerAngles.y}, {t.eulerAngles.z}");
                            if (t.GetComponentInChildren<AnimationOnWalk>())
                            {
                                AnimationOnWalk ModelManager = t.GetComponentInChildren<AnimationOnWalk>();
                                GUILayout.Label($"Item Selected: {MultiplayerManager.instance.gc.GetItemNames()[ModelManager.CurrentItemSlot]} (ID:{ModelManager.CurrentItemSlot})");
                            }
                        }
                        else if (player.IsLocal)
                        {
                            GUILayout.Label("Actor ---");
                            GUILayout.Label($"Position: {MultiplayerManager.instance.gc.playerTransform.position.x}, {MultiplayerManager.instance.gc.playerTransform.position.y}, {MultiplayerManager.instance.gc.playerTransform.position.z}");
                            GUILayout.Label($"Rotation: {MultiplayerManager.instance.gc.playerTransform.eulerAngles.x}, {MultiplayerManager.instance.gc.playerTransform.eulerAngles.y}, {MultiplayerManager.instance.gc.playerTransform.eulerAngles.z}");
                            GUILayout.Label($"Item Selected: {MultiplayerManager.instance.gc.GetItemNames()[MultiplayerManager.instance.gc.item[MultiplayerManager.instance.gc.itemSelected]]} (ID:{MultiplayerManager.instance.gc.item[MultiplayerManager.instance.gc.itemSelected]})");
                        }
                        if (GUILayout.Button("Jumpscare Player"))
                        {
                            MultiplayerManager.instance.client.photonView.RPC("PUN_Action", RpcTarget.AllViaServer, "captured:" + player.ActorNumber, player.UserId); ;
                        }
                        if (GUILayout.Button("Force Spectate Player"))
                        {
                            MultiplayerManager.instance.client.photonView.RPC("PUN_Action", RpcTarget.AllViaServer, "escaped:" + player.ActorNumber, player.UserId); ;
                        }
                        GUILayout.Space(0.2f);
                    }

                    GUILayout.BeginHorizontal();
                    commandText = GUILayout.TextField(commandText, 250, GUILayout.Width(textFieldWidth));
                    if (GUILayout.Button("Kick"))
                    {
                        RaiseEventOptions options = new RaiseEventOptions()
                        {
                            TargetActors = new int[] { player.ActorNumber }
                        };
                        PhotonNetwork.RaiseEvent(99, commandText, options, ExitGames.Client.Photon.SendOptions.SendReliable);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(1f);

                GUILayout.Label("=== Settings ===");
                if (MultiplayerManager.instance.CurrentSettings != null)
                {
                    GUILayout.Label("Host ---");
                    MultiplayerManager.GameSettings settings = MultiplayerManager.instance.CurrentSettings;
                    GUILayout.Label("Notebooks (-1=Auto): " + settings.Notebooks);
                    GUILayout.Label("JoinInProgress (0=No,1=Spectating,2=Yes): " + settings.JoinInProgress);
                    GUILayout.Label("Baldi Player ID (-2=Random,-1=AI): " + settings.PickedBaldi);
                    GUILayout.Label("Random Events? " + settings.RandomEvents);
                    GUILayout.Label("Second Floor? " + settings.SecondFloor);
                    if (MultiplayerManager.instance.gc != null)
                    {
                        if (CharactersEnabled == null || CharactersEnabled.Length <= 0 || CharactersEnabled[0] == null)
                        {
                            FogScript FOG = FindObjectOfType<FogScript>();
                            CharactersEnabled = FOG.OrderOfCharacters;
                        }
                        GUILayout.Label("Characters Enabled ---");
                        for (int i = 0; i < CharactersEnabled.Length; i++)
                        {
                            GUILayout.Label(CharactersEnabled[i].name + ": " + CharactersEnabled[i].activeSelf);
                        }
                    }
                }
                else
                {
                    GUILayout.Label("NULL");
                }
                GUILayout.EndScrollView();
            }
        }
        else if (Toggle == 1)
        {
            if (!PhotonNetwork.IsConnected)
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("CANNOT DISPLAY CONTENT");
                GUI.contentColor = Color.white;
                GUILayout.Label("PhotonNetwork.IsConnected === false");
            }
            else if (PhotonNetwork.InRoom)
            {
                GUILayout.Label("=== ME ===");
                Player Me = PhotonNetwork.LocalPlayer;
                GUILayout.Label("Name: " + Me.NickName);
                GUILayout.Label("User: " + Me.UserId);
                GUILayout.Label("ID: " + Me.ActorNumber);
                // Now the game
                GUILayout.Space(1f);
                GUILayout.Label("=== GAME ===");
                MultiplayerManager m = MultiplayerManager.instance;
                if (m.client.Started)
                {
                    float Duration = m.gc.time;
                    int minutes = Mathf.FloorToInt(Duration / 60F);
                    int seconds = Mathf.FloorToInt(Duration - minutes * 60);

                    string Timer = string.Format("{0:0}:{1:00}", minutes, seconds);
                    GUILayout.Label("Duration: " + Timer);
                    int HasCaptured = 0;
                    foreach (bool b in m.client.CapturedPeople.Values)
                    {
                        if (b) HasCaptured++;
                    }
                    GUILayout.Label("Captured: " + HasCaptured + "/" + m.client.CapturedPeople.Count);
                }
                else
                {
                    GUILayout.Label("Started === false");
                }
                // Stats
                GUILayout.Space(1f);
                GUILayout.Label("=== Photon ===");
                GUILayout.Label("Ping: " + PhotonNetwork.GetPing());
                GUILayout.Label("Resent Packets: " + PhotonNetwork.ResentReliableCommands);
            }
            else
            {
                GUILayout.Label("=== ME ===");
                Player Me = PhotonNetwork.LocalPlayer;
                GUILayout.Label("Name: " + Me.NickName);
                GUILayout.Label("ID: " + Me.ActorNumber);
                GUILayout.Label("User: " + Me.UserId);
                GUILayout.Label("Actor: " + Me.ActorNumber);
                GUILayout.Space(1f);
                GUILayout.Label("=== Master ===");
                GUILayout.Label("Total Players: " + PhotonNetwork.CountOfPlayers);
                GUILayout.Label("Players in Rooms: " + PhotonNetwork.CountOfPlayersInRooms);
                GUILayout.Label("Total Rooms: " + PhotonNetwork.CountOfRooms);
                GUILayout.Label("Players on Master: " + PhotonNetwork.CountOfPlayersOnMaster);
            }
        }
        else if (Toggle == 2)
        {
            if (!PhotonNetwork.IsConnected)
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("CANNOT DISPLAY CONTENT");
                GUI.contentColor = Color.white;
                GUILayout.Label("PhotonNetwork.IsConnected === false");
            }
            else if (!PhotonNetwork.InRoom)
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("CANNOT DISPLAY CONTENT");
                GUI.contentColor = Color.white;
                GUILayout.Label("PhotonNetwork.IsConnected === true");
                GUILayout.Label("PhotonNetwork.InRoom === false");
            }
            else if (!MultiplayerManager.instance.client.Started)
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("CANNOT DISPLAY CONTENT");
                GUI.contentColor = Color.white;
                GUILayout.Label("PhotonNetwork.IsConnected === true");
                GUILayout.Label("PhotonNetwork.InRoom === true");
                GUILayout.Label("Started === false");
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Turn Noclip (Triggers)"))
                {
                    MultiplayerManager.instance.gc.playerColider.isTrigger = true;
                }
                if (GUILayout.Button("Turn Noclip (No Triggers)"))
                {
                    MultiplayerManager.instance.gc.playerColider.enabled = false;
                }
                if (GUILayout.Button("X Noclip (Triggers)"))
                {
                    MultiplayerManager.instance.gc.playerColider.isTrigger = false;
                }
                if (GUILayout.Button("X Noclip (No Triggers)"))
                {
                    MultiplayerManager.instance.gc.playerColider.enabled = true;
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Speed (10)"))
                {
                    MultiplayerManager.instance.gc.player.walkSpeed = 10f;
                    MultiplayerManager.instance.gc.player.runSpeed = 16f;
                }
                if (GUILayout.Button("Speed (25)"))
                {
                    MultiplayerManager.instance.gc.player.walkSpeed = 25f;
                    MultiplayerManager.instance.gc.player.runSpeed = 31f;
                }
                if (GUILayout.Button("Speed (45)"))
                {
                    MultiplayerManager.instance.gc.player.walkSpeed = 45f;
                    MultiplayerManager.instance.gc.player.runSpeed = 51f;
                }
                if (GUILayout.Button("Speed (55)"))
                {
                    MultiplayerManager.instance.gc.player.walkSpeed = 55f;
                    MultiplayerManager.instance.gc.player.runSpeed = 61f;
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("0 Notebooks"))
                {
                    ClientSend.SendAction("notebook:none:0:" + (!Settings.GetBool("NoYCTP") ? "true" : "false"));
                }
                if (GUILayout.Button("2 Notebooks"))
                {
                    ClientSend.SendAction("notebook:none:2:" + (!Settings.GetBool("NoYCTP") ? "true" : "false"));
                }
                if (GUILayout.Button("7 Notebooks"))
                {
                    ClientSend.SendAction("notebook:none:7:" + (!Settings.GetBool("NoYCTP") ? "true" : "false"));
                }
                if (GUILayout.Button("Max Notebooks"))
                {
                    ClientSend.SendAction("notebook:none:" + MultiplayerManager.instance.gc.maxNotebooks + ":" + (!Settings.GetBool("NoYCTP") ? "true" : "false"));
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("+1 Anger"))
                {
                    if (MultiplayerManager.instance.isBaldi)
                    {
                        MultiplayerManager.instance.gc.baldiScrpt.GetAngry(1f, false);
                    }
                    else
                    {
                        MultiplayerManager.instance.gc.baldiScrpt.GetAngry(1f);
                    }
                }
                if (GUILayout.Button("+50 Anger"))
                {
                    if (MultiplayerManager.instance.isBaldi)
                    {
                        MultiplayerManager.instance.gc.baldiScrpt.GetAngry(50f, false);
                    }
                    else
                    {
                        MultiplayerManager.instance.gc.baldiScrpt.GetAngry(50f);
                    }
                }
                if (GUILayout.Button("-50 Anger"))
                {
                    if (MultiplayerManager.instance.isBaldi)
                    {
                        MultiplayerManager.instance.gc.baldiScrpt.GetAngry(-50f, false);
                    }
                    else
                    {
                        MultiplayerManager.instance.gc.baldiScrpt.GetAngry(-50f);
                    }
                }
                if (GUILayout.Button("-1 Anger"))
                {
                    if (MultiplayerManager.instance.isBaldi)
                    {
                        MultiplayerManager.instance.gc.baldiScrpt.GetAngry(-1f, false);
                    }
                    else
                    {
                        MultiplayerManager.instance.gc.baldiScrpt.GetAngry(-1f);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Get Captured"))
                {
                    MultiplayerManager.instance.client.GameOver();
                }
                if (GUILayout.Button("Escape!"))
                {
                    MultiplayerManager.instance.client.Escape();
                }
                if (GUILayout.Button("Annoy Sound (Baldi)"))
                {
                    ClientSend.PlaySoundClip("Baldi", "DarkSky_NOO");
                }
                if (GUILayout.Button("Apple"))
                {
                    MultiplayerManager.instance.gc.baldiScrpt.baldiPriority = MultiplayerManager.instance.gc.baldiScrpt.currentPriority;
                    MultiplayerManager.instance.gc.player.appleEat = true;
                    MultiplayerManager.instance.gc.UseApple();
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Disconnect"))
                {
                    PhotonNetwork.Disconnect();
                }
                if (GUILayout.Button("Leave Room"))
                {
                    PhotonNetwork.LeaveRoom();
                }
                if (GUILayout.Button("End Game (Jumpscare)"))
                {
                    MultiplayerManager.instance.client.photonView.RPC("PUN_Action", RpcTarget.AllViaServer, "captured.end", PhotonNetwork.LocalPlayer.UserId);
                }
                if (GUILayout.Button("End Game (Main Menu)"))
                {
                    MultiplayerManager.instance.client.photonView.RPC("PUN_Action", RpcTarget.AllViaServer, "end", PhotonNetwork.LocalPlayer.UserId);
                }
                GUILayout.EndHorizontal();
            }
        }
        else if (Toggle == 3)
        {
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2);
            // Iterate through the recorded logs.
            for (int i = 0; i < logs.Count; i++)
            {
                var log = logs[i];
                // Combine identical messages if collapse option is chosen.
                if (collapse)
                {
                    var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

                    if (messageSameAsPrevious)
                    {
                        continue;
                    }
                }
                GUILayout.Label("(" + log.date.ToString() + ") " + log.message);
            }
            GUILayout.EndScrollView();
            // Buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(clearLabel))
            {
                logs.Clear();
                rlogs.Clear();
            }
            noAudioEvents = GUILayout.Toggle(noAudioEvents, "No Audio/State Events", GUILayout.ExpandWidth(false));
            collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button(closeLabel))
        {
            show = false;
        }
        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(titleBarRect);
    }

    void OnEvent(EventData data)
    {
        int actorNr = data.Sender;
        Player p = null;
        if (actorNr > 0 && PhotonNetwork.CurrentRoom != null)
        {
            p = PhotonNetwork.CurrentRoom.GetPlayer(actorNr);
        }
        switch (data.Code)
        {
            case 1:
            case 2:
            case 3:
            case 5:
                // Movement unreliable packets, skip!
                break;
            case 6:
                if (noAudioEvents) break;
                rlogs.Add(new Log()
                {
                    message = $"6 Animation Event: {((object[])data.CustomData)[0]} | {((object[])data.CustomData)[1]} | {((object[])data.CustomData)[2]} | {((object[])data.CustomData)[3]}",
                    date = DateTime.Now.ToString()
                });
                logs = rlogs.ToList();
                logs.Reverse();
                break;
            case 8:
                Player player2 = PhotonNetwork.CurrentRoom.GetPlayer(data.Sender);
                rlogs.Add(new Log()
                {
                    message = $"7 Alarm Placement Event: Position {SchoolSaves.Utils.ToVector3((float[])data.CustomData)} by {player2.NickName} ({player2.ActorNumber}) : {player2.UserId}",
                    date = DateTime.Now.ToString()
                });
                logs = rlogs.ToList();
                logs.Reverse();
                break;
            case 9:
                Player player3 = PhotonNetwork.CurrentRoom.GetPlayer(data.Sender);
                rlogs.Add(new Log()
                {
                    message = $"8 Alarm Wine Event: {((object[])data.CustomData)[0]} | {((object[])data.CustomData)[1]} by {player3.NickName} ({player3.ActorNumber}) : {player3.UserId}",
                    date = DateTime.Now.ToString()
                });
                logs = rlogs.ToList();
                logs.Reverse();
                break;
            case EventCode.Join:
                if (p != null)
                {
                    rlogs.Add(new Log()
                    {
                        message = $"255 Player Join: {p} (UID: {p.UserId})",
                        date = DateTime.Now.ToString()
                    });
                    logs = rlogs.ToList();
                    logs.Reverse();
                }
                break;
            case EventCode.Leave:
                if (p != null)
                {
                    rlogs.Add(new Log()
                    {
                        message = $"254 Player Leave: {p} (UID: {p.UserId}) (Is Inactive? {p.IsInactive})",
                        date = DateTime.Now.ToString()
                    });
                    logs = rlogs.ToList();
                    logs.Reverse();
                }
                else
                {
                    rlogs.Add(new Log()
                    {
                        message = $"254 Player Leave: Can't be identified",
                        date = DateTime.Now.ToString()
                    });
                    logs = rlogs.ToList();
                    logs.Reverse();
                }
                break;
            case 200: // Pun event
                Hashtable rpcData = data.CustomData as Hashtable;

                string msg;

                try
                {
                    string inMethodName;
                    if (rpcData.ContainsKey((byte)5))
                    {
                        int rpcIndex = (byte)rpcData[(byte)5];  // LIMITS RPC COUNT
                        if (rpcIndex > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
                        {
                            throw new Exception("Could not find RPC with index: " + rpcIndex + ". Going to ignore! Check PhotonServerSettings.RpcList");
                        }
                        else
                        {
                            inMethodName = PhotonNetwork.PhotonServerSettings.RpcList[rpcIndex];
                        }
                    }
                    else
                    {
                        inMethodName = (string)rpcData[(byte)3];
                    }
                    if ((inMethodName == "PUN_PlayAudio" || inMethodName == "StateCharacterSet") && noAudioEvents)
                    {
                        return;
                    }

                    object[] arguments = null;
                    if (rpcData.ContainsKey((byte)4))
                    {
                        arguments = (object[])rpcData[(byte)4];
                    }

                    List<string> args = new List<string>();
                    if (arguments != null)
                    {
                        if (inMethodName == "PUN_Action")
                        {
                            args.Add(arguments[0].ToString());
                        }
                        else
                        {
                            foreach (object arg in arguments)
                            {
                                args.Add(arg.ToString());
                            }
                        }
                        msg = $"RPCReceive {inMethodName}: ({string.Join(',', args)})";
                    }
                    else
                    {
                        msg = $"RPCReceive {inMethodName}";
                    }
                }
                catch (System.Exception ex)
                {
                    msg = ex.Message;
                    Debug.LogException(ex);
                }

                // GET USER
                if (p != null)
                {
                    msg += $" - sent by {p} (UID: {p.UserId})";
                }
                else
                {
                    msg += " - sent by NULL";
                }

                rlogs.Add(new Log()
                {
                    message = msg,
                    date = DateTime.Now.ToString()
                });
                logs = rlogs.ToList();
                logs.Reverse();
                break;
        }
    }

    public void LogSentEvent(string inMethodName, RpcTarget target, params object[] arguments)
    {
        if ((inMethodName == "PUN_PlayAudio" || inMethodName == "StateCharacterSet") && noAudioEvents)
        {
            return;
        }
        string msg;
        try
        {
            List<string> args = new List<string>();
            if (arguments != null)
            {
                if (inMethodName == "PUN_Action")
                {
                    args.Add(arguments[0].ToString());
                }
                else
                {
                    foreach (object arg in arguments)
                    {
                        args.Add(arg.ToString());
                    }
                }
                msg = $"RPCSend {inMethodName}: ({string.Join(',', args)})";
            }
            else
            {
                msg = $"RPCSend {inMethodName}";
            }
        }
        catch (System.Exception ex)
        {
            msg = ex.Message;
            Debug.LogException(ex);
        }
        // GET TARGET
        msg += " - RpcTarget." + target.ToString();

        rlogs.Add(new Log()
        {
            message = msg,
            date = DateTime.Now.ToString()
        });
        logs = rlogs.ToList();
        logs.Reverse();
    }
}

public struct Log
{
    public string message;
    public string date;
}
#endif