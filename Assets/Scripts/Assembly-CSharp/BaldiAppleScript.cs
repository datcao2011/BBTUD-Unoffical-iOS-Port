using UnityEngine;

public class BaldiAppleScript : MonoBehaviour
{
    public AudioClip bal_thanks;

    public AudioClip aud_Yum;

    private float times;

    private float delay1;

    private float times2;

    private void Update()
    {
        if (playSound)
        {
            if (Time.deltaTime == 0f)
            {
                delay1 -= Time.unscaledDeltaTime;
            }
            else
            {
                delay1 -= Time.deltaTime;
            }
            if (delay1 <= 0f)
            {
                delay1 = 0.12f;
                crunch1.PlayOneShot(crunchClip1);
                crunch1.PlayOneShot(crunchClip2);
            }
            times = Mathf.Round(UnityEngine.Random.Range(0f, 7f));
            if ((double)times > 5.99999)
            {
                times2 = Mathf.Round(UnityEngine.Random.Range(0f, 8f));
                times = 0f;
            }
            if ((double)times2 == 8.0)
            {
                bal_apple.PlayOneShot(aud_Yum);
                times2 = 0f;
            }
        }
        if (ps.appleEat & !baldiS)
        {
            baldiS = true;
            baldiNormal.SetActive(false);
            baldiApple.SetActive(true);
            bal_apple.Stop();
            pitch = Main.pitch;
            setPitch = Mathf.Clamp(pitch / Settings.GetFloat("TimeSpeed", 1f), 0.1f, 3f); // Double checking to make sure it doesn't go infinity and beyond!
            bal_apple.pitch = pitch;
            bal_apple.PlayOneShot(bal_thanks);
            delay = 4.3f;
            done1 = false;
        }
        delay -= Time.deltaTime * setPitch;
        backToBaldi -= Time.deltaTime * setPitch;
        if (delay <= 0f & !done1)
        {
            done1 = true;
            delay1 = 0f;
            bal_apple.PlayOneShot(aud_Yum);
            pitch = Main.pitch;
            backToBaldi = 15f;
            done2 = false;
            playSound = true;
            baldiSprite.SetBool("Eating", true);
        }
        if (backToBaldi <= 0f & !done2)
        {
            done2 = true;
            playSound = false;
            crunch1.Stop();
            crunch2.Stop();
            baldiSprite.SetBool("Eating", false);
            baldiApple.SetActive(false);
            baldiNormal.SetActive(true);
            ps.appleEat = false;
            ps.SetBackBaldiPriority();
            baldiS = false;
        }
    }

    private void FixedUpdate()
    {
        if (baldiS)
        {
            pitch = Main.pitch;
            bal_apple.pitch = pitch;
            crunch1.pitch = pitch;
            crunch2.pitch = pitch;
        }
    }

    private float pitch;

    private float setPitch;

    public PlayerScript ps;

    public Animator baldiSprite;

    public AudioSource bal_apple;

    public AudioSource crunch1;

    public AudioSource crunch2;

    public AudioSource Main;

    public AudioClip crunchClip1;

    public AudioClip crunchClip2;

    public bool done1;

    public bool done2;

    private bool playSound;

    public bool baldiS;

    public float delay;

    public float backToBaldi;

    public GameObject baldiApple;

    public GameObject baldiNormal;
}
