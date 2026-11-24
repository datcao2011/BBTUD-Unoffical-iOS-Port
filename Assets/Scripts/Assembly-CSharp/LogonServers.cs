using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LogonServers : MonoBehaviour
{
    public GameObject connection;

    public GameObject failure;

    public GameObject logonScreen;

    public static GameObject thisLogonScreen;

    private bool connect;

    private void Start()
    {
        thisLogonScreen = logonScreen;
        StartConnection();
        connect = true;
    }

    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable & connect)
        {
            connect = false;
            StartConnection();
        }
        else if (Application.internetReachability != NetworkReachability.NotReachable & !connect)
        {
            connect = true;
            StartConnection();
        }

    }

    public void StartConnection()
    {
        StopAllCoroutines();
        StartCoroutine(Connect());
    }

    private class StatusResponse
    {
        public string message;
        public bool available;
    }

    private IEnumerator Connect()
    {
        logonScreen.SetActive(false);
        connection.SetActive(true);
        connection.GetComponentInChildren<TMPro.TMP_Text>().text = "Connecting to server";
        failure.SetActive(false);
        UnityWebRequest request = UnityWebRequest.Get(InputManager.siteRequestUrl + "bbtudCommunityStatus?v=" + VersionInput.currentVersion);
        yield return request.SendWebRequest();
        if (!string.IsNullOrEmpty(request.error) && request.result != UnityWebRequest.Result.ProtocolError)
        {
            InputManager.DisplayException(new System.Exception(request.error), 5f);
            connection.SetActive(false);
            failure.SetActive(true);
        }
        StatusResponse response;
        try
        {
            response = JsonUtility.FromJson<StatusResponse>(request.downloadHandler.text);
            if (response == null) throw new System.Exception("Non-type JSON response received: " + request.downloadHandler.text.Substring(0, 30));
        }
        catch (System.Exception ex)
        {
            response = new StatusResponse
            {
                available = false,
                message = ex.Message + " | " + request.downloadHandler.text
            };
        }
        if (!response.available)
        {
            InputManager.DisplayException(new System.Exception(response.message), 15f);
            connection.SetActive(false);
            failure.SetActive(true);
            yield break;
        }
        console.log(response.message);
        if (Settings.HasKey("Account") & Settings.HasKey("AccountSession"))
        {
            AccountScript.script.StartCoroutine(AccountScript.script.logonFromLaunchProcess(Settings.GetString("Account"), Settings.GetString("AccountSession")));
            connection.GetComponentInChildren<TMPro.TMP_Text>().text = "Connecting account";
            failure.SetActive(false);
            Settings.DeleteKey("Account");
            Settings.DeleteKey("AccountSession");
            yield break;
        }
        logonScreen.SetActive(true);
        connection.SetActive(false);
        failure.SetActive(false);
        yield break;
    }
}
