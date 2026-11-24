using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;
using System.Text.RegularExpressions;
using GameJolt.BBTUD;

public class AccountManager : MonoBehaviour
{
    private static AccountManager instance;

    private static Texture userProfile;

    private static string userName;

    private static string sessionId;

    public static int coins = -1;

    private class SessionResponse
    {
        public bool valid;
        public string error = "Servers are too busy!";
        public string username;
        public string gamejolt;
    }

    private class ProfileResponse
    {
        public bool success;
        public string error;
        public string profileUrl;
        public string tag;
    }

    private class DeleteResponse
    {
        public bool success;
        public string error;
        public string message;
        public string username;
    }

    // private static string getUserId() // TODO: Go to wix editor and make a user id variable
    // {
    //     return userId;
    // }

    public static bool hasInstance()
    {
        return instance != null;
    }

    public static string getUsername()
    {
        if (!instance) return "";
        return userName;
    }

    public static Texture getProfilePicture()
    {
        return userProfile;
    }

    public static string getSessionId()
    {
        return sessionId;
    }

    public static void GetLatestProfile() => WaitGetProfile();

    public static Coroutine WaitGetProfile()
    {
        if (!instance) throw new MissingReferenceException("Instance has not been created.");
        return instance.StartCoroutine(instance.getProfilePicture(userName));
    }

    public static void SetData(string username, string sessionId)
    {
        if (!instance) throw new MissingReferenceException("Instance has not been created.");
        userName = username;
        AccountManager.sessionId = sessionId;
        instance.StartCoroutine(instance.validateSession(sessionId));
        Log("Data has been received. Validating session...");
    }

    public static void SetData_LoadingManager(string username, string sessionId)
    {
        Log("Session was validated by the Loading Manager.");
        AccountManager.sessionId = sessionId;
        userName = username;
        instance.IsReattempt = false;
    }

    public static void ClearData()
    {
        if (!instance) throw new MissingReferenceException("Instance has not been created.");
        userName = "";
        sessionId = "";
        UserTag = "";
        userProfile = null;
        coins = -1;
        Settings.DeleteKey("Account");
        Settings.DeleteKey("AccountSession");

        if (FindObjectOfType<GameJoltManager>()) FindObjectOfType<GameJoltManager>().DestroyManager();
    }

    public static void LogoutSession()
    {
        if (!instance) throw new MissingReferenceException("Instance has not been created.");
        Log("Deleting session...");
        instance.StartCoroutine(instance.deleteSession(sessionId));
    }

    private bool IsReattempt;

    private IEnumerator validateSession(string sessionId)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(InputManager.siteRequestUrl + "validateSession?id=" + sessionId + "&gjinfo=1");
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    LogException(new System.Exception("Unable to connect to " + webRequest.url + " | " + webRequest.error));
                    ClearData();
                    IsReattempt = false;
                    yield break;
                case UnityWebRequest.Result.DataProcessingError:
                    LogException(new System.Exception("Receiving data from " + webRequest.url + " encountered an error | " + webRequest.error));
                    ClearData();
                    IsReattempt = false;
                    yield break;
            }
        }
        SessionResponse data;
        try
        {
            data = JsonUtility.FromJson<SessionResponse>(webRequest.downloadHandler.text);
        }
        catch (System.Exception ex)
        {
            data = new SessionResponse()
            {
                error = ex.Message
            };
        }
        if (data.valid)
        {
            Log("Session was validated.");
            userName = data.username;
            if (userProfile == null)
            {
                Log("Getting profile picture of " + userName + "...");
                StartCoroutine(getProfilePicture(userName));
            }
            if (!string.IsNullOrEmpty(data.gamejolt)) GameJoltManager.Create(data.gamejolt);
            IsReattempt = false;
        }
        else if ((data.error == "Servers are too busy!" || string.IsNullOrEmpty(data.error)) && !IsReattempt)
        {
            LogWarn("Uh oh! Looks like servers are too busy right now. Re-trying one more time!");
            IsReattempt = true;
            StartCoroutine(validateSession(sessionId));
        }
        else
        {
            LogException(new System.Exception("Session could not be validated: " + data.error));
            ClearData();
            IsReattempt = false;
        }
        yield break;
    }

    private bool IsLeaderboardReattempt;

    private string TransformData(Dictionary<string, object> Data)
    {
        string Result = "{";
        foreach (KeyValuePair<string, object> Pair in Data)
        {
            Result += $"\"{Pair.Key}\":";
            Type type = Pair.Value.GetType();
            if (type == typeof(int) || type == typeof(float))
            {
                Result += Pair.Value.ToString().Replace(',', '.'); // This was recently fixed, which is not on the recent patch.
            }
            else if (type == typeof(bool))
            {
                if ((bool)Pair.Value)
                {
                    Result += "true";
                }
                else
                {
                    Result += "false";
                }
            }
            else
            {
                Result += "\"" + Regex.Escape(Pair.Value.ToString()) + "\"";
            }
            Result += ",";
        }
        return Result.Substring(0, Result.Length - 1) + "}";
    }

    private bool IsSubmittingLeaderboard = false;

    private IEnumerator submitLeaderboard(string table, Dictionary<string, object> Data, EventMessage msg = null)
    {
        if (!Settings.GetBool("AllowLeaderboardStats", true))
        {
            //#if !UNITY_EDITOR
            Debug.LogWarning("Leaderboard stats are ignored. If you are supposed to submit your leaderboard stats, take a screenshot of this:\nUSER=" + userName + "\nTABLE=" + table + "\nDATA=" + TransformData(Data));
            //#endif
            yield break;
        }
        if (IsSubmittingLeaderboard) yield break;
        IsSubmittingLeaderboard = true;
        if (msg == null)
        {
            msg = InputManager.DisplayEvent("Submitting your scores online...", Color.grey, float.PositiveInfinity);
        }
        if (!Data.ContainsKey("Platform"))
        {
            Data.Add("GameJolt", GameJoltManager.Instance != null);
            Data.Add("Platform", Platforms.CurrentDevice.currentPlatform.ToString());
        }
        string strJson = TransformData(Data);
        using (UnityWebRequest webRequest = new UnityWebRequest(InputManager.siteRequestUrl + "GameJolt/Scoreboard/Add?sessionId=" + sessionId + "&table=" + table, UnityWebRequest.kHttpVerbPOST))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(strJson));
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "application/json");
            yield return webRequest.SendWebRequest();
            IsSubmittingLeaderboard = false;
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        LogError("Unable to connect to " + webRequest.url + " | " + webRequest.error);
                        IsLeaderboardReattempt = false;
                        msg.Disappear();
                        InputManager.DisplayEvent("We are unable to connect (check your internet connection)", Color.red, 10f, (msg2) =>
                        {
                            msg2.Disappear();
                            StartCoroutine(submitLeaderboard(table, Data));
                        }, "Submit");
                        yield break;
                    case UnityWebRequest.Result.DataProcessingError:
                        LogError("Receiving data from " + webRequest.url + " encountered an error | " + webRequest.error);
                        IsLeaderboardReattempt = false;
                        msg.Disappear();
                        InputManager.DisplayEvent(webRequest.error, Color.red, 10f, (msg2) =>
                        {
                            msg2.Disappear();
                            StartCoroutine(submitLeaderboard(table, Data));
                        }, "Retry");
                        yield break;
                }
            }
            JSONNode data;
            try
            {
                data = JSON.Parse(webRequest.downloadHandler.text);
            }
            catch
            {
                data = JSON.Parse("{\"success\":\"false\",\"message\":\"JSON couldn't be parsed: " + Regex.Escape(webRequest.downloadHandler.text) + "\"}");
            }
            if (data != null && !data.IsNull)
            {
                if (data["success"] != null && !data["success"].IsNull && data["success"].Value == "true")
                {
                    IsLeaderboardReattempt = false;
                    Log("Successfully submitted!");
                    msg.Disappear();
                }
                else if (data["message"] == null)
                {
                    LogError("Leaderboard couldn't be uploaded: " + webRequest.downloadHandler.text);
                    Debug.LogError("Screenshot this and send to SWA if you want these scores to appear:\nUSER=" + userName + "\nTABLE=" + table + "\nDATA=" + TransformData(Data));
                    IsLeaderboardReattempt = false;
                    msg.Disappear();
                    InputManager.DisplayEvent("We couldn't get the error message, check your logs and send the screenshot to SWA", Color.red, 10f, (msg2) =>
                    {
                        msg2.Disappear();
                        StartCoroutine(submitLeaderboard(table, Data));
                    }, "Retry");
                }
                else if ((data["message"].Value == "Servers are too busy!" || string.IsNullOrEmpty(data["message"].Value)) && !IsLeaderboardReattempt)
                {
                    LogWarn("Uh oh! Looks like servers are too busy right now. Re-trying one more time!");
                    IsLeaderboardReattempt = true;
                    StartCoroutine(submitLeaderboard(table, Data, msg));
                    msg.SetText("This is taking longer than usual, re-attempting to submit");
                }
                else
                {
                    LogError("Leaderboard couldn't be uploaded: " + data["message"].Value);
                    Debug.LogError("Screenshot this and send to SWA if you want these scores to appear:\nUSER=" + userName + "\nTABLE=" + table + "\nDATA=" + TransformData(Data));
                    IsLeaderboardReattempt = false;
                    msg.Disappear();
                    if (data["message"].Value == "Servers are too busy!")
                    {
                        InputManager.DisplayEvent("We're sorry but we couldn't submit your scores", Color.red, 10f, (msg2) =>
                        {
                            msg2.Disappear();
                            StartCoroutine(submitLeaderboard(table, Data));
                        }, "Retry");
                    }
                    else
                    {
                        InputManager.DisplayEvent("We're sorry but we couldn't submit your scores", Color.red, 10f);
                    }
                }
            }
            else
            {
                LogError("Leaderboard couldn't be uploaded: " + webRequest.downloadHandler.text);
                Debug.LogError("Screenshot this and send to SWA if you want these scores to appear:\nUSER=" + userName + "\nTABLE=" + table + "\nDATA=" + TransformData(Data));
                IsLeaderboardReattempt = false;
                msg.Disappear();
                InputManager.DisplayEvent("An error has occurred so we couldn't submit your scores", Color.red, 10f, (msg2) =>
                {
                    msg2.Disappear();
                    StartCoroutine(submitLeaderboard(table, Data));
                }, "Retry");
            }
        }
        yield break;
    }

    public static void GetProfilePicture(string name)
    {
        instance.StartCoroutine(instance.getProfilePicture(name));
    }

    public static void SubmitEndless(string level, int Notebooks, float time, bool o, bool d)
    {
        if (instance.IsSubmittingLeaderboard) return;
        instance.StartCoroutine(instance.submitLeaderboard("endless", new Dictionary<string, object>() {
            {"scene", level},
            {"notebooks", Notebooks},
            {"time", time},
            {"math", o},
            {"ci", d}
        }));
    }

    public static void SubmitCampingScore(int Score, float time)
    {
        if (instance.IsSubmittingLeaderboard) return;
        instance.StartCoroutine(instance.submitLeaderboard("camping", new Dictionary<string, object>() {
            {"score", Score},
            {"time_remaining", time}
        }));
    }

    public static void SubmitFarmScore(float time, int MazeID)
    {
        if (instance.IsSubmittingLeaderboard) return;
        instance.StartCoroutine(instance.submitLeaderboard("maze", new Dictionary<string, object>() {
            {"time", time},
            {"maze", MazeID}
        }));
    }

    public static void SubmitFarmScore(int Score, float time)
    {
        if (instance.IsSubmittingLeaderboard) return;
        instance.StartCoroutine(instance.submitLeaderboard("barn", new Dictionary<string, object>() {
            {"score", Score},
            {"time", time}
        }));
    }

    [System.Serializable]
    private class SyncCoin
    {
        public int coins;
        public string sessionId;
    }

    public static void SetCoins(int coins, System.Action OnCompleted = null)
    {
        if (!instance)
        {
            Debug.LogError("Restart the game to sync coins.");
            return;
        }
        instance.StartCoroutine(instance.SyncCoins(coins, OnCompleted));
    }

    private IEnumerator SyncCoins(int coins, System.Action OnCompleted)
    {
        SyncCoin coinJson = new SyncCoin
        {
            coins = coins,
            sessionId = Settings.GetString("AccountSession")
        };
        string json = JsonUtility.ToJson(coinJson);
        using (UnityWebRequest webRequest = UnityWebRequest.Put(InputManager.siteRequestUrl + "coins/set", json))
        {
            webRequest.timeout = 15;
            webRequest.SetRequestHeader("Content-Type", "text/plain");
            yield return webRequest.SendWebRequest();
            if (OnCompleted != null) OnCompleted();
        }
        yield break;
    }

    public static Texture LatestGotProfilePicture { get; private set; }
    public static string UserTag { get; private set; } = "";
    public static string LatestUserTag { get; private set; } = "";

    public static bool RetrievingProfilePicture;

    private IEnumerator getProfilePicture(string userName)
    {
        bool IsUserNull = string.IsNullOrEmpty(userName);
        if (IsUserNull) userName = "DefaultMP";
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            RetrievingProfilePicture = false;
            yield break;
        }
        UnityWebRequest webRequest = UnityWebRequest.Get(InputManager.siteRequestUrl + "profilePicture?username=" + userName + "&tag=1");
        RetrievingProfilePicture = true;
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    LogException(new System.Exception("Unable to connect to " + webRequest.url + " | " + webRequest.error));
                    RetrievingProfilePicture = false;
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    LogException(new System.Exception("Receiving data from " + webRequest.url + " encountered an error | " + webRequest.error));
                    RetrievingProfilePicture = false;
                    break;
            }
        }
        ProfileResponse data;
        try
        {
            data = JsonUtility.FromJson<ProfileResponse>(webRequest.downloadHandler.text);
        }
        catch (System.Exception ex)
        {
            data = new ProfileResponse
            {
                error = ex.Message
            };
        }
        if (data.success)
        {
            if (!string.IsNullOrEmpty(sessionId) && userName != getUsername()) StartCoroutine(getCoins());
            UnityWebRequest request2 = UnityWebRequestTexture.GetTexture(data.profileUrl);
            yield return request2.SendWebRequest();
            switch (request2.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    LogException(new System.Exception("Unable to connect to " + request2.url + " | " + request2.error));
                    RetrievingProfilePicture = false;
                    yield break;
                case UnityWebRequest.Result.DataProcessingError:
                    LogException(new System.Exception("The recipient's profile picture is .webp, or a file that's not supported!"));
                    RetrievingProfilePicture = false;
                    yield break;
                case UnityWebRequest.Result.ProtocolError:
                    LogException(new System.Exception("Error while sending request to " + request2.url + " | " + request2.error));
                    RetrievingProfilePicture = false;
                    yield break;
            }
            if (request2.responseCode != 200)
            {
                LogException(new System.Exception("Unable to receive profile picture of " + userName + " (Error Code " + webRequest.responseCode + "): " + data.error));
                RetrievingProfilePicture = false;
                yield break;
            }
            if (IsUserNull || (!string.IsNullOrEmpty(AccountManager.userName) && AccountManager.userName.ToLower() == userName.ToLower()))
            {
                userProfile = ((DownloadHandlerTexture)request2.downloadHandler).texture;
                UserTag = data.tag;
                RetrievingProfilePicture = false;
            }
            else
            {
                LatestGotProfilePicture = ((DownloadHandlerTexture)request2.downloadHandler).texture;
                LatestUserTag = data.tag;
                RetrievingProfilePicture = false;
            }
            Log("Profile texture was received.");
        }
        else
        {
            RetrievingProfilePicture = false;
            if (string.IsNullOrEmpty(data.error))
            {
                bool Me = IsUserNull || (!string.IsNullOrEmpty(AccountManager.userName) && AccountManager.userName.ToLower() == userName.ToLower());
                if (Me)
                {
                    if (userProfile == null)
                    {
                        LogException(new System.Exception("We can't receive your profile picture right now. We'll try again when you come to this screen again."));
                    }
                    else
                    {
                        LogError("Unable to update the user's profile picture at this time.");
                    }
                }
                else
                {
                    LogException(new System.Exception(userName + "'s profile picture will remain blank at this time."));
                }
            }
            else
            {
                LogException(new System.Exception("Unable to receive profile picture of " + userName + " (Error Code " + webRequest.responseCode + "): " + data.error));
            }
        }
        yield break;
    }

    private IEnumerator getCoins()
    {
        Log("Getting online coins...");
        string sessionId = Settings.GetString("AccountSession");
        UnityWebRequest webRequest = UnityWebRequest.Get(InputManager.siteRequestUrl + "coins/get?id=" + sessionId);
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    LogException(new System.Exception("Unable to connect to " + webRequest.url + " | " + webRequest.error));
                    yield break;
                case UnityWebRequest.Result.DataProcessingError:
                    LogException(new System.Exception("Receiving data from " + webRequest.url + " encountered an error | " + webRequest.error));
                    yield break;
            }
        }
        if (webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            LogException(new System.Exception(webRequest.error + ": " + webRequest.downloadHandler.text));
            yield break;
        }
        int toInt;
        bool isNum = int.TryParse(webRequest.downloadHandler.text, out toInt);
        if (!isNum)
        {
            LogException(new System.Exception(webRequest.downloadHandler.text));
            yield break;
        }
        coins = toInt;
        if (coins == 500)
        {
            Log("The coin is set to 500, which usually no coins have been set. It will not set the coins on the device.");
            yield break;
        }
        Log("Setting local device coins to " + coins);
        CoinSaveSystem.Save(coins, false);
        yield break;
    }

    private IEnumerator deleteSession(string sessionId)
    {
        yield return new WaitWhile(() => AccountManager.RetrievingProfilePicture);
        EventMessage evnet = InputManager.DisplayEvent("Logging out from " + userName + "...", float.PositiveInfinity);
        UnityWebRequest webRequest = UnityWebRequest.Delete(InputManager.siteRequestUrl + "deleteSession/" + sessionId);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();
        ClearData();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    evnet.Disappear();
                    LogException(new System.Exception("Unable to connect to " + webRequest.url + " | " + webRequest.error));
                    yield break;
                case UnityWebRequest.Result.DataProcessingError:
                    evnet.Disappear();
                    LogException(new System.Exception("Receiving data from " + webRequest.url + " encountered an error | " + webRequest.error));
                    yield break;
            }
        }
        evnet.Disappear();
        DeleteResponse data = JsonUtility.FromJson<DeleteResponse>(webRequest.downloadHandler.text);
        if (data.success)
        {
            Log("Session has been deleted. User " + data.username + " has logged out.");
        }
        else
        {
            LogException(new System.Exception("Session could not be deleted: " + data.error));
        }
        yield break;
    }

    [HideInCallstack]
    public static void Log(string str)
    {
        console.log("[Account Manager] " + str);
    }

    [HideInCallstack]
    public static void LogWarn(string str)
    {
        console.warn("[Account Manager] " + str);
    }

    [HideInCallstack]
    public static void LogError(string str)
    {
        console.error("[Account Manager] " + str);
    }

    [HideInCallstack]
    public static void LogException(System.Exception exception)
    {
        InputManager.DisplayException(exception, 5f);
        Debug.LogException(exception);
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Log("Initialized.");
    }
}