using UnityEngine;
using TMPro;
using System;
using Photon.Pun;
using Multiplayer;

public class SystemTimeScript : MonoBehaviour
{

    // public TMP_Text timeHourText;

    // public TMP_Text timeMinuteText;

    // public TMP_Text timeSecondsText;

    // public TMP_Text amorpmText;

    // public TMP_Text uselessText1;

    // public TMP_Text uselessText2;

    public TMP_Text timeText;

    public bool threeDarkness;

    public GameControllerScript gc;

    private bool doIt;

    public bool time;

    private string achievement;

    private bool jumpscare;

    public float survival;

    public AudioSource creepySound1;

    public AudioSource creepySound2;

    public AudioSource creepySound3;

    public AudioSource creepySound4;

    public AudioSource creepySound5;

    public AudioSource creepySound6;

    public AudioSource creepySound7;

    public GameObject redStatic;

    public BaldiScript baldiScript;

    private bool TimeShown = true;

    private void Start()
    {
        TimeShown = Settings.GetString("TimeHud", "true") == "true";
        Settings.OnSettingChanged += OnSettingChanged;
        if (!TimeShown)
        {
            timeText.text = "";
        }
    }

    private void OnSettingChanged(string name, object val)
    {
        if (name == "TimeHud")
        {
            TimeShown = (string)val == "true";
            if (!TimeShown) timeText.text = "";
        }
    }

    private void OnDestroy()
    {
        Settings.OnSettingChanged -= OnSettingChanged;
    }

    private void Update()
    {
        if (!TimeShown)
        {
            if (Settings.GetBool("Debug"))
            {
                timeText.text = "<size=7><color=grey>" + InputManager.Stats + "</color></size>";
            }
            return;
        }
        int sysHour = DateTime.Now.Hour;
        string timeStr = DateTime.Now.ToLongTimeString();
        if (Settings.GetBool("Debug"))
        {
            timeStr += "\n<size=7><color=grey>" + InputManager.Stats + "</color></size>";
        }
        if (time)
        {
            survival += Time.deltaTime;
            baldiScript.gameController.debugMode = true;
            if (survival >= 30f & !jumpscare)
            {
                if (!creepySound2.isPlaying)
                {
                    creepySound2.Play();

                }
            }
            if (survival >= 60f & !jumpscare)
            {
                if (!creepySound3.isPlaying)
                {
                    creepySound3.Play();
                }
            }
            if (survival >= 90f)
            {
                if (!creepySound4.isPlaying)
                {
                    creepySound4.Play();
                }
            }
            if (survival >= 120f & !jumpscare)
            {
                if (!creepySound5.isPlaying)
                {
                    creepySound5.Play();
                }
            }
            if (survival >= 150f & !jumpscare)
            {
                if (!creepySound6.isPlaying)
                {
                    creepySound6.Play();
                }
            }
            if (survival >= 180f & !jumpscare)
            {
                if (!creepySound7.isPlaying)
                {
                    creepySound7.Play();
                    redStatic.SetActive(true);
                    FindObjectOfType<FogScript>().stopFog = true;
                    FindObjectOfType<FogControllerScript>().FogDensity(0.09f);
                    FindObjectOfType<FogControllerScript>().ChangeFogColor(Color.red);
                }
            }
            if (survival >= 210f)
            {
                if (!jumpscare)
                {
                    jumpscare = true;
                    creepySound1.Stop();
                    creepySound2.Stop();
                    creepySound3.Stop();
                    creepySound4.Stop();
                    creepySound5.Stop();
                    creepySound6.Stop();
                    creepySound7.Stop();
                    redStatic.SetActive(false);
                    baldiScript.SpoopyJumpscare();
                }
            }
        }
        // if (sysHour == 13)
        // {
        //     timeHourText.text = "1";
        // }
        // else if (sysHour == 14)
        // {
        //     timeHourText.text = "2";
        // }
        // else if (sysHour == 15)
        // {
        //     timeHourText.text = "3";
        // }
        // else if (sysHour == 16)
        // {
        //     timeHourText.text = "4";
        // }
        // else if (sysHour == 17)
        // {
        //     timeHourText.text = "5";
        // }
        // else if (sysHour == 18)
        // {
        //     timeHourText.text = "6";
        // }
        // else if (sysHour == 19)
        // {
        //     timeHourText.text = "7";
        // }
        // else if (sysHour == 20)
        // {
        //     timeHourText.text = "8";
        // }
        // else if (sysHour == 21)
        // {
        //     timeHourText.text = "9";
        // }
        // else if (sysHour == 22)
        // {
        //     timeHourText.text = "10";
        // }
        // else if (sysHour == 23)
        // {
        //     timeHourText.text = "11";
        // }
        // else if (sysHour == 0)
        // {
        //     timeHourText.text = "12";
        // }
        // else
        // {
        //     timeHourText.text = sysHour.ToString();
        // }
        // if (sysMinute < 10)
        // {
        //     timeMinuteText.text = "0" + sysMinute.ToString();
        // }
        // else
        // {
        //     timeMinuteText.text = sysMinute.ToString();
        // }
        // if (sysSeconds < 10)
        // {
        //     timeSecondsText.text = "0" + sysSeconds.ToString();
        // }
        // else
        // {
        //     timeSecondsText.text = sysSeconds.ToString();
        // }
        // if (sysHour < 12)
        // {
        //     amorpmText.text = "AM";
        // }
        // else
        // {
        //     amorpmText.text = "PM";
        // }
        timeText.text = timeStr;
        if (sysHour == 3)
        {
            RedText();
            threeDarkness = true;
        }
        else
        {
            threeDarkness = false;
            if (gc == null)
            {
                BlackText();
            }
            else
            {
                if (gc.exitModeCheatActive || gc.scene == "CampScene")
                {
                    WhiteText();
                }
                else
                {
                    BlackText();
                }
            }
        }
        if (gc == null)
        {
            return;
        }
        if (sysHour == 3 & gc.scene == InputManager.IsSchoolScene())
        {
            gc.schoolMusic.Stop();
            gc.learnMusic.Stop();
            if (!doIt)
            {
                doIt = true;
                threeAM();
            }
        }
    }

    public void WhiteText()
    {
        // timeHourText.color = Color.white;
        // timeMinuteText.color = Color.white;
        // timeSecondsText.color = Color.white;
        // amorpmText.color = Color.white;
        // uselessText1.color = Color.white;
        // uselessText2.color = Color.white;
        timeText.color = Color.white;
    }

    public void BlackText()
    {
        // timeHourText.color = Color.black;
        // timeMinuteText.color = Color.black;
        // timeSecondsText.color = Color.black;
        // amorpmText.color = Color.black;
        // uselessText1.color = Color.black;
        // uselessText2.color = Color.black;
        timeText.color = Color.black;
    }

    public void RedText()
    {
        // timeHourText.color = Color.red;
        // timeMinuteText.color = Color.red;
        // timeSecondsText.color = Color.red;
        // amorpmText.color = Color.red;
        // uselessText1.color = Color.red;
        // uselessText2.color = Color.red;
        timeText.color = Color.red;
    }

    public void threeAM()
    {
        creepySound1.Play();
        time = true;
        if (MultiplayerManager.isActive)
        {
            FindObjectOfType<Chat>().isOpened = true;
            FindObjectOfType<Chat>().SendMsg("was kicked for playing at 3 AM and cheating.");
            FindObjectOfType<Chat>().isOpened = false;
            Client.DisconnectAction = (ev, cause) => { };
            PhotonNetwork.Disconnect();
            FindObjectOfType<Chat>().AddMessage("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nAccess to chat by pressing the chat button.");
        }
    }
}
