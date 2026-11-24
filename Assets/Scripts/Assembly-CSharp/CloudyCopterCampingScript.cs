using System.Collections;
using UnityEngine;

public class CloudyCopterCampingScript : MonoBehaviour
{
    public float timeToBlowTime;

    public AudioSource audioDevice;

    public AudioClip blow;

    public AudioClip pah;

    public GameObject sprite;

    public FireScript fire;

    public bool blowing;

    private void Start()
    {
        StartCoroutine(TimeToBlow());
    }

    private void Update()
    {
        if (blowing)
        {
            if (!fire.fireDied)
            {
                fire.timer -= Time.deltaTime * 3f;
            }
            else
            {
                sprite.SetActive(false);
                audioDevice.loop = false;
            }
        }
    }

    private IEnumerator TimeToBlow()
    {
        timeToBlowTime = Random.Range(20f, 100f);
        while (timeToBlowTime > 0f)
        {
            if (!playerOnTrigger)
                timeToBlowTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Blow());
        yield break;
    }

    private bool playerOnTrigger;

    private IEnumerator Blow()
    {
        sprite.SetActive(true);
        blowing = true;
        audioDevice.clip = blow;
        audioDevice.loop = true;
        audioDevice.Play();
        yield break;
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.transform.tag == "Player" || other.transform.tag == "NPC") & blowing)
        {
            blowing = false;
            sprite.SetActive(false);
            audioDevice.Stop();
            audioDevice.PlayOneShotWC(pah);
            StartCoroutine(TimeToBlow());
        }
        else if ((other.transform.tag == "Player" || other.transform.tag == "NPC") & !blowing)
        {
            playerOnTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.transform.tag == "Player" || other.transform.tag == "NPC") & !blowing)
        {
            playerOnTrigger = false;
        }
    }
}
