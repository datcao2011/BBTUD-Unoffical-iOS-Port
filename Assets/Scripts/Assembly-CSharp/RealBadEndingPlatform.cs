using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class RealBadEndingPlatform : MonoBehaviour
{
    public Transform Baldi;

    public Animator BaldiAnim;

    public GameObject BaldiGB;

    public AudioSource BaldiSlap;

    public AudioClip Slap;

    private PlayerScript player;

    private GameControllerScript gc;

    private float Duration = 0f;

    private int Stage;

    public bool SummonBaldi;

    public GameObject Signs;

    private float timeToMove;

    public float Interval;

    private void Start()
    {
        FindObjectOfType<Daylight>().Stop();
        Camera.main.fieldOfView = 60f;
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        player.transform.position = transform.position + new Vector3(0f, 4f, 0f);
        player.SetPosition(transform.position + new Vector3(0f, 4f, 0f));
        player.SetRotation(Quaternion.Euler(new Vector3(0f, 180f, 0f)));
        player.SetHudOff();
        gc = FindObjectOfType<GameControllerScript>();
        gc.SetAllCharacters(false);
        gc.EnableExitMode(false);
        gc.GetComponent<AudioSource>().mute = true;
        gc.GetComponent<AudioSource>().Stop();
        gc.StopNoise();
        gc.GetComponent<FogScript>().stopFog = true;
        for (int i = 0; i < 6; i++)
        {
            gc.LoseItem(i);
        }
        RenderSettings.fog = false;
        RenderSettings.fogDensity = 0f;
        GameObject.Find("Environment").SetActive(false);
        timeToMove = 3f;
        player.cameraScript.gameOverFadeTransform.localScale = new Vector3(2f, 2f, 2f);
    }

    private void Update()
    {
        player.stamina = 0f;
        if (SummonBaldi)
        {
            player.walkSpeed = (Duration >= 8f) ? 0f : 10f;
            Duration += Time.deltaTime;
            if (this.timeToMove > 0f)
            {
                this.timeToMove -= 1f * Time.deltaTime;
            }
            else
            {
                this.timeToMove = Interval;
                BaldiSlap.PlayOneShotWC(Slap);
                BaldiAnim.SetTrigger("slap");
            }
            if (Duration >= 5f)
            {
                if (Baldi.position.z < player.transform.position.z)
                {
                    Baldi.position += new Vector3(0f, 0f, Time.deltaTime * 55f);
                }
                else if (Stage == 2)
                {
                    Stage = 3;
                    gc.audioDevice.mute = false;
                    gc.baldi.SetActive(true);
                    gc.baldiScrpt.spoopyText.GetComponent<TMPro.TMP_Text>().text = gc.baldiScrpt.spoopyText.GetComponent<TMPro.TMP_Text>().text.Replace("YOU MADE A JUMPSCARE!", "YOU GOT YOURSELF BANNED!");
                    gc.Ban();
                }
                if (gc.player.gameOver)
                {
                    enabled = false;
                }
            }
            if (Stage == 0 && Duration >= 8f)
            {
                Stage = 1;
                player.SetPosition(new Vector3(-755f, 4f, player.transform.position.z));
            }
            if (Stage == 1 && Duration >= 14f)
            {
                Stage = 2;
                player.SetRotation(Quaternion.Euler(0f, 180f, 0f));
            }
        }
        else
        {
            player.walkSpeed = player.transform.position.z >= 170f ? 6f : 35f;
        }
    }

    private void LateUpdate()
    {
        if (Duration >= 14f)
        {
            player.SetRotation(Quaternion.Euler(0f, 180f, 0f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            SummonBaldi = true;
            BaldiGB.SetActive(true);
            Signs.SetActive(false);
            player.cameraScript.DenyRotationYMovement = true;
        }
    }
}
