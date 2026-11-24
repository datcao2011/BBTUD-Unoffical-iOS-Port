using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Multiplayer;
using System.Collections;
using System.Collections.Generic;

public class CountDownSyncing : MonoBehaviour
{
    private Client client;
    private MultiplayerManager manager;
    public static bool e001;

    public TMPro.TMP_Text TextToEdit;
    public TMPro.TMP_Text _TextToEdit;

#if HAS_EVENT_PLUGGED
    private void Awake()
    {
        client = GetComponent<Client>();
        manager = GetComponent<MultiplayerManager>();
        SetText("Connecting to master server");
        client.EventServer_Connect();
        manager.isBaldi = false;
        e001 = false;
    }

    public void SetText(string text)
    {
        if (TextToEdit != null && IsPerformancePlatform()) TextToEdit.text = text;
    }

    public void C001()
    {
        StartCoroutine(IE001());
    }

    public void C002()
    {
        SceneManager.LoadScene("Starter");
        InputManager.DisplayEvent("Unable to join room, please check if there's an available server.", Color.red, 10f);
        Destroy(gameObject, 0f);
    }

    public void C003()
    {
        SetText("Waiting for management");
        PhotonNetwork.AutomaticallySyncScene = true;
        StartCoroutine(IE002());
    }

    private bool IsPerformancePlatform()
    {
#if (UNITY_EDITOR || UNITY_ANDROID || UNITY_WEBGL)
        return true;
#else
        return GameOptimizer.OptimizationDefault;
#endif
    }

    private IEnumerator IE002()
    {
        UnityEngine.Events.UnityAction<Scene, Scene> action = null;
        action = (scene1, scene2) =>
        {
            e001 = true;
            SceneManager.activeSceneChanged -= action;
        };
        SceneManager.activeSceneChanged += action;
        yield return new WaitForSecondsRealtime(0.08f);
        SceneLoader sl = FindObjectOfType<SceneLoader>();
        float progress = PhotonNetwork.LevelLoadingProgress;
        if (IsPerformancePlatform()) sl.textProgress.color = Color.white;
        manager.PlayerList = GetLatestPlayerList();
        SetText("Loading level assets");
        while (progress < 1f)
        {
            progress = Mathf.Clamp01(PhotonNetwork.LevelLoadingProgress / .9f);
            sl.slider.value = progress;
            float progressText = progress * 100f;
            sl.textProgress.text = Mathf.CeilToInt(progressText) + "%";
            yield return null;
        }
        sl.slider.value = 1f;
        sl.textProgress.text = "100%";
        sl.status.fontStyle = TMPro.FontStyles.Normal;
        sl.status.text = "!!!";
        Debug.unityLogger.Log("[EVENT MANAGER]", "HERE WE GO!");
        yield break;
    }

    public MultiplayerManager.PlayerListItem[] GetLatestPlayerList()
    {
        List<MultiplayerManager.PlayerListItem> playerListItems = new List<MultiplayerManager.PlayerListItem>();
        MultiplayerManager.PlayerListItem You = new MultiplayerManager.PlayerListItem();
        You.username = PhotonNetwork.LocalPlayer.NickName;
        You.tag = (string)PhotonNetwork.LocalPlayer.CustomProperties["tag"] ?? "";
        You.device = (string)PhotonNetwork.LocalPlayer.CustomProperties["device"] ?? "null";
        playerListItems.Add(You);

        foreach (Player play in PhotonNetwork.PlayerListOthers)
        {
            MultiplayerManager.PlayerListItem ite = new MultiplayerManager.PlayerListItem();
            if (play.NickName == "SERVER") continue;
            ite.username = play.NickName;
            ite.tag = (play.CustomProperties == null ? "" : (string)play.CustomProperties["tag"]) ?? "";
            ite.device = (play.CustomProperties == null ? "" : (string)play.CustomProperties["device"]) ?? "null";
            playerListItems.Add(ite);
        }
        return playerListItems.ToArray();
    }

    private IEnumerator IE001()
    {
        SetText("Finding available server");
        string roomId = "-";
        UnityWebRequest req = UnityWebRequest.Get(InputManager.siteRequestUrl + "Event/RoomId");
        yield return req.SendWebRequest();
        roomId = req.downloadHandler.text ?? "-";
        SetText("Joining server");
        client.JoinRoom(roomId);
        yield break;
    }
#endif
}