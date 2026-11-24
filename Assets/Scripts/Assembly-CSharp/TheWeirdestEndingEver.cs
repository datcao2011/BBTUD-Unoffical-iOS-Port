using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TheWeirdestEndingEver : MonoBehaviour
{
    public AudioSource audioDevice;

    private PlayerScript player;

    private GameControllerScript gc;

    public EndingElevator Elevator1;

    [SerializeField]
    private AudioClip CloseSound_Old;

    [SerializeField]
    private AudioClip CloseSound;

    [SerializeField]
    private GameObject Maze;

    [SerializeField]
    private GameObject Park;

    [SerializeField]
    private GameObject Wedding;

    [SerializeField]
    private GameObject LastArea;

    private int Stage = 1;

    public GameObject[] HUDElements;

    public GameObject CheatMenu;

    public MinimapScript Minimap;

    public Camera MinimapCamera;

    public NotebookScript Notebook;

    [SerializeField]
    private NotebookScript LastNotebook;

    [SerializeField]
    private AudioClip DarkSky_NoClip;

    private void Start()
    {
        FindObjectOfType<Daylight>().Stop();
        Camera.main.fieldOfView = 60f;
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        player.transform.position = new Vector3(-755f, 4f, -55f);
        player.SetPosition(new Vector3(-755f, 4f, -55f));
        player.SetRotation(Quaternion.Euler(new Vector3(0f, 180f, 0f)));
        gc = FindObjectOfType<GameControllerScript>();
        CheatMenu = gc.cheatConsole;
        gc.SetAllCharacters(false);
        gc.EnableExitMode(false);
        gc.CM.DestroyAllCaptions(false);
        gc.cheatingConsole.schoolEventSystem.StopEventNOW();
        gc.cheatingConsole.schoolEventSystem.PauseRandomEvents();
        Minimap.enabled = false;
        Minimap.miniAnim.gameObject.SetActive(false);
        MinimapCamera.enabled = false;
        StartCoroutine(NoCheating());
        foreach (GameObject HUDElement in HUDElements)
        {
            HUDElement.SetActive(false);
        }
        gc.GetComponent<AudioSource>().mute = true;
        gc.GetComponent<AudioSource>().Stop();
        gc.StopNoise();
        gc.schoolMusic.mute = true;
        gc.GetComponent<FogScript>().stopFog = true;
        for (int i = 0; i < 6; i++)
        {
            gc.LoseItem(i);
        }
        RenderSettings.fog = false;
        RenderSettings.fogDensity = 0f;
        if (audioDevice == null)
        {
            audioDevice = gameObject.AddComponent<AudioSource>();
            audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.AAmbience : null;
        }
        audioDevice.PlayOneShot(gc.aud_Switch, 1f);
        GameObject.Find("Environment").SetActive(false);
    }
    private float time;

    private void Update()
    {
        player.useStamina = true;
        player.walkSpeed = 10f;
        player.runSpeed = 16f;
        if (Stage == 1 && time < 1f)
        {
            time += Time.deltaTime;
            foreach (GameObject HUDElement in HUDElements)
            {
                HUDElement.SetActive(false);
            }
        }
        if (Stage == 2)
        {
            if (Notebook.isUp)
            {
                Notebook.questions[3] = Notebook.questions[3].Replace("{GOAL}", GetNextGoalTime());
            }
            else if (player.transform.position.x <= -322.5582f)
            {
                Stage = 3;
                StartCoroutine(FadeToBlackToNext());
            }
        }
        if (Stage == 4)
        {
            if (Time.timeScale != 0f)
            {
                if (!LastNotebook.isUp)
                {
                    Stage = 5;
                    Debug.Log("ENDING FINISHED FINALLY!");
                    if (!AchievementManager.Instance.HasEarned((int)Achievements.HardestAndGoodReward))
                    {
                        if (CoinSaveSystem.Load() <= 5000000)
                        {
                            CoinSaveSystem.Save(CoinSaveSystem.Load() + 10000000);
                        }
                        AchievementManager.Instance.Earn(Achievements.HardestAndGoodReward);
                    }
                    SceneManager.LoadScene("Starter");
                }
            }
        }
    }

    private string GetNextGoalTime()
    {
        System.DateTime Now = System.DateTime.Now;
        System.DateTime Goal = System.DateTime.Now.AddMinutes(-Now.Minute).AddSeconds(-Now.Second).AddMilliseconds(-Now.Millisecond);
        while (Now > Goal.AddMinutes(-2))
        {
            Goal = Goal.AddMinutes(5);
        }
        return Goal.ToShortTimeString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            if (Stage == 1)
            {
                StartCoroutine(TimeStage1());
            }
        }
    }

    private IEnumerator NoCheating()
    {
        while (true)
        {
            CheatMenu.SetActive(false);
            if (gc.cheatConsoleActive)
            {
                if (Stage == 3 && System.DateTime.Now < GoalScript.Goal)
                {
                    audioDevice.PlayOneShotWC(DarkSky_NoClip);
                }
                gc.reticle.SetActive(true);
                gc.LockMouse();
            }
            gc.cheatConsoleActive = false;
            yield return null;
        }
    }

    public void Next()
    {
        Wedding.SetActive(false);
        LastArea.SetActive(true);
        Stage = 4;
        player.transform.position = new Vector3(-755f, 4f, 65f);
        player.SetPosition(new Vector3(-755f, 4f, 65f));
        player.SetRotation(Quaternion.Euler(new Vector3(0f, 0f, 0f)));
    }

    private IEnumerator TimeStage1()
    {
        Stage = 2;
        Elevator1.Elevator.PlayOneShotWC(CloseSound_Old);
        Elevator1.Anim.SetTrigger("close");
        Elevator1.Timer = 5f;
        yield return new WaitForSeconds(5f);
        Maze.SetActive(false);
        Park.SetActive(true);
        yield break;
    }

    private IEnumerator FadeToBlackToNext()
    {
        Stage = 3;
        Elevator1.gameObject.SetActive(false);
        Park.SetActive(false);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        RenderSettings.ambientLight = Color.black;
        GameObject HUD = HUDElements[0].transform.parent.gameObject;
        HUD.SetActive(false);
        yield return new WaitForSeconds(1f);
        RenderSettings.ambientLight = Color.white;
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        HUD.SetActive(true);
        Wedding.SetActive(true);
        player.transform.position = new Vector3(-765f, 4f, 145f);
        player.SetPosition(new Vector3(-765f, 4f, 145f));
        player.SetRotation(Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        HudScreenTextScript text = HUD.GetComponent<HudScreenTextScript>();
        RawImage BG = text.hudScreen.GetComponent<RawImage>();
        BG.color = new Color(0f, 0f, 0f, BG.color.a);
        text.hudText.enableAutoSizing = true;
        text.hudText.fontSizeMin = 4f;
        text.hudText.fontSizeMax = 14f;
        yield break;
    }
}