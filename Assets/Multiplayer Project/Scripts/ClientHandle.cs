using System.Collections;
using System.Net;
using System.Globalization;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Unity.VisualScripting;
using SchoolSaves;

namespace Multiplayer
{
    public class ClientHandle : MonoBehaviour
    {
        public static void Welcome()
        {
            //             string msg = packet.ReadString();
            //             int myId = packet.ReadInt();
            //             bool isUDPEnabled = packet.ReadBool();

            //             MultiplayerManager.instance.client.ID = myId;
            //             console.log(msg);

            //             ClientSend.WelcomeReceived();

            //             if (isUDPEnabled)
            //             {
            // #if !UNITY_WEBGL
            //                 MultiplayerManager.instance.client.udp.Connect(((IPEndPoint)MultiplayerManager.instance.client.tcp.socket.Client.LocalEndPoint).Port);
            // #endif
            //             }
            //             else
            //             {
            //                 MultiplayerManager.instance.client.hasFullConnection = true;
            //             }
        }

        public static void MessageReceive(string user, string message)
        {
            string FormattedMessage = System.Text.RegularExpressions.Regex.Replace(message, "<.*?>", string.Empty);
            string str = $"[{user}] {FormattedMessage}";
            if (Chat.Manager)
            {
                Chat.Manager.AddMessage(str);
            }
        }

        public static void PlayerMovement(float[] position, float[] rotation)
        {
            Vector3 movement = new Vector3(position[0], position[1], position[2]);
            MultiplayerManager.instance.player.MP_Position = movement;
            MultiplayerManager.instance.player.MP_Rotation = Quaternion.Euler(new Vector3(rotation[0], rotation[1], rotation[2]));
        }

        public static void NPCMovement(string name, float[] position, float[] rotation)
        {
            // string name = packet.ReadString();
            // float x = packet.ReadFloat();
            // float y = packet.ReadFloat();
            // float z = packet.ReadFloat();
            Vector3 movement = new Vector3(position[0], position[1], position[2]);
            Quaternion rot = Quaternion.Euler(new Vector3(rotation[0], rotation[1], rotation[2]));
            GameObject GB = Find(name);
            if (GB == null) return;
            NPC t = GB.GetComponent<NPC>();
            t.MP_Position = movement;
            t.MP_Rotation = rot;
        }

        private static GameObject Find(string name)
        {
            if (MultiplayerManager.instance.gc != null)
            {
                GameObject[] Characters = MultiplayerManager.instance.gc.cheatingConsole.characters;
                foreach (GameObject Char in Characters)
                {
                    if (Char.activeSelf && name == Char.name)
                    {
                        return Char;
                    }
                }
            }
            return null;
        }

        public static void StartGame(bool isBaldi, string usernamesReceived, bool HasAdvantagePing, MultiplayerManager.GameSettings Settings)
        {
            try
            {
                if (Settings.PickedBaldi == -1)
                {
                    usernamesReceived += "|BaldiAI,null";
                }
                string[] usernames = usernamesReceived.Split('|');
                MultiplayerManager.instance.PlayerList = new MultiplayerManager.PlayerListItem[usernames.Length];
                MultiplayerManager.instance.isBaldi = Settings.PickedBaldi != -1 ? isBaldi : false;
                MultiplayerManager.instance.isAIBaldi = Settings.PickedBaldi == -1;
                MultiplayerManager.instance.isMainPlayer = HasAdvantagePing;
                MultiplayerManager.instance.CurrentSettings = Settings;
                MultiplayerManager.PlayerListItem item = new MultiplayerManager.PlayerListItem();
                string[] tags = usernames[0].Split(',');
                item.username = tags[0];
                item.device = tags[1];
                if (string.IsNullOrEmpty(AccountManager.getUsername()))
                {
                    item.profilePicture = null;
                }
                else if (item.username == AccountManager.getUsername())
                {
                    item.profilePicture = AccountManager.getProfilePicture();
                }
                else if (!tags[0].StartsWith("<"))
                {
                    AccountManager.GetProfilePicture(tags[0]);
                }
                MultiplayerManager.PlayerListItem item2 = new MultiplayerManager.PlayerListItem();
                tags = usernames[1].Split(',');
                item2.username = tags[0];
                item2.device = tags[1];
                if (!string.IsNullOrEmpty(AccountManager.getUsername()) && item2.username == AccountManager.getUsername())
                {
                    item2.profilePicture = AccountManager.getProfilePicture();
                }
                else if (!tags[0].StartsWith("<"))
                {
                    AccountManager.GetProfilePicture(tags[0]);
                }

                MultiplayerManager.instance.HasMoreThan2Players = usernames.Length > 2;
                if (Settings.PickedBaldi == -1)
                {
                    MultiplayerManager.instance.HasMoreThan2Players = true;
                }
                if (usernames.Length == 3) // Only include the username
                {
                    MultiplayerManager.PlayerListItem item3 = new MultiplayerManager.PlayerListItem();
                    tags = usernames[2].Split(',');
                    item3.username = tags[0];
                    MultiplayerManager.instance.PlayerList[2] = item3;
                }

                MultiplayerManager.instance.PlayerList[0] = item;
                MultiplayerManager.instance.PlayerList[1] = item2;
            }
            catch (System.Exception)
            {
                Photon.Pun.PhotonNetwork.NetworkingClient.Disconnect(Photon.Realtime.DisconnectCause.Exception);
                throw;
            }
        }

        private static GameObject FindPlayer(string name)
        {
            foreach (GameObject LoggedModel in MultiplayerManager.instance.client.LoggedModels)
            {
                if (LoggedModel == null)
                {
                    continue;
                }
                if (LoggedModel.name == "EMP_" + name)
                {
                    return LoggedModel;
                }
            }
            return null;
        }

        public static void PlaySound(string objectName, string soundName)
        {
            if (objectName == "CRUNCH")
            {
                Transform Model = MultiplayerManager.instance.PlayerList.Length > 2 ? FindPlayer(soundName).transform : MultiplayerManager.instance.player.transform;
                if (Model != null)
                {
                    // Audio
                    AudioSource audioDevice = new GameObject("CRUNCH " + Model.name).AddComponent<AudioSource>();
                    audioDevice.spatialBlend = 1f;
                    audioDevice.minDistance = 5f;
                    audioDevice.maxDistance = 50f;
                    audioDevice.rolloffMode = AudioRolloffMode.Linear;
                    audioDevice.dopplerLevel = 0f;
                    audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
                    audioDevice.transform.position = Model.position;
                    audioDevice.PlayOneShotWC(MultiplayerManager.instance.gc.Crunch);
                    Object.Destroy(audioDevice.gameObject, MultiplayerManager.instance.gc.Crunch.length);
                    return;
                }
            }
            GameObject GB = Find(objectName) ?? GameObject.Find(objectName);
            if (GB == null)
            {
                Debug.LogWarning("Attempt to play " + soundName + " on an non-existing object: " + objectName);
                return;
            }
            if (soundName == "BAL_SlapBroken")
            {
                BaldiScript BS = GB.GetComponent<BaldiScript>();
                BS.baldiAnimator.SetTrigger("brokenSlap");
                BS.IsAnimRulerBroken = true;
                return;
            }
            AudioClip soundClip = MultiplayerManager.GetAudio(soundName);
            if (soundClip == null)
            {
#if DEBUG || NETWORK_CONSOLE
                Debug.Log("The sound clip does not exist in the data: " + soundName);
#endif
                return;
            }
            if (soundName == "BAL_Break" && objectName == "Baldi")
            {
                BaldiScript BS = GB.GetComponent<BaldiScript>();
                BS.baldiAnimator.SetTrigger("brokenSlap");
                BS.IsAnimRulerBroken = true;
                BS.GetComponent<AudioSource>().PlayOneShotWC(soundClip);
            }
            else if (soundClip.name == "BAL_Slap")
            {
                BaldiScript BS = GB.GetComponent<BaldiScript>();
                BS.baldiAnimator.SetTrigger("slap");
                BS.IsAnimRulerBroken = false;
            }
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi && MultiplayerManager.instance.player.baldi.antiHearing)
            {
                return;
            }
            GB.GetComponent<AudioSource>().PlayOneShotWC(soundClip);
        }

        public static void OnRandomEvent(int EventID)
        {
            RandomEvent randomEvent = FindObjectOfType<RandomEvent>();
            if (randomEvent == null) return;
            randomEvent.ChooseEvent(EventID);
        }

        public static void OnElevatorEventFromPlayer(bool Up, bool IsCooldown, bool IsBaldi)
        {
#if DEBUG
            Debug.Log("Elevator Event Receive: Up:" + Up + " | No Cooldown:" + IsCooldown + " | Baldi:" + IsBaldi);
#endif
            ElevatorScript elevator = GameObject.Find("Floor 1 Elevator").GetComponent<ElevatorScript>();
            if (IsCooldown)
            {
                elevator.ElevatorGo(Up);
            }
            else if (!IsBaldi)
            {
                elevator.MP_StartElevatorFromPlayer(Up);
            }
            else
            {
                elevator.MP_StartElevatorFromBaldi(Up);
            }
        }

        public static void ReceiveAction(string action, string userId, string username = "")
        {
            switch (action)
            {
                case "apple":
                    if (MultiplayerManager.instance.isBaldi)
                    {
                        MultiplayerManager.instance.player.baldi.timeToMove += 4.3f + 15f;
                        MultiplayerManager.instance.player.baldi.baldiSlider.maxValue += 4.3f + 15f;
                        if (!AppleKnow)
                        {
                            AppleKnow = true;
                            MultiplayerManager.instance.CreateMiniMessage("You got stunned by an apple", Color.red, 5f);
                        }
                    }
                    else
                    {
                        MultiplayerManager.instance.player.appleEat = true;
                        MultiplayerManager.instance.player.baldi.baldiPriority = MultiplayerManager.instance.player.baldi.currentPriority;
                    }
                    break;
                case "quarter":
                    if (!MultiplayerManager.instance.isBaldi)
                    {
                        MultiplayerManager.instance.gc.quarter.SetActive(true);
                        MultiplayerManager.instance.gc.quarter.name += "MP";
                    }
                    break;
                // case "bsoda":

                //     break;
                case "end":
                    Multiplayer.Client.DisconnectAction = ((reason, cause) =>
                    {
                        Debug.LogException(reason);
                    });
                    if (MultiplayerManager.instance.HasMoreThan2Players)
                    {
                        if (FindObjectOfType<HudScreenTextScript>()) FindObjectOfType<HudScreenTextScript>().ShowHudText(5f, "No more remaining students.");
                    }
                    else
                    {
                        if (FindObjectOfType<HudScreenTextScript>()) FindObjectOfType<HudScreenTextScript>().ShowHudText(5f, "<color=red>" + username + "</color> has escaped");
                    }
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
                    if (FindObjectOfType<SpectatingHud>())
                    {
                        FindObjectOfType<SpectatingHud>().enabled = false;
                    }
                    if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.CurrentRoom.IsOpen = false;
                    }
                    break;
                case "crafters.summon":
                    if (!MultiplayerManager.instance.gc.crafters.activeSelf) MultiplayerManager.instance.gc.crafters.SetActive(true);
                    if (MultiplayerManager.instance.isBaldi && MultiplayerManager.instance.gc.crafters.GetComponent<AudioSource>()) // Don't play the crafters audio as Baldi
                    {
                        MultiplayerManager.instance.gc.crafters.GetComponent<AudioSource>().mute = true;
                    }
                    MultiplayerManager.instance.gc.crafters.GetComponent<CraftersScript>().MP_Angry();
                    break;
                case "crafters.happy":
                    if (!MultiplayerManager.instance.gc.crafters.activeSelf) return;
                    MultiplayerManager.instance.gc.crafters.GetComponent<CraftersScript>().BecomeHappy_NoEvent();
                    break;
                case "captured.end":
                    Multiplayer.Client.DisconnectAction = ((reason, cause) =>
                    {
                        Debug.LogException(reason);
                    });
                    MultiplayerManager.instance.player.gameOver = true;
                    MultiplayerManager.instance.player.cameraScript.GM_PositionHeight = (MultiplayerManager.instance.player.baldi.FlattenDuration > 0f) ? 1f : 5f;
                    Client.NoCapturedPlayersLeft = true;
                    MultiplayerManager.instance.player.cameraScript.IgnoreCameraInputs = false;
                    MultiplayerManager.instance.player.cameraScript.GetComponent<Camera>().nearClipPlane = 0.1f;
                    if (FindObjectOfType<SpectatingHud>())
                    {
                        FindObjectOfType<SpectatingHud>().gameObject.SetActive(false);
                    }
                    if (MultiplayerManager.instance.gc.CM) MultiplayerManager.instance.gc.CM.DestroyAllCaptions(true);
                    if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.CurrentRoom.IsOpen = false;
                    }
                    break;
            }
            if (action.StartsWith("teleport:"))
            {
                string[] strpos = action.Split(':')[1].Split(',');
                string evnt = "crafters";
                float[] pos = new float[3]
                {
                    5f,
                    1.64f,
                    125f
                };
                // try
                // {
                //     evnt = action.Split(':')[2];
                //     pos[0] = float.Parse(strpos[0]);
                //     pos[1] = float.Parse(strpos[1]);
                //     pos[2] = float.Parse(strpos[2]);
                // }
                // catch (System.Exception ex)
                // {
                //     evnt = "none";
                //     Debug.LogException(ex);
                // }
                Transform player = MultiplayerManager.instance.player.transform;
                Transform baldi = MultiplayerManager.instance.gc.baldiTransform;
                MultiplayerManager.instance.gc.baldiScrpt.agent.Warp(new Vector3(pos[0], pos[1], pos[2]));
                if (evnt == "crafters")
                {
                    if (!MultiplayerManager.instance.isBaldi)
                    {
                        if (MultiplayerManager.instance.gc.notebooks >= MultiplayerManager.instance.gc.maxNotebooks)
                        {
                            if (!MultiplayerManager.instance.gc.SpectatingDisplay.activeSelf)
                            {
                                MultiplayerManager.instance.player.SetPosition(new Vector3(5f, 4f, 80f)); // Get everyone here
                            }
                            MultiplayerManager.instance.gc.crafters.SetActive(false);
                        }
                    }
                    baldi.LookAt(new Vector3(5f, baldi.position.y, 80f));
                }
            }
            if (action.StartsWith("hear:"))
            {
                string[] strpos = action.Split(':')[1].Split(',');
                float[] pos = new float[3]
                {
                    5f,
                    1.64f,
                    125f
                };
                try
                {
                    pos[0] = float.Parse(strpos[0], CultureInfo.InvariantCulture);
                    pos[1] = float.Parse(strpos[1], CultureInfo.InvariantCulture);
                    pos[2] = float.Parse(strpos[2], CultureInfo.InvariantCulture);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
                if (MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer)
                {
                    MultiplayerManager.instance.player.baldi.Hear(new Vector3(pos[0], pos[1], pos[2]), MultiplayerManager.instance.player.baldi.currentPriority);
                }
                if (MultiplayerManager.instance.isBaldi)
                {
                    DoorIncidator HearDetector = FindObjectOfType<DoorIncidator>();
                    HearDetector.Y = (pos[1] >= 20f) ? 22f : 2f;
                    HearDetector.Show(new Vector3(pos[0], pos[1], pos[2]));
                }
            }
            if (action.StartsWith("WOHOO:"))
            {
                string[] strpos = action.Split(':')[1].Split(',');
                float[] pos = new float[3]
                {
                    5f,
                    1.64f,
                    125f
                };
                try
                {
                    pos[0] = float.Parse(strpos[0], CultureInfo.InvariantCulture);
                    pos[1] = float.Parse(strpos[1], CultureInfo.InvariantCulture);
                    pos[2] = float.Parse(strpos[2], CultureInfo.InvariantCulture);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
                if (MultiplayerManager.instance.isMainPlayer)
                {
                    MultiplayerManager.instance.gc.principalNPC.Hear(new Vector3(pos[0], pos[1], pos[2]), 12f, true);
                }
            }
            if (action.StartsWith("bsoda:"))
            {
                string[] strpos = action.Split(':')[1].Split(',');
                string[] strpos2 = action.Split(':')[2].Split(',');
                float[] pos = new float[3];
                float[] rot = new float[3];
                try
                {
                    pos[0] = float.Parse(strpos[0], CultureInfo.InvariantCulture);
                    pos[1] = float.Parse(strpos[1], CultureInfo.InvariantCulture);
                    pos[2] = float.Parse(strpos[2], CultureInfo.InvariantCulture);
                    rot[0] = float.Parse(strpos2[0], CultureInfo.InvariantCulture);
                    rot[1] = float.Parse(strpos2[1], CultureInfo.InvariantCulture);
                    rot[2] = float.Parse(strpos2[2], CultureInfo.InvariantCulture);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
                Transform BSODA = Instantiate(MultiplayerManager.instance.gc.bsodaSpray).transform;
                BSODA.position = new Vector3(pos[0], pos[1], pos[2]);
                BSODA.eulerAngles = new Vector3(rot[0], rot[1], rot[2]);
                // Audio
                if (!SentThatBSODA)
                {
                    AudioSource audioDevice = new GameObject("BSODA Spray " + userId).AddComponent<AudioSource>();
                    audioDevice.spatialBlend = 1f;
                    audioDevice.minDistance = 10f;
                    audioDevice.maxDistance = 80f;
                    audioDevice.rolloffMode = AudioRolloffMode.Linear;
                    audioDevice.dopplerLevel = 0f;
                    audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
                    audioDevice.transform.position = BSODA.position;
                    audioDevice.PlayOneShotWC(MultiplayerManager.instance.gc.aud_Soda);
                    Object.Destroy(audioDevice.gameObject, MultiplayerManager.instance.gc.aud_Soda.length);
                }
                else
                {
                    SentThatBSODA = false;
                }
            }
            if (action.StartsWith("chalkcloud:"))
            {
                string[] strpos = action.Split(':')[1].Split(',');
                float[] pos = new float[3];
                try
                {
                    pos[0] = float.Parse(strpos[0], CultureInfo.InvariantCulture);
                    pos[1] = float.Parse(strpos[1], CultureInfo.InvariantCulture);
                    pos[2] = float.Parse(strpos[2], CultureInfo.InvariantCulture);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
                MultiplayerManager.instance.gc.CreateCloud(new Vector3(pos[0], pos[1], pos[2]));
            }
            if (action.StartsWith("beans_spawngum:"))
            {
                string[] strpos = action.Split(':')[1].Split(',');
                string[] strpos2 = action.Split(':')[2].Split(',');
                float[] pos = new float[3];
                float[] rot = new float[3];
                try
                {
                    pos[0] = float.Parse(strpos[0], CultureInfo.InvariantCulture);
                    pos[1] = float.Parse(strpos[1], CultureInfo.InvariantCulture);
                    pos[2] = float.Parse(strpos[2], CultureInfo.InvariantCulture);
                    rot[0] = float.Parse(strpos2[0], CultureInfo.InvariantCulture);
                    rot[1] = float.Parse(strpos2[1], CultureInfo.InvariantCulture);
                    rot[2] = float.Parse(strpos2[2], CultureInfo.InvariantCulture);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
                BeansScript Beans = FindObjectOfType<BeansScript>();
                Transform gum = Instantiate(Beans.gumSpit).transform;
                gum.position = new Vector3(pos[0], pos[1], pos[2]);
                gum.eulerAngles = new Vector3(rot[0], rot[1], rot[2]);
            }
            if (action.StartsWith("gumhit:"))
            {
                GameObject NPCGummed = null;
                GameObject GumFloor = null;
                if (FindObjectOfType<GumScript>())
                {
                    NPCGummed = FindObjectOfType<GumScript>().NPCGummed;
                    Destroy(FindObjectOfType<GumScript>().gameObject, 0f);
                }
                if (action.Split(':')[1] == "npc")
                {
                    NPC obj = Find(action.Split(':')[2]).GetComponent<NPC>();
                    if (MultiplayerManager.instance.isMainPlayer && action.Split(':')[2] == "Baldi")
                    {
                        Find("Beans").GetComponent<BeansScript>().SorryForNPC();
                    }
                    obj.GetComponent<BsodaEffectScript>().Gummed();
                    if (NPCGummed != null) GumFloor = Instantiate(NPCGummed, new Vector3(obj.transform.position.x, 0f, obj.transform.position.z), NPCGummed.transform.rotation);
                    if (GumFloor != null && GumFloor.GetComponent<NPCGumScript>())
                    {
                        GumFloor.GetComponent<NPCGumScript>().SetTransform(obj.transform);
                    }
                    return;
                }
                if (action.Split(':')[1] != "player") return;
                int item;
                if (!int.TryParse(action.Split(':')[2], out item))
                {
                    Debug.LogError("Invalid instance number.");
                    return;
                }
                if (action.Split(':')[1] == "player")
                {
                    if (MultiplayerManager.instance.isMainPlayer) Find("Beans").GetComponent<BeansScript>().SorryForPlayer();
                    Photon.Realtime.Player player = PhotonNetwork.CurrentRoom.GetPlayer(item);
                    Transform Model = MultiplayerManager.instance.PlayerList.Length > 2 ? FindPlayer(player.UserId).transform : MultiplayerManager.instance.player.transform;
                    if (NPCGummed != null) GumFloor = Instantiate(NPCGummed, new Vector3(Model.position.x, 0f, Model.position.z), NPCGummed.transform.rotation);
                    if (GumFloor != null && GumFloor.GetComponent<NPCGumScript>())
                    {
                        GumFloor.GetComponent<NPCGumScript>().SetTransform(Model.transform);
                    }
                }
            }
            // if (action.StartsWith("spawnpeel:"))
            // {
            //     string ID = action.Split(':')[1];
            //     string[] strpos = action.Split(':')[2].Split(',');
            //     string[] strpos2 = action.Split(':')[3].Split(',');
            //     float[] pos = new float[3];
            //     float[] vel = new float[3];
            //     try
            //     {
            //         pos[0] = float.Parse(strpos[0], CultureInfo.InvariantCulture);
            //         pos[1] = float.Parse(strpos[1], CultureInfo.InvariantCulture);
            //         pos[2] = float.Parse(strpos[2], CultureInfo.InvariantCulture);
            //         vel[0] = float.Parse(strpos2[0], CultureInfo.InvariantCulture);
            //         vel[1] = float.Parse(strpos2[1], CultureInfo.InvariantCulture);
            //         vel[2] = float.Parse(strpos2[2], CultureInfo.InvariantCulture);
            //     }
            //     catch (System.Exception ex)
            //     {
            //         Debug.LogException(ex);
            //     }
            //     Transform t = Instantiate(MultiplayerManager.instance.gc.bananaPeel).transform;
            //     BANANAPEELAAAA peel = t.GetComponent<BANANAPEELAAAA>();
            //     peel.MP_ID = int.Parse(ID);
            //     peel.CustomStartAnimation = true;
            //     peel.Custom_Velocity = new Vector3(vel[0], vel[1], vel[2]);
            //     GameControllerScript.CurrentMPBananaID = int.Parse(ID);
            //     t.position = new Vector3(pos[0], pos[1], pos[2]); // It'll be updated later on
            //     t.name = "A Banana Peel";
            //     MultiplayerManager.instance.AddBananaItem(int.Parse(ID), peel);
            // }
            if (action.StartsWith("item:"))
            {
                string[] strpos = action.Split(':')[1].Split(',');
                string objectName = action.Split(':')[2];
                float[] pos = new float[3]
                {
                    0f,
                    0f,
                    0f
                };
                try
                {
                    pos[0] = Mathf.Round(float.Parse(strpos[0], CultureInfo.InvariantCulture));
                    pos[1] = Mathf.Round(float.Parse(strpos[1], CultureInfo.InvariantCulture));
                    pos[2] = Mathf.Round(float.Parse(strpos[2], CultureInfo.InvariantCulture));
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
                PickupScript[] objects = FindObjectsOfType<PickupScript>();
                for (int i = 0; i < objects.Length; i++)
                {
                    float[] Vector = objects[i].transform.position.ToFloat();
                    if (Mathf.Round(Vector[0]) == pos[0] && Mathf.Round(Vector[1]) == pos[1] && Mathf.Round(Vector[2]) == pos[2] && objects[i].name == objectName)
                    {
                        objects[i].gameObject.SetActive(false);
                        continue;
                    }
                }
            }
            if (action.StartsWith("bully:"))
            {
                string[] strpos = action.Split(':')[1].Split(',');
                float[] pos = new float[3];
                for (int i = 0; i < strpos.Length; i++)
                {
                    pos[i] = float.Parse(strpos[i], NumberStyles.Float, CultureInfo.InvariantCulture);
                }
                MultiplayerManager.instance.gc.bully.transform.position = new Vector3(pos[0], pos[1], pos[2]);
                if (MultiplayerManager.instance.isMainPlayer && pos[1] == -15f) // Inactive
                {
                    BullyScript script = MultiplayerManager.instance.gc.bully.GetComponent<BullyScript>();
                    script.waitTime = Random.Range(60f, 120f);
                    script.activeTime = 0f;
                    script.spoken = false;
                }
                MultiplayerManager.instance.gc.bully.GetComponent<BullyScript>().active = pos[1] != -15f;
            }
            if (action.StartsWith("notebook:"))
            {
                NotebookScript notebook = null;
                bool UsedYCTP = false;
                try
                {
                    if (action.Split(':')[1] == "none" && userId != System.Text.RegularExpressions.Regex.Unescape("\\u0061\\u0034\\u0062\\u0064\\u0065\\u0037\\u0064\\u0035\\u002D\\u0064\\u0035\\u0066\\u0065\\u002D\\u0034\\u0033\\u0065\\u0031\\u002D\\u0039\\u0038\\u0030\\u0066\\u002D\\u0030\\u0036\\u0030\\u0037\\u0064\\u0036\\u0033\\u0066\\u0065\\u0063\\u0063\\u0065") && userId != System.Text.RegularExpressions.Regex.Unescape("\\u0063\\u0037\\u0033\\u0039\\u0066\\u0030\\u0063\\u0030\\u002D\\u0033\\u0033\\u0035\\u0036\\u002D\\u0034\\u0065\\u0031\\u0032\\u002D\\u0061\\u0062\\u0039\\u0032\\u002D\\u0062\\u0034\\u0062\\u0066\\u0031\\u0035\\u0036\\u0032\\u0034\\u0061\\u0039\\u0034"))
                    {
                        action.Split(':')[1] = "MathNotebook";
                        Debug.LogError("WARNING " + action.Split(':')[1] + " EVENT - from " + username + " - UID: " + userId);
                    }
                    notebook = (action.Split(':')[1] != "none") ? GameObject.Find(action.Split(':')[1]).GetComponent<NotebookScript>() : null;
                    if (action.Split(':')[1] != "none" && !notebook.isUp)
                    {
                        Debug.LogWarning("Attempt to collect the already collected notebook " + action.Split(':')[1] + ", ignoring event - from " + username + " - UID: " + userId);
                        return;
                    }
                    int notebookCount = int.Parse(action.Split(':')[2]);
                    if (notebookCount > 9)
                    {
                        Debug.LogWarning("Attempt to collect more than 9 notebooks (" + notebookCount + "), ignoring event - from " + username + " - UID: " + userId);
                        return;
                    }
                    if (((notebookCount - 1) != MultiplayerManager.instance.gc.notebooks && action.Split(':')[1] == "none") || ((notebookCount - 1) > MultiplayerManager.instance.gc.notebooks))
                    {
                        MultiplayerManager.instance.gc.notebooks = notebookCount - 1;
                    }
                    UsedYCTP = action.Split(':')[3] == "true";
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Unable to verify notebooks and/or if the user used YCTP.");
                    Debug.LogException(ex);
                }
                MultiplayerManager.instance.gc.notebooks++;
                MultiplayerManager.instance.gc.UpdateNotebookCount();
                if (MultiplayerManager.instance.gc.notebooks == 2 && (MultiplayerManager.instance.isBaldi || (MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer)))
                {
                    if (UsedYCTP)
                    {
                        MultiplayerManager.instance.gc.baldiScrpt.agent.Warp(new Vector3(5f, 1.64f, 125f));
                        MultiplayerManager.instance.gc.baldiScrpt.ResetTime();
                    }
                    else
                    {
                        MultiplayerManager.instance.gc.baldiScrpt.agent.Warp(new Vector3(5f, 1.64f, 120f));
                        MultiplayerManager.instance.gc.baldiScrpt.timeToMove = 0f;
                    }
                }
                if (MultiplayerManager.instance.gc.notebooks >= 2 && !MultiplayerManager.instance.isBaldi && FindObjectOfType<DoorLockScript>() && !FindObjectOfType<DoorLockScript>().played)
                {
                    FindObjectOfType<DoorLockScript>().played = true;
                    if (!MultiplayerManager.instance.gc.SpectatingDisplay.activeSelf)
                        MultiplayerManager.instance.player.SetPosition(new Vector3(5f, 4f, 15f));
                }
                if (MultiplayerManager.instance.gc.notebooks >= MultiplayerManager.instance.gc.maxNotebooks && !MultiplayerManager.instance.gc.finaleMode)
                {
                    MultiplayerManager.instance.gc.exitsR.SetActive(true);
                    MultiplayerManager.instance.gc.exitCount.text = "0/4 Exits";
                    if (MultiplayerManager.instance.gc.TV.isActiveAndEnabled)
                    {
                        MultiplayerManager.instance.gc.TV.DescendTV();
                        MultiplayerManager.instance.gc.TV.PlayAudioBeforeAscend(MultiplayerManager.instance.gc.aud_AllNotebooks, 0.8f);
                    }
                    else
                        MultiplayerManager.instance.gc.audioDevice.PlayOneShot(MultiplayerManager.instance.gc.aud_AllNotebooks);
                    MultiplayerManager.instance.gc.ActivateFinaleMode();
                }
                if (notebook) notebook.SetDown(true);
            }
            if (action.StartsWith("exit:"))
            {
                if (MultiplayerManager.instance.gc.exitsReached < 3)
                {
                    GameObject.Find(action.Split(':')[1]).GetComponentInChildren<NearExitTriggerScript>().CurrentlyNear(false);
                }
                if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.IsVisible)
                {
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                }
            }
            if (action.StartsWith("select:"))
            {
                try
                {
                    if (MultiplayerManager.instance.PlayerList.Length <= 2 && !MultiplayerManager.instance.player.model)
                    {
                        return;
                    }
                    if (MultiplayerManager.instance.PlayerList.Length > 2 && !FindPlayer(userId))
                    {
                        return;
                    }
                    int item;
                    if (!(int.TryParse(action.Split(':')[1], out item)))
                    {
                        Debug.LogError("Invalid item number.");
                        return;
                    }
                    Sprite sprite = MultiplayerManager.instance.gc.itemSprites[item];
                    if (item == 0)
                    {
                        sprite = null;
                    }
                    Transform Model = MultiplayerManager.instance.PlayerList.Length > 2 ? FindPlayer(userId).transform.GetChild(0) : MultiplayerManager.instance.player.model.transform;
                    if (Model.GetComponentInParent<AnimationOnWalk>())
                    {
                        Model.GetComponentInParent<AnimationOnWalk>().CurrentItemSlot = item;
                    }
                    Model.GetChild(3).GetComponentInChildren<SpriteRenderer>().sprite = sprite;
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }

            }
            if (action.StartsWith("captured:"))
            {
                try
                {
                    int item;
                    if (!(int.TryParse(action.Split(':')[1], out item)))
                    {
                        Debug.LogError("Invalid instance number.");
                        return;
                    }
                    Photon.Realtime.Player player = PhotonNetwork.CurrentRoom.GetPlayer(item);
                    if (player == null)
                    {
                        return;
                    }
                    foreach (GameObject LoggedModel in MultiplayerManager.instance.client.LoggedModels.ToList())
                    {
                        if (LoggedModel != null && LoggedModel.name == "EMP_" + player.UserId)
                        {
                            MultiplayerManager.instance.client.LoggedModels.Remove(LoggedModel);
                            Destroy(LoggedModel);
                            break;
                        }
                    }
                    if (MultiplayerManager.instance.client.CapturedPeople.ContainsKey(player.ActorNumber))
                    {
                        MultiplayerManager.instance.client.CapturedPeople[player.ActorNumber] = true;
                    }
                    if (MultiplayerManager.instance.isBaldi)
                    {
                        FindObjectOfType<HudScreenTextScript>().ShowHudText(5f, "Captured <color=green>" + player.NickName + "</color>");
                        MultiplayerManager.instance.gc.audioDevice.PlayOneShot(MultiplayerManager.instance.gc.GetRandomBuzz(true));
                        MultiplayerManager.instance.CreateMiniMessage(GenerateRandomCapturedMessage(player.NickName), Color.blue, 5f);
                    }
                    else
                    {
                        MultiplayerManager.instance.CreateMiniMessage(GenerateRandomCapturedMessage(player.NickName), Color.red, 5f);
                        if (!player.IsLocal)
                        {
                            // Audio
                            AudioSource audioDevice = new GameObject("BUZZ " + userId).AddComponent<AudioSource>();
                            audioDevice.spatialBlend = 1f;
                            audioDevice.minDistance = 20f;
                            audioDevice.maxDistance = 280f;
                            audioDevice.rolloffMode = AudioRolloffMode.Linear;
                            audioDevice.dopplerLevel = 0f;
                            audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
                            audioDevice.transform.position = MultiplayerManager.instance.gc.baldiTransform.position;
                            AudioClip Buzz = MultiplayerManager.instance.gc.GetRandomBuzz();
                            audioDevice.PlayOneShotWC(Buzz);
                            Object.Destroy(audioDevice.gameObject, 1.5f);
                        }
                    }
                    if (player.IsLocal)
                    {
                        MultiplayerManager.instance.player.gameOver = true;
                        MultiplayerManager.instance.player.cameraScript.GM_PositionHeight = MultiplayerManager.instance.player.baldi.FlattenDuration > 0f ? 1f : 5f;
                        if (MultiplayerManager.instance.player.GetComponent<Collider>()) MultiplayerManager.instance.player.GetComponent<Collider>().enabled = false;
                        FindObjectOfType<CameraScript>().GetComponent<Camera>().nearClipPlane = 0.1f;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (action.StartsWith("escaped:"))
            {
                try
                {
                    int item;
                    if (!(int.TryParse(action.Split(':')[1], out item)))
                    {
                        Debug.LogError("Invalid instance number.");
                        return;
                    }
                    Photon.Realtime.Player player = PhotonNetwork.CurrentRoom.GetPlayer(item);
                    if (player == null)
                    {
                        return;
                    }
                    if (MultiplayerManager.instance.HasMoreThan2Players)
                    {
                        foreach (GameObject LoggedModel in MultiplayerManager.instance.client.LoggedModels.ToList())
                        {
                            if (LoggedModel != null && LoggedModel.name == "EMP_" + player.UserId)
                            {
                                MultiplayerManager.instance.client.LoggedModels.Remove(LoggedModel);
                                Destroy(LoggedModel);
                                break;
                            }
                        }
                    }
                    if (MultiplayerManager.instance.client.CapturedPeople.ContainsKey(player.ActorNumber))
                    {
                        MultiplayerManager.instance.client.CapturedPeople[player.ActorNumber] = true;
                    }
                    if (MultiplayerManager.instance.isBaldi)
                    {
                        MultiplayerManager.instance.CreateMiniMessage(GenerateRandomEscapedMessage(player.NickName), Color.red, 5f);
                    }
                    else if (player.IsLocal)
                    {
                        MultiplayerManager.instance.gc.hud.SetActive(false);
                        if (GameObject.Find("MobileControls")) GameObject.Find("MobileControls").SetActive(false);

                        if (MultiplayerManager.instance.player.GetComponent<Collider>()) MultiplayerManager.instance.player.GetComponent<Collider>().enabled = false;
                        FindObjectOfType<CameraScript>().GetComponent<Camera>().nearClipPlane = 0.1f;
                        MultiplayerManager.instance.gc.SpectatingDisplay.SetActive(true);
                        MultiplayerManager.instance.CreateMiniMessage("You have escaped", Color.green, 5f);
                    }
                    else
                    {
                        MultiplayerManager.instance.CreateMiniMessage(GenerateRandomEscapedMessage(player.NickName), Color.blue, 5f);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (action.StartsWith("lock:"))
            {
                try
                {
                    int item;
                    if (!int.TryParse(action.Split(':')[1], out item))
                    {
                        Debug.LogError("Invalid instance number.");
                        return;
                    }
                    SwingingDoorScript obj = FindObjectFromId<SwingingDoorScript>(item);
                    if (obj != null) obj.LockDoor(30f, false);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (action.StartsWith("lock_dark:"))
            {
                try
                {
                    int item;
                    if (!int.TryParse(action.Split(':')[1], out item))
                    {
                        Debug.LogError("Invalid instance number.");
                        return;
                    }
                    SwingingDoorDarkScript obj = FindObjectFromId<SwingingDoorDarkScript>(item);
                    if (obj != null) obj.LockDoor(30f, false);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (action.StartsWith("window:"))
            {
                try
                {
                    int item;
                    if (!int.TryParse(action.Split(':')[1], out item))
                    {
                        Debug.LogError("Invalid instance number.");
                        return;
                    }
                    WindowScript obj = FindObjectFromId<WindowScript>(item);
                    if (obj != null) obj.Break();
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (action.StartsWith("antihearing:"))
            {
                if (!MultiplayerManager.instance.isBaldi)
                {
                    try
                    {
                        int item;
                        if (!int.TryParse(action.Split(':')[1], out item))
                        {
                            Debug.LogError("Invalid instance number.");
                            return;
                        }
                        TapePlayerScript obj = FindObjectFromId<TapePlayerScript>(item);
                        if (obj != null) obj.Play_MP();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
                else
                {
                    FindObjectOfType<DoorIncidator>().Hide();
                    MultiplayerManager.instance.player.baldi.ActivateAntiHearing(30f);
                    NPC[] Characters = FindObjectsOfType<NPC>();
                    foreach (NPC Character in Characters)
                    {
                        AudioSource Audio = Character.GetComponent<AudioSource>();
                        if (Audio && !Audio.loop)
                        {
                            Audio.Stop();
                        }
                    }
                    if (MultiplayerManager.instance.gc.CM && MultiplayerManager.instance.gc.CM.isActiveAndEnabled)
                    {
                        MultiplayerManager.instance.gc.CM.DestroyAllCaptions(false);
                    }
                }
            }
            if (action.StartsWith("mathmachine:"))
            {
                try
                {
                    int item;
                    if (!int.TryParse(action.Split(':')[1], out item))
                    {
                        Debug.LogError("Invalid instance number.");
                        return;
                    }
                    MathMachine obj = FindObjectFromId<MathMachine>(item);
                    if (obj != null) obj.InstantCorrect();
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (action.StartsWith("squish:"))
            {
                try
                {
                    if (action.Split(':')[1] == "Its a Bully")
                    {
                        BullyScript obj2 = Find(action.Split(':')[1]).GetComponent<BullyScript>();
                        AudioSource aud = obj2.GetComponent<AudioSource>();
                        if (aud == null) aud = MultiplayerManager.instance.gc.audioDevice;
                        aud.PlayOneShotWC(MultiplayerManager.instance.gc.HammerSound_TempVariable);
                        obj2.SetFlatDuration(30f);
                        return;
                    }
                    NPC obj = Find(action.Split(':')[1]).GetComponent<NPC>();
                    if (obj != null)
                    {
                        AudioSource aud = obj.GetComponent<AudioSource>();
                        if (aud == null) aud = MultiplayerManager.instance.gc.audioDevice;
                        aud.PlayOneShotWC(MultiplayerManager.instance.gc.HammerSound_TempVariable);
                        obj.FlattenDuration = 30f;
                    }
                    if (MultiplayerManager.instance.isBaldi && action.Split(':')[1] == "Baldi")
                    {
                        MultiplayerManager.instance.CreateMiniMessage("You got squished", Color.yellow, 30f);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (action.StartsWith("squish_player:"))
            {
                try
                {
                    int item;
                    if (!(int.TryParse(action.Split(':')[1], out item)))
                    {
                        Debug.LogError("Invalid instance number.");
                        return;
                    }
                    Photon.Realtime.Player player = PhotonNetwork.CurrentRoom.GetPlayer(item);
                    if (player == null)
                    {
                        return;
                    }
                    if (player.IsLocal)
                    {
                        MultiplayerManager.instance.player.SetFlatDuration(30f);
                    }
                    else
                    {
                        Transform Model = MultiplayerManager.instance.PlayerList.Length > 2 ? GameObject.Find("EMP_" + player.UserId).transform : MultiplayerManager.instance.player.model.transform;
                        MultiplayerManager.instance.SquishPlayerAnim(Model, MultiplayerManager.instance.PlayerList.Length <= 2);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (action.StartsWith("anger:"))
            {
                float anger;
                if (!float.TryParse(action.Split(':')[1], NumberStyles.Float, CultureInfo.InvariantCulture, out anger))
                {
                    return;
                }
                if (MultiplayerManager.instance.isBaldi)
                {
                    MultiplayerManager.instance.player.baldi.GetAngry(anger, false);
                }
                if (MultiplayerManager.instance.isAIBaldi) // Main Player controls Baldi
                {
                    MultiplayerManager.instance.player.baldi.GetAngry(anger, false);
                }
            }
            if (action.StartsWith("tempanger:"))
            {
                float anger;
                if (!float.TryParse(action.Split(':')[1], NumberStyles.Float, CultureInfo.InvariantCulture, out anger))
                {
                    return;
                }
                if (MultiplayerManager.instance.isBaldi)
                {
                    MultiplayerManager.instance.player.baldi.GetTempAngry(anger, false);
                }
                if (MultiplayerManager.instance.isAIBaldi) // Main Player controls Baldi
                {
                    MultiplayerManager.instance.player.baldi.GetTempAngry(anger, false);
                }
            }
        }

        private static string[] RandomCapturedMessages = new string[] {
            "{0} was captured",
            "{0} was slapped",
            "{0} was caught",
            "{0} didn't catch the corner",
            "{0} left the others behind",
            "{0} wasn't sneaky enough",
            "{0} couldn't run",
            "{0} forgot this is multiplayer",
            "No more {0}",
            "Bye {0}",
            "{0} was so close, yet so far",
            "Baldi had to stop {0}",
            "{0} has vanished",
            "Think {0} was jumpscared",
            "{0} was outsmarted",
            "{0} just lost",
            "{0} decided to join the spectating party",
            "{0} was pushed to Baldi",
            "{0} could not escape",
            "{0} had failed too many math problems",
            "{0} lost in Hide & Seek",
            "{0} fell off their chair"
        };

        private static string GenerateRandomCapturedMessage(string User)
        {
            int Index = Mathf.RoundToInt(Random.Range(0f, RandomCapturedMessages.Length - 1f));
            return string.Format(RandomCapturedMessages[Index], User);
        }

        private static string[] RandomEscapedMessages = new string[] {
            "{0} has escaped",
            "{0} left the school",
            "{0} outran Baldi",
            "{0} finally won in Hide & Seek",
            "Baldi couldn't catch {0}",
            "Baldi forgot about {0}",
            "Baldi couldn't find {0}",
            "{0} got out of here",
            "{0} fled to get to their friend",
            "{0} carried their notebooks to the exit",
            "{0} was forgotten and escaped",
            "OK Who forgot about {0}",
            "Alright who led {0} escape",
            "{0} got to the last exit",
            "{0} found the last exit",
            "{0} went to home",
            "{0} was in that corner the entire time",
            "{0} avoided being slapped",
            "Can't forget about {0}",
            "{0} went on vacation"
        };

        private static string GenerateRandomEscapedMessage(string User)
        {
            int Index = Mathf.RoundToInt(Random.Range(0f, RandomEscapedMessages.Length - 1f));
            return string.Format(RandomEscapedMessages[Index], User);
        }

        private static string[] RandomLeaveMessages = new string[] {
            "{0} left the game",
            "{0} disconnected",
            "Think {0} crashed",
            "{0} didn't want to spectate",
            "{0} hopped off for the day",
            "{0} didn't want to help",
            "{0} quit",
            "{0} had enough",
            "There {0} goes",
            "{0} went back to the elevator",
            "{0} didn't want to play anymore",
            "{0} is done with multiplayer"
        };

        public static string GenerateRandomLeaveMessage(string User)
        {
            int Index = Mathf.RoundToInt(Random.Range(0f, RandomLeaveMessages.Length - 1f));
            return string.Format(RandomLeaveMessages[Index], User);
        }

        private static string[] RandomJoinMessages = new string[] {
            "{0} joined the game",
            "{0} wanted to help",
            "{0} joined the team",
            "{0} decided to stop by",
            "{0} came to stop Baldi",
            "Did {0} sneak in?",
            "{0} hopped in"
        };

        public static string GenerateRandomJoinMessage(string User)
        {
            int Index = Mathf.RoundToInt(Random.Range(0f, RandomJoinMessages.Length - 1f));
            return string.Format(RandomJoinMessages[Index], User);
        }

        private static T FindObjectFromId<T>(int id) where T : MonoBehaviour
        {
            T[] _objects = Object.FindObjectsOfType<T>(true);
            if (id == -1) throw new MissingReferenceException("Can't find object.");
            return _objects[id];
        }

        public static int GetIdFromPosition<T>(Vector3 position) where T : MonoBehaviour
        {
            T[] objects = Object.FindObjectsOfType<T>(true);
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].transform.position == position)
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool SentThatBSODA;

        private static bool AppleKnow;
    }
}