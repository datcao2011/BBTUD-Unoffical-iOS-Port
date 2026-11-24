using System.Collections;
using UnityEngine;
using TMPro;

// Token: 0x0200000C RID: 12
public class FireScript : MonoBehaviour
{
    public AudioSource fireSound;

    public GameObject fire;

    public TMP_Text fireTimer;

    public TMP_Text scoreText;

    public TMP_Text wowText;

    public GameObject wow;

    public float score;

    public float extraScore;

    public float timer;

    public float delay;

    public float fireT;

    public GameObject lightfire;

    public Light fireLight;

    public GameObject particle;

    public Transform player;

    public CampingScript camp;

    public bool fireDied;

    public bool buildingFire;

    public AudioSource wowbal;

    private void Start()
    {
        timer = 30f;
        fireT = 1f;
        scoreText.text = "Score: " + score.ToString();
    }

    private void Update()
    {
        fireLight.spotAngle = timer;
        if (Time.timeScale != 0)
        {
            if (timer > 0f)
            {
                timer -= 1f * Time.deltaTime;
            }
            fireTimer.text = "Fire Fuel Time: " + timer.ToString("0");
            if (timer < 0f)
            {
                timer = 0f;
            }
            if (timer == 0f)
            {
                FireDied();
            }
            if (delay > 0f)
            {
                delay -= 1f * Time.deltaTime;
            }
            if (delay == 0f)
            {
                wow.SetActive(false);
            }
            if (delay < 0f)
            {
                delay = 0f;
            }
            if (fireT > 0f)
            {
                fireT -= 1f * Time.deltaTime;
            }
            if (fireT < 0f)
            {
                fireT = 0f;
            }
            if (!fireDied)
            {

                if (fireT == 0f)
                {
                    score += ((int)timer * 10f);
                    fireT = 1f;
                    UpdateScoreCount();
                }
            }

        }
    }

    public void FireDied()
    {
        fireDied = true;
        fireSound.Stop();
        fire.SetActive(false);
        lightfire.SetActive(false);
        particle.SetActive(false);
        camp.BaldiMad();
    }

    public void UpdateScoreCount()
    {
        scoreText.text = "Score: " + score.ToString();
    }
    public float InitialTimer;

    public void WoodInFire()
    {
        if (camp.sticks == 0) return;

        timer += Mathf.RoundToInt((float)camp.sticks * 8f);
        if ((int)timer > (int)InitialTimer)
        {
            timer = InitialTimer;
            wowbal.Play();
            delay = 3f;
            wow.SetActive(true);
            extraScore = camp.sticks * 1225f;
            wowText.text = "WOW!\nTHAT'S A LOT OF STICKS!\nEXTRA SCORE: " + extraScore.ToString();
            score += extraScore;
            extraScore = 0f;
            UpdateScoreCount();
            if (FindObjectOfType<CaptionsManager>())
            {
                FindObjectOfType<CaptionsManager>().DestroyAllCaptions(false);
            }
        }
        camp.clearSticks();
    }
}
