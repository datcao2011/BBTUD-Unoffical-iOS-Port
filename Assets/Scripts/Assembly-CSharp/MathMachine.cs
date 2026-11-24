using UnityEngine;
using TMPro;
using Multiplayer;

public class MathMachine : MonoBehaviour
{
    private float solution;

    public AudioSource audioDevice;

    public AudioClip wow;

    public NotebookScript notebook;

    public AudioClip incorrect;

    public GameObject balloonPrefab;

    public MathMachineTrigger trigger;

    public GameObject[] objects;

    public TMP_Text[] texts;

    private void Start()
    {
        trigger.mathMachine = this;
        notebook.isUp = true;
        notebook.SetDown();
        objects[0].SetActive(false);
        objects[1].SetActive(false);
        int equation = Mathf.RoundToInt(Random.Range(0f, 1.2f));
        float num1 = Mathf.Round(Random.Range(0f, 9f));
        float num2 = Mathf.Round(Random.Range(0f, 9f));
        if (equation == 0)
        {
            solution = num1 + num2;
            texts[0].text = num1.ToString();
            texts[1].text = "+";
            texts[2].text = num2.ToString();
            texts[3].text = "?";
            lastText = num1 + "|+|" + num2 + "|?";
            // This was occurring the crash.
            // If the solution is over 9, the game should have use a different solution less than 9
            // However, the solution value didn't change in this code block and lead to the game crashing since the solution is still over 9 with no value changed.
            // while (solution > 9f)
            // {
            //     num1 = Mathf.Round(Random.Range(0f, 9f));
            //     num2 = Mathf.Round(Random.Range(0f, 9f));
            //     texts[0].text = num1.ToString();
            //     texts[1].text = "-";
            //     texts[2].text = num2.ToString();
            //     texts[3].text = "?";
            //     lastText = num1 + "|-|" + num2 + "|?";
            // }
            // A solution is to set a problem of 6 + 3 instead to the math machine.
            if (solution > 9f)
            {
                solution = 6f + 3f;
                num1 = 6f;
                num2 = 3f;
                texts[0].text = num1.ToString();
                texts[1].text = "+";
                texts[2].text = num2.ToString();
                texts[3].text = "?";
                lastText = num1 + "|+|" + num2 + "|?";
            }
        }
        if (equation == 1)
        {
            solution = num1 - num2;
            texts[0].text = num1.ToString();
            texts[1].text = "-";
            texts[2].text = num2.ToString();
            texts[3].text = "?";
            lastText = num1 + "|-|" + num2 + "|?";
            // while (solution < 0f)
            // {
            //     num1 = Mathf.Round(Random.Range(0f, 9f));
            //     num2 = Mathf.Round(Random.Range(0f, 9f));
            //     texts[0].text = num1.ToString();
            //     texts[1].text = "-";
            //     texts[2].text = num2.ToString();
            //     texts[3].text = "?";
            //     lastText = num1 + "|-|" + num2 + "|?";
            // }
            if (solution < 0f)
            {
                solution = 9f - 7f;
                num1 = 9f;
                num2 = 7f;
                texts[0].text = num1.ToString();
                texts[1].text = "-";
                texts[2].text = num2.ToString();
                texts[3].text = "?";
                lastText = num1 + "|-|" + num2 + "|?";
            }
        }
    }

    private bool notebookIsPickup;

    public bool solved;

    private string lastText;

    private bool hasReacted;

    private void Update()
    {
        if (!notebookIsPickup & !notebook.up & solved)
        {
            notebookIsPickup = true;
        }
        if (notebook.up & notebookIsPickup)
        {
            MathMachine newComponent = gameObject.AddComponent<MathMachine>();
            newComponent.notebook = notebook;
            newComponent.objects = new GameObject[2]
            {
                objects[0],
                objects[1],
            };
            newComponent.texts = new TMP_Text[4]
            {
                texts[0],
                texts[1],
                texts[2],
                texts[3]
            };
            newComponent.incorrect = incorrect;
            newComponent.wow = wow;
            newComponent.audioDevice = audioDevice;
            newComponent.balloonPrefab = balloonPrefab;
            newComponent.correctMesh = correctMesh;
            newComponent.incorrectMesh = incorrectMesh;
            newComponent.defaultMesh = defaultMesh;
            newComponent.lightUpMesh = lightUpMesh;
            newComponent.renderer2 = renderer2;
            newComponent.trigger = trigger;
            GameObject gameobject = Object.Instantiate(balloonPrefab, balloonPrefab.transform.position, balloonPrefab.transform.rotation);
            foreach (Transform balloon in gameobject.transform)
            {
                balloon.GetComponent<MathBalloon>().machine = newComponent;
            }
            gameobject.transform.parent = transform.parent.parent;
            gameobject.transform.localPosition = new Vector3(0f, 0f, 0f);
            gameobject.transform.localRotation = Quaternion.identity;
            DestroyImmediate(this);
        }
        if (!screensaver)
        {
            if (!notebook.isActiveAndEnabled & !hasReacted)
            {
                hasReacted = true;
                texts[0].text = "E";
                texts[1].text = "r";
                texts[2].text = "r";
                texts[3].text = "?";
                StopScreenSaver();
                StartScreenSaver();
            }
            else if (notebook.isActiveAndEnabled & hasReacted & !screensaver)
            {
                hasReacted = false;
                renderer2.material = defaultMesh;
                string[] spliter = lastText.Split('|');
                texts[0].text = spliter[0];
                texts[1].text = spliter[1];
                texts[2].text = spliter[2];
                texts[3].text = spliter[3];
            }
        }
        // // For Development Purposes (Don't forget to remove this)
        // if (Input.GetKeyDown(KeyCode.F5))
        // {
        //     StartScreenSaver();
        // }
        // if (Input.GetKeyDown(KeyCode.F6))
        // {
        //     StopScreenSaver();
        // }
    }

    public bool screensaver;

    public void AnswerProblem(float answer)
    {
        if (!solved & !hasReacted & !screensaver)
        {
            texts[3].text = "";
            if (answer == solution)
            {
                if (BirthdayBash.Instance == null || !BirthdayBash.Instance.glitchActive)
                    audioDevice.PlayOneShot(wow);
                notebook.SetUp();
                solved = true;
                StartCoroutine(correctDelay());
                if (MultiplayerManager.isActive)
                {
                    ClientSend.SendActionToOtherPlayer("mathmachine:" + ClientHandle.GetIdFromPosition<MathMachine>(transform.position));
                }
                return;
            }
            if (!FindObjectOfType<GameControllerScript>().spoopMode)
            {
                FindObjectOfType<GameControllerScript>().ActivateSpoopMode();
            }
            if (FindObjectOfType<BaldiScript>())
            {
                FindObjectOfType<BaldiScript>().GetAngry(1f);
                FindObjectOfType<BaldiScript>().Hear(transform.position, 5f);
            }
            StartCoroutine(incorrectDelay());
        }
    }

    public void InstantCorrect()
    {
        if (solved) return;
        texts[3].text = "";
        if (BirthdayBash.Instance == null || !BirthdayBash.Instance.glitchActive)
            audioDevice.PlayOneShot(wow);
        notebook.SetUp();
        solved = true;
        StartCoroutine(correctDelay());
    }

    public Material correctMesh;

    public Material defaultMesh;

    public Material incorrectMesh;

    public Material lightUpMesh;

    public MeshRenderer renderer2;

    public void StartScreenSaver()
    {
        screensaver = true;
        StartCoroutine(ScreenSaver());
    }

    public void StopScreenSaver()
    {
        StopAllCoroutines();
        screensaver = false;
        if (notebook.isActiveAndEnabled)
        {
            renderer2.material = defaultMesh;
            string[] spliter = lastText.Split('|');
            texts[0].text = spliter[0];
            texts[1].text = spliter[1];
            texts[2].text = spliter[2];
            texts[3].text = spliter[3];
        }
    }

    private System.Collections.IEnumerator ScreenSaver()
    {
        renderer2.material = lightUpMesh;
        texts[0].text = "M";
        texts[1].text = "A";
        texts[2].text = "T";
        texts[3].text = "H";
        yield return new WaitForSeconds(5f);
        StartCoroutine(ScreenSaver_FlashMesh());
        yield return StartCoroutine(ScreenSaver_AddNumber(UnityEngine.Random.Range(0, 6), UnityEngine.Random.Range(0, 5)));
        StartCoroutine(ScreenSaver_FlashMesh());
        yield return StartCoroutine(ScreenSaver_SubtractNumber(UnityEngine.Random.Range(6, 10), UnityEngine.Random.Range(0, 5)));
        StartCoroutine(ScreenSaver_FlashMesh());
        yield return StartCoroutine(ScreenSaver_AddNumber(UnityEngine.Random.Range(0, 6), UnityEngine.Random.Range(0, 5)));
        StartCoroutine(ScreenSaver_FlashMesh());
        yield return StartCoroutine(ScreenSaver_SubtractNumber(UnityEngine.Random.Range(6, 10), UnityEngine.Random.Range(0, 5)));
        yield return StartCoroutine(ScreenSaver_Final());
        StartCoroutine(ScreenSaver());
        yield break;
    }

    private bool meshToggle;

    private System.Collections.IEnumerator ScreenSaver_FlashMesh()
    {
        if (meshToggle) { renderer2.material = incorrectMesh; meshToggle = !meshToggle; } else { renderer2.material = correctMesh; meshToggle = !meshToggle; }
        yield return new WaitForSeconds(2.4f);
        if (meshToggle) { renderer2.material = incorrectMesh; meshToggle = !meshToggle; } else { renderer2.material = correctMesh; meshToggle = !meshToggle; }
        yield return new WaitForSeconds(1.8f);
        if (meshToggle) { renderer2.material = incorrectMesh; meshToggle = !meshToggle; } else { renderer2.material = correctMesh; meshToggle = !meshToggle; }
        yield return new WaitForSeconds(1.5f);
        if (meshToggle) { renderer2.material = incorrectMesh; meshToggle = !meshToggle; } else { renderer2.material = correctMesh; meshToggle = !meshToggle; }
        yield break;
    }

    private System.Collections.IEnumerator ScreenSaver_Final()
    {
        renderer2.material = defaultMesh;
        string num1 = "M";
        string num2 = "T";
        string answer = "H";
        texts[0].text = "";
        texts[1].text = "";
        texts[2].text = "";
        texts[3].text = "";
        yield return new WaitForSeconds(0.8f);
        texts[0].text = num1;
        yield return new WaitForSeconds(0.8f);
        texts[1].text = "A";
        yield return new WaitForSeconds(0.8f);
        texts[2].text = num2;
        yield return new WaitForSeconds(0.8f);
        texts[3].text = answer;
        renderer2.material = lightUpMesh;
        yield return new WaitForSeconds(0.5f);
        texts[0].text = "";
        texts[1].text = "";
        texts[2].text = "";
        texts[3].text = "";
        renderer2.material = defaultMesh;
        yield return new WaitForSeconds(0.5f);
        texts[0].text = num1;
        texts[1].text = "A";
        texts[2].text = num2;
        texts[3].text = answer;
        renderer2.material = lightUpMesh;
        yield return new WaitForSeconds(0.5f);
        texts[0].text = "";
        texts[1].text = "";
        texts[2].text = "";
        texts[3].text = "";
        renderer2.material = defaultMesh;
        yield return new WaitForSeconds(0.5f);
        texts[0].text = num1;
        texts[1].text = "A";
        texts[2].text = num2;
        texts[3].text = answer;
        renderer2.material = lightUpMesh;
        yield return new WaitForSeconds(10f);
        yield break;
    }

    private System.Collections.IEnumerator ScreenSaver_AddNumber(int num1, int num2)
    {
        int answer = num1 + num2;
        texts[0].text = "";
        texts[1].text = "";
        texts[2].text = "";
        texts[3].text = "";
        yield return new WaitForSeconds(0.8f);
        texts[0].text = num1.ToString();
        yield return new WaitForSeconds(0.8f);
        texts[1].text = "+";
        yield return new WaitForSeconds(0.8f);
        texts[2].text = num2.ToString();
        yield return new WaitForSeconds(0.8f);
        texts[3].text = answer.ToString();
        yield return new WaitForSeconds(0.5f);
        texts[3].text = "";
        yield return new WaitForSeconds(0.5f);
        texts[0].text = num1.ToString();
        texts[1].text = "+";
        texts[2].text = num2.ToString();
        texts[3].text = answer.ToString();
        yield return new WaitForSeconds(0.5f);
        texts[3].text = "";
        yield return new WaitForSeconds(0.5f);
        texts[0].text = num1.ToString();
        texts[1].text = "+";
        texts[2].text = num2.ToString();
        texts[3].text = answer.ToString();
        yield return new WaitForSeconds(0.5f);
        yield break;
    }

    private System.Collections.IEnumerator ScreenSaver_SubtractNumber(int num1, int num2)
    {
        int answer = num1 - num2;
        texts[0].text = "";
        texts[1].text = "";
        texts[2].text = "";
        texts[3].text = "";
        yield return new WaitForSeconds(0.8f);
        texts[0].text = num1.ToString();
        yield return new WaitForSeconds(0.8f);
        texts[1].text = "-";
        yield return new WaitForSeconds(0.8f);
        texts[2].text = num2.ToString();
        yield return new WaitForSeconds(0.8f);
        texts[3].text = answer.ToString();
        yield return new WaitForSeconds(0.5f);
        texts[3].text = "";
        yield return new WaitForSeconds(0.5f);
        texts[0].text = num1.ToString();
        texts[1].text = "-";
        texts[2].text = num2.ToString();
        texts[3].text = answer.ToString();
        yield return new WaitForSeconds(0.5f);
        texts[3].text = "";
        yield return new WaitForSeconds(0.5f);
        texts[0].text = num1.ToString();
        texts[1].text = "-";
        texts[2].text = num2.ToString();
        texts[3].text = answer.ToString();
        yield return new WaitForSeconds(0.5f);
        yield break;
    }

    private System.Collections.IEnumerator incorrectDelay()
    {
        renderer2.material = incorrectMesh;
        if (BirthdayBash.Instance == null || !BirthdayBash.Instance.glitchActive) audioDevice.PlayOneShot(incorrect);
        objects[0].SetActive(false);
        objects[1].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        objects[1].SetActive(false);
        yield return new WaitForSeconds(0.25f);
        objects[1].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        objects[1].SetActive(false);
        yield return new WaitForSeconds(0.25f);
        objects[1].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        objects[1].SetActive(false);
        yield return new WaitForSeconds(0.25f);
        objects[1].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        objects[1].SetActive(false);
        texts[3].text = "?";
        yield break;
    }

    private System.Collections.IEnumerator correctDelay()
    {
        lastText = lastText.Replace("?", solution.ToString());
        renderer2.material = correctMesh;
        objects[0].SetActive(true);
        objects[1].SetActive(false);
        yield return new WaitForSeconds(0.25f);
        objects[0].SetActive(false);
        yield return new WaitForSeconds(0.25f);
        objects[0].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        objects[0].SetActive(false);
        yield return new WaitForSeconds(0.25f);
        objects[0].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        objects[0].SetActive(false);
        yield return new WaitForSeconds(0.25f);
        objects[0].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        objects[0].SetActive(false);
        texts[3].text = solution.ToString();
        yield break;
    }
}