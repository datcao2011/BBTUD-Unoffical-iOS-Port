using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace Multiplayer
{
    public class LobbyPlayerListing : MonoBehaviourPunCallbacks
    {
        private bool CurrentlyFetchingProfilePicture;

        public GameObject Player_1;
        public GameObject Player_2;
        public GameObject Player_3;
        public TMP_Text MorePlayerCount;

        public GameObject HostControls;
        public GameObject SettingsPanel;
        public GameObject CharacterEnabler;
        public GameObject GoButton;

        [Header("Settings Items")]
        public TMP_Dropdown MaxPlayerDropdown;
        public TMP_Dropdown RandomEventsDropdown;
        public TMP_Dropdown SecondFloorDropdown;
        public TMP_Dropdown NotebooksDropdown;
        public TMP_Dropdown CustomBaldiDropdown;
        public TMP_Dropdown InProgressDropdown;
        public TMP_Dropdown YCTPDropdown;

        public MultiplayerManager.GameSettings Settings = new MultiplayerManager.GameSettings();

        // Start is called before the first frame update
        private void Start()
        {
            RandomEventsDropdown.onValueChanged.AddListener((Index) =>
            {
                Settings.RandomEvents = Index == 0;
            });
            SecondFloorDropdown.onValueChanged.AddListener((Index) =>
            {
                Settings.SecondFloor = Index == 0;
                List<string> Options = new List<string>();
                Options.AddRange(new string[] { "Automatic", "2", "3", "4", "5", "6", "7" });
                if (Settings.SecondFloor)
                {
                    Options.AddRange(new string[] { "8", "9" });
                }
                int LastValue = NotebooksDropdown.value;
                NotebooksDropdown.ClearOptions();
                NotebooksDropdown.AddOptions(Options);
                NotebooksDropdown.value = (int)Mathf.Clamp(LastValue, 0f, Options.Count - 1);
                NotebooksDropdown.RefreshShownValue();
            });
            NotebooksDropdown.onValueChanged.AddListener((Index) =>
            {
                Settings.Notebooks = (Index == 0) ? -1 : (Index + 1);
            });
            CustomBaldiDropdown.onValueChanged.AddListener((Index) =>
            {
                if (Index <= 1)
                {
                    Settings.PickedBaldi = Index - 2;
#if DEBUG
                    Debug.Log("AI Baldi: " + Settings.PickedBaldi + " - " + (Settings.PickedBaldi == -1));
#endif
                }
                else
                {
                    Player[] OtherPlayers = PhotonNetwork.PlayerListOthers;
                    Settings.PickedBaldi = Index == 2 ? PhotonNetwork.LocalPlayer.ActorNumber : OtherPlayers[Index - 3].ActorNumber;
#if DEBUG
                    Debug.Log("Current Baldi set to: " + Settings.PickedBaldi + " - " + PhotonNetwork.CurrentRoom.GetPlayer(Settings.PickedBaldi).NickName);
#endif
                }
            });
            InProgressDropdown.onValueChanged.AddListener((Index) =>
            {
                Settings.JoinInProgress = Index;
            });
            YCTPDropdown.onValueChanged.AddListener((Index) =>
            {
                Settings.YCTPUsage = Index;
            });
        }

        public override void OnEnable()
        {
            base.OnEnable();
            UpdateList();

            // If you're
            MaxPlayerDropdown.SetValueWithoutNotify(2);
            MaxPlayerDropdown.RefreshShownValue();
            // RandomEventsDropdown.value = RandomEventsDropdown.value;
            // NotebooksDropdown.value = NotebooksDropdown.value;
            // SecondFloorDropdown.value = SecondFloorDropdown.value;
            // InProgressDropdown.value = InProgressDropdown.value;

            HostControls.SetActive(false);
            SettingsPanel.SetActive(false);
            CharacterEnabler.SetActive(false);
            GoButton.SetActive(false);
        }

        private void Update()
        {
            if (PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible)
            {
                HostControls.SetActive(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.IsOpen && !MultiplayerManager.instance.client.Started);
                GoButton.SetActive(PhotonNetwork.CurrentRoom.PlayerCount > 1);
            }
        }

        public void OnMaxPlayersChanged(int index)
        {
            if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;
            switch (index)
            {
                case 0:
                    MultiplayerManager.instance.client.SetMaxPlayers(2);
                    break;
                case 1:
                    MultiplayerManager.instance.client.SetMaxPlayers(3);
                    break;
                case 3:
                    MultiplayerManager.instance.client.SetMaxPlayers(8);
                    break;
                case 4:
                    MultiplayerManager.instance.client.SetMaxPlayers(12);
                    break;
                case 5:
                    MultiplayerManager.instance.client.SetMaxPlayers(16);
                    break;
                default:
                    MultiplayerManager.instance.client.SetMaxPlayers(4);
                    break;
            }
        }

        public void EnableCharacter(int i)
        {
            Settings.CharacterActivated[i] = !Settings.CharacterActivated[i];
#if DEBUG
            Debug.Log("Character " + (i + 1) + ": " + Settings.CharacterActivated[i]);
#endif
        }

        public void StartTheGame()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                MultiplayerManager.instance.client.StartPreGame();
            }
        }

        public override void OnJoinedRoom()
        {
            UpdateList();
        }

        // Update is called once per frame
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UpdateList();
        }

        public override void OnPlayerLeftRoom(Player newPlayer)
        {
            UpdateList();
        }

        private void UpdateList()
        {
            Photon.Realtime.Player[] Players;
            if (PhotonNetwork.InRoom)
            {
                MorePlayerCount.gameObject.SetActive((PhotonNetwork.CurrentRoom.PlayerCount - 4) > 0);
                MorePlayerCount.text = $"(+{PhotonNetwork.CurrentRoom.PlayerCount - 4} Players)";
                Players = PhotonNetwork.PlayerListOthers;

                int LastValue = CustomBaldiDropdown.value;
                CustomBaldiDropdown.ClearOptions();
                List<string> Options = new List<string>();
                Options.AddRange(new string[] { "Random", "AI", "<color=green>" + PhotonNetwork.LocalPlayer.NickName + "</color>" });
                foreach (Player player in Players)
                {
                    Options.Add(WithLan(player.NickName, player.ActorNumber));
                }
                CustomBaldiDropdown.AddOptions(Options);
                CustomBaldiDropdown.value = (int)Mathf.Clamp(LastValue, 0f, Options.Count - 1);
                CustomBaldiDropdown.RefreshShownValue();
            }
            else
            {
                MorePlayerCount.gameObject.SetActive(false);
                Player_1.SetActive(false);
                Player_2.SetActive(false);
                Player_3.SetActive(false);
                Player_1.GetComponentInChildren<TMP_Text>().text = "";
                Player_1.GetComponentInChildren<RawImage>().texture = null;
                Player_2.GetComponentInChildren<TMP_Text>().text = "";
                Player_2.GetComponentInChildren<RawImage>().texture = null;
                Player_3.GetComponentInChildren<TMP_Text>().text = "";
                Player_3.GetComponentInChildren<RawImage>().texture = null;
                return;
            }
            #region Player 1
            if (Players.Length >= 1)
            {
                if (!Player_1.activeSelf || (Player_1.GetComponentInChildren<TMP_Text>().text != "" && RemoveTags(Player_1.GetComponentInChildren<TMP_Text>().text) != RemoveTags(Players[0].NickName)))
                {
                    StartCoroutine(FetchProfilePicture(Players[0].NickName, Player_1.GetComponentInChildren<RawImage>(), Player_1.GetComponentInChildren<TMP_Text>(), Players[0].ActorNumber));
                    Player_1.GetComponentInChildren<TMP_Text>().text = WithLan(Players[0].NickName ?? "(null)", Players[0].ActorNumber);
                }
                Player_1.SetActive(true);
            }
            else
            {
                Player_1.SetActive(false);
                Player_1.GetComponentInChildren<TMP_Text>().text = "";
                Player_1.GetComponentInChildren<RawImage>().texture = null;
            }
            #endregion
            #region Player 2
            if (Players.Length >= 2)
            {
                if (!Player_2.activeSelf || (Player_2.GetComponentInChildren<TMP_Text>().text != "" && RemoveTags(Player_2.GetComponentInChildren<TMP_Text>().text) != RemoveTags(Players[1].NickName)))
                {
                    StartCoroutine(FetchProfilePicture(Players[1].NickName, Player_2.GetComponentInChildren<RawImage>(), Player_2.GetComponentInChildren<TMP_Text>(), Players[1].ActorNumber));
                    Player_2.GetComponentInChildren<TMP_Text>().text = WithLan(Players[1].NickName ?? "(null)", Players[1].ActorNumber);
                }
                Player_2.SetActive(true);
            }
            else
            {
                Player_2.SetActive(false);
                Player_2.GetComponentInChildren<TMP_Text>().text = "";
                Player_2.GetComponentInChildren<RawImage>().texture = null;
            }
            #endregion
            #region Player 3
            if (Players.Length >= 3)
            {
                if (!Player_3.activeSelf || (Player_3.GetComponentInChildren<TMP_Text>().text != "" && RemoveTags(Player_3.GetComponentInChildren<TMP_Text>().text) != RemoveTags(Players[2].NickName)))
                {
                    StartCoroutine(FetchProfilePicture(Players[2].NickName, Player_3.GetComponentInChildren<RawImage>(), Player_3.GetComponentInChildren<TMP_Text>(), Players[2].ActorNumber));
                    Player_3.GetComponentInChildren<TMP_Text>().text = WithLan(Players[2].NickName ?? "(null)", Players[2].ActorNumber);
                }
                Player_3.SetActive(true);
            }
            else
            {
                Player_3.SetActive(false);
                Player_3.GetComponentInChildren<TMP_Text>().text = "";
                Player_3.GetComponentInChildren<RawImage>().texture = null;
            }
            #endregion
        }

        public static string RemoveTags(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", string.Empty);
        }

        private IEnumerator FetchProfilePicture(string username, RawImage ProfilePicture, TMP_Text UsernameText, int actor)
        {
            while (CurrentlyFetchingProfilePicture)
                yield return null;
            if (username.StartsWith("<"))
            {
                ProfilePicture.texture = MultiplayerManager.instance.Default;
                yield break;
            }
            AccountManager.GetProfilePicture(username);
            CurrentlyFetchingProfilePicture = true;
            float Timeout = 15f;
            while (Timeout > 0f && AccountManager.RetrievingProfilePicture)
            {
                Timeout -= Time.unscaledDeltaTime;
                yield return null;
            }
            CurrentlyFetchingProfilePicture = false;
            if (Timeout <= 0f)
            {
                ProfilePicture.texture = MultiplayerManager.instance.Default;
                InputManager.DisplayException(new System.Exception("Profile picture for " + username + " is not unavailable at this time."), 5f);
                yield break;
            }
            ProfilePicture.texture = AccountManager.LatestGotProfilePicture;
            UsernameText.text = WithLan(NameTagColor(AccountManager.LatestUserTag, username), actor);
            yield break;
        }

        private string WithLan(string name, int actor)
        {
            if (string.IsNullOrEmpty(name)) return name;
            if (MultiplayerManager.instance != null && MultiplayerManager.instance.client != null && MultiplayerManager.instance.client.HasLanPeerFor(actor))
                return name + MultiplayerManager.LanTag;
            return name;
        }

        private string NameTagColor(string tag, string user)
        {
            switch (tag.ToLower())
            {
                case "creator":
                    return "<color=green>" + user;
                case "helper":
                    return "<color=#2f98ad>" + user;
                case "dev":
                case "verified":
                    return "<color=blue>" + user;
                case "yt":
                    return user + " <color=red>YT";
                case "supporter":
                    return "<color=yellow>" + user;
            }
            return user;
        }
    }
}
