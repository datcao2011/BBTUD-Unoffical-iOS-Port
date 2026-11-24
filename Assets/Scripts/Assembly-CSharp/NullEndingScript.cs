// YouWonScript
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NullEndingScript : MonoBehaviour
{
    public AudioSource audioDevice;

    public AudioClip nullClip;

    public GameObject characters;

    public Animator NullAnim;

    private bool initEnding;

    [SerializeField]
    private NullRotation[] LastNULLRotations;

    private float time;

    private NullRotationManager Manager;

    private void Start()
    {
        Camera.main.fieldOfView = 60f;
        Camera.main.farClipPlane = 200f;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        // Clear this in order to boost framerate
        NullRotationManager SchoolRhythm = NullManager.Instance.Music.SchoolRhythm;
        if (Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
        {
            StartCoroutine(DeleteRemaining());
        }
        Manager = SchoolRhythm;
        SchoolRhythm.Intensity = 0f;
        NullManager.Instance.transform.parent.gameObject.SetActive(false);
        GameObject.Find("Player").GetComponent<PlayerScript>().SetPosition(transform.position);
        GameObject.Find("Player").GetComponent<PlayerScript>().SetHudOff();
        GameObject.Find("GameController").GetComponent<GameControllerScript>().SetAllCharacters(false);
        GameObject.Find("GameController").GetComponent<GameControllerScript>().EnableExitMode(false);
        GameObject.Find("GameController").GetComponent<AudioSource>().mute = true;
        GameObject.Find("GameController").GetComponent<AudioSource>().Stop();
        GameObject.Find("GameController").GetComponent<GameControllerScript>().StopNoise();
        GameObject.Find("GameController").GetComponent<FogScript>().stopFog = true;
        RenderSettings.fog = false;
        if (this.audioDevice == null)
        {
            this.audioDevice = gameObject.AddComponent<AudioSource>();
            audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ACharacters : null;
        }
        GameObject.Find("Environment").SetActive(false);
    }

    private bool InitEnding_Glitch;

    private void Update()
    {
        GameObject.Find("Player").GetComponent<PlayerScript>().stamina = 0f;
        GameObject.Find("Player").GetComponent<PlayerScript>().walkSpeed = 10f;
        if (initEnding && !audioDevice.isPlaying)
        {
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.Earn(Achievements.undefined);
            }
            SceneManager.LoadScene("Starter");
        }
        if (initEnding)
        {
            time += Time.unscaledDeltaTime;
            if (time >= 7.8f && time < 10.08f)
            {
                for (int i = 0; i < LastNULLRotations.Length; i++) LastNULLRotations[i].Tick();
                if (!InitEnding_Glitch)
                {
                    InitEnding_Glitch = true;
                    StartCoroutine(GlitchLast());
                }
            }
        }
    }

    private IEnumerator GlitchLast()
    {
        if (Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
        {
            yield break;
        }
        Manager.Multiplier = 1f;
        Manager.NegativeVal = 1f;
        float GlitchIntensity = 0f;
        float GlitchMultiplier = 0.25f;
        float GlitchTimer = 0.025f;
        int B = 0;
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = InputManager.instance.ASFX;
        source.PlayOneShot(NullManager.Instance.NULL.GameOver);
        while (time < 10.08f)
        {
            GlitchMultiplier += Time.unscaledDeltaTime * 0.375f;
            if (B == 0)
            {
                GlitchIntensity += Time.unscaledDeltaTime * GlitchMultiplier;
                if (GlitchIntensity >= GlitchTimer * (1f + Mathf.Clamp(GlitchMultiplier * 0.2f, 0f, 10f)))
                {
                    B = 1;
                }
            }
            else if (B == 1)
            {
                GlitchIntensity -= Time.unscaledDeltaTime * GlitchMultiplier;
                if (GlitchIntensity <= 0f)
                {
                    if (GlitchTimer >= 0.1f)
                    {
                        B = 2;
                    }
                    else
                    {
                        B = 0;
                    }
                }
            }
            else if (B == 2)
            {
                GlitchIntensity -= Time.unscaledDeltaTime * GlitchMultiplier;
                if (GlitchIntensity <= GlitchTimer * (1f + Mathf.Clamp(GlitchMultiplier * 0.2f, 0f, 10f)) * -1f)
                {
                    B = 3;
                }
            }
            else if (B == 3)
            {
                GlitchIntensity += Time.unscaledDeltaTime * GlitchMultiplier;
                if (GlitchIntensity >= 0f)
                {
                    B = 0;
                }
            }
            Manager.Intensity = GlitchIntensity * GlitchMultiplier;
            yield return null;
        }
        Manager.Multiplier = 0f;
        Manager.Intensity = 0f;
        for (int i = 0; i < LastNULLRotations.Length; i++) LastNULLRotations[i].Tick();
        source.Stop();
        Destroy(source);
        StartCoroutine(DeleteRemaining());
        yield break;
    }

    private IEnumerator DeleteRemaining()
    {
        foreach (NullRotation rotation in LastNULLRotations)
        {
            Destroy(rotation);
            yield return null;
        }
        yield break;
    }

    public void InitEnding()
    {
        characters.SetActive(true);
        audioDevice.clip = nullClip;
        audioDevice.Play();
        NullAnim.SetTrigger("play");
        initEnding = true;
    }
}