using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Platforms;
using System.Collections;

public class CampingScript : MonoBehaviour
{

    public AudioSource bal_instructions;

    public Animator baldiCamp;

    public GameObject tutorBaldi;

    public GameObject baldi;

    private bool played;

    public GameObject finalText;

    public TMP_Text timeText;

    public float time;

    public float score;

    private bool winTrue;

    public TMP_Text sticksText;

    public GameObject playerObject;

    public Transform player;

    public FireScript camping;

    public AudioSource win;

    public PlayerScript ps;

    private int audioInQueue;

    public GameObject crafters;

    public CraftersScript craftersScript;

    private PlatPlayer inputPlayer;

    public GameObject Particle;

    private void Awake()
    {
        Optimized = Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault);
        if (Optimized)
        {
            RenderSettings.ambientLight = Color.white;
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.02f;
        }
        Particle.SetActive(false);
    }

    private void Start()
    {
        inputPlayer = PlatTools.GetPlatformPlayer();
        bal_instructions.Play();
        played = true;
        time = 180f;
        UpdateStickCount();
        RenderSettings.skybox.SetFloat("_Exposure", 0.38f);
    }

    private void Update()
    {
        if (!bal_instructions.isPlaying)
        {
            baldiCamp.SetBool("BaldiCampTalking", false);
        }
        else
        {
            baldiCamp.SetBool("BaldiCampTalking", true);
        }
        if (!bal_instructions.isPlaying & played)
        {
            if (tutorBaldi.activeSelf)
            {
                if (FindObjectOfType<CaptionsManager>())
                {
                    FindObjectOfType<CaptionsManager>().DeleteClipCaption(bal_instructions.gameObject, bal_instructions.clip.name);
                }
            }
            tutorBaldi.SetActive(false);
        }
        if (Time.timeScale != 0f)
        {
            if (time > 0f)
            {
                time -= 1f * Time.deltaTime;
            }
            timeText.text = "Time Left: " + time.ToString("0");
            if (time < 0f)
            {
                time = 0f;
            }
            if (time == 0f)
            {
                Win();
            }
        }
        if (!winTrue & (inputPlayer.GetButtonDown("DropItem")) & Time.timeScale != 0f & sticks > 0)
        {
            DropSticks(playerTransform.position);
        }
        else if (winTrue & (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Cancel") || OnTap()))
        {
            SceneManager.LoadScene("Starter");
        }
        if (ps.detected && !craftersScript.disabled)
        {
            ActivateCrafters(true);
        }
        else if (!craftersScript.angry)
        {
            ActivateCrafters(false);
            craftersScript.anger = 0f;
            craftersScript.gettingAngry = false;
        }
    }

    private bool Active = true;

    public bool Optimized;

    private void ActivateCrafters(bool b)
    {
        if (Active != b)
        {
            crafters.SetActive(b);
            if (!Optimized)
            {
                if (b)
                {
                    StopCoroutine("FogOff");
                    StartCoroutine("FogOn");
                }
                else
                {
                    StopCoroutine("FogOn");
                    StartCoroutine("FogOff");
                }
            }
        }
        Active = b;
    }

    private float fogDivisor = 60f;

    private IEnumerator FogOn()
    {
        float i = RenderSettings.skybox.GetFloat("_Exposure");
        float j = i;
        RenderSettings.fog = true;
        while (i > 0f)
        {
            i -= Time.deltaTime * 0.15f;
            RenderSettings.skybox.SetFloat("_Exposure", i);
            RenderSettings.fogDensity = (j - i) / this.fogDivisor;
            yield return null;
        }
        yield break;
    }

    // Token: 0x0600004A RID: 74 RVA: 0x00003F10 File Offset: 0x00002110
    private IEnumerator FogOff()
    {
        float i = RenderSettings.skybox.GetFloat("_Exposure");
        float j = RenderSettings.fogDensity;
        while (i < 0.38f)
        {
            i += Time.deltaTime * 0.15f;
            RenderSettings.skybox.SetFloat("_Exposure", i);
            RenderSettings.fogDensity = (j - i) / this.fogDivisor;
            yield return null;
        }
        RenderSettings.skybox.SetFloat("_Exposure", 0.38f);
        RenderSettings.fog = false;
        yield break;
    }

    public void BaldiMad()
    {
        baldi.SetActive(true);
        bal_instructions.Stop();
        tutorBaldi.SetActive(false);
        if (FindObjectOfType<CaptionsManager>())
        {
            FindObjectOfType<CaptionsManager>().DeleteClipCaption(bal_instructions.gameObject, bal_instructions.clip.name);
        }
    }

    public bool OnTap()
    {
        if (!Input.touchSupported)
        {
            return false;
        }
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).phase == TouchPhase.Began;
        }
        return false;
    }

    public GameObject winScreen;

    public void Win()
    {
        if (!MultiplayerManager.isActive) Time.timeScale = 0f;
        winTrue = true;
        winScreen.SetActive(true);
    }

    public void AddSticks(int amount)
    {
        this.sticks += amount;
        this.UpdateStickCount();
    }

    public int sticks;

    public void UpdateStickCount()
    {
        sticksText.text = sticks + " sticks";
    }

    public void clearSticks()
    {
        this.AddSticks(-this.sticks);
    }

    public void DropSticks(Vector3 pos)
    {
        GameObject gameObject = Object.Instantiate(stickClone, new Vector3(pos.x, 4.5f, pos.z), playerTransform.rotation);
        gameObject.GetComponent<WoodPickupScript>().player = playerTransform;
        gameObject.GetComponent<WoodPickupScript>().camp = thisScript;
        gameObject.GetComponent<WoodPickupScript>().clickCursor = clickCursorObject;
        gameObject.transform.name = "FirewoodPickup";
        AddSticks(-1);
    }

    public GameObject stickClone;

    public CampingScript thisScript;

    public GameObject clickCursorObject;

    public Transform playerTransform;
}