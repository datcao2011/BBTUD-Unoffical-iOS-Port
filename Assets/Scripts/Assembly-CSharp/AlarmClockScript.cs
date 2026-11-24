// AlarmClockScript
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using Multiplayer;

public class AlarmClockScript : MonoBehaviour
{
    public float timeLeft;

    private float lifeSpan;

    private bool rang;

    public BaldiScript baldi;

    public AudioClip ring;

    public AudioClip Wind;

    public TMP_Text TimerText;

    public Transform Arrow;

    public AudioSource audioDevice;

    public GameObject clickCursor;

    private GameObject click;

    private Transform player;

    private void Start()
    {
        timeLeft = 30f;
        lifeSpan = 33.3f;
        player = baldi.player;
        int minutes = Mathf.FloorToInt((float)timeLeft / 60F);
        int seconds = Mathf.CeilToInt((float)timeLeft - minutes * 60);

        string Timer = string.Format("{0:0}:{1:00}", minutes, seconds);
        TimerText.text = Timer;
        Arrow.localEulerAngles = new Vector3(0f, 0f, timeLeft * -6f);
    }

    private void Update()
    {
        if (timeLeft >= 0f)
        {
            timeLeft -= Time.deltaTime;
            int minutes = Mathf.FloorToInt((float)timeLeft / 60F);
            int seconds = Mathf.CeilToInt((float)timeLeft - minutes * 60);
            if (seconds == 60)
            {
                minutes += 1;
                seconds = 0;
            }
            string Timer = string.Format("{0:00}:{1:00}", minutes, seconds);
            TimerText.text = Timer;
            Arrow.localEulerAngles = new Vector3(0f, 0f, timeLeft * -6f);
        }
        else if (!rang)
        {
            Alarm();
        }
        if (lifeSpan >= 0f)
        {
            lifeSpan -= Time.deltaTime;
        }
        else
        {
            Object.Destroy(base.gameObject, 0f);
        }
        // Alarm clock update
        bool isHovering = false;
        if (Vector3.Distance(player.position, base.transform.position) < 10f)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit raycastHit;
            isHovering = Physics.Raycast(ray, out raycastHit) && raycastHit.transform == transform;
            if (InteractScript.Intracting && Time.timeScale != 0f)
            {
                if (isHovering)
                {
                    WindIt();
                }
            }
        }
        // This was moved outside so it won't glitch out
        if (isHovering && !rang)
        {
            if (click == null)
                click = Object.Instantiate(clickCursor);
        }
        else
        {
            if (click != null)
                Object.Destroy(click);
        }
    }

    private void WindIt()
    {
        if (!rang)
        {
            audioDevice.PlayOneShot(Wind);
            if (timeLeft > 45f)
            {
                timeLeft = 45f;
            }
            else if (timeLeft > 30f)
            {
                timeLeft = 30f;
            }
            else if (timeLeft > 15f)
            {
                timeLeft = 15f;
            }
            else
            {
                timeLeft = 60f;
            }
            lifeSpan = timeLeft + 3.3f;
            if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi)
            {
                ClientSend.Tick((int)timeLeft, transform.position);
            }
        }
    }

    public void MP_Tick(int Seconds)
    {
        if (!rang)
        {
            audioDevice.PlayOneShot(Wind);
            timeLeft = Seconds;
            lifeSpan = timeLeft + 3.3f;
        }
    }

    private void Alarm()
    {
        rang = true;
        TimerText.text = "00:00";
        Arrow.localEulerAngles = new Vector3(0f, 0f, 0f);
        if (baldi.isActiveAndEnabled && !(MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer))
            baldi.Hear(base.transform.position, 8f);
        if (baldi.gameController.DoctorReflex != null && baldi.gameController.DoctorReflex.Angry && !baldi.gameController.DoctorReflex.WantsTheHammer)
        {
            baldi.gameController.DoctorReflex.Hear(transform.position); // Hear like Baldi
        }
        if (NullManager.Instance != null)
        {
            NullManager.Instance.NULL.Hear(base.transform.position, 8f);
        }
        audioDevice.clip = ring;
        audioDevice.loop = false;
        audioDevice.Play();
    }
}