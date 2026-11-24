using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EndingYouBeatMyGame : MonoBehaviour
{
    public bool StartMusic;

    private float Duration;

    private AudioSource Music;

    [SerializeField]
    private AudioSource Baldi;

    [SerializeField]
    private SpriteRenderer BaldiSprite;

    [SerializeField]
    private Sprite[] BaldiSprites = new Sprite[2];

    [SerializeField]
    private AudioClip[] BaldiSays;

    private int BaldiSaysCount;

    private PlayerScript player;

    private GameControllerScript gc;

    [SerializeField]
    private AudioSource ElevatorSound;

    private float GlitchTime;

    private float LightingChange;

    private int GlitchCount;

    public GameObject loadMainMenu;

    // Start is called before the first frame update
    private void Start()
    {
        Camera.main.fieldOfView = 60f;
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        player.transform.position = new Vector3(-754f, 4f, -55f);
        player.SetPosition(new Vector3(-754f, 4f, -55f));
        player.SetRotation(Quaternion.Euler(new Vector3(0f, 180f, 0f)));
        player.SetHudOff();
        gc = FindObjectOfType<GameControllerScript>();
        if (gc.schoolMusic.isPlaying)
        {
            gc.schoolMusic.Stop();
        }
        gc.SetAllCharacters(false);
        gc.EnableExitMode(false);
        for (int i = 0; i < 6; i++)
        {
            gc.LoseItem(i);
        }
        gc.GetComponent<AudioSource>().mute = true;
        gc.GetComponent<AudioSource>().Stop();
        gc.StopNoise();
        gc.GetComponent<FogScript>().stopFog = true;
        RenderSettings.fog = false;
        RenderSettings.fogDensity = 0f;
        Music = GetComponent<AudioSource>();
        Music.PlayOneShot(gc.aud_Switch, 1f);
        GlitchTime = Random.Range(25f, 35f);
    }

    // Update is called once per frame
    private void Update()
    {
        player.stamina = 0f;
        player.walkSpeed = 10f;
        if (StartMusic)
        {
            if (Duration == 0f)
            {
                Music.Play();
            }
            Duration += Time.deltaTime;
            if (BaldiSaysCount == 0 && Duration >= 1.25f)
            {
                Baldi.PlayOneShotWC(BaldiSays[BaldiSaysCount]);
                BaldiSaysCount++;
            }
            else if (BaldiSaysCount == 1 && Duration >= 3.375f)
            {
                Baldi.PlayOneShotWC(BaldiSays[BaldiSaysCount]);
                BaldiSaysCount++;
            }
            else if (BaldiSaysCount == 2 && Duration >= 5.375f)
            {
                Baldi.PlayOneShotWC(BaldiSays[BaldiSaysCount]);
                BaldiSaysCount++;
            }
            else if (BaldiSaysCount == 3 && Duration >= 7.375f)
            {
                Baldi.PlayOneShotWC(BaldiSays[BaldiSaysCount]);
                BaldiSaysCount++;
            }
            else if (BaldiSaysCount == 4 && Duration >= 9.5f)
            {
                Baldi.PlayOneShotWC(BaldiSays[BaldiSaysCount]);
                BaldiSaysCount++;
            }
            else if (BaldiSaysCount == 5 && Duration >= 11.5f)
            {
                Baldi.PlayOneShotWC(BaldiSays[BaldiSaysCount]);
                BaldiSaysCount++;
            }
            else if (BaldiSaysCount == 6 && Duration >= 13.625f)
            {
                Baldi.PlayOneShotWC(BaldiSays[BaldiSaysCount]);
                BaldiSaysCount++;
            }
            else if (BaldiSaysCount == 7 && Duration >= 15.615f)
            {
                Baldi.PlayOneShotWC(BaldiSays[BaldiSaysCount]);
                BaldiSaysCount++;
            }
            if (Duration < 20f)
            {
                BaldiSprite.sprite = Baldi.isPlaying ? BaldiSprites[1] : BaldiSprites[0];
            }
            if (Duration >= GlitchTime)
            {
                if (Duration < (GlitchTime + 2f))
                {
                    BaldiSprite.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
                    if (LightingChange <= 0f)
                    {
                        LightingChange = 0.5f;
                        RenderSettings.ambientLight = Random.ColorHSV(0f, 1f, 0.75f, 1f, 1f, 1f, 1f, 1f);
                    }
                    else
                    {
                        LightingChange -= Time.deltaTime;
                    }
                    SwitchSprites();
                }
                else if (BaldiSaysCount == 8)
                {
                    BaldiSprite.color = Color.black;
                    RenderSettings.ambientLight = Color.black;
                    BaldiSaysCount = 9;
                    gc.CM.DestroyAllCaptions(false);
                    StartCoroutine(CrashSound());
                    GameObject.Find("MobileControls").SetActive(false);
                    loadMainMenu.SetActive(true);
                    if (Baldi.name == "DRREFLEXXX")
                    {
                        AchievementManager.Instance.Earn(Achievements.REFLEXGLITCH);
                    }
                }
            }
        }
        else if (ElevatorSound.isPlaying)
        {
            StartMusic = true;
        }
    }

    private void SwitchSprites()
    {
        BaldiSprite.sprite = (BaldiSprite.sprite == BaldiSprites[0]) ? BaldiSprites[1] : BaldiSprites[0];
        if (BaldiSprite.sprite == BaldiSprites[1])
        {
            Baldi.transform.localPosition = new Vector3(0f, 1.75f, Random.Range(-14f, 14f));
            Baldi.PlayOneShotWC(BaldiSays[GlitchCount]);
            GlitchCount++;
            if (GlitchCount >= BaldiSays.Length)
            {
                GlitchCount = 0;
            }
        }
    }

    private IEnumerator CrashSound()
    {
        Baldi.Stop();
        Baldi.clip = BaldiSays[Random.Range(0, BaldiSays.Length)];
        Baldi.Play();
        float audioTime = Random.Range(0f, Baldi.clip.length);
        Baldi.time = audioTime;
        float time = 5f;
        while (time > 0f)
        {
            time -= Time.unscaledDeltaTime;
            Baldi.time = audioTime;
            Baldi.transform.localPosition = new Vector3(0f, 1.75f, Random.Range(-14f, 14f));
            yield return null;
        }
        Baldi.Stop();
        yield break;
    }
}
