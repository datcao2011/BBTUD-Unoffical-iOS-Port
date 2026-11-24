using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    //UI
    public TMP_Text timeLabel; //only use if your timer uses a label
    public Button timerButton; //used to disable button when needed
                               //TIME ELEMENTS

    public Image TheImage;
    private DateTime NextDateTime;
    private bool Counting;

    private void Start()
    {
        if (Settings.HasKey("_DailyTime"))
        {
            NextDateTime = SetUnixTimeStamp(double.Parse(Settings.GetString("_DailyTime")));
            if (DateTime.UtcNow < NextDateTime)
            {
                Counting = true;
                timerButton.interactable = false;
            }
            else
            {
                TheImage.fillAmount = 0f;
                timeLabel.text = "GET DAILY MYSTERY DOOR!";
            }
        }
        else
        {
            TheImage.fillAmount = 0f;
            timeLabel.text = "GET DAILY MYSTERY DOOR!";
        }
    }

    private void Update()
    {
        if (Counting)
        {
            TimeSpan ts = NextDateTime - DateTime.UtcNow;
            TheImage.fillAmount = Mathf.InverseLerp(0, 86400, (int)ts.TotalSeconds);
            timeLabel.text = $"Ready in\n{ts.Hours}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
            if (DateTime.UtcNow >= NextDateTime)
            {
                Counting = false;
                TheImage.fillAmount = 0f;
                timeLabel.text = "GET DAILY MYSTERY DOOR!";
                timerButton.interactable = true;
            }
        }
    }

    public static DateTime SetUnixTimeStamp(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        epochStart = epochStart.AddSeconds(unixTimeStamp);
        return epochStart;
    }

    public void ClaimReward()
    {
        Settings.SetString("_DailyTime", GetUnixTimeStamp(DateTime.UtcNow.AddDays(1)).ToString());
        NextDateTime = DateTime.UtcNow.AddDays(1);
        timerButton.interactable = false;
        Counting = true;
    }

    private double GetUnixTimeStamp(DateTime time)
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        double cur_time = (int)(time - epochStart).TotalSeconds;
        return cur_time;
    }
}