using UnityEngine;
using System;
using TMPro;

public class EventTimer : MonoBehaviour
{
    public TMP_Text eventTimer;

    private void Update()
    {
        // DateTime eventDate = DateTime.Parse("7/15/2020 2:00 PM");
        // TimeSpan eventTimer = eventDate.Subtract(System.DateTime.Now);
        // this.eventTimer.text = eventTimer.Days + "d " + eventTimer.Hours + "h " + eventTimer.Minutes + "m " + eventTimer.Seconds + "s";
        // if (eventTimer.TotalSeconds <= 0)
        // {
        //     this.eventTimer.text = "{\"EventStarted\" : true}";
        //     FindObjectOfType<PlayerScript>().HudOffForTheRestOfScene();
        // }
    }
}