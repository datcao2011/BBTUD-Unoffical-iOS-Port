// NotebookScript
using UnityEngine;

public class NotebookScript : MonoBehaviour
{
    public float openingDistance;

    public GameControllerScript gc;

    public BaldiScript bsc;

    public float respawnTime;

    public bool up = true;

    public Transform player;

    public GameObject learningGame;

    public AudioSource audioDevice;

    public GameObject clickCursor;

    public GameObject click;

    public string notebookType;

    public string[] hints = new string[10];

    public string[] questions = new string[10];

    public string[] answers = new string[10];

    public bool[] opinion = new bool[10];

    public string impossibleQuestion;

    public bool isUp = true;
    private float y;
    private void Awake()
    {
        y = transform.position.y;
    }

    private void Update()
    {
        if (gc.mode == "endless" || gc.mode == "event")
        {
            if (respawnTime > 0f)
            {
                if ((base.transform.position - player.position).magnitude > 60f)
                {
                    respawnTime -= Time.deltaTime;
                }
            }
            else if (!up & !isUp)
            {
                isUp = true;
                Transform transform = base.transform;
                Vector3 position = base.transform.position;
                float x = position.x;
                Vector3 position2 = base.transform.position;
                transform.position = new Vector3(x, y, position2.z);
                up = true;
                audioDevice.Play();
            }
        }
        Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit raycastHit;
        bool IsHovering = !gc.player.IsFlat && Vector3.Distance(player.position, base.transform.position) < openingDistance && Physics.Raycast(ray, out raycastHit) && (raycastHit.transform.tag == "Notebook");
        if (InteractScript.Intracting && ((!MultiplayerManager.isActive && Time.timeScale != 0f) || (MultiplayerManager.isActive && !MultiplayerManager.instance.PauseMovement)))
        {
            if (IsHovering)
            {
                Transform transform2 = base.transform;
                Vector3 position3 = base.transform.position;
                float x2 = position3.x;
                Vector3 position4 = base.transform.position;
                transform2.position = new Vector3(x2, -20f, position4.z);
                respawnTime = 120f;
                up = false;
                isUp = false;
                gc.CollectNotebook();
                Object.Destroy(click);
                if (MultiplayerManager.isActive)
                {
                    if (MultiplayerManager.instance.CurrentSettings.YCTPUsage == 2)
                    {
                        if (notebookType != "NoYCTP" && notebookType != "Note")
                        {
                            learningGame = gc.GetComponent<FogScript>().OrderOfNotebooks[0].GetComponent<NotebookScript>().learningGame;
                            notebookType = "Math";
                        }
                    }
                    // Check if isn't Baldi to send the event over
                    if (!MultiplayerManager.instance.isBaldi)
                    {
                        Multiplayer.ClientSend.SendActionToOtherPlayer("notebook:" + gameObject.name + ":" + gc.notebooks + ":" + (NeedsToUseYCTP() ? "true" : "false"));
                        if (gc.notebooks == 2 && MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer)
                        {
                            if (NeedsToUseYCTP())
                            {
                                MultiplayerManager.instance.gc.baldiScrpt.agent.Warp(new Vector3(5f, 1.64f, 125f));
                                MultiplayerManager.instance.gc.baldiScrpt.ResetTime();
                            }
                            else
                            {
                                MultiplayerManager.instance.gc.baldiScrpt.agent.Warp(new Vector3(5f, 1.64f, 120f));
                                MultiplayerManager.instance.gc.baldiScrpt.timeToMove = 0f;
                            }
                        }
                    }
                }
                if (notebookType == string.Empty)
                {
                    notebookType = "Math";
                }
                if (notebookType == "Math")
                {
                    if (!NeedsToUseYCTP())
                    {
                        if (gc.notebooks >= 2)
                        {
                            gc.GetAngry(gc.mode == "endless" ? Random.Range(0.5f, 1.9f) : (1f + Random.Range(0f, 0.9f)));
                        }
                        gc.DeactivateLearningGame(null);
                    }
                    else
                    {
                        GameObject gameObject = Object.Instantiate(learningGame);
                        gameObject.GetComponent<MathGameScript>().gc = gc;
                        gameObject.GetComponent<MathGameScript>().baldiScript = bsc;
                        gameObject.GetComponent<MathGameScript>().playerPosition = player.position;
                    }
                }
                else if (notebookType == "NoYCTP")
                {
                    gc.DeactivateLearningGame(null);
                }
                else if (notebookType == "NULL")
                {
                    if (NeedsToUseYCTP())
                    {
                        GameObject gameObject = Object.Instantiate(learningGame);
                        gameObject.GetComponent<NullGameScript>().gc = gc;
                    }
                    else
                    {
                        NullManager.Instance.NULL.GetAngry(0.2f);
                        gc.DeactivateLearningGame(null);
                    }
                }
                else if (notebookType == "Note")
                {
                    GameObject gameObject = Object.Instantiate(learningGame);
                    gameObject.GetComponent<NoteYCTPScript>().gc = gc;
                    gameObject.GetComponent<NoteYCTPScript>().questions = questions;
                }
                else
                {
                    if (!NeedsToUseYCTP())
                    {
                        if (gc.notebooks >= 2)
                        {
                            gc.GetAngry(gc.mode == "endless" ? Random.Range(0.5f, 1.9f) : (1f + Random.Range(0f, 0.9f)));
                        }
                        gc.DeactivateLearningGame(null);
                    }
                    else
                    {
                        GameObject gameObject = Object.Instantiate(learningGame);
                        gameObject.GetComponent<OtherYCTPGame>().gc = gc;
                        gameObject.GetComponent<OtherYCTPGame>().baldiScript = bsc;
                        gameObject.GetComponent<OtherYCTPGame>().playerPosition = player.position;
                        gameObject.GetComponent<OtherYCTPGame>().notebookType = notebookType;
                        gameObject.GetComponent<OtherYCTPGame>().hints = hints;
                        gameObject.GetComponent<OtherYCTPGame>().questions = questions;
                        gameObject.GetComponent<OtherYCTPGame>().answers = answers;
                        gameObject.GetComponent<OtherYCTPGame>().impossibleQuestion = impossibleQuestion;
                        gameObject.GetComponent<OtherYCTPGame>().opinion = opinion;
                    }
                }
            }
        }
        if (IsHovering)
        {
            if (click == null)
                click = Object.Instantiate(clickCursor);
        }
        else
        {
            if (click != null)
                Object.Destroy(click);
        }
    }

    private bool NeedsToUseYCTP()
    {
        if (MultiplayerManager.isActive)
        {
            if (MultiplayerManager.instance.CurrentSettings.YCTPUsage > 0)
            {
                return MultiplayerManager.instance.CurrentSettings.YCTPUsage >= 1 && MultiplayerManager.instance.CurrentSettings.YCTPUsage <= 2;
            }
        }
        else if (Settings.GetString("ModeType", "Normal") == "Speedy" || Settings.GetString("ModeType", "Normal") == "Stealthy") return false;
        return !Settings.GetBool("NoYCTP");
    }

    public void SetDown(bool HasCollected = false)
    {
        Transform transform2 = base.transform;
        Vector3 position3 = base.transform.position;
        float x2 = position3.x;
        Vector3 position4 = base.transform.position;
        transform2.position = new Vector3(x2, -20f, position4.z);
        up = false;
        if (HasCollected)
        {
            isUp = false;
        }
    }

    public void SetUp()
    {
        Transform transform = base.transform;
        Vector3 position = base.transform.position;
        float x = position.x;
        Vector3 position2 = base.transform.position;
        transform.position = new Vector3(x, y, position2.z);
        up = true;
    }
}