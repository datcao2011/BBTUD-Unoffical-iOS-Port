using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using UnityEngine.UI;

public class AccountScript : MonoBehaviour
{
    public TMP_InputField username;

    public GameObject load;

    public GameObject sceneLoad;

    public GameObject failure;

    public TMP_Text failureText;

    public TMP_InputField password;

    public TMP_InputField GJRegister1;

    public TMP_InputField GJRegister2;

    public Button RetryButton;

    public string urlTo;
    public TMP_Text websitetext;
    public GameObject websiteDialog;
    public static string LOAD_TO_SCENE = "Warning";

    public bool LoggingWithGameJolt;

    public UnityEngine.UI.Button GameJoltButton;

    public static AccountScript script;

    private void Awake()
    {
        script = this;
        QualitySettings.SetQualityLevel(1);
        if (LOAD_TO_SCENE == "Logon")
        {
            LOAD_TO_SCENE = "Warning";
        }
    }

    private void Update()
    {
        if ((username.gameObject.activeSelf) && Input.GetKeyDown(KeyCode.Return))
        {
            Logon();
        }
    }

    public void GoToWebsite(string url)
    {
        websiteDialog.SetActive(true);
        urlTo = url;
        websitetext.text = "This button is trying to open a website:\n" + urlTo + "\n\nGo to website?";
    }

    public void GoToSite()
    {
        websiteDialog.SetActive(false);
        Application.OpenURL(urlTo);
    }

    public void Logon()
    {
        if (busy)
        {
            return;
        }
        busy = true;
        string user = username.text;
        string pass = password.text;
        StartCoroutine(logonProcess(user, pass));
    }

    public void ShowLoading()
    {
        load.SetActive(true);
        failure.SetActive(false);
    }

    public void ShowFailure(string failure)
    {
        load.SetActive(false);
        this.failure.SetActive(true);
        failureText.text = failure;
        busy = false;
        RetryButton.gameObject.SetActive(false);
    }

    public void ShowFailureRetry(string failure, UnityEngine.Events.UnityAction OnRetry)
    {
        load.SetActive(false);
        this.failure.SetActive(true);
        failureText.text = failure;
        busy = false;
        RetryButton.gameObject.SetActive(true);
        RetryButton.onClick.RemoveAllListeners();
        RetryButton.onClick.AddListener(OnRetry);
    }

    public void Register()
    {
        if (busy)
        {
            return;
        }
        busy = true;
        string user = GJRegister1.text;
        string token = GJRegister2.text;
        StartCoroutine(registerProcess(user, token));
    }

    private bool busy;

    public static string md5hash(string str)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] byteHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            string hash = BitConverter.ToString(byteHash).Replace("-", "");
            return hash.ToLower();
        }
    }

    public IEnumerator logonFromLaunchProcess(string user, string sessionId)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(InputManager.siteRequestUrl + "validateSession?id=" + sessionId + "&gjinfo=1");
        yield return webRequest.SendWebRequest();
        bool success;
        RequestResponse3 data;
        try
        {
            data = JsonUtility.FromJson<RequestResponse3>(webRequest.downloadHandler.text);
            if (data == null) throw new Exception("Non-type JSON response received: " + webRequest.downloadHandler.text.Substring(0, 30));
        }
        catch (Exception ex)
        {
            data = new RequestResponse3
            {
                error = ex.Message,
                valid = false
            };
        }
        if (data.valid == false)
        {
            LogonServers.thisLogonScreen.SetActive(true);
            if (data.error != "Servers are too busy!")
            {
                ShowFailure("Could not connect to your account. Your session may have expired. Please login again to reconnect.");
                console.error("Server could not connect: (Server responded a code of " + webRequest.responseCode + "): " + data.error);
                InputManager.DisplayException(new System.UnauthorizedAccessException(data.error), 5f);
            }
            else
            {
                ShowFailureRetry("Servers are too busy right now. You'll have to login again.", () =>
                {
                    failure.SetActive(false);
                    StartCoroutine(logonFromLaunchProcess(user, sessionId));
                    FindObjectOfType<LogonServers>().connection.SetActive(true);
                    LogonServers.thisLogonScreen.SetActive(false);
                });
            }
            FindObjectOfType<LogonServers>().connection.SetActive(false);
            Settings.DeleteKey("Account");
            Settings.DeleteKey("AccountSession");
            success = false;
        }
        else if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            LogonServers.thisLogonScreen.SetActive(true);
            ShowFailure("No internet connection. Please connect to internet to login.");
            console.error("Server could not connect: (" + webRequest.error + ")");
            InputManager.DisplayException(new System.Exception(webRequest.error), 5f);
            FindObjectOfType<LogonServers>().connection.SetActive(false);
            Settings.DeleteKey("Account");
            Settings.DeleteKey("AccountSession");
            success = false;
        }
        else
        {
            success = true;
        }
        if (success)
        {
            Settings.SetString("Account", data.username);
            Settings.SetString("AccountSession", sessionId);
            if (!string.IsNullOrEmpty(data.gamejolt)) GameJolt.BBTUD.GameJoltManager.Create(data.gamejolt);
            AccountManager.SetData(data.username, sessionId);
            sceneLoad.SetActive(true);
            UnityEngine.SceneManagement.SceneManager.LoadScene(LOAD_TO_SCENE);
        }
        yield break;
    }

    public class RequestResponse
    {
        public bool success;
        public string error = "Uh oh! Servers are too busy right now. Please login again. If it still fails, wait a bit before retrying.";
        public int errorCode;
        public string message;
        public string toSessionCreate;
    }

    public class RequestResponse2
    {
        public bool success;
        public string error = "Uh oh! Servers are too busy right now. Please login again. If it still fails, wait a bit before retrying.";
        public string username;
        public string sessionId;
    }

    public class RequestResponse3
    {
        public bool valid;
        public string error = "Uh oh! Servers are too busy right now. Please login again. If it still fails, wait a bit before retrying.";
        public string username;
        public string gamejolt;
    }

    public void ToggleGameJoltLogin()
    {
        if (busy) return;
        LoggingWithGameJolt = !LoggingWithGameJolt;
        username.text = "";
        password.text = "";
        if (LoggingWithGameJolt)
        {
            GameJoltButton.GetComponentInChildren<TMP_Text>().text = "Exit GameJolt Login";
            username.placeholder.GetComponent<TMP_Text>().text = "GameJolt Username";
            password.placeholder.GetComponent<TMP_Text>().text = "Account Game Token";
        }
        else
        {
            GameJoltButton.GetComponentInChildren<TMP_Text>().text = "Login with GameJolt";
            username.placeholder.GetComponent<TMP_Text>().text = "Username";
            password.placeholder.GetComponent<TMP_Text>().text = "Password";
        }
    }

    private IEnumerator logonProcess(string user, string passOrToken)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(LoggingWithGameJolt ? InputManager.siteRequestUrl + "GameJolt/Authentication/Login?username=" + user + "&token=" + Uri.EscapeDataString(passOrToken) : InputManager.siteRequestUrl + "loginAccount?username=" + user + "&hash=" + md5hash(passOrToken));
        console.log("Verifying details...");
        ShowLoading();
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.DataProcessingError)
        {
            ShowFailure("Could not connect to website. (" + webRequest.error + ")");
            console.error("Server could not connect: (" + webRequest.error + ")");
            busy = false;
            yield break;
        }
        bool success;
        RequestResponse data;
        try
        {
            data = JsonUtility.FromJson<RequestResponse>(webRequest.downloadHandler.text);
            if (data == null) throw new Exception("Non-type JSON response received: " + webRequest.downloadHandler.text.Substring(0, 30));
        }
        catch (System.Exception ex)
        {
            ShowFailure(ex.Message);
            busy = false;
            InputManager.DisplayException(ex, 20f);
            Debug.Log(webRequest.downloadHandler.text);
            throw;
        }

        if (data.success == false)
        {
            if ((data.error ?? data.message).Contains("too busy", StringComparison.InvariantCultureIgnoreCase))
            {
                ShowFailureRetry((data.error ?? data.message) + " (Status Code: " + webRequest.responseCode + ")", () => {
                    Logon();
                });
            }
            else if (!LoggingWithGameJolt)
            {
                ShowFailure(data.error + " (Status Code: " + webRequest.responseCode + ")");
            }
            else
            {
                ShowFailure((data.error ?? data.message) + " (Status Code: " + webRequest.responseCode + ")");
            }
            console.error("Server responded an error: (Server responded a code of " + webRequest.responseCode + ") Error: " + data.error);
            success = false;
        }
        else if (data.success)
        {
            success = true;
        }
        else
        {
            success = false;
        }
        if (success)
        {
            StartCoroutine(createSession(data.toSessionCreate));
            yield break;
            // Settings.SetString("Account", data.username);
            // Settings.SetString("AccountPass", pass);
            // sceneLoad.SetActive(true);
            // yield return new WaitForSecondsRealtime(1f);
            // UnityEngine.SceneManagement.SceneManager.LoadScene("Warning");
        }
        busy = false;
        yield break;
    }

    public class SessionRequest
    {
        public string credentials;
        public string sIF;
    }

    private IEnumerator createSession(string toSessionCreate)
    {
        SessionRequest wwwForm = new SessionRequest();
        console.log("Creating session...");
        GetComponent<CanvasGroup>().interactable = false;
        wwwForm.credentials = toSessionCreate;
#if !UNITY_WEBGL
        wwwForm.sIF = "BBTUD Application";
#else
        wwwForm.sIF = "BBTUD Web App";
#endif
        string strJson = JsonUtility.ToJson(wwwForm);
        // #if UNITY_WEBGL
        //         webRequest2 = UnityWebRequest.Get(InputManager.siteRequestUrl + "WebGLCS?sIF=BBTUD Web App&credentials=" + toSessionCreate);
        // #else
        using (UnityWebRequest webRequest2 = new UnityWebRequest(InputManager.siteRequestUrl + "createSession", UnityWebRequest.kHttpVerbPOST))
        {
            webRequest2.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(strJson));
            webRequest2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest2.SetRequestHeader("Content-Type", "application/json");
            webRequest2.SetRequestHeader("Accept", "application/json");
            //#endif
            yield return webRequest2.SendWebRequest();
            bool success;
            if (webRequest2.responseCode == 404)
            {
                ShowFailure("An error occurred while creating session. Contact SWA about this issue. (Status Code: " + webRequest2.responseCode + ")");
                InputManager.DisplayException(new Exception("POST Request cannot be performed since the POST page doesn't exist."), 5f);
                console.error("Session Server responded an error: (Server responded a code of " + webRequest2.responseCode + ") Error: " + webRequest2.error);
                GetComponent<CanvasGroup>().interactable = true;
                yield break;
            }
            RequestResponse2 data;
            try
            {
                data = JsonUtility.FromJson<RequestResponse2>(webRequest2.downloadHandler.text);
                if (data == null) throw new Exception("Non-type JSON response received: " + webRequest2.downloadHandler.text.Substring(0, 30));
            }
            catch (System.Exception ex)
            {
                ShowFailure(ex.Message);
                busy = false;
                GetComponent<CanvasGroup>().interactable = true;
                InputManager.DisplayException(ex, 20f);
                Debug.Log(webRequest2.downloadHandler.text);
                throw;
            }
            if (webRequest2.result == UnityWebRequest.Result.ConnectionError || webRequest2.result == UnityWebRequest.Result.DataProcessingError)
            {
                ShowFailure("Could not connect to website. (" + webRequest2.error + ")");
                console.error("Server could not connect: (" + webRequest2.error + ")");
                success = false;
                GetComponent<CanvasGroup>().interactable = true;
            }
            else if (!data.success)
            {

                if (data.error.Contains("servers are too busy", StringComparison.InvariantCultureIgnoreCase))
                {
                    ShowFailureRetry(data.error, () =>
                    {
                        Logon();
                    });
                }
                else
                {
                    ShowFailure("Could not create session at this time. Please try again later. (Status Code: " + webRequest2.responseCode + ")");
                    InputManager.DisplayException(new Exception(data.error), 15f);
                    console.error("Session Server responded an error: (Server responded a code of " + webRequest2.responseCode + ") Error: " + data.error);
                    console.log(webRequest2.downloadHandler.text);
                }
                success = false;
                GetComponent<CanvasGroup>().interactable = true;
            }
            else
            {
                success = true;
            }
            if (success)
            {
                if (string.IsNullOrEmpty(data.sessionId))
                {
                    ShowFailure("The request succeeded, but the session ID wasn't received from the server. (Status Code: " + webRequest2.responseCode + ")");
                    InputManager.DisplayException(new Exception("Request succeeded, but returned no data."), 15f);
                    console.error("Session Server returned no data.");
                    yield break;
                }
                Settings.SetString("Account", data.username);
                Settings.SetString("AccountSession", data.sessionId);
                AccountManager.SetData(data.username, data.sessionId);
                sceneLoad.SetActive(true);
                yield return new WaitForSecondsRealtime(1f);
                UnityEngine.SceneManagement.SceneManager.LoadScene(LOAD_TO_SCENE);
            }
            busy = false;
            GetComponent<CanvasGroup>().interactable = true;
        }
        yield break;
    }

    private float secondsElapsed;

    public class GameJoltRequest
    {
        public bool success;
        public string message;
    }

    private IEnumerator registerProcess(string user, string token)
    {
        Dictionary<string, object> json = new Dictionary<string, object>();
        json.Add("username", user);
        json.Add("token", token);
        ShowLoading();
        var entries = json.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
        string strJson = "{" + string.Join(",", entries) + "}";
        using (UnityWebRequest webRequest2 = new UnityWebRequest(InputManager.siteRequestUrl + "GameJolt/Authentication/Register", UnityWebRequest.kHttpVerbPOST))
        {
            webRequest2.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(strJson));
            webRequest2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest2.SetRequestHeader("Content-Type", "application/json");
            webRequest2.SetRequestHeader("Accept", "application/json");
            //#endif
            yield return webRequest2.SendWebRequest();
            bool success;
            if (webRequest2.responseCode == 404)
            {
                ShowFailure("An error occurred while creating session. Contact SWA about this issue. (Status Code: " + webRequest2.responseCode + ")");
                InputManager.DisplayException(new Exception("POST Request cannot be performed since the POST page doesn't exist."), 5f);
                console.error("Session Server responded an error: (Server responded a code of " + webRequest2.responseCode + ") Error: " + webRequest2.error);
                yield break;
            }
            GameJoltRequest data;
            try
            {
                data = JsonUtility.FromJson<GameJoltRequest>(webRequest2.downloadHandler.text);
                if (data == null) throw new Exception("Non-type JSON response received: " + webRequest2.downloadHandler.text.Substring(0, 30));
            }
            catch (System.Exception ex)
            {
                ShowFailure(ex.Message);
                busy = false;
                InputManager.DisplayException(ex, 20f);
                Debug.Log(webRequest2.downloadHandler.text);
                throw;
            }
            if (webRequest2.result == UnityWebRequest.Result.ConnectionError || webRequest2.result == UnityWebRequest.Result.DataProcessingError)
            {
                ShowFailure("Could not connect to website. (" + webRequest2.error + ")");
                console.error("Server could not connect: (" + webRequest2.error + ")");
                success = false;
            }
            else if (!data.success)
            {
                ShowFailure(data.message + " (Status Code: " + webRequest2.responseCode + ")");
                console.error("Server responded an error: (Server responded a code of " + webRequest2.responseCode + ") Error: " + data.message);
                success = false;
            }
            else
            {
                success = true;
            }
            if (success)
            {
                load.SetActive(false);
                InputManager.DisplayEvent("Your account was successfully created. GO BACK to the login screen and LOGIN!", 10f);
            }
            busy = false;
        }
        yield break;
    }
}
