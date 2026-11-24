// MathGameScript
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Platforms;
using System.Collections;
using TMPro;
using System.Threading;

public class MathGameScript : MonoBehaviour
{
    public GameControllerScript gc;

    public BaldiScript baldiScript;

    public Vector3 playerPosition;

    public GameObject mathGame;

    public RawImage[] results = new RawImage[3];

    public Texture correct;

    public Texture incorrect;

    public TMP_InputField playerAnswer;

    public TMP_Text questionText;

    public TMP_Text questionText2;

    public TMP_Text questionText3;

    public Animator baldiFeed;

    public Transform baldiFeedTransform;

    public AudioClip bal_plus;

    public AudioClip bal_minus;

    public AudioClip bal_times;

    public AudioClip bal_divided;

    public AudioClip bal_equals;

    public AudioClip bal_howto;

    public AudioClip bal_intro;

    public AudioClip bal_screech;

    public AudioClip[] bal_numbers = new AudioClip[10];

    public AudioClip[] bal_praises = new AudioClip[5];

    public AudioClip[] bal_problems = new AudioClip[3];

    public AudioClip me_lookout;

    private float endDelay;

    private int problem;

    private int audioInQueue;

    private float num1;

    public GameObject clickObject;

    private float num2;

    private float num3;

    private int sign;

    public float solution;

    private string[] hintText = new string[2]
    {
        "READY OR NOT, HERE I COME!",
        "DO YOU KNOW MATH?"
    };


    private string[] endlessText = new string[2]
    {
        "READY FOR ANOTHER CHALLANGE?",
        "WOWIE! YOU GOT AN ANOTHER NOTEBOOK."
    };

    public bool questionInProgress;

    private bool impossibleMode;

    private bool mode42; // Why this is still in the code after 5 years lol

    private int problemsWrong;

    public string fourty2;

    //private bool BaldiInvisible;

    private AudioClip[] audioQueue = new AudioClip[20];

    public AudioSource baldiAudio;

    private int notebooks;

    private void Start()
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi)
        {
            MultiplayerManager.instance.PauseMovement = true;
        }
        if (gc.CM != null && gc.CM.isActiveAndEnabled)
        {
            gc.CM.DestroyAllCaptions2D();
        }
        baldiAudio.pitch = Settings.GetFloat("TimeSpeed");
        gc.ActivateLearningGame();
        if (gc.HudVersion == "2")
        {
            transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        notebooks = gc.notebooks; // This is cached for Multiplayer.
        if (notebooks <= 2)
        {
            QueueAudio(bal_intro);
        }
        if (notebooks == 1)
        {
            QueueAudio(bal_howto);
        }
        NewProblem();
        questionInProgress = false;
        if (gc.spoopMode)
        {
            baldiFeedTransform.position = new Vector3(-1000f, -1000f, 0f);
            //BaldiInvisible = true;
        }
        questionInProgress = true;
    }

    private bool IsParseable()
    {
        if (playerAnswer.text == string.Empty) return false;
        float i = 0;
        return float.TryParse(playerAnswer.text, out i);
    }

    private bool RightAnswer()
    {
        if (!IsParseable()) return false;
        string i = float.Parse(playerAnswer.text).ToString("0.000");
        playerAnswer.text = i;
        return i == solution.ToString("0.000");
    }

    private void Update()
    {
        fourty2 = Settings.GetString("FourtyTwoMode");
        if (!baldiAudio.isPlaying)
        {
            if (audioInQueue > 0 & !gc.spoopMode)
            {
                PlayQueue();
            }
            baldiFeed.SetBool("talking", false);
        }
        else
        {
            baldiFeed.SetBool("talking", true);
        }
        if ((Input.GetKeyDown("return") || Input.GetKeyDown("enter")) & questionInProgress)
        {
            questionInProgress = false;
            CheckAnswer();
        }
        if (problem > 3)
        {
            endDelay -= 1f * Time.unscaledDeltaTime;
            if (MultiplayerManager.isActive)
            {
                if (gc.player.rb.velocity.magnitude > 0.8f)
                {
                    endDelay = 0f;
                }
            }
            if (endDelay <= 0f)
            {
                ExitGame();
            }
        }
    }

    public string DontDebugThis_DoTheCode;

    private void NewProblem()
    {
        if (notebooks == 2 & impossibleMode & this.playerAnswer.text == SB737IDefeatedBaldiVideo_At37Seconds)
        {
            StartCoroutine(CheatText(DontDebugThis_DoTheCode));
        }
        playerAnswer.text = string.Empty;
        problem++;
        gc.SetLearnMusicStage(problem);
        if (problem <= 3)
        {
            QueueAudio(bal_problems[problem - 1]);
            if (problemsWrong > 0 || (gc.mode == "story" & (problem <= 2 || notebooks <= 1)) || (gc.mode == "endless" & (problem <= 2 || notebooks != 2)) || gc.mode == "event")////problem 2, notebooks 1
            {
                num1 = (float)Mathf.RoundToInt(Random.Range(0f, 9f));
                this.num2 = (float)Mathf.RoundToInt(Random.Range(0f, 9f));
                sign = Mathf.RoundToInt(Random.Range(0f, 3f));
                if (gc.mode == "event" && sign == 3)
                {
                    sign = 0;
                }
                QueueAudio(bal_numbers[Mathf.RoundToInt(num1)]);
                if (sign == 0)
                {
                    solution = num1 + this.num2;
                    questionText.text = "SOLVE MATH Q" + problem + ": \n \n" + num1 + "+" + this.num2 + "=";
                    QueueAudio(bal_plus);
                }
                else if (sign == 1)
                {
                    solution = num1 - this.num2;

                    questionText.text = "SOLVE MATH Q" + problem + ": \n \n" + num1 + "-" + this.num2 + "=";
                    QueueAudio(bal_minus);
                }
                else if (sign == 2)
                {
                    solution = num1 * this.num2;
                    questionText.text = "SOLVE MATH Q" + problem + ": \n \n" + num1 + "x" + this.num2 + "=";
                    QueueAudio(bal_times);
                }
                else if (sign == 3)
                {
                    if (notebooks <= 2)
                    {
                        num2 = GetSimpleDivisionProblem();
                    }
                    else if (this.num2 == 0f)
                    {
                        console.warn(num1 + " divided by 0 is not possible so changed the value to 1.");
                        this.num2 = 1f;
                    }
                    solution = num1 / this.num2;
                    questionText.text = "SOLVE MATH Q" + problem + ": \n \n" + num1 + "÷" + this.num2 + "=";
                    QueueAudio(bal_divided);
                }
                QueueAudio(bal_numbers[Mathf.RoundToInt(this.num2)]);
                QueueAudio(bal_equals);
            }
            else
            {
                impossibleMode = true;
                num1 = Random.Range(1f, 9999f);
                this.num2 = Random.Range(1f, 9999f);
                num3 = Random.Range(1f, 9999f);
                sign = 0;//Mathf.RoundToInt(Random.Range(0f, 1f));
                QueueAudio(gc.GetRandomBuzz(true));
                if (sign == 0)
                {
                    solution = num1 + num2 * num3;
                    questionText.text = "SOLVE MATH Q" + problem + ": \n" + num1 + "+(" + this.num2 + "x" + num3 + "=";
                    QueueAudio(bal_plus);
                    QueueAudio(gc.GetRandomBuzz(true));
                    QueueAudio(bal_times);
                    QueueAudio(gc.GetRandomBuzz(true));
                }
                else if (sign == 1)
                {
                    solution = num1 + num2 / num3;
                    questionText.text = "SOLVE MATH Q" + problem + ": \n" + num1 + "+(" + this.num2 + "÷" + num3 + "=";
                    QueueAudio(bal_divided);
                    QueueAudio(gc.GetRandomBuzz(true));
                    QueueAudio(bal_plus);
                    QueueAudio(gc.GetRandomBuzz(true));
                }
                num1 = Random.Range(1f, 9999f);
                this.num2 = Random.Range(1f, 9999f);
                num3 = Random.Range(1f, 9999f);
                if (sign == 0)
                {
                    questionText2.text = "SOLVE MATH Q" + problem + ": \n" + num1 + "+(" + this.num2 + "x" + num3 + "=";
                }
                else if (sign == 1)
                {
                    questionText2.text = "SOLVE MATH Q" + problem + ": \n (" + num1 + "÷" + this.num2 + ")+" + num3 + "=";
                }
                num1 = Random.Range(1f, 9999f);
                this.num2 = Random.Range(1f, 9999f);
                num3 = Random.Range(1f, 9999f);
                if (sign == 0)
                {
                    questionText3.text = "SOLVE MATH Q" + problem + ": \n" + num1 + "+(" + this.num2 + "x" + num3 + "=";
                }
                else if (sign == 1)
                {
                    questionText3.text = "SOLVE MATH Q" + problem + ": \n (" + num1 + "÷" + this.num2 + ")+" + num3 + "=";
                }
                QueueAudio(bal_equals);
            }
            if (!InputManager.usingTouch)
                playerAnswer.ActivateInputField();
            questionInProgress = true;
        }
        else
        {
            if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi)
            {
                MultiplayerManager.instance.PauseMovement = false;
            }
            endDelay = 5f;
            if (!gc.spoopMode)
            {
                questionText.text = "YOU PASSED MATH. I AM SO PROUD OF YOU.";
                if (notebooks >= 3)
                {
                    questionText2.text = "CAN YOU READ THIS, PLAYER?";
                    questionText3.text = "I BET YOU CAN'T READ THIS PROPERLY!";
                }
            }
            else if (gc.mode == "story" & problemsWrong <= 0)
            {
                questionText.text = "YOU PASSED MATH BUT I'M STILL MAD!";
                questionText2.text = string.Empty;
                questionText3.text = string.Empty;
            }
            else if (gc.mode == "endless" & problemsWrong <= 0)
            {
                int num2 = Mathf.RoundToInt(Random.Range(0f, 1f));
                questionText.text = endlessText[num2];
                questionText2.text = string.Empty;
                questionText3.text = string.Empty;
            }
            else if (gc.mode == "story" & problemsWrong >= 3)
            {
                questionText.text = "YOU FAILED MATH! WHY?";
                questionText2.text = string.Empty;
                questionText3.text = string.Empty;
                gc.failedNotebooks++;
            }
            else
            {
                int num2 = Mathf.RoundToInt(Random.Range(0f, 1f));
                questionText.text = (BirthdayBash.Instance == null || !BirthdayBash.Instance.glitchActive) ? hintText[num2] : "Object reference not set to an instance of an object.";
                questionText2.text = string.Empty;
                questionText3.text = string.Empty;
            }
        }
    }

    private int GetSimpleDivisionProblem()
    {
        switch (num1)
        {
            case 0:
                return 1;
            case 4:
                return 2;
            case 6:
                return (Mathf.RoundToInt(Random.Range(0f, 1f)) == 1) ? 3 : 2;
            case 8:
                return (Mathf.RoundToInt(Random.Range(0f, 1f)) == 1) ? 4 : 2;
            case 9:
                return 3;
        }
        return (Mathf.RoundToInt(Random.Range(0f, 1f)) == 1) ? 1 : (int)num1;
    }

    private bool sixsixsix;

    public string SB737IDefeatedBaldiVideo_At37Seconds;

    public void CheckAnswer()
    {
        if (this.playerAnswer.text == "31718")
        {
            StartCoroutine(CheatText("THIS IS WHERE IT ALL BEGAN"));
            SceneManager.LoadSceneAsync("TestRoom");
        }
        else if (this.playerAnswer.text == "53045009")
        {
            StartCoroutine(CheatText("USE THESE TO STICK TO THE CEILING!"));
            gc.Fliparoo();
        }
        else if (playerAnswer.text == "666")
        {
            StartCoroutine(CheatText("Y̸̤̭̓̒̉̕͘͠Ó̵͍̺́͑̀͗Ü̵̡̩̱͎͈̱̻̻̈́̈̍̀̄͊͗͆̃̆́̈́ͅR̴̢̘͊͐̃̎̑̓͊̒͑̿͊̔͝͠ ̸̧̡̡̛̛̼̫̜̬̙̼̥͎̮̑̇̿̒̚̚ͅN̶̹̥͙̠̪̉Ȅ̵͓͚̙͍̝̟̞̞̌͐͑̈́͂̚͜X̸̡͍̿̀̐͆͊͐̉̈̽̿͆̂͜T̶̛͉̮̩͍̫̃͛̓́̅̾́̉͊͘̕͝͝!̸́͋̅̈́̓̈́͜͠"));
            sixsixsix = true;
            baldiFeedTransform.position = new Vector3(-1000f, -1000f, -1000f);
            problem = 3;
        }
        if (problem <= 3)
        {
            if (playerAnswer.text.StartsWith(".") || playerAnswer.text.StartsWith(","))
            {
                playerAnswer.text = "0" + playerAnswer.text;
            }
            if (!IsParseable() && playerAnswer.text != string.Empty)
            {
                questionInProgress = true;
                return;
            }

            if (RightAnswer() || (fourty2 == "true" && this.playerAnswer.text.Replace(",", ".") == "42.000") || (notebooks == 2 && impossibleMode && this.playerAnswer.text.Replace(",", ".") == SB737IDefeatedBaldiVideo_At37Seconds))
            {
                // Check for anger. We can't get away with the player answering the other 2 questions wrong on purpose.
                if (this.problemsWrong > 0 && gc.spoopMode && problem == 3)
                {
                    baldiScript.GetTempAngry(-0.25f);
                    baldiScript.GetAngry(1f);
                }
                this.results[this.problem - 1].texture = this.correct;
                this.baldiAudio.Stop();
                if (gc.CM != null && gc.CM.isActiveAndEnabled)
                {
                    gc.CM.DestroyAllCaptions2D();
                }
                this.ClearAudioQueue();
                int num = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 4f));
                this.QueueAudio(this.bal_praises[num]);
                this.NewProblem();
            }
            else
            {

                this.problemsWrong++;
                this.results[this.problem - 1].texture = this.incorrect;
                if (BirthdayBash.Instance != null) BirthdayBash.Instance.GlitchBook();
                if (!this.gc.spoopMode)
                {
                    this.baldiFeed.SetTrigger("angry");
                    if (gc.CM != null && gc.CM.isActiveAndEnabled)
                    {
                        gc.CM.DestroyAllCaptions2D();
                    }
                    this.gc.ActivateSpoopMode();
                    if (playerAnswer.text == "666")
                    {
                        if (gc.CM != null && gc.CM.isActiveAndEnabled)
                        {
                            gc.CM.DestroyAllCaptions2D();
                        }
                        this.gc.audioDevice.Stop();
                        gc.audioDevice.PlayOneShotWC(gc.aud_Hang);
                        gc.audioDevice.PlayOneShotWC(me_lookout);
                    }
                }
                if (this.gc.mode == "story")
                {
                    if (this.problem == 3)
                    {
                        this.baldiScript.GetAngry(1f);
                    }
                    else
                    {
                        if (gc.spoopMode)
                            this.baldiScript.GetTempAngry(0.25f);
                        else
                            baldiScript.GetAngry(1f);
                    }
                }
                else
                {
                    this.baldiScript.GetAngry(1f);
                }
                this.ClearAudioQueue();
                this.baldiAudio.Stop();
                this.NewProblem();
            }

        }
    }

    // Token: 0x060000A0 RID: 160 RVA: 0x00006170 File Offset: 0x00004570
    private void QueueAudio(AudioClip sound)
    {
        this.audioQueue[this.audioInQueue] = sound;
        this.audioInQueue++;
    }

    // Token: 0x060000A1 RID: 161 RVA: 0x0000618E File Offset: 0x0000458E
    private void PlayQueue()
    {
        this.baldiAudio.PlayOneShotWC(this.audioQueue[0]);
        this.UnqueueAudio();
    }

    // Token: 0x060000A2 RID: 162 RVA: 0x000061AC File Offset: 0x000045AC
    private void UnqueueAudio()
    {
        for (int i = 1; i < this.audioInQueue; i++)
        {
            this.audioQueue[i - 1] = this.audioQueue[i];
        }
        this.audioInQueue--;
    }

    // Token: 0x060000A3 RID: 163 RVA: 0x000061F0 File Offset: 0x000045F0
    private void ClearAudioQueue()
    {
        this.audioInQueue = 0;
    }

    // Token: 0x060000A4 RID: 164 RVA: 0x000061FC File Offset: 0x000045FC
    private void ExitGame()
    {
        if (this.problemsWrong <= 0 && gc.spoopMode)
        {
            this.baldiScript.GetAngry(-1f);
            if (MultiplayerManager.isActive && notebooks <= 1) // It has to be one notebook.
            {
                gc.quarter.SetActive(true);
                Multiplayer.ClientSend.SendActionToOtherPlayer("quarter");
            }
            // baldiScript.SetPriority(80 + Random.Range(20.9f, 40f) * 5f - 10f, Random.Range(60f, 90f));
            // baldiScript.Hear(gc.playerTransform.position, 3f);
            // baldiScript.SetLocation(gc.playerTransform.position - gc.playerTransform.forward * 100f + gc.playerTransform.right * 100f);
        }
        if (problemsWrong >= 3)
        {
            baldiScript.Hear(gc.playerTransform.position, 7f);
        }
        if (sixsixsix)
        {
            baldiScript.SpoopyJumpscare();
        }
        this.gc.DeactivateLearningGame(base.gameObject);
    }

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

    public string LastTypedText;

    public void ButtonOK()
    {
        if (questionInProgress)
        {
            questionInProgress = false;
            CheckAnswer();
        }
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
