using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameJolt.BBTUD;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Leaderboards : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Username;

    [SerializeField]
    private RawImage ProfilePicture;

    [SerializeField]
    private TMP_Text MyScore;

    private float NextUpdate;

    [SerializeField]
    private RectTransform RefreshIcon;

    [SerializeField]
    private ScrollRect scrollView;

    [SerializeField]
    private GameObject GB;

    [SerializeField]
    private TMP_Dropdown Dropdown;

    private List<GameObject> Scores = new List<GameObject>();

    private bool IsRefreshing = false;

    private UnityWebRequest Current;

    private class ScoreItem
    {
        public string Score;
        public string Username;
        public int Sort;
        public string Details;
        public string Stored;
    }

    [SerializeField]
    private Gradient ColorRank;

    private void OnEnable()
    {
        ProfilePicture.texture = AccountManager.getProfilePicture();
        Refresh();
    }

    public void Refresh()
    {
        if (IsRefreshing) return;
        IsRefreshing = true;
        UpdateHeadText(-1, "-");
        StartCoroutine(FetchScores());
    }

    public void RefreshAndCancelCheckingMyScore()
    {
        if (Current != null)
        {
            StopCoroutine("FetchMyScore");
            Current.Abort();
            Current.Dispose();
            Current = null;
        }
        IsRefreshing = false;
        Refresh();
    }

    private int DropValue;

    private IEnumerator FetchScores()
    {
        DropValue = Dropdown.value;
        Dropdown.interactable = false;
        UnityWebRequest request = UnityWebRequest.Get(InputManager.siteRequestUrl + "GameJolt/Scoreboard/View?table=" + GetValue(DropValue));
        IsRefreshing = true;
        yield return request.SendWebRequest();
        Dropdown.interactable = true;

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Fetched scoreboard: " + Dropdown.options[DropValue].text);
            JSONNode node = JSON.Parse(request.downloadHandler.text);
            foreach (GameObject Score in Scores)
            {
                Destroy(Score, 0f);
            }
            if (node["scores"].IsArray)
            {
                List<ScoreItem> ScoreItems = new List<ScoreItem>();
                foreach (JSONNode score in node["scores"].AsArray)
                {
                    string User = (score["guest"].Value.Length > 0) ? score["guest"].Value : score["user"].Value;
                    try
                    {
                        ScoreItems.Add(new ScoreItem()
                        {
                            Score = score["score"].Value,
                            Username = User,
                            Sort = int.Parse(score["sort"].Value),
                            Details = score["extra_data"].Value,
                            Stored = GetDate(score["stored_timestamp"].AsDouble, score["stored"].Value)
                        });
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }

                bool IsMaze = DropValue == 3;
                IOrderedEnumerable<ScoreItem> Ordered = IsMaze ? ScoreItems.OrderBy(x => x.Sort) : ScoreItems.OrderByDescending(x => x.Sort);
                int Rank = 0;
                foreach (ScoreItem item in Ordered)
                {
                    Rank++;
                    GameObject ITEM = Instantiate(GB, scrollView.content);
                    ITEM.SetActive(true);
                    TMP_Text[] Texts = ITEM.GetComponentsInChildren<TMP_Text>();
                    Texts[0].text = "#" + Rank;
                    Texts[1].text = item.Username;
                    Texts[2].text = item.Score;
                    Texts[3].text = item.Details;
                    Texts[4].text = item.Stored;
                    Scores.Add(ITEM);
                }
                StartCoroutine("FetchMyScore");
            }
            else
            {
                Debug.Log("No scores available!");
                GameObject ITEM = Instantiate(GB, scrollView.content);
                ITEM.SetActive(true);
                TMP_Text[] Texts = ITEM.GetComponentsInChildren<TMP_Text>();
                Texts[0].text = "#0";
                Texts[1].text = "";
                Texts[2].text = "<color=red>No Scores Yet!</color>";
                Texts[3].text = "";
                Texts[4].text = "Be The First!";
                Scores.Add(ITEM);
                StartCoroutine("FetchMyScore");
            }
        }
        else if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            IsRefreshing = false;
            JSONNode node;
            try
            {
                node = JSON.Parse(request.downloadHandler.text);
            }
            catch
            {
                Debug.LogError(request.downloadHandler.text);
                InputManager.DisplayEvent("We couldn't load the leaderboard: " + request.downloadHandler.text, Color.red, 5f);
                yield break;
            }
            Debug.LogError(node["message"].Value);
            InputManager.DisplayEvent("We couldn't load the leaderboard: " + node["message"].Value, Color.red, 5f);
        }
        else
        {
            Debug.LogError(request.error);
            InputManager.DisplayEvent("We couldn't load the leaderboard: " + request.error, Color.red, 5f);
            IsRefreshing = false;
        }
        yield break;
    }

    private string GetDate(double T, string OldString)
    {
        DateTime date = DailyReward.SetUnixTimeStamp(T);
        DateTime now = DateTime.UtcNow;
        if ((now - date).TotalDays <= 180)
        {
            return OldString;
        }
        return date.ToShortDateString() + " " + date.ToShortTimeString();
    }

    private IEnumerator FetchMyScore()
    {
        bool UseGameJoltAccount = GameJoltManager.Instance != null;
        Current = UnityWebRequest.Get(InputManager.siteRequestUrl + "GameJolt/Scoreboard/ViewMyScore?table=" + GetValue(DropValue) + "&type=" + (UseGameJoltAccount ? ("gamejolt&username=" + GameJoltManager.Instance.Username + "&token=" + GameJoltManager.Instance.GameToken) : ("bbtud&username=" + AccountManager.getUsername())));
        yield return Current.SendWebRequest();
        IsRefreshing = false;
        if (Current.result == UnityWebRequest.Result.Success)
        {
            JSONNode node = JSON.Parse(Current.downloadHandler.text);
            if (node["scores"].IsArray)
            {
                if (node["scores"].AsArray.Count <= 0)
                {
                    InputManager.DisplayException(new System.Exception("Your stats couldn't be loaded: [\"scores\"] array length is zero"), 5f);
                }
                else
                {
                    UpdateHeadText(node["rank"].AsInt, node["scores"].AsArray[0]["score"].Value);
                }
            }
            else
            {
                InputManager.DisplayException(new System.Exception("Your stats couldn't be loaded: [\"scores\"] is not an array"), 5f);
            }
        }
        else if (Current.result == UnityWebRequest.Result.ProtocolError)
        {
            JSONNode node;
            try
            {
                node = JSON.Parse(Current.downloadHandler.text);
            }
            catch
            {
                Debug.LogError(Current.downloadHandler.text);
                InputManager.DisplayEvent("We couldn't load your stats: " + Current.downloadHandler.text, Color.red, 5f);
                yield break;
            }
            if (node["message"].Value == "User is not in the leaderboard.")
            {
                UpdateHeadText(0, "No Score Yet!");
                yield break;
            }
            Debug.LogError(node["message"].Value);
            InputManager.DisplayEvent("We couldn't load your stats: " + node["message"].Value, Color.red, 5f);
        }
        else
        {
            Debug.LogError(Current.error);
            InputManager.DisplayException(new System.Exception("Your stats couldn't be loaded: " + Current.error), 3f);
        }
        Current.Dispose();
        Current = null;
        yield break;
    }

    private string GetValue(int Val)
    {
        switch (Val)
        {
            case 1:
                return "endless_classic";
            case 2:
                return "camping";
            case 3:
                return "maze";
            case 4:
                return "barn";
        }
        return "endless";
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsRefreshing)
        {
            NextUpdate += Time.deltaTime;
            if (NextUpdate >= 0.1f)
            {
                RefreshIcon.Rotate(new Vector3(0f, 0f, -45f));
                NextUpdate = 0f;
            }
        }
    }

    private void UpdateHeadText(int Rank, string Bottom)
    {
        string RankText;
        if (Rank == -1)
        {
            RankText = " <color=grey>#";
        }
        else if (Rank > 100 || Rank == 0)
        {
            RankText = " <color=grey>#" + Rank;
        }
        else
        {
            RankText = " <color=#" + ColorUtility.ToHtmlStringRGB(ColorRank.Evaluate(Rank / 100f)) + ">#" + Rank;
        }
        Username.text = AccountManager.getUsername() + RankText;
        MyScore.text = Bottom;
    }
}
