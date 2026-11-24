using UnityEngine;
using Platforms;


public class RollTrigger : MonoBehaviour
{
    private PlatPlayer player;

    public AudioClip rollSound;

    private int stage = 0;

    private int mobileStage = 0;

    private float finalTime;

    private float rollTime;

    private bool rolled;

    AudioSource audioDevice;

    public NotebookScript[] notebooks;

    public RollBaller rollBaller;

    [TextArea]
    public string text;

    [TextArea]
    public string text2;

    private void Start()
    {
        player = PlatTools.GetPlatformPlayer();
        if (Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault)) gameObject.SetActive(false);
        if (MultiplayerManager.isActive) gameObject.SetActive(false);
        if (NullManager.Instance != null) { Destroy(gameObject); }
    }

    public void Roll()
    {
        mobileStage++;
        if (mobileStage == 3)
        {
            gameObject.SetActive(true);
            stage = 9;
        }
    }

    private void Update()
    {
        if (stage == 0 && Input.GetKeyDown(KeyCode.B))
        {
            stage++;
        }
        else if (stage == 1 && Input.GetKeyDown(KeyCode.A))
        {
            stage++;
        }
        else if (stage == 2 && Input.GetKeyDown(KeyCode.L))
        {
            stage++;
        }
        else if (stage == 3 && Input.GetKeyDown(KeyCode.D))
        {
            stage++;
        }
        else if (stage == 4 && Input.GetKeyDown(KeyCode.I))
        {
            stage++;
        }
        else if (stage == 5 && Input.GetKeyDown(KeyCode.R))
        {
            stage++;
        }
        else if (stage == 6 && Input.GetKeyDown(KeyCode.O))
        {
            stage++;
        }
        else if (stage >= 7 && stage < 9 && Input.GetKeyDown(KeyCode.L))
        {
            stage++;
        }
        else if (stage >= 9 && !rolled && !FindObjectOfType<GameControllerScript>().finaleMode && Time.timeScale != 0f)
        {
            if (MultiplayerManager.isActive) return;
            rolled = true;
            AchievementManager.Instance.Earn(Achievements.BaldiRolled);
            CreateAudioSource();
            audioDevice.clip = rollSound;
            audioDevice.Play();
            GameControllerScript gc = FindObjectOfType<GameControllerScript>();
            if (gc.schoolMusic.isPlaying)
                FindObjectOfType<RandomEvent>().FadeMusicDown(gc.schoolMusic);
            notebooks = FindObjectsOfType<NotebookScript>();

            foreach (NotebookScript activation in notebooks)
            {
                activation.gameObject.SetActive(false);
            }
            finalTime = rollSound.length;
            currentNotebooks = gc.notebooks;
            gc.SetAllCharacters(false);
            gc.notebookCount.text = "BaldiRolled!";
            FindObjectOfType<HudScreenTextScript>().ShowHudText(80f, text);
            gc.baldiTutor.SetActive(false);
            gc.notebooks = 0;
            ElevatorScript.U(true);
            FindObjectOfType<RandomEvent>().StopEventNOW();
            FindObjectOfType<RandomEvent>().PauseRandomEvents();
            FindObjectOfType<LockDownDoorMain>().lockDownDoorInstantAll(false);

        }
        else if (player.GetAnyButtonDown)
        {
            stage = 0;
        }

        if (audioDevice != null)
        {
            rollTime += Time.unscaledDeltaTime;
            if (rollTime >= 29.5f & !rollBaller.enabled)
            {
                rollBaller.enabled = true;
            }
            if (rollTime >= 70f)
            {
                rollBaller.Colorize();
            }
            if (rollTime >= 80f & !opened)
            {
                opened = true;
                FindObjectOfType<HudScreenTextScript>().ShowHudText(133f, text2);
                FindObjectOfType<GameControllerScript>().notebooks = 2;
                FindObjectOfType<LockDownDoorMain>().lockDownDoorInstantAll(true);
            }
            if (rollTime >= finalTime & !finished)
            {
                finished = true;
                GameControllerScript gc = FindObjectOfType<GameControllerScript>();
                gc.notebooks = currentNotebooks;
                gc.UpdateNotebookCount();
                gc.SetAllCharacters(true);
                if (!gc.spoopMode)
                {
                    gc.baldiTutor.SetActive(true);
                    FindObjectOfType<RandomEvent>().FadeMusicUp(gc.schoolMusic);
                }
                ElevatorScript.U(false);
                FindObjectOfType<RandomEvent>().ResumeRandomEvents();
                foreach (NotebookScript notebook in notebooks)
                {
                    notebook.gameObject.SetActive(true);
                }
                rollBaller.RevertEverything();
                Destroy(this);
            }
        }
    }

    private bool finished;

    private bool opened;

    int currentNotebooks;

    private void CreateAudioSource()
    {
        audioDevice = gameObject.AddComponent<AudioSource>();
        audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.AMusic : null;
    }
}
