using UnityEngine;
using UnityEngine.Networking;
using System;
using TMPro;
using System.Collections;

public class CountDownTimer : MonoBehaviour
{
    public TMP_Text text;
    public AudioSource audioDevice;
    public AudioClip audio1;
    public AudioClip audio2;

    // private string[] e001 = new string[]
    // {
    //     "<color=red><i><u>D</u></i></color>id you know that bully can get detention?",
    //     "<color=red><i><u>A</u></i></color>n apple a day keeps a teacher away.",
    //     "<color=red><i><u>N</u></i></color>ever this school can be destroyed.",
    //     "<color=red><i><u>G</u></i></color>rab any supplies. If you don't have them, the school have already you need.",
    //     "<color=red><i><u>E</u></i></color>ven you can improve this school, come join the school now.",
    //     "<color=red><i><u>R</u></i></color>aising your grade in here impact your effort a lot!"
    // };

    private int e002; // e001 Index
#if HAS_EVENT_PLUGGED
    public static int e007;
#endif

    public Animator e006;

    private DateTime eventDate;
    private DateTime localEventDate;

    private bool dateFound;

    private void Start()
    {
        StartCoroutine(GetEventDate());
    }
    private IEnumerator GetEventDate()
    {
        // if (Application.internetReachability == NetworkReachability.NotReachable) // The computer detects there isn't internet, skips sending request process.
        // {
        //     yield break;
        // }
        // UnityWebRequest webRequest = UnityWebRequest.Get(InputManager.siteRequestUrl + "eventDate?v=" + VersionInput.currentVersion);
        // yield return webRequest.SendWebRequest();
        // if (webRequest.result != UnityWebRequest.Result.Success)
        // {
        //     console.error(webRequest.error);
        //     text.text = webRequest.error;
        //     yield break;
        // }
        // string str = webRequest.downloadHandler.text;
        // if (str.Contains("error:"))
        // {
        //     console.error(str.Split(':')[1]);
        //     text.text = str.Split(':')[1];
        //     yield break;
        // }
        // if (webRequest.responseCode != 200)
        // {
        //     console.error(webRequest.error);
        //     text.text = webRequest.error;
        //     yield break;
        // }
        // string[] seperator = str.Split('|');

        // string date = seperator[0];
        // string time = seperator[1];

        // string[] sepDate = date.Split('/');
        // string[] sepTime = time.Split(':');

        // int month = int.Parse(sepDate[0]);
        // int day = int.Parse(sepDate[1]);
        // int year = int.Parse(sepDate[2]);

        // int hours = int.Parse(sepTime[0]);
        // int minutes = int.Parse(sepTime[1]);
        // int seconds = int.Parse(sepTime[2]);

        // eventDate = new DateTime(year, month, day, hours, minutes, seconds, DateTimeKind.Utc);
        // localEventDate = eventDate.ToLocalTime();
        // e002 = e001.Length;
        // dateFound = true;
        yield break;
    }

    float e005;

    public void TestEvent()
    {
        // localEventDate = DateTime.Now.AddSeconds(30);
        // eventDate = DateTime.Now.AddSeconds(30);
    }

    private void Update()
    {
        // if (dateFound)
        // {
        //     TimeSpan timer = eventDate.Subtract(DateTime.UtcNow);
        //     if (timer.Days > 28)
        //     {
        //         if (e003 > 0f)
        //         {
        //             e003 -= Time.deltaTime;
        //         }
        //         else
        //         {
        //             e003 = 5f;
        //             e002++;
        //             if (e002 >= e001.Length)
        //             {
        //                 e002 = 0;
        //             }
        //             text.text = e001[e002];
        //         }
        //     }
        //     else if (timer.Days > 21)
        //     {
        //         text.text = "D A N G E R";
        //         text.color = Color.red;
        //     }
        //     else if (timer.Days > 1)
        //     {
        //         if (e005 != timer.Minutes)
        //         {
        //             e005 = timer.Minutes;
        //             audioDevice.PlayOneShot(audio1);
        //             text.color = Color.white;
        //             text.text = timer.Days + ":" + timer.Hours.ToString("00") + ":" + timer.Minutes.ToString("00") + "\nminutes";
        //         }
        //     }
        //     else if (timer.Hours > 0)
        //     {
        //         if (audioDevice.clip != audio2)
        //         {
        //             e004();
        //         }
        //         if (e005 != timer.Seconds)
        //         {
        //             e005 = timer.Seconds;
        //             audioDevice.PlayOneShot(audio1);
        //             text.color = Color.yellow;
        //             text.text = timer.Hours + ":" + timer.Minutes.ToString("00") + ":" + timer.Seconds.ToString("00");
        //         }
        //     }
        //     else if (timer.TotalSeconds > 0)
        //     {
        //         if (audioDevice.clip != audio2)
        //         {
        //             e004();
        //         }
        //         if (e005 != timer.Seconds)
        //         {
        //             e005 = timer.Seconds;
        //             audioDevice.PlayOneShot(audio1);
        //             text.color = Color.red;
        //             text.text = timer.Minutes + ":" + timer.Seconds.ToString("00");
        //         }
        //     }
        //     else
        //     {
        //         if (audioDevice.clip != audio2)
        //         {
        //             e004();
        //         }
        //         text.color = Color.red;
        //         text.text = "...\n...";
        //     }
        // }
    }

    private void e004()
    {
        // audioDevice.clip = audio2;
        // audioDevice.loop = true;
        // audioDevice.Play();
    }

    // public TMP_Text tmp_text;

    // public Light lights;

    // private float dotdotdotTime;

    // private int lastSecondsCaptured;

    // public AudioSource beep;
    // public bool eventStarted;
    // public AudioSource siren;
    // public Animator DssjkD98gZDJg9f0dDh30FDSG;
    // private bool stopSiren;

    // public static CountDownTimer instance { get; protected set; }

    // public Material whiteSkybox;

    // public GameObject hud;

    // public GameObject evilCube;

    // private void Start()
    // {
    //     instance = this;
    //     tmp_text.text = "";
    // }

    // public static void StopSiren()
    // {
    //     Destroy(instance.evilCube);
    //     instance.stopSiren = true;
    //     instance.siren.Stop();
    //     instance.lights.gameObject.SetActive(false);
    //     instance.tmp_text.gameObject.SetActive(false);
    //     instance.gameObject.SetActive(false);
    // }

    // public void BackNormal()
    // {
    //     Camera.main.clearFlags = CameraClearFlags.Skybox;
    //     Camera.main.backgroundColor = Color.black;
    //     RenderSettings.skybox = whiteSkybox;
    // }

    // public IEnumerator StartEvent()
    // {
    //     hud.SetActive(false);
    //     FindObjectOfType<RandomEvent>().StopEventNOW();
    //     FindObjectOfType<RandomEvent>().FadeMusicDown(FindObjectOfType<RandomEvent>().gameController.schoolMusic);
    //     FindObjectOfType<RandomEvent>().PauseRandomEvents();
    //     yield return new WaitForSecondsRealtime(10f);
    //     DssjkD98gZDJg9f0dDh30FDSG.SetTrigger("TriggerEvent");
    //     yield break;
    // }

    // private void Update()
    // {
    //     DateTime eventDate = new DateTime(2020, 10, 31, 20, 0, 0); // UTC Time format (EST: 4:00 PM)
    //     TimeSpan span = eventDate.Subtract(System.DateTime.UtcNow); // Subtract the UTC date.
    //     if (span.Days > 14)
    //     {
    //         dotdotdotTime += Time.deltaTime;
    //         if (dotdotdotTime >= 2f)
    //         {
    //             tmp_text.text = ".";
    //         }
    //         if (dotdotdotTime >= 4f)
    //         {
    //             tmp_text.text = ". .";
    //         }
    //         if (dotdotdotTime >= 6f)
    //         {
    //             tmp_text.text = ". . .";
    //         }
    //         if (dotdotdotTime >= 8f)
    //         {
    //             dotdotdotTime = 0f;
    //             tmp_text.text = "";
    //         }
    //     }
    //     else if (span.Days > 7)
    //     {
    //         if (!siren.isPlaying)
    //         {
    //             siren.Play();
    //         }
    //         tmp_text.color = Color.red;
    //         lights.color = Color.red;
    //         tmp_text.text = "EVILVERSE DETECTED! Estimated landing: " + span.Days + " Days.";
    //     }
    //     else if (span.TotalMinutes > -10)
    //     {
    //         if (span.Seconds != lastSecondsCaptured & !stopSiren)
    //         {
    //             lastSecondsCaptured = span.Seconds;
    //             beep.Play();
    //         }
    //         string input;
    //         if (span.Days > 0)
    //         {
    //             input = span.Days.ToString("00") + ":" + span.Hours.ToString("00") + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
    //         }
    //         else if (span.Hours > 0)
    //         {
    //             input = span.Hours.ToString("00") + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
    //         }
    //         else if (span.Minutes > 0)
    //         {
    //             tmp_text.color = Color.red;
    //             lights.color = Color.red;
    //             if (!siren.isPlaying)
    //             {
    //                 siren.Play();
    //             }
    //             input = span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
    //         }
    //         else if (span.TotalSeconds > 15)
    //         {
    //             tmp_text.color = Color.red;
    //             lights.color = Color.red;
    //             if (!siren.isPlaying)
    //             {
    //                 siren.Play();
    //             }
    //             input = "- EVILVERSE LANDING ALERT -\n" + span.Seconds.ToString("0") + ":" + span.Milliseconds.ToString("000");
    //         }
    //         else if (span.TotalSeconds > 0)
    //         {
    //             tmp_text.color = Color.red;
    //             lights.color = Color.red;
    //             if (!siren.isPlaying)
    //             {
    //                 siren.Play();
    //             }
    //             input = "- TAKE COVER! -\n" + span.Seconds.ToString("0") + ":" + span.Milliseconds.ToString("0");
    //         }
    //         else
    //         {
    //             tmp_text.color = Color.red;
    //             lights.color = Color.red;
    //             if (!eventStarted)
    //             {
    //                 eventStarted = true;
    //                 StartCoroutine(StartEvent());
    //             }
    //             if (!siren.isPlaying & !stopSiren)
    //             {
    //                 siren.Play();
    //             }
    //             input = "! - EVILVERSE IS LANDING! TAKE COVER! - !";
    //         }
    //         tmp_text.text = input;
    //     }
    //     else
    //     {
    //         BackNormal();
    //         StopSiren();
    //     }
    // }
}
