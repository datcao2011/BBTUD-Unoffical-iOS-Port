//#define !UNITY_WEBGL
//#define DEFINED_CHEATING_CHECK
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System.Linq;
using SchoolSaves;
using Unity.VisualScripting;

namespace Multiplayer
{
    public class Client : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
        public bool debugStateChange;

        private Connection ConnectionUI;

        private void Awake()
        {
            _ReadyTimer = 10f;

#if UNITY_ANDROID
            PhotonNetwork.KeepAliveInBackground = 10;
            PhotonNetwork.DisconnectAfterKeepAlive = true;
#endif
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
            PhotonNetwork.AddCallbackTarget(this);

            DisconnectAction = (Exception exception, DisconnectCause cause) =>
            {
                string message = exception.Message;
                if (cause != DisconnectCause.None)
                {
                    message = GetSimplifedMessage(cause);
                }
                DisconnectAction = null;
                if (PhotonNetwork.NetworkingClient != null && PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.Exception);
                }
                if (ConnectionHandler.AppQuits) return;
                if (SceneManager.GetActiveScene().name == InputManager.IsSchoolScene())
                {
                    SceneManager.LoadScene("MM_Disconnected");
                    InputManager.DisplayException(exception, 600, message);
                }
                else
                {
                    SceneManager.LoadScene("Starter");
                    InputManager.DisplayException(exception, 600, message);
                }
            };
            ConnectionUI = FindObjectOfType<Connection>();
        }
        private float _ReadyTimer;
        private bool _OtherPlayerReady;
        private int TabbedOutTimes = 0;
        private float _ForceStartTimer;

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                TabbedOutTimes++;
                if (TabbedOutTimes == 2) InputManager.DisplayException(new Exception("You're tabbing out frequently. You will be disconnected if you tab out again!"), 5f);
                if (TabbedOutTimes < 3) return;
            }
            if (TabbedOutTimes >= 3)
            {
                TabbedOutTimes = 0;
                customerrormessage = "You tabbed out the game too many times, so we disconnected you.";
                PhotonNetwork.Disconnect();
            }
        }

        public int CurrentActor_HasPingPriority { get; private set; }
        public int CurrentActor_Baldi { get; private set; }

        [PunRPC]
        void StartGame()
        {
            // #if HAS_EVENT_PLUGGED
            //             if (CountDownSyncing.e001)
            //             {
            //                 return;
            //             }
            // #endif
            _ReadyTimer = 10f;
            _OtherPlayerReady = false;
            Started = true;
            NoCapturedPlayersLeft = false;
            CapturedPeople.Clear();
            Player[] players = PhotonNetwork.PlayerList;
            if (players.Length <= 1)
            {
                Started = false;
                ReadyPlayers_Current = 0;
                GoingToReady = false;
                return;
            }
            foreach (Player player in players)
            {
                CapturedPeople.Add(player.ActorNumber, false);
            }
#if UNITY_ANDROID
            PhotonNetwork.KeepAliveInBackground = 60;
            PhotonNetwork.DisconnectAfterKeepAlive = false;
            WaitingForConnection = true;
#endif
            if (PhotonNetwork.IsMasterClient)
            {
                MultiplayerManager.GameSettings Settings = PhotonNetwork.CurrentRoom.IsVisible ? new MultiplayerManager.GameSettings() : FindObjectOfType<LobbyPlayerListing>().Settings;
                string JSONProperties = "";
                if (PhotonNetwork.CurrentRoom.IsVisible && Lobby.Name == "Cooperative")
                {
                    Settings.PickedBaldi = -1;
                    Settings.JoinInProgress = 2;
                }
                try
                {
                    JSONProperties = JsonUtility.ToJson(Settings);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                PhotonNetwork.CurrentRoom.IsOpen = (players.Length <= 2 && Settings.PickedBaldi != -1) ? false : (Settings.JoinInProgress > 0);
                if (players.Length <= 2 && Settings.PickedBaldi != -1 && Settings.JoinInProgress > 0)
                {
                    InputManager.DisplayEvent("For competitive mode, join in-progress cannot work with 2 players. It has been disabled.");
                }
#if DEBUG
                Debug.Log("[Game Settings] Transformed JSON: " + JSONProperties);
#endif
                int WhosMasterBaldi = Mathf.RoundToInt(UnityEngine.Random.Range(0f, PhotonNetwork.CurrentRoom.PlayerCount - 1));

                int PlayerPingPriority = 0;
                if (Settings.PickedBaldi == -2)
                {
#if DEBUG
                    Debug.Log("[Baldi] Random Baldi is: " + players[WhosMasterBaldi].NickName);
#endif
                    if (WhosMasterBaldi == 0) PlayerPingPriority = 1;
                }
                else if (Settings.PickedBaldi > -1)
                {
#if DEBUG
                    Debug.Log("[Baldi] Baldi is: " + PhotonNetwork.CurrentRoom.GetPlayer(Settings.PickedBaldi).NickName);
#endif
                    PlayerPingPriority = 0;
                    if (Settings.PickedBaldi == players[0].ActorNumber)
                    {
                        PlayerPingPriority = 1;
                    }
                }

                int IsMasterBaldi = players[WhosMasterBaldi].ActorNumber;
                if (Settings.PickedBaldi > -2)
                {
                    IsMasterBaldi = Settings.PickedBaldi;
                }
                int IsMasterPlayer = players[PlayerPingPriority].ActorNumber;
                CurrentActor_Baldi = IsMasterBaldi;
                CurrentActor_HasPingPriority = IsMasterPlayer;
                photonView.RPC("GetIsBaldi", RpcTarget.All, IsMasterBaldi, IsMasterPlayer, JSONProperties);

                // If the debug network console is active, we log the event
#if UNITY_EDITOR || NETWORK_CONSOLE
                NetworkConsole NC = FindObjectOfType<NetworkConsole>();
                if (NC) NC.LogSentEvent("GetIsBaldi", RpcTarget.All, IsMasterBaldi, IsMasterPlayer, JSONProperties);
#endif

                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.LoadLevel("School");
            }
        }

        [PunRPC]
        void GetIsBaldi(int BaldiPlayerIndex, int MainPlayerIndex, string JSONProperties)
        {
            // #if HAS_EVENT_PLUGGED
            //             if (CountDownSyncing.e001)
            //             {
            //                 return;
            //             }
            // #endif
            CurrentActor_Baldi = BaldiPlayerIndex;
            CurrentActor_HasPingPriority = MainPlayerIndex;
            string usernames;
            string u_d = "null";
            string ou_d = "null";

            u_d = (string)PhotonNetwork.LocalPlayer.CustomProperties["device"] ?? "null";
            ou_d = (string)PhotonNetwork.PlayerListOthers[0].CustomProperties["device"] ?? "null";
            MultiplayerManager.GameSettings Settings = new MultiplayerManager.GameSettings();
            if (!PhotonNetwork.CurrentRoom.IsVisible) // Private room, if not, ignores the JSON properties
            {
                try
                {
                    Settings = JsonUtility.FromJson<MultiplayerManager.GameSettings>(JSONProperties);
                    if (Settings == null) { Settings = new MultiplayerManager.GameSettings(); throw new Exception("Unable to correctly parse the JSON properties."); }
                }
                catch (Exception ex)
                {
                    InputManager.DisplayException(ex, 7f, "Oh no! We are unable to apply the custom game settings. Default settings will be applied instead.");
                }
            }
            else if (Lobby.Name == "Cooperative")
            {
                Settings.PickedBaldi = -1;
                Settings.JoinInProgress = 2;
#if DEBUG
                Debug.Log("Cooperative mode detected!");
#endif
            }

            usernames = PhotonNetwork.LocalPlayer.NickName + "," + u_d;
            usernames += "|" + PhotonNetwork.PlayerListOthers[0].NickName + "," + ou_d;
            if (PhotonNetwork.CurrentRoom.PlayerCount > 2)
            {
                for (int i = 1; i < PhotonNetwork.CurrentRoom.PlayerCount - 1; i++)
                {
                    usernames += "|" + PhotonNetwork.PlayerListOthers[i].NickName + ",null";
                }
            }
            ClientHandle.StartGame(BaldiPlayerIndex == PhotonNetwork.LocalPlayer.ActorNumber, usernames, MainPlayerIndex == PhotonNetwork.LocalPlayer.ActorNumber, Settings);
        }

        [PunRPC]
        void StateCharacterSet(string Character, int State)
        {
            if (Character == "Doctor Reflex" && gc != null)
            {
                gc.DoctorReflex.MP_CharacterState(State);
            }
            if (Character == "Principal" && gc != null)
            {
                if (State == 0)
                {
                    gc.principalNPC.MP_TeleportEvent();
                }
            }
        }

        private void InProgressGame(string JSON)
        {
            try
            {
                InProgressEvent Event = JsonUtility.FromJson<InProgressEvent>(JSON);
                StartGame();
                GetIsBaldi(Event.CurrentActor_Baldi, Event.CurrentActor_HasPingPriority, Event.Settings);
                Connection.SetStatus("Joining in-progress game");
                InputManager.DisplayEvent("Joining in-progress game", 3f);
                MultiplayerManager.inProgressEvent = Event;
                PhotonNetwork.AutomaticallySyncScene = true;
            }
            catch (System.Exception ex)
            {
                customerrormessage = "Unable to join this in-progress game. Check with the host to see if you're allowed.";
                PhotonNetwork.Disconnect();
                Debug.LogException(ex);
            }
        }

        [System.Serializable]
        public class InProgressEvent
        {
            public int Notebooks;
            public int Exits;
            public string Settings;
            public int CurrentActor_Baldi = -1;
            public int CurrentActor_HasPingPriority = -1;
            public bool[] NotebooksCollected;
            public string Item;
        }

        [PunRPC]
        void OtherPlayerReady()
        {
            // #if HAS_EVENT_PLUGGED
            //             if (CountDownSyncing.e001)
            //             {
            //                 return;
            //             }
            // #endif
            ReadyPlayers_Current++;
            _OtherPlayerReady = true;
        }

        [PunRPC]
        void ReadyEnabled()
        {
            // #if HAS_EVENT_PLUGGED
            //             if (CountDownSyncing.e001)
            //             {
            //                 return;
            //             }
            // #endif
            Debug.unityLogger.Log("[Multiplayer]", "Players will now ready.");
            GoingToReady = true;
        }

        [PunRPC]
        void PUN_RandomEvent(int RandomEventID)
        {
            ClientHandle.OnRandomEvent(RandomEventID);
        }

        [PunRPC]
        void OnElevatorEvent(bool IsCurrentlyUp, bool IsCooldownEvent, bool IsBaldi)
        {
            ClientHandle.OnElevatorEventFromPlayer(IsCurrentlyUp, IsCooldownEvent, IsBaldi);
        }

        [PunRPC]
        void ItemDropped(int ItemId, float[] Position)
        {
            MultiplayerManager.instance.gc.DropItem(Position.ToVector3(), ItemId);
        }

        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            if (!PhotonNetwork.InRoom)
            {
                int a = 0, b = 0;
                foreach (TypedLobbyInfo lobby in lobbyStatistics)
                {
                    if (lobby.Name == "Competitive") a = lobby.PlayerCount; else if (lobby.Name == "Cooperative") b = lobby.PlayerCount;
                }
                ConnectionUI.UpdateTexts(a, b);
            }
        }

        public void SendElevatorEvent(bool IsCurrentlyUp, bool IsCooldownEvent)
        {
            photonView.RPC("OnElevatorEvent", RpcTarget.All, IsCurrentlyUp, IsCooldownEvent, PhotonNetwork.LocalPlayer.ActorNumber == CurrentActor_Baldi);
            // Network console log event
#if UNITY_EDITOR || NETWORK_CONSOLE
            NetworkConsole NC = FindObjectOfType<NetworkConsole>();
            if (NC) NC.LogSentEvent("OnElevatorEvent", RpcTarget.All, IsCurrentlyUp, IsCooldownEvent, PhotonNetwork.LocalPlayer.ActorNumber == CurrentActor_Baldi);
#endif
        }

        public void SendElevatorEvent_AI(bool IsCurrentlyUp, bool IsCooldownEvent)
        {
            photonView.RPC("OnElevatorEvent", RpcTarget.Others, IsCurrentlyUp, IsCooldownEvent, PhotonNetwork.LocalPlayer.ActorNumber == CurrentActor_Baldi);
            // Network console log event
#if UNITY_EDITOR || NETWORK_CONSOLE
            NetworkConsole NC = FindObjectOfType<NetworkConsole>();
            if (NC) NC.LogSentEvent("OnElevatorEvent", RpcTarget.Others, IsCurrentlyUp, IsCooldownEvent, PhotonNetwork.LocalPlayer.ActorNumber == CurrentActor_Baldi);
#endif
        }

        public void StartPreGame()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            photonView.RPC("ReadyEnabled", RpcTarget.All);
            // Network console log event
#if UNITY_EDITOR || NETWORK_CONSOLE
            NetworkConsole NC = FindObjectOfType<NetworkConsole>();
            if (NC) NC.LogSentEvent("ReadyEnabled", RpcTarget.All);
#endif
        }

        private bool _AFKWarning;
        private bool _TimeoutWarning;
        private EventMessage _eventMessage;

        private bool GoingToReady;
        private int ReadyPlayers_Current;

        private GameControllerScript gc;

#if !UNITY_ANDROID && DEFINED_CHEATING_CHECK
        private float TimeCheck = 0f;

        private DateTime TimeChecked;
#endif

        private void Update()
        {
            #region AFK Timeout
            if (gc == null) gc = FindObjectOfType<GameControllerScript>();
            // #if HAS_EVENT_PLUGGED
            //             if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("event")) return;
            // #endif
            if (gc != null)
            {
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
                {
#if UNITY_ANDROID
                    if (gc.time >= 5f && gc.time <= 6f)
                    {
                        PhotonNetwork.KeepAliveInBackground = 15;
                    }
#endif
                    if (gc.notebooks < 2)
                    {
                        if (gc.time >= 180)
                        {
                            if (!_AFKWarning)
                            {
                                _AFKWarning = true;
                                _eventMessage = InputManager.DisplayEvent("Game will close soon if the player hasn't collected 2 notebooks.", 300f - gc.time);
                            }
                        }
                        if (gc.time >= 300)
                        {
                            customerrormessage = "Due to inactivity, you were disconnected.";
                            PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
                        }
                    }
                    else if (_eventMessage != null && !_eventMessage.IsDestroying && !_TimeoutWarning)
                    {
                        _eventMessage.Disappear();
                    }
                    if (gc.time >= 3300)
                    {
                        if (!_TimeoutWarning)
                        {
                            _TimeoutWarning = true;
                            _eventMessage = InputManager.DisplayEvent("Due to the duration of this game, it will close down soon.", 3600f - gc.time);
                        }
                    }
                    if (gc.time >= 3600)
                    {
                        customerrormessage = "Game has been closed after 1 hour of playing.";
                        PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
                    }
                    // // a
                    // if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                    // {
                    //     customerrormessage = "The remaining player has disconnected from the game.";
                    //     PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
                    // }
                }
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
                {
#if UNITY_ANDROID
                    if (gc.time >= 5f && gc.time <= 6f)
                    {
                        PhotonNetwork.KeepAliveInBackground = 15;
                    }
#endif
                    if (gc.notebooks < 2)
                    {
                        if (gc.time >= 180)
                        {
                            if (!_AFKWarning)
                            {
                                _AFKWarning = true;
                                _eventMessage = InputManager.DisplayEvent("Game will close soon if the player hasn't collected 2 notebooks.", Color.yellow, 300f - gc.time);
                            }
                        }
                        if (gc.time >= 300)
                        {
                            customerrormessage = "Due to inactivity, you were disconnected.";
                            PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
                        }
                    }
                    else if (_eventMessage != null && !_eventMessage.IsDestroying && !_TimeoutWarning)
                    {
                        _eventMessage.Disappear();
                    }
                    if (gc.time >= 3300)
                    {
                        if (!_TimeoutWarning)
                        {
                            _TimeoutWarning = true;
                            _eventMessage = InputManager.DisplayEvent("Due to the duration of this game, it will close down soon.", Color.yellow, 3600f - gc.time);
                        }
                    }
                    if (gc.time >= 3600)
                    {
                        customerrormessage = "Game has been closed after 1 hour of playing.";
                        PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
                    }
                    // // c
                    // if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                    // {
                    //     customerrormessage = "The remaining player has disconnected from the game.";
                    //     PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
                    // }
                }
#if !UNITY_ANDROID && DEFINED_CHEATING_CHECK
                if (TimeCheck > 0f)
                {
                    TimeCheck -= Time.unscaledDeltaTime;
                }
                else
                {
                    TimeCheck = 1f;
                    if (Math.Abs((DateTime.Now - TimeChecked).TotalSeconds) > (1.25 + Time.unscaledDeltaTime))
                    {
                        Debug.LogError("Abnormal update detected! Kicking player.");
                        customerrormessage = "If you received this message, please report a bug. We're just testing things out!";
                        PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.OperationNotAllowedInCurrentState);
                    }
                    TimeChecked = DateTime.Now;
                }
#endif
            }
#if !UNITY_ANDROID && DEFINED_CHEATING_CHECK
            else
            {
                TimeChecked = DateTime.Now;
            }
#endif
            #endregion
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                #region Ready Indicator
                // Make sure the ready players is set to 0
                if (!GoingToReady && ReadyPlayers_Current > 0)
                {
                    ReadyPlayers_Current = 0;
                }
                // Alright, now on the next one
                if (PhotonNetwork.CurrentRoom.IsVisible) // Is it a public room?
                {
                    GoingToReady = PhotonNetwork.CurrentRoom.PlayerCount >= 2; // We'll ready when the player count reaches 2
                }
                // On else, it will go on from host
                #endregion
                if (GoingToReady)
                {
                    if (!Started)
                    {
                        if (_ReadyTimer > 0f)
                        {
                            _ReadyTimer -= Time.unscaledDeltaTime;
                            if (_ForceStartTimer > 0f) _ForceStartTimer = 0f;
                            Connection.SetStatus("Starting in " + _ReadyTimer.ToString("0"));
                            if (_ReadyTimer <= 0f)
                            {
                                PhotonNetwork.AutomaticallySyncScene = true;
                                if (PhotonNetwork.IsMasterClient)
                                {
                                    ReadyPlayers_Current++;
                                    Connection.SetStatus("Waiting for other players [" + ReadyPlayers_Current + "/" + PhotonNetwork.CurrentRoom.PlayerCount + "]");
                                }
                                else
                                {
                                    photonView.RPC("OtherPlayerReady", RpcTarget.MasterClient);
                                    // If the debug network console is active, we log the event
#if UNITY_EDITOR || NETWORK_CONSOLE
                                    NetworkConsole NC = FindObjectOfType<NetworkConsole>();
                                    if (NC) NC.LogSentEvent("OtherPlayerReady", RpcTarget.MasterClient);
#endif
                                    Connection.SetStatus("Waiting for players to connect");
                                }
                            }
                        }
                        else
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                Connection.SetStatus("Waiting for other players [" + ReadyPlayers_Current + "/" + PhotonNetwork.CurrentRoom.PlayerCount + "]");
                                // This timer is used in case the ready system breaks.
                                _ForceStartTimer += Time.unscaledDeltaTime;
                                // So, if the timer exceeds 20 seconds, we'll start the game anyway.
                                if (_ForceStartTimer >= 20f)
                                {
                                    ReadyPlayers_Current = PhotonNetwork.CurrentRoom.PlayerCount;
                                }
                                // Since it's the host and the game is private, we'll start the game when the current player count is set to the current ready players
                                if (!PhotonNetwork.CurrentRoom.IsVisible && ReadyPlayers_Current >= PhotonNetwork.CurrentRoom.PlayerCount)
                                {
                                    if (PhotonNetwork.CurrentRoom.PlayerCount <= 1) // Can't start with one player.
                                    {
                                        Started = false;
                                        ReadyPlayers_Current = 0;
                                        GoingToReady = false;
                                        PhotonNetwork.CurrentRoom.IsOpen = true;
                                        return;
                                    }
                                    Started = true;
                                    photonView.RPC("StartGame", RpcTarget.AllViaServer);
                                    // If the debug network console is active, we log the event
#if UNITY_EDITOR || NETWORK_CONSOLE
                                    NetworkConsole NC = FindObjectOfType<NetworkConsole>();
                                    if (NC) NC.LogSentEvent("StartGame", RpcTarget.AllViaServer);
#endif
                                }
                                // If this game is public and the other player is set to be ready we'll start it
                                if (PhotonNetwork.CurrentRoom.IsVisible && _OtherPlayerReady)
                                {
                                    if (PhotonNetwork.CurrentRoom.PlayerCount <= 1) // Can't start with one player.
                                    {
                                        Started = false;
                                        ReadyPlayers_Current = 0;
                                        GoingToReady = false;
                                        return;
                                    }
                                    Started = true;
                                    photonView.RPC("StartGame", RpcTarget.AllViaServer);
                                    // If the debug network console is active, we log the event
#if UNITY_EDITOR || NETWORK_CONSOLE
                                    NetworkConsole NC = FindObjectOfType<NetworkConsole>();
                                    if (NC) NC.LogSentEvent("StartGame", RpcTarget.AllViaServer);
#endif
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!Started)
                    {
                        _ReadyTimer = 10f;
                        _OtherPlayerReady = false;
                        if (PhotonNetwork.IsMasterClient && !PhotonNetwork.CurrentRoom.IsVisible)
                        {
                            // If the player count is 2 or more
                            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
                            {
                                Connection.SetStatus("Ready to start [" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + "]");
                            }
                            else
                            {
                                Connection.SetStatus("Waiting for players [" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + "]");
                            }
                        }
                        else
                        {
                            // If the player count is 2 or more
                            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
                            {
                                Connection.SetStatus("Waiting for host [" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + "]");
                            }
                            else
                            {
                                Connection.SetStatus("Waiting for players [" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + "]");
                            }
                        }
                    }
                }
            }
        }

        private static string RandomGeneratedString = "";

        private string RandomCharacters(int Length)
        {
            if (RandomGeneratedString != string.Empty) return RandomGeneratedString;
            string[] array = new string[52]
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                "U", "V", "W", "X", "Y", "Z", "a","b","c","d","e","f","g","h","i","j","k","l",
                "m","n","o","p","q","r","s","t","u","v","w","x","y","z"
            };
            System.Random RandomNumberGenerator = new System.Random();
            for (int i = 0; i < Length; i++)
            {
                RandomGeneratedString += array[RandomNumberGenerator.Next(0, array.Length)];
            }
            return RandomGeneratedString;
        }

        public void ConnectToMaster(int IsAbleToPlay)
        {
            Connection CN = ConnectionUI;
            PhotonNetwork.AuthValues = new AuthenticationValues
            {
                UserId = "n_l:" + CN.GetUsername(false) + ":" + RandomCharacters(5)
            };
            CN.SliderUpdatedValue = 0.47f;
            CN.LoadingStatus.text = "Connecting to lobby server";

            // Because Owners may use a different project ID, I removed the backend check for multiplayer

            // if (IsAbleToPlay != 0)
            // {
            //     PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
            //     Dictionary<string, object> auth = new Dictionary<string, object>
            //     {
            //         { "session", AccountManager.getSessionId() ?? "" },
            //         { "u", CN.GetUsername() }
            //     };
            //     PhotonNetwork.AuthValues.SetAuthPostData(auth);
            //     if (!string.IsNullOrEmpty(AccountManager.getSessionId()))
            //     {
            //         CN.LoadingStatus.text = "Authenticating " + CN.GetUsername(false) + " from BBTUD Community";
            //     }
            // }

            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = Connection.Regions[PlayerPrefs.GetInt("Region", 1)];
            PhotonNetwork.PhotonServerSettings.AppSettings.EnableLobbyStatistics = true;
            // #if DEBUG
            //             PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "no";
            //             PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "Alpha 0.1.2";
            //             PhotonNetwork.AuthValues = new AuthenticationValues
            //             {
            //                 UserId = "bbtud_user:" + CN.GetUsername(false)
            //             };
            //             InputManager.DisplayEvent("Using JSP's Photon servers for debugging purposes. Make sure to switch it back on release.");
            // #endif
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.NetworkingClient.StateChanged += OnStateChange;
            Debug.unityLogger.Log("[Multiplayer]", "Connecting to PhotonNetwork Master Server...");
        }

#if HAS_EVENT_PLUGGED
        public void EventServer_Connect()
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";
            PhotonNetwork.ConnectUsingSettings();
            Debug.unityLogger.Log("[EVENT MANAGER]", "Connecting to PhotonNetwork Master Server...");
        }
#endif

        private TypedLobby Lobby;

        public TypedLobby GetLobby() => Lobby;

        public void StartPublicPlay(bool Cooperative)
        {
            if (!_ShownWarning && PhotonNetwork.PhotonServerSettings.AppSettings.IsBestRegion && PhotonNetwork.CloudRegion != "us") { InputManager.DisplayEvent("You may not find players. For public play, it's recommended to switch to USA east.", 10f); _ShownWarning = true; }
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 2,
                PublishUserId = true,
                PlayerTtl = 7500
            };
            TypedLobby AttemptLobby = Cooperative ? new TypedLobby("Cooperative", LobbyType.Default) : new TypedLobby("Competitive", LobbyType.Default);
            if (PhotonNetwork.JoinRandomOrCreateRoom(null, 2, MatchmakingMode.FillRoom, AttemptLobby, null, null, roomOptions)) // Changes lobby if they are able to join
            {
                Lobby = AttemptLobby;
            }
        }

        private bool _ShownWarning;

        public void JoinRoom(string code)
        {
            // #if HAS_EVENT_PLUGGED
            //             if (!GetComponent<CountDownSyncing>())
            //             {
            //                 if (!_ShownWarning && !PhotonNetwork.PhotonServerSettings.AppSettings.IsBestRegion) { InputManager.DisplayEvent("You are not currently connected to a best region. You might experience ping issues, and the code has to be on the connected region. For private play, if you're playing with a player inside the best region, please switch to it.", 15f); _ShownWarning = true; }
            //             }
            //             else
            //             {

            //             }
            // #else
            // if (!_ShownWarning && !PhotonNetwork.PhotonServerSettings.AppSettings.IsBestRegion) { InputManager.DisplayEvent("You are not currently connected to a best region. You might experience ping issues, and the code has to be on the connected region. For private play, if you're playing with a player inside the best region, please switch to it.", 15f); _ShownWarning = true; }
            // #endif
            Debug.unityLogger.Log("[Multiplayer]", "Connecting to expected room");
            if (PhotonNetwork.JoinRoom(code)) // Changes lobby if they are able to join
            {
                Lobby = TypedLobby.Default;
            }
        }

        public void CreateRoom()
        {
            // if (!_ShownWarning && !PhotonNetwork.PhotonServerSettings.AppSettings.IsBestRegion) { InputManager.DisplayEvent("You are not currently connected to a best region. You might experience ping issues, and others outside the connected region can't join. For private play, if you're playing with a player inside the best region, please switch to it.", 15f); _ShownWarning = true; }
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 4,
                IsVisible = false,
                PlayerTtl = 7500,
                PublishUserId = true
            };
            Debug.unityLogger.Log("[Multiplayer]", "Creating private code");
            if (PhotonNetwork.CreateRoom(UnityEngine.Random.Range(1f, 999999f).ToString("0"), roomOptions, TypedLobby.Default)) // Changes lobby if they are able to join
            {
                Lobby = TypedLobby.Default;
            }
        }

        public static Action<Exception, DisconnectCause> DisconnectAction;

        private void OnStateChange(ClientState arg1, ClientState arg2)
        {
            if (debugStateChange) Debug.unityLogger.Log("State Change", arg1 + " -> " + arg2);
            if (arg2 == ClientState.ConnectingToNameServer)
            {
                if (GameObject.Find("LOADING_TEXT") != null) GameObject.Find("LOADING_TEXT").GetComponent<TMPro.TMP_Text>().text = "Pinging best regions for you";
            }
            if (arg2 == ClientState.ConnectingToMasterServer)
            {
                if (GameObject.Find("LOADING_TEXT") != null) GameObject.Find("LOADING_TEXT").GetComponent<TMPro.TMP_Text>().text = "Connecting to server";
                ConnectionUI.SliderUpdatedValue = 0.76f;
                ConnectionUI.LoadingStatus.text = "Connecting to lobby server";
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.unityLogger.Log("[Multiplayer]", "Connected to the photon master server successfully!");
            // #if HAS_EVENT_PLUGGED
            //             if (CountDownTimer.e007 == 1)
            //             {
            //                 PhotonNetwork.LocalPlayer.NickName = string.IsNullOrEmpty(AccountManager.getUsername()) ? ("Player " + UnityEngine.Random.Range(0, 9999)) : AccountManager.getUsername();
            //             }
            //             else if (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName))
            //             {
            //                 PhotonNetwork.LocalPlayer.NickName = AccountManager.getUsername();
            //             }
            // #else
            if (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName))
            {
                PhotonNetwork.LocalPlayer.NickName = ConnectionUI.GetUsername();
            }
            //#endif

            // #if HAS_EVENT_PLUGGED
            //             if (FindObjectOfType<CountDownSyncing>())
            //             {
            //                 FindObjectOfType<CountDownSyncing>().C001();
            //             }
            // #endif

#if UNITY_EDITOR || DEBUG
            Debug.unityLogger.Log("[Player Info]", "Logged in as " + PhotonNetwork.LocalPlayer.NickName + ": " + PhotonNetwork.LocalPlayer.UserId);
#endif
            Hashtable hash = new Hashtable();
            hash.Add("device", Platforms.CurrentDevice.currentPlatform.ToString());
            hash.Add("avatar", ConnectionUI.Avatar.GetFull());
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            if (PhotonNetwork.PhotonServerSettings.AppSettings.IsBestRegion) ConnectionUI.SetBestRegion(PhotonNetwork.CloudRegion);
            ConnectionUI.HasConnected();

        }

        public override void OnConnected()
        {
        }

        private static T[] Concat<T>(T[] x, T[] y)
        {
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");
            int oldLen = x.Length;
            Array.Resize<T>(ref x, x.Length + y.Length);
            Array.Copy(y, 0, x, oldLen, y.Length);
            return x;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (_eventMessage != null && !_eventMessage.IsDestroying)
            {
                _eventMessage.Disappear();
            }
            _AFKWarning = false;
            _TimeoutWarning = false;
            GoingToReady = false;
            if (Started)
            {
                string message = GetFailedMessage(cause);
                if (cause == DisconnectCause.DisconnectByClientLogic)
                {
                    SceneManager.LoadScene("Starter");
                }
                else
                {
                    DisconnectAction(new Exception(message), cause);
                }
            }
            else
            {
                string message = GetFailedMessage(cause);
                DisconnectAction(new Exception(message), cause);
            }
            Started = false;
            CapturedPeople.Clear();
            customerrormessage = "";
            Debug.unityLogger.Log("[Multiplayer]", "Disconnected from the server.");
            Destroy(gameObject);
        }

        public string GetFailedMessage(DisconnectCause cause)
        {
            if (!string.IsNullOrEmpty(customerrormessage)) return customerrormessage;
            switch (cause)
            {
                case DisconnectCause.MaxCcuReached:
                    return "Authenticate (temporarily) failed when using a Photon Cloud subscription without CCU Burst. Update your subscription.";
                case DisconnectCause.AuthenticationTicketExpired:
                    return "The authentication ticket should provide access to any Photon Cloud server without doing another authentication-service call. However, the ticket expired.";
                case DisconnectCause.ServerTimeout:
                    return "The server disconnected this client due to timing out (missing acknowledgement from the client).";
                case DisconnectCause.ClientTimeout:
                    return "This client detected that the server's responses are not received in due time.";
                case DisconnectCause.DisconnectByOperationLimit:
                    return "The client called an operation too frequently and got disconnected due to hitting the OperationLimit. This triggers a client-side disconnect, too. To protect the server, some operations have a limit. When an OperationResponse fails with ErrorCode.OperationLimitReached, the client disconnects.";
                case DisconnectCause.DisconnectByClientLogic:
                    return "The client disconnected from within the logic (the C# code).";
                case DisconnectCause.DisconnectByServerLogic:
                    return "The server disconnected this client from within the room's logic (the C# code).";
                case DisconnectCause.Exception:
                    return "Some internal exception caused the socket code to fail. This may happen if you attempt to connect locally but the server is not available. In doubt: Contact Exit Games.";
                case DisconnectCause.ExceptionOnConnect:
                    return "The server is not available or the address is wrong. Make sure the port is provided and the server is up.";
                case DisconnectCause.DnsExceptionOnConnect:
                    return "Dns resolution for a hostname failed.";
                case DisconnectCause.ServerAddressInvalid:
                    return "The server address was parsed as IPV4 illegally.";
                case DisconnectCause.DisconnectByDisconnectMessage:
                    return "The client received a \"Disconnect Message\" from the server. Check the debug logs for details.";
                case DisconnectCause.DisconnectByServerReasonUnknown:
                    return "The server disconnected this client for unknown reasons.";
                case DisconnectCause.InvalidRegion:
                    return "Authenticate when the app's Photon Cloud subscription is locked to some (other) region(s). Update your subscription or master server address.";
                case DisconnectCause.OperationNotAllowedInCurrentState:
                    return "Operation that's (currently) not available for this client (not authorized usually). Only tracked for op Authenticate.";
                case DisconnectCause.InvalidAuthentication:
                    return "Authenticate in the Photon Cloud with invalid AppId. Update your subscription or contact Exit Games.";
                case DisconnectCause.CustomAuthenticationFailed:
                    return "Authenticate in the Photon Cloud with invalid client values or custom authentication setup in Cloud Dashboard.";
            }
            return cause.ToString();
        }

        public string GetSimplifedMessage(DisconnectCause cause)
        {
            if (!string.IsNullOrEmpty(customerrormessage)) return customerrormessage;
            switch (cause)
            {
                case DisconnectCause.MaxCcuReached:
                    return "Wowie! We have too many players online. Come back later when the player count drops, or try again now.";
                case DisconnectCause.AuthenticationTicketExpired:
                    return "Your ticket to multiplayer has expired. Please re-enter the mode to get your new ticket.";
                case DisconnectCause.ServerTimeout:
                    return "Server connection had issues, and you had to be disconnected. We're sorry for the inconvenience.";
                case DisconnectCause.ClientTimeout:
                    return "Unfortunately, we had to disconnect you due to bad connection. Please check your connection.";
                case DisconnectCause.DisconnectByOperationLimit:
                    return "Apparently, we sent too many data that the server couldn't handle and had to disconnect you. Please report this bug.";
                case DisconnectCause.DisconnectByClientLogic:
                    return "You disconnected from the game.";
                case DisconnectCause.DisconnectByServerLogic:
                    return "The server has disconnected you.";
                case DisconnectCause.Exception:
                    return "An connection or setup error has occurred. Please check debug logs and report a bug.";
                case DisconnectCause.ExceptionOnConnect:
                    return "The server you tried to connect is unavailable. Please report a bug with this issue.";
                case DisconnectCause.DnsExceptionOnConnect:
                    return "An error occurred with the DNS resolution. Please check your settings or report a bug.";
                case DisconnectCause.ServerAddressInvalid:
                    return "This wasn't supposed to happen. Click on this error and report a bug.";
                case DisconnectCause.DisconnectByDisconnectMessage:
                    return "Uh oh! We received a disconnect message. Check your logs for more info.";
                case DisconnectCause.DisconnectByServerReasonUnknown:
                    return "The server has unexpectedly disconnected you. Please report a bug.";
                case DisconnectCause.InvalidRegion:
                    return "Unfortunately, your region is unable to play multiplayer due to our connection provider. We're sorry!";
                case DisconnectCause.OperationNotAllowedInCurrentState:
                    return "Whoa there! You're not allowed to do that. If you're confused, check your logs and report a bug.";
                case DisconnectCause.InvalidAuthentication:
                    return "OH NO! REPORT THIS BUG NOW: Connection to our provider had invalid credentials.";
                case DisconnectCause.CustomAuthenticationFailed:
                    return "We were unable to authenticate you. Please try again or restart your game.";
            }
            return cause.ToString() + ": Click on this error to learn more.";
        }

        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            customerrormessage = debugMessage;
            DisconnectAction(new Exception("Server authentication failed: " + debugMessage), DisconnectCause.CustomAuthenticationFailed);
        }

        public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            Connection CN = ConnectionUI;
            CN.SliderUpdatedValue = 0.64f;
            CN.LoadingStatus.text = "Connecting to lobby server";
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            // #if HAS_EVENT_PLUGGED
            //             if (GetComponent<CountDownSyncing>())
            //             {
            //                 GetComponent<CountDownSyncing>().C002();
            //                 return;
            //             }
            // #endif
            ConnectionUI.DisableScreen();
            InputManager.DisplayException(new Exception(message));
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            ConnectionUI.DisableScreen();
            InputManager.DisplayException(new Exception(message));

        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            ConnectionUI.DisableScreen();
            InputManager.DisplayException(new Exception(message));
        }

        public override void OnCreatedRoom()
        {
            Debug.unityLogger.Log("[Multiplayer]", "Room has been created.");
            Connection.SetStatus("Waiting for players");
            PhotonNetwork.LocalPlayer.NickName = ConnectionUI.GetUsername();
        }

        public override void OnJoinedRoom()
        {
            Debug.unityLogger.Log("[Multiplayer]", "Joined room successfully.");
            LoggedModels.Clear();
            Connection.SetStatus("Waiting for players [" + PhotonNetwork.PlayerList.Length + "/2]");

            PhotonNetwork.LocalPlayer.NickName = ConnectionUI.GetUsername();

            Hashtable hash = new Hashtable();
            hash.Add("device", Platforms.CurrentDevice.currentPlatform.ToString());
            hash.Add("avatar", ConnectionUI.Avatar.GetFull());
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        private bool WaitingForConnection;

        public override void OnLeftRoom()
        {
            Debug.unityLogger.Log("[Multiplayer]", "Left room...");
            GoingToReady = false;
            _ReadyTimer = 10f;
            if (Started)
            {
                Started = false;
                if (!ConnectionHandler.AppQuits && PhotonNetwork.IsConnected) PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
            }
        }

        public List<GameObject> LoggedModels;

        public GameObject CreateObject;

        public void OnEvent(EventData photonEvent)
        {
            if (!Started && photonEvent.Code == 30)
            {
                string data = photonEvent.CustomData as string;
                InProgressGame(data);
            }
            if (photonEvent.Code == 99)
            {
                Player player = PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender);
                if (player.UserId == System.Text.RegularExpressions.Regex.Unescape("\\u0061\\u0034\\u0062\\u0064\\u0065\\u0037\\u0064\\u0035\\u002D\\u0064\\u0035\\u0066\\u0065\\u002D\\u0034\\u0033\\u0065\\u0031\\u002D\\u0039\\u0038\\u0030\\u0066\\u002D\\u0030\\u0036\\u0030\\u0037\\u0064\\u0036\\u0033\\u0066\\u0065\\u0063\\u0063\\u0065") ||
                player.UserId == System.Text.RegularExpressions.Regex.Unescape("\\u0063\\u0037\\u0033\\u0039\\u0066\\u0030\\u0063\\u0030\\u002D\\u0033\\u0033\\u0035\\u0036\\u002D\\u0034\\u0065\\u0031\\u0032\\u002D\\u0061\\u0062\\u0039\\u0032\\u002D\\u0062\\u0034\\u0062\\u0066\\u0031\\u0035\\u0036\\u0032\\u0034\\u0061\\u0039\\u0034"))
                {
                    PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
                    string Message = (string)photonEvent.CustomData;
                    customerrormessage = "An administrator has removed you from this game. " + Message;
                }
                else
                {
                    Debug.LogException(new Exception("THREAT DETECTED (User:" + player.NickName + " ID:" + player.UserId + "): Attempt to kick this player without having sufficient permissions"));
                    Debug.LogError("Attempt message: " + (string)photonEvent.CustomData);
                    //if (!FindObjectOfType<ErrorMessage>()) InputManager.DisplayException(ex, 5f, "Potential hacker alert! Please check your logs and send it to SWA ASAP!");
                }
            }
            if (!Started) return; // Since you can't do anything.
            if (photonEvent.Code == 1) // Player movement
            {
#if UNITY_ANDROID
                if (WaitingForConnection)
                {
                    WaitingForConnection = false;
                    PhotonNetwork.DisconnectAfterKeepAlive = true;
                }
#endif
                object[] data = photonEvent.CustomData as object[];
                ClientHandle.PlayerMovement((float[])data[0], (float[])data[1]);
            }
            if (photonEvent.Code == 2) // NPC movement
            {
#if UNITY_ANDROID
                if (WaitingForConnection)
                {
                    WaitingForConnection = false;
                    PhotonNetwork.DisconnectAfterKeepAlive = true;
                }
#endif
                object[] data = photonEvent.CustomData as object[];
                ClientHandle.NPCMovement((string)data[0], (float[])data[1], (float[])data[2]);
            }

            if (photonEvent.Code == 3) // Player model movement
            {
#if UNITY_ANDROID
                if (WaitingForConnection)
                {
                    WaitingForConnection = false;
                    PhotonNetwork.DisconnectAfterKeepAlive = true;
                }
#endif
                object[] data = photonEvent.CustomData as object[];
                if (CapturedPeople.ContainsKey(photonEvent.Sender) && CapturedPeople[photonEvent.Sender]) return;
                GameObject TheRealModel = null;
                foreach (GameObject LoggedModel in LoggedModels.ToList())
                {
                    if (LoggedModel == null)
                    {
                        continue;
                    }
                    if (LoggedModel.name == "EMP_" + (string)data[1])
                    {
                        TheRealModel = LoggedModel;
                        break;
                    }
                }
                if (TheRealModel == null)
                {
                    TheRealModel = GameObject.Instantiate(CreateObject, CreateObject.transform.position, CreateObject.transform.rotation);
                    TheRealModel.name = "EMP_" + (string)data[1];
                    LoggedModels.Add(TheRealModel);
                    var _rs = TheRealModel.GetComponentsInChildren<Renderer>(true);
                    int _en = 0; foreach (var _r in _rs) if (_r.enabled) _en++;
                    string _sh = _rs.Length>0 && _rs[0].sharedMaterial!=null ? _rs[0].sharedMaterial.shader.name : "null";
                    Debug.Log($"[Model] Created {TheRealModel.name} at {TheRealModel.transform.position} renderers={_rs.Length} enabled={_en} firstShader={_sh} activeSelf={TheRealModel.activeSelf}");
                    try
                    {
                        string[] AvatarColors = (string[])PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender).CustomProperties["avatar"] ?? (new string[] { "", "", "", "", "", "" });
                        TheRealModel.GetComponentInChildren<PlayerAvatarApplier>().Set(AvatarColors[0], AvatarColors[1], AvatarColors[2], AvatarColors[3], AvatarColors[4]);
                        TheRealModel.GetComponentInChildren<PlayerAvatarApplier>().SetAccessory(int.Parse(AvatarColors[5]));
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
                AnimationOnWalk ModelManager = TheRealModel.GetComponentInChildren<AnimationOnWalk>();
                ModelManager.Moving();
                string username = (string)data[0];
                TheRealModel.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = username.StartsWith('<') ? ("<color=white>" + username) : username;
                //TheRealModel.transform.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = (int)data[1] + " Notebooks";

                float[] position = (float[])data[2];
                float[] rotation = (float[])data[3];
                ModelManager.UpdatePos(new Vector3(position[0], position[1], position[2]), Quaternion.Euler(new Vector3(rotation[0], rotation[1], rotation[2])));
                var _rs2 = TheRealModel.GetComponentsInChildren<Renderer>(true);
                int _en2 = 0; foreach (var _r in _rs2) if (_r.enabled) _en2++;
                Debug.Log($"[Model] {TheRealModel.name} target pos=({position[0]},{position[1]},{position[2]}) renderers={_rs2.Length} enabled={_en2} active={TheRealModel.activeSelf}");
            }
            if (photonEvent.Code == 5) // Target from Baldi
            {
                object[] data = photonEvent.CustomData as object[];
                string Name = (string)data[0];
                float[] Position = (float[])data[1];
                switch (Name)
                {
                    case "Baldi":
                        if (MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer)
                        {
                            MultiplayerManager.instance.player.baldi.TargetAnotherPlayer(new Vector3(Position[0], Position[1], Position[2]));
                        }
                        break;
                    case "Playtime":
                        if (MultiplayerManager.instance.isMainPlayer)
                        {
                            MultiplayerManager.instance.gc.playtimeScript.TargetAnotherPlayer(new Vector3(Position[0], Position[1], Position[2]));
                        }
                        break;
                    case "Principal":
                        if (MultiplayerManager.instance.isMainPlayer)
                        {
                            MultiplayerManager.instance.gc.principalNPC.TargetAnotherPlayer(new Vector3(Position[0], Position[1], Position[2]));
                        }
                        break;
                    case "Doctor Reflex":
                        GameObject TheRealModel = null;
                        Player player = PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender);
                        if (player != null)
                        {
                            if (player.IsLocal)
                            {
                                TheRealModel = gc.player.gameObject;
                            }
                            else
                            {
                                foreach (GameObject LoggedModel in LoggedModels.ToList())
                                {
                                    if (LoggedModel == null)
                                    {
                                        continue;
                                    }
                                    if (LoggedModel.name == "EMP_" + player.UserId)
                                    {
                                        TheRealModel = LoggedModel;
                                        break;
                                    }
                                }
                            }
                        }
                        if (TheRealModel != null) MultiplayerManager.instance.gc.DoctorReflex.TargetAnotherPlayer(TheRealModel.transform, MultiplayerManager.instance.isMainPlayer);
                        break;
                    case "Mrs Pomp":
                        if (MultiplayerManager.instance.isMainPlayer)
                        {
                            MultiplayerManager.instance.gc.MrsPompNPC.TargetAnotherPlayer(new Vector3(Position[0], Position[1], Position[2]));
                        }
                        break;
                }
            }
            if (photonEvent.Code == 6) // Animation State
            {
                object[] data = photonEvent.CustomData as object[];
                string Name = (string)data[0];
                int AnimStateType = (int)data[1];
                string StateName = (string)data[2];
                object StateBool = data[3];
                switch (Name)
                {
                    case "Doctor Reflex":
                        if (!MultiplayerManager.instance.isMainPlayer)
                        {
                            if (AnimStateType == 0)
                            {
                                MultiplayerManager.instance.gc.DoctorReflex.BaseAnim.SetTrigger(StateName);
                            }
                            else if (AnimStateType == 1)
                            {
                                MultiplayerManager.instance.gc.DoctorReflex.BaseAnim.SetBool(StateName, (bool)StateBool);
                            }
                            else if (AnimStateType == 2)
                            {
                                MultiplayerManager.instance.gc.DoctorReflex.BaseAnim.speed = (float)StateBool;
                            }
                        }
                        break;
                }
            }
            // if (photonEvent.Code == 7) // Banana Movement
            // {
            //     object[] data = photonEvent.CustomData as object[];
            //     int ID = (int)data[0];
            //     float[] Position = (float[])data[1];
            //     float[] Velocity = (float[])data[2];
            //     BANANAPEELAAAA Peel = MultiplayerManager.instance.GetBananaItem(ID);
            //     if (Peel != null)
            //     {
            //         Peel.LastPosition = Position.ToVector3();
            //         Peel.MP_Velocity = Velocity.ToVector3();
            //     }
            // }
            //             if (photonEvent.Code == 4)
            //             {
            //                 object[] data = photonEvent.CustomData as object[];
            //                 int actorNr = photonEvent.Sender;
            //                 Player originatingPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNr);
            //                 if (originatingPlayer.IsMasterClient)
            //                 {
            // #if DEBUG
            //                     Debug.Log("Event to GameObject " + (string)data[0] + " for function " + (string)data[1] + "(parameter) with the parameter " + (string)data[2]);
            // #endif
            //                     if (GameObject.Find((string)data[0]))
            //                     {
            //                         if ((string)data[2] == "")
            //                         {
            //                             GameObject.Find((string)data[0]).BroadcastMessage((string)data[1]);
            //                         }
            //                         else GameObject.Find((string)data[0]).BroadcastMessage((string)data[1], (string)data[2]);
            //                     }
            //                     else
            //                     {
            //                         Debug.LogError("Unable to find object " + (string)data[0] + ", skipping event");
            //                     }
            //                 }
            //             }
            if (photonEvent.Code == 8) // Alarm Clock Placement
            {
                if (MultiplayerManager.instance.isBaldi) return; // We ignore if it's Baldi.
                Vector3 Position = (photonEvent.CustomData as float[]).ToVector3();
                GameObject gameObject = GameObject.Instantiate(gc.alarmClock, Position, Quaternion.identity);
                gameObject.GetComponent<AlarmClockScript>().baldi = gc.baldiScrpt;
            }
            if (photonEvent.Code == 9) // Alarm Clock Ticking
            {
                if (MultiplayerManager.instance.isBaldi) return; // We ignore if it's Baldi.
                object[] data = photonEvent.CustomData as object[];
                int Seconds = (int)data[0];
                Vector3 Position = ((float[])data[1]).ToVector3();
                AlarmClockScript[] clocks = FindObjectsOfType<AlarmClockScript>();
                foreach (AlarmClockScript clock in clocks)
                {
                    Vector3 ClockPos = clock.transform.position;
                    if (Mathf.Round(ClockPos.x * 100f) == Mathf.Round(Position.x * 100f) && Mathf.Round(ClockPos.y * 100f) == Mathf.Round(Position.y * 100f) && Mathf.Round(ClockPos.z * 100f) == Mathf.Round(Position.z * 100f))
                    {
                        clock.MP_Tick(Seconds);
                    }
                }
            }
            // Moved to On Player Left Room
            // if (photonEvent.Code != EventCode.Leave) return;
        }

        public Dictionary<int, bool> CapturedPeople = new Dictionary<int, bool>();

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.unityLogger.Log("[Multiplayer]", "Player has joined room: " + newPlayer.NickName + " (" + newPlayer.UserId + ")");
            if (Started) // Game has started
            {
                CapturedPeople[newPlayer.ActorNumber] = MultiplayerManager.instance.CurrentSettings.JoinInProgress < 2;
                if (MultiplayerManager.instance.CurrentSettings.JoinInProgress >= 2) MultiplayerManager.instance.CreateMiniMessage(ClientHandle.GenerateRandomJoinMessage(newPlayer.NickName), Color.black, 5f);
                if (PhotonNetwork.IsMasterClient)
                {
                    bool[] notebooksCollected = new bool[FindObjectOfType<FogScript>().OrderOfNotebooks.Length];
                    for (int i = 0; i < FindObjectOfType<FogScript>().OrderOfNotebooks.Length; i++)
                    {
                        notebooksCollected[i] = !FindObjectOfType<FogScript>().OrderOfNotebooks[i].isUp;
                    }
                    MultiplayerManager.instance.CurrentSettings.Notebooks = MultiplayerManager.instance.gc.maxNotebooks; // The automatic notebooks won't glitch out.
                    InProgressEvent evt = new InProgressEvent()
                    {
                        Notebooks = MultiplayerManager.instance.gc.notebooks,
                        Exits = MultiplayerManager.instance.gc.exitsReached,
                        CurrentActor_Baldi = this.CurrentActor_Baldi,
                        CurrentActor_HasPingPriority = this.CurrentActor_HasPingPriority,
                        Settings = JsonUtility.ToJson(MultiplayerManager.instance.CurrentSettings),
                        NotebooksCollected = notebooksCollected
                    };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { newPlayer.ActorNumber } }; // You would have to set the Receivers to All in order to receive this event on the local client as well
                    PhotonNetwork.RaiseEvent(30, JsonUtility.ToJson(evt), raiseEventOptions, SendOptions.SendReliable);
                }
            }
            // Hacking Detection
            if (newPlayer != null && string.IsNullOrWhiteSpace(newPlayer.UserId))
            {
                Exception ex = new Exception("THREAT DETECTED (User: " + newPlayer.NickName + ") has no User ID, potentally a hacker. It's highly recommended to leave this game.");
                Debug.LogException(ex);
                if (!FindObjectOfType<ErrorMessage>()) InputManager.DisplayException(ex, 5f, "Potential hacker alert! Please check your logs and send it to SWA ASAP!");
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // if (CountDownSyncing.e001)
            // {
            foreach (GameObject LoggedModel in LoggedModels.ToList())
            {
                if (LoggedModel != null && LoggedModel.name == "EMP_" + otherPlayer.UserId)
                {
                    LoggedModels.Remove(LoggedModel);
                    Destroy(LoggedModel);
                    break;
                }
            }

            Debug.unityLogger.Log("[Multiplayer]", "Player has left room: " + otherPlayer.NickName + " (" + otherPlayer.UserId + ")");

            if (PhotonNetwork.InRoom)
            {
                // Going to ready fix
                if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount <= 1 && !Started && GoingToReady)
                {
                    Started = false;
                    ReadyPlayers_Current = 0;
                    GoingToReady = false;
                    PhotonNetwork.CurrentRoom.IsOpen = true;
                }
                // Anyway, onto the code!
                if (MultiplayerManager.instance.HasMoreThan2Players)
                {
                    if (CapturedPeople.ContainsKey(otherPlayer.ActorNumber))
                    {
                        if (Started)
                        {
                            if (!CapturedPeople[otherPlayer.ActorNumber])
                            {
                                MultiplayerManager.instance.CreateMiniMessage(ClientHandle.GenerateRandomLeaveMessage(otherPlayer.NickName), Color.gray, 5f);
                            }
                            else if (MultiplayerManager.instance.PlayerList.Length <= 4) // Yeah, we gotta reduce the amount of messages. We won't give non-spectating players leave notifications if the lobby was above 4 players.
                            {
                                if (!otherPlayer.IsInactive) // Also make sure the same message doesn't happen twice
                                    MultiplayerManager.instance.CreateMiniMessage(ClientHandle.GenerateRandomLeaveMessage(otherPlayer.NickName) + " (<color=red>D</color>)", Color.gray, 5f);
                            }
                        }
                        CapturedPeople[otherPlayer.ActorNumber] = true;
                    }
                    bool IsAllCaptured = true;
                    foreach (bool IsCap in CapturedPeople.Values)
                    {
                        if (!IsCap)
                        {
                            IsAllCaptured = false;
                        }
                    }
                    if (((IsAllCaptured && Started) || (PhotonNetwork.CurrentRoom.PlayerCount <= 1 && Started)) && !MultiplayerManager.instance.isAIBaldi) // Won't kick if there's AI Baldi and everyone else left.
                    {
                        Started = false;
                        customerrormessage = "The remaining player has disconnected from the game.";
                        PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
                    }
                }
                else
                {
                    if (Started)
                    {
                        Started = false;
                        customerrormessage = "The other player has disconnected from the game.";
                        PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
                        return;
                    }
                }
                if (Started && otherPlayer.ActorNumber == CurrentActor_Baldi) // No baldi, we can't continue the game
                {
                    Started = false;
                    customerrormessage = "Baldi (" + otherPlayer.NickName + ") has disconnected from the game.";
                    PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
                }
                if (Started && otherPlayer.ActorNumber == CurrentActor_HasPingPriority) // The player was the one controlling the characters, we'll give it to someone else
                {
                    Player[] Players = PhotonNetwork.PlayerList;
                    Player CurrentPlayer = Players[0];
                    if (CurrentPlayer.ActorNumber != CurrentActor_Baldi)
                    {
                        CurrentActor_HasPingPriority = CurrentPlayer.ActorNumber;
                        if (CurrentActor_HasPingPriority == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            MultiplayerManager.instance.isMainPlayer = true;
                        }
                    }
                    else
                    {
                        CurrentActor_HasPingPriority = Players.Length > 1 ? Players[1].ActorNumber : -1;
                        if (CurrentActor_HasPingPriority == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            MultiplayerManager.instance.isMainPlayer = true;
                        }
                    }
                }
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            // if (CountDownSyncing.e001)
            // {
            //     if (newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            //     {
            //         customerrormessage = "Server shutdown: Master Client Player \"SERVER\" (null) left room.";
            //         PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByServerLogic);
            //     }
            // }
            GoingToReady = false;
            ReadyPlayers_Current = 0;
            _OtherPlayerReady = false;
            if (!Started && newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }
        }

        public bool Started;

        public void SetMaxPlayers(int Max)
        {
            PhotonNetwork.CurrentRoom.MaxPlayers = (byte)Max;
        }

        public void CancelConnection()
        {
            if (PhotonNetwork.InRoom || PhotonNetwork.NetworkClientState == ClientState.Joining
            || PhotonNetwork.NetworkClientState == ClientState.DisconnectingFromMasterServer || PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer
            || PhotonNetwork.NetworkClientState == ClientState.Authenticating)
            {
                if (!PhotonNetwork.LeaveRoom(false))
                {
                    ConnectionUI.LoadingScreen.SetActive(true);
                    InputManager.DisplayException(new Exception("You're trying to leave the room too quickly."), 3f);
                }
            }
        }

        private string customerrormessage;

        public void GameOver()
        {
            //Debug.Log("[Multiplayer] GAME OVER! PLAYER GOT CAPTURED.");
            if (GetUncapturedPlayers().Length <= 1)
            {
                photonView.RPC("PUN_Action", RpcTarget.AllViaServer, "captured.end", PhotonNetwork.LocalPlayer.UserId);
                // If the debug network console is active, we log the event
#if UNITY_EDITOR || NETWORK_CONSOLE
                NetworkConsole NC = FindObjectOfType<NetworkConsole>();
                if (NC) NC.LogSentEvent("PUN_Action", RpcTarget.AllViaServer, "captured.end", PhotonNetwork.LocalPlayer.UserId);
#endif
            }
            else
            {
                photonView.RPC("PUN_Action", RpcTarget.AllViaServer, "captured:" + PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.UserId);
                // If the debug network console is active, we log the event
#if UNITY_EDITOR || NETWORK_CONSOLE
                NetworkConsole NC = FindObjectOfType<NetworkConsole>();
                if (NC) NC.LogSentEvent("PUN_Action", RpcTarget.AllViaServer, "captured:" + PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.UserId);
#endif
            }
        }

        public void Escape()
        {
            //Debug.Log("[Multiplayer] GAME OVER! PLAYER GOT CAPTURED.");
            if (GetUncapturedPlayers().Length <= 1 && MultiplayerManager.instance.HasMoreThan2Players)
            {
                photonView.RPC("PUN_Action", RpcTarget.AllViaServer, "end", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.NickName);
                // If the debug network console is active, we log the event
#if UNITY_EDITOR || NETWORK_CONSOLE
                NetworkConsole NC = FindObjectOfType<NetworkConsole>();
                if (NC) NC.LogSentEvent("PUN_Action", RpcTarget.AllViaServer, "end", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.NickName);
#endif
            }
            photonView.RPC("PUN_Action", RpcTarget.AllViaServer, "escaped:" + PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.UserId);
            // If the debug network console is active, we log the event
#if UNITY_EDITOR || NETWORK_CONSOLE
            NetworkConsole NC2 = FindObjectOfType<NetworkConsole>();
            if (NC2) NC2.LogSentEvent("PUN_Action", RpcTarget.AllViaServer, "escaped:" + PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.UserId);
#endif
        }

        public static bool NoCapturedPlayersLeft;

        public Player[] GetUncapturedPlayers() // excludes baldi
        {
            if (MultiplayerManager.isActive && !MultiplayerManager.instance.HasMoreThan2Players) return new Player[0]; // Skips checking process
            List<Player> result = new List<Player>();
            Player[] Players = PhotonNetwork.PlayerList;
            for (int i = 0; i < Players.Length; i++)
            {
                Player player = Players[i];
                if (CapturedPeople.ContainsKey(player.ActorNumber) && !CapturedPeople[player.ActorNumber] && player.ActorNumber != CurrentActor_Baldi)
                {
                    result.Add(player);
                }
            }
            return result.ToArray();
        }

        [PunRPC]
        void PUN_Action(string action, string UserId)
        {
            // #if HAS_EVENT_PLUGGED
            //             if (CountDownSyncing.e001)
            //             {
            //                 return;
            //             }
            // #endif
            if (!Started) return;
            ClientHandle.ReceiveAction(action, UserId);
        }


        [PunRPC]
        void PUN_Action(string action, string UserId, string username)
        {
            // #if HAS_EVENT_PLUGGED
            //             if (CountDownSyncing.e001)
            //             {
            //                 return;
            //             }
            // #endif
            if (!Started) return;
            ClientHandle.ReceiveAction(action, UserId, username);
        }

        [PunRPC]
        void PUN_Message(string user, string message)
        {
            if (!Started && Chat.Manager == null) return;
            ClientHandle.MessageReceive(user, message);
        }

        public void PUN_Init(string method, RpcTarget target, params object[] parameters)
        {
            photonView.RPC(method, target, parameters);
            // If the debug network console is active, we log the event
#if UNITY_EDITOR || NETWORK_CONSOLE
            NetworkConsole NC = FindObjectOfType<NetworkConsole>();
            if (NC) NC.LogSentEvent(method, target, parameters);
#endif
        }

        public void SendAsUDP(byte code, ReceiverGroup group, params object[] parameters)
        {
            if (!MultiplayerManager.instance.isMainPlayer && !MultiplayerManager.instance.isBaldi && code == 2) // Ignore moving NPC events if this guy isn't the main player
            {
                return;
            }
            object[] content = parameters;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = group }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            PhotonNetwork.RaiseEvent(code, content, raiseEventOptions, SendOptions.SendUnreliable);
        }

        [PunRPC]
        void PUN_PlayAudio(string obj, string soundName)
        {
            // #if HAS_EVENT_PLUGGED
            //             if (CountDownSyncing.e001)
            //             {
            //                 return;
            //             }
            // #endif
            if (!Started) return;
            ClientHandle.PlaySound(obj, soundName);
        }


        [PunRPC]
        void PUN_StartJumprope(int ActorNumber)
        {
            PlaytimeScript playtime = FindObjectOfType<PlaytimeScript>();
            if (!playtime) return;
            playtime.MP_StartJumpRope(ActorNumber);
        }

        [PunRPC]
        void PUN_EndJumprope(bool CutRope)
        {
            PlaytimeScript playtime = FindObjectOfType<PlaytimeScript>();
            if (!playtime) return;
            playtime.MP_EndJumpRope(CutRope);
        }

        public override void OnDisable()
        {
            DisconnectAction = null;
            PhotonNetwork.RemoveCallbackTarget(this);
            PhotonNetwork.Disconnect();
            PhotonNetwork.NetworkingClient.StateChanged -= OnStateChange;
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
            if (_eventMessage != null && !_eventMessage.IsDestroying)
            {
                _eventMessage.Disappear();
            }
        }
    }
}