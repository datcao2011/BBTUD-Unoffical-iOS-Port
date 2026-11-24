// MathGameScript
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Platforms;
using System.Collections;
using TMPro;
using System.Threading;

public class NullGameScript : MonoBehaviour
{
    public GameControllerScript gc;

    public TMP_InputField playerAnswer;

    public TMP_Text questionText;

    public TMP_Text questionText2;

    public TMP_Text questionText3;

    private float endDelay;

    private string[] endlessText = new string[]
    {
        "Okay well let's make this quick.",
        "This won't be fun.",
        "We're doing this the hard way, huh?",
        "Don't you know when to stop?!",
        "STOP",
        "I'm smarter than Baldi by the way.",
        "You can't escape for long.",
        "I'll make you quit by force.",
        "..."
    };

    public bool questionInProgress;

    private void Start()
    {
        gc.ActivateLearningGame();
        if (gc.HudVersion == "2")
        {
            transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        NewProblem();
        endDelay = 3f;
        questionInProgress = false;
    }

    private void Update()
    {
        if ((Input.GetKeyDown("return") || Input.GetKeyDown("enter")))
        {
            CheckAnswer();
        }
        endDelay -= 1f * Time.unscaledDeltaTime;
        if (endDelay <= 0f)
        {
            ExitGame();
        }
    }

    private void NewProblem()
    {
        questionText.text = endlessText[Random.Range(0, endlessText.Length)];
    }

    private string LastTypedText;

    public void InputAnswer(string input)
    {
        LastTypedText = playerAnswer.text;
        if (input == "-" && LastTypedText.Length > 0)
        {
            return;
        }
        if (input == ".")
        {
            string Seperator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (LastTypedText.Contains(Seperator))
            {
                return;
            }
            input = Seperator;
        }
        playerAnswer.text = this.playerAnswer.text + input;
    }

    public void ButtonClear()
    {
        playerAnswer.text = string.Empty;
    }

    public void Backspace()
    {
        if (playerAnswer.text.Length > 0)
            playerAnswer.text = playerAnswer.text.Substring(0, playerAnswer.text.Length - 1);
    }

    private bool sixsixsix;

    public void CheckAnswer()
    {
        if (this.playerAnswer.text == "31718")
        {
            StartCoroutine(CheatText("THIS IS WHERE IT ALL BEGAN"));
            SceneManager.LoadSceneAsync("TestRoom");
            endDelay = 3f;
            return;
        }
        else if (this.playerAnswer.text == "53045009")
        {
            StartCoroutine(CheatText("USE THESE TO STICK TO THE CEILING!"));
            gc.Fliparoo();
            endDelay = 0.1f;
            return;
        }
        else if (playerAnswer.text == "666")
        {
            StartCoroutine(CheatText("Y̸̤̭̓̒̉̕͘͠Ó̵͍̺́͑̀͗Ü̵̡̩̱͎͈̱̻̻̈́̈̍̀̄͊͗͆̃̆́̈́ͅR̴̢̘͊͐̃̎̑̓͊̒͑̿͊̔͝͠ ̸̧̡̡̛̛̼̫̜̬̙̼̥͎̮̑̇̿̒̚̚ͅN̶̹̥͙̠̪̉Ȅ̵͓͚̙͍̝̟̞̞̌͐͑̈́͂̚͜X̸̡͍̿̀̐͆͊͐̉̈̽̿͆̂͜T̶̛͉̮̩͍̫̃͛̓́̅̾́̉͊͘̕͝͝!̸́͋̅̈́̓̈́͜͠"));
            sixsixsix = true;
            endDelay = 0.1f;
            return;
        }
        ExitGame();
    }

    // Token: 0x060000A4 RID: 164 RVA: 0x000061FC File Offset: 0x000045FC
    public void ExitGame()
    {
        NullManager.Instance.NULL.GetAngry(0.2f);
        if (sixsixsix)
        {
            NullManager.Instance.NULL.transform.position = gc.playerTransform.position;
        }
        this.gc.DeactivateLearningGame(base.gameObject);
    }

    private IEnumerator CheatText(string text)
    {
        for (; ; )
        {
            this.questionText.text = text;
            this.questionText2.text = string.Empty;
            this.questionText3.text = string.Empty;
            yield return new WaitForEndOfFrame();
        }
    }
}
