// YouWonScript
using System.Collections;
using UnityEngine;

public class BadEndingScript : MonoBehaviour
{
    public AudioSource audioDevice;

    public AudioClip music;

    public AudioClip endSound;

    public GameObject loadMainMenu;

    public Material nightSky;

    public Gradient ColorTransition;

    public Transform Player;

    public GameObject Bulbs;

    public GameObject UnlitBulbs;

    private PlayerScript player;

    private void Start()
    {
        FindObjectOfType<Daylight>().Stop();
        Camera.main.fieldOfView = 60f;
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        player.transform.position = new Vector3(-784f, 4f, -55f);
        player.SetPosition(new Vector3(-784f, 4f, -55f));
        player.SetRotation(Quaternion.Euler(new Vector3(0f, 180f, 0f)));
        player.SetHudOff();
        GameControllerScript gc = FindObjectOfType<GameControllerScript>();
        gc.SetAllCharacters(false);
        gc.EnableExitMode(false);
        gc.SetNoclip(false);
        gc.GetComponent<AudioSource>().mute = true;
        gc.GetComponent<AudioSource>().Stop();
        gc.StopNoise();
        gc.GetComponent<FogScript>().stopFog = true;
        for (int i = 0; i < 6; i++)
        {
            gc.LoseItem(i);
        }
        RenderSettings.fog = false;
        RenderSettings.fogColor = ColorTransition.Evaluate(0f);
        RenderSettings.fogDensity = 0f;
        if (audioDevice == null)
        {
            audioDevice = gameObject.AddComponent<AudioSource>();
            audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.AAmbience : null;
        }
        audioDevice.PlayOneShot(gc.aud_Switch, 1f);
        GameObject.Find("Environment").SetActive(false);
    }
    private bool IsNighttime;
    private void Update()
    {
        player.stamina = 0f;
        player.walkSpeed = 10f;
        if (!IsNighttime && Player.position.z >= 30f)
        {
            IsNighttime = true;
            NightTimeEnter();
        }
    }

    private void NightTimeEnter()
    {
        StartCoroutine(NighttimeFade());
    }

    private IEnumerator NighttimeFade()
    {
        RenderSettings.skybox = nightSky;
        RenderSettings.fog = true;
        audioDevice.clip = music;
        audioDevice.loop = true;
        audioDevice.Play();
        audioDevice.volume = 0f;
        float Val = 0f;
        while (Val < 1f)
        {
            Val += Time.deltaTime;
            if (Val > 1f) Val = 1f;
            audioDevice.volume = Val;
            RenderSettings.fogColor = ColorTransition.Evaluate(Val);
            RenderSettings.fogDensity = 0.07f * Val;
            yield return null;
        }
        UnlitBulbs.SetActive(false);
        Bulbs.SetActive(true);
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
            audio.pitch = 0.5f * Settings.GetFloat("TimeSpeed", 1f);
            audio.PlayOneShot(endSound);
            StartCoroutine(EndDelay(endSound.length));
            if (player.gc.failedNotebooks >= player.gc.notebooks)
            {
                AchievementManager.Instance.Earn(Achievements.CheatedEnding);
            }
        }
    }

    private System.Collections.IEnumerator EndDelay(float endDelay)
    {
        yield return new WaitForSecondsRealtime(endDelay + 4 * Settings.GetFloat("TimeSpeed", 1f));
        audioDevice.Stop();
        GameObject.Find("MobileControls").SetActive(false);
        loadMainMenu.SetActive(true);
        yield break;
    }
}