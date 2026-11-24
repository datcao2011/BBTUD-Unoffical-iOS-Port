using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;
using System.Collections.Generic;
using SimpleJSON;

namespace GameJolt.BBTUD
{
    public class GameJoltManager : MonoBehaviour
    {
        public static GameJoltManager Instance { get; private set; }

        public string Username;

        public string GameToken;

        private static readonly bool AllowSessions = true;
        private bool Session;

        private bool FetchingSessions;
        private float NextSessionPing = 30f;

        public static void Create(string info)
        {
            if (Instance != null)
            {
                return;
            }
            GameJoltManager gjManager = new GameObject("GameJolt Manager").AddComponent<GameJoltManager>();

            string[] tags = info.Split("*:*");
            gjManager.Username = tags[0];
            gjManager.GameToken = tags[1];

            Debug.Log("[GameJolt Manager] User was logged in with GameJolt linked. Manager is now active.");
            if (AllowSessions)
            {
                Instance.StartSession();
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject, 0f);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (Session && !FetchingSessions)
            {
                if (NextSessionPing > 0f)
                {
                    NextSessionPing -= Time.unscaledDeltaTime;
                }
                else
                {
                    Ping();
                }
            }
        }

        private void StartSession()
        {
            FetchingSessions = true;
            Dictionary<string, object> Details = new Dictionary<string, object>
            {
                { "username", Username },
                { "token", GameToken }
            };
            StartCoroutine(SendRequest("GameJolt/FreeSessionAPI/Open", Details, json =>
            {
                FetchingSessions = false;
                Session = false;
                if (json != null)
                {
                    if (json["success"].Value == "true")
                    {
                        NextSessionPing = 1f;
                        Session = true;
#if DEBUG
                        Debug.Log("[GameJolt Manager] Session started");
#endif
                    }
                    else
                    {
#if DEBUG
                        Debug.LogError(json["message"]);
#endif
                    }
                }
            }));
        }

        private string CurrentGameJoltStatus = "active";

        private void Ping()
        {
            FetchingSessions = true;
            CurrentGameJoltStatus = Application.isFocused ? "active" : "idle";
            Dictionary<string, object> Details = new Dictionary<string, object>
            {
                { "username", Username },
                { "token", GameToken },
                { "status", CurrentGameJoltStatus }
            };
#if DEBUG
            Debug.Log("[GameJolt Manager] Pinging with status " + CurrentGameJoltStatus + " at " + System.DateTime.Now.ToString());
#endif
            StartCoroutine(SendRequest("GameJolt/FreeSessionAPI/Ping", Details, json =>
            {
                FetchingSessions = false;
                Session = false;
                if (json != null)
                {
                    if (json["success"].Value == "true")
                    {
                        NextSessionPing = 30f;
                        Session = true;
#if DEBUG
                        Debug.Log("[GameJolt Manager] PING!");
#endif
                    }
                    else
                    {
                        StartSession();
#if DEBUG
                        Debug.LogError(json["message"]);
#endif
                    }
                }
            }));
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (Session && CurrentGameJoltStatus == "idle")
            {
                NextSessionPing = 0f;
            }
        }

        public static void Achieve(string trophy_id)
        {
            if (!Instance)
            {
                Debug.LogError("[GameJolt Manager] Can't Achieve(achievementId) because instance hasn't been created.");
                return;
            }
#if DEBUG
            Debug.Log("[GameJolt Manager] Achieving trophy...");
#endif
            Dictionary<string, object> Details = new Dictionary<string, object>
            {
                { "achievementId", trophy_id },
                { "sessionId", AccountManager.getSessionId() }
            };
            Instance.StartCoroutine(Instance.SendRequest("GameJolt/Achievement", Details));
        }

        public static void Achievements(System.Action<JSONNode> callback)
        {
            if (!Instance)
            {
                Debug.LogError("[GameJolt Manager] Can't Achievements(callback) because instance hasn't been created.");
                return;
            }
#if DEBUG
            Debug.Log("[GameJolt Manager] Receiving achievements...");
#endif
            Instance.StartCoroutine(Instance.SendRequest("GameJolt/Achievements?sessionId=" + AccountManager.getSessionId(), callback));
        }

        private IEnumerator SendRequest(string url, System.Action<JSONNode> json)
        {
            UnityWebRequest req = UnityWebRequest.Get(InputManager.siteRequestUrl + url);
            yield return req.SendWebRequest();
            if (string.IsNullOrEmpty(req.downloadHandler.text) || !req.GetResponseHeader("Content-Type").Contains("application/json")) { json(null); yield break; }
            json(JSON.Parse(req.downloadHandler.text));
            yield break;
        }

        private int Attempts = 0;

        private IEnumerator SendRequest(string url, Dictionary<string, object> json)
        {
            var entries = json.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
            string strJson = "{" + string.Join(",", entries) + "}";
            using (UnityWebRequest req = new UnityWebRequest(InputManager.siteRequestUrl + url, UnityWebRequest.kHttpVerbPOST))
            {
                req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(strJson));
                req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                req.SetRequestHeader("Accept", "application/json");
                //#endif
                yield return req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.ProtocolError && req.responseCode == 500 && Attempts <= 4)
                {
                    Attempts++;
                    Debug.LogWarning("Unable to achieve trophy due to server being busy - Retrying (" + Attempts + " out of 5)");
                    StartCoroutine(SendRequest(url, json));
                    yield break;
                }
            }
            Attempts = 0;
            yield break;
        }

        private IEnumerator SendRequest(string url, Dictionary<string, object> json, System.Action<JSONNode> response)
        {
            var entries = json.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
            string strJson = "{" + string.Join(",", entries) + "}";
            using (UnityWebRequest req = new UnityWebRequest(InputManager.siteRequestUrl + url, UnityWebRequest.kHttpVerbPOST))
            {
                req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(strJson));
                req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                req.SetRequestHeader("Accept", "application/json");
                //#endif
                yield return req.SendWebRequest();
                if (string.IsNullOrEmpty(req.downloadHandler.text) || !req.GetResponseHeader("Content-Type").Contains("application/json")) { response(null); yield break; }
                response(JSON.Parse(req.downloadHandler.text));
            }
            yield break;
        }

        public void DestroyManager()
        {
            Destroy(gameObject, 0f);
        }
    }
}