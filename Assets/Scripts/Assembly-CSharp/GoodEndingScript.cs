// YouWonScript
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GoodEndingScript : MonoBehaviour
{
    public AudioSource audioDevice;

    public DoorScript door;

    private bool initEnding = false;

    private bool Phase2;

    private PlayerScript player;

    public NavMeshSurface Surface;

    [SerializeField]
    private NotebookScript Notebook;

    [SerializeField]
    private EndingFriend Friend;

    [SerializeField]
    private AudioSource ElevatorSource;

    [SerializeField]
    private AudioClip ElevatorClose;

    [SerializeField]
    private Animator Elevator;

    [SerializeField]
    private CameraScript Cam;

    [SerializeField]
    private GameObject p;

    [SerializeField]
    private AudioClip g;

    private bool Phase3;

    public TMP_Text hudText;

    public GameObject hudScreen;

    public GameObject MobileControls;

    public void ShowHudText(string text)
    {
        hudText.alignment = text.Contains("Bell Event") ? TextAlignmentOptions.Top : TextAlignmentOptions.Center;
        hudText.text = System.Text.RegularExpressions.Regex.Unescape(text);
        hudScreen.SetActive(true);
    }

    public void HideHudText()
    {
        hudScreen.SetActive(false);
        hudText.text = string.Empty;
    }

    private void Start()
    {
        hudText.color = Color.gray;
        Camera.main.fieldOfView = 60f;
        GameObject.Find("Player").transform.position = new Vector3(-754f, 4f, -55f);
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        player.SetPosition(new Vector3(-754f, 4f, -55f));
        player.SetRotation(Quaternion.Euler(new Vector3(0f, 180f, 0f)));
        player.SetHudOff();
        GameControllerScript gc = FindObjectOfType<GameControllerScript>();
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
        hudScreen.transform.SetParent(GameObject.Find("Display").transform);
        hudScreen.GetComponent<RawImage>().enabled = false;
        RenderSettings.fog = false;
        if (this.audioDevice == null)
        {
            this.audioDevice = gameObject.AddComponent<AudioSource>();
            audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
        }
        this.audioDevice.PlayOneShot(gc.aud_Switch, 1f);
        Surface.BuildNavMesh();
    }

    public void PlayHappyMusic()
    {
        this.audioDevice.Play();
    }

    private void Update()
    {
        player.stamina = Phase2 ? 100f : 0f;
        player.walkSpeed = 10f;
        player.runSpeed = 16f;
        if (Phase2)
        {
            if (!initEnding && door.bDoorOpen)
            {
                InitEnding();
            }
        }
        if (initEnding)
        {
            if (player.transform.position.z <= -30f)
            {
                if (!Phase3)
                {
                    Phase3 = true;
                    Elevator.SetTrigger("close");
                    ElevatorSource.pitch = 0.9f;
                    ElevatorSource.PlayOneShotWC(ElevatorClose);
                    StartCoroutine(PhaseThree());
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!Phase2 && !Notebook.isUp)
        {
            door.bDoorOpen = false;
            door.inside.sharedMaterial = door.closed;
            door.CloseDoor();
            Phase2 = true;
            Friend.ActivateWalking();
        }
    }

    private void InitEnding()
    {
        initEnding = true;
        RenderSettings.ambientLight = Color.gray;
        audioDevice.pitch = 0.5f;
        audioDevice.Play();
    }

    // [SerializeField]
    // private AudioClip FogLoop;

    private IEnumerator PhaseThree()
    {
        yield return new WaitForSeconds(0.4f);
        StartCoroutine("UnlockCursors");
        RenderSettings.ambientLight = Color.black;
        float Pitch = 0.5f;
        while (Pitch < 2.1f)
        {
            Pitch += Time.deltaTime;
            if (Pitch >= 2.1f)
            {
                Pitch = 2.1f;
            }
            audioDevice.pitch = Pitch;
            yield return null;
        }
        Cursor.SetCursor(null, hotSpot, cursorMode);
        yield return new WaitForSeconds(1f);
        MobileControls.SetActive(false);
        Cam.IgnoreCameraInputs = true;
        Camera.main.transform.position = transform.position + new Vector3(0f, Camera.main.transform.position.y, -4f);
        Camera.main.transform.rotation = Quaternion.identity;
        StopCoroutine("UnlockCursors");
        CursorControllerScript.LockCursor();
        p.SetActive(true);
        // ElevatorSource.clip = FogLoop;
        // ElevatorSource.pitch = 0.8f;
        // ElevatorSource.volume = 0f;
        // ElevatorSource.loop = true;
        // ElevatorSource.Play();
        yield return new WaitForSeconds(6f);
        ShowHudText("\\u0053\\u006F\\u0020\\u0079\\u006F\\u0075\\u0020\\u0062\\u006F\\u0074\\u0068\\u0020\\u0061\\u0072\\u0065\\u006E\\u0027\\u0074\\u0020\\u0077\\u0068\\u006F\\u0020\\u0049\\u0027\\u006D\\u0020\\u006C\\u006F\\u006F\\u006B\\u0069\\u006E\\u0067\\u0020\\u0066\\u006F\\u0072\\u002E");
        yield return new WaitForSeconds(5f);
        ShowHudText("\\u0057\\u0065\\u006C\\u006C\\u0020\\u0049\\u0020\\u0064\\u006F\\u006E\\u0027\\u0074\\u0020\\u0073\\u0065\\u0065\\u0020\\u0061\\u006E\\u0079\\u0020\\u0072\\u0065\\u0061\\u0073\\u006F\\u006E\\u0020\\u0066\\u006F\\u0072\\u0020\\u006B\\u0065\\u0065\\u0070\\u0069\\u006E\\u0067\\u0020\\u0079\\u006F\\u0075\\u0020\\u0062\\u006F\\u0074\\u0068\\u0020\\u0068\\u0065\\u0072\\u0065\\u0020\\u0062\\u0075\\u0074\\u0020\\u0064\\u006F\\u006E\\u0027\\u0074\\u0020\\u0074\\u0068\\u0069\\u006E\\u006B\\u0020\\u0074\\u0068\\u0069\\u0073\\u0020\\u0069\\u0073\\u0020\\u0074\\u0068\\u0065\\u0020\\u006F\\u006E\\u006C\\u0079\\u0020\\u0074\\u0069\\u006D\\u0065\\u0020\\u0079\\u006F\\u0075\\u0027\\u006C\\u006C\\u0020\\u0073\\u0065\\u0065\\u0020\\u006D\\u0065\\u002E");
        yield return new WaitForSeconds(8f);
        ShowHudText("\\u0049\\u0020\\u0070\\u006C\\u0061\\u006E\\u0020\\u0073\\u006F\\u006D\\u0065\\u0074\\u0068\\u0069\\u006E\\u0067\\u0020\\u0066\\u006F\\u0072\\u0020\\u0074\\u0068\\u0065\\u0020\\u0077\\u0068\\u006F\\u006C\\u0065\\u0020\\u0077\\u006F\\u0072\\u006C\\u0064\\u002C\\u0020\\u0062\\u0065\\u0069\\u006E\\u0067\\u0020\\u0068\\u0061\\u0070\\u0070\\u0069\\u0065\\u0072\\u0020\\u006F\\u006E\\u0020\\u0077\\u0068\\u0061\\u0074\\u0020\\u0069\\u0074\\u0027\\u0073\\u0020\\u0073\\u0075\\u0070\\u0070\\u006F\\u0073\\u0065\\u0064\\u0020\\u0074\\u006F\\u0020\\u0062\\u0065\\u002E");
        yield return new WaitForSeconds(7f);
        ShowHudText("\\u0059\\u006F\\u0075\\u0020\\u0062\\u006F\\u0074\\u0068\\u0020\\u0061\\u0072\\u0065\\u0020\\u006C\\u0075\\u0063\\u006B\\u0079\\u0020\\u0074\\u0068\\u0061\\u0074\\u0027\\u006C\\u006C\\u0020\\u006E\\u0065\\u0076\\u0065\\u0072\\u0020\\u0068\\u0061\\u0070\\u0070\\u0065\\u006E\\u0020\\u0062\\u0065\\u0063\\u0061\\u0075\\u0073\\u0065\\u0020\\u0049\\u0020\\u0073\\u0065\\u0061\\u0072\\u0063\\u0068\\u0065\\u0064\\u0020\\u0074\\u0068\\u0065\\u0020\\u0065\\u006E\\u0074\\u0069\\u0072\\u0065\\u0020\\u0077\\u006F\\u0072\\u006C\\u0064\\u0020\\u0066\\u006F\\u0072\\u0020\\u0074\\u0068\\u0061\\u0074\\u002E");
        yield return new WaitForSeconds(7f);
        ShowHudText("\\u0042\\u0075\\u0074\\u0020\\u0074\\u0068\\u0061\\u0074\\u0027\\u0073\\u0020\\u006E\\u006F\\u0074\\u0068\\u0069\\u006E\\u0067\\u0020\\u0074\\u006F\\u0020\\u0066\\u006F\\u0072\\u0067\\u0065\\u0074\\u002E\\u0020\\u0049\\u0027\\u006C\\u006C\\u0020\\u006E\\u006F\\u0074\\u0020\\u0068\\u0065\\u0073\\u0069\\u0074\\u0061\\u0074\\u0065\\u0020\\u0074\\u006F\\u0020\\u0063\\u006F\\u006D\\u0065\\u0020\\u0062\\u0061\\u0063\\u006B\\u0020\\u0074\\u006F\\u0020\\u0074\\u0068\\u0069\\u0073\\u0020\\u0077\\u006F\\u0072\\u006C\\u0064\\u0020\\u0069\\u0066\\u0020\\u0074\\u0068\\u0061\\u0074\\u0027\\u0073\\u0020\\u0074\\u0068\\u0065\\u0020\\u006C\\u0061\\u0073\\u0074\\u0020\\u0074\\u0068\\u0069\\u006E\\u0067\\u0020\\u0049\\u0020\\u006E\\u0065\\u0065\\u0064\\u0020\\u0074\\u006F\\u0020\\u0064\\u006F\\u002E");
        yield return new WaitForSeconds(8f);
        StartCoroutine("UnlockCursors");
        Cursor.SetCursor(cursor, hotSpot, cursorMode);
        HideHudText();
        p.SetActive(false);
        ElevatorSource.Stop();
        ElevatorSource.pitch = 0.5f;
        ElevatorSource.volume = 0.3f;
        ElevatorSource.PlayOneShot(g);
        yield return new WaitForSeconds(3f);
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.Earn(Achievements.SecretEnding);
        }
        SceneManager.LoadScene("Starter");
        yield break;
    }

    [SerializeField]
    private Texture2D cursor;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    private IEnumerator UnlockCursors()
    {
        for (; ; )
        {
            CursorControllerScript.UnlockCursor();
            yield return new WaitForEndOfFrame();
        }
    }

    // private System.Collections.IEnumerator EndDelay()
    // {
    //     StartCoroutine(UnlockCursors());
    //     yield return new WaitForSeconds(60f);
    //     ApplicationConfigure.closeApplication();
    //     yield break;
    // }
}