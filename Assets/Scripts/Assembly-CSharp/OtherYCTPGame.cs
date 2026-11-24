using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Platforms;
using Multiplayer;

public class OtherYCTPGame : MonoBehaviour
{
    public GameControllerScript gc;

    public BaldiScript baldiScript;

    public Vector3 playerPosition;

    public RawImage[] results = new RawImage[3];

    public Texture correct;

    public Texture incorrect;

    public TMP_InputField playerAnswer;

    public TMP_Text questionText;

    public TMP_Text questionText2;

    public TMP_Text questionText3;

    public Animator baldiFeed;

    public Transform baldiFeedTransform;

    public AudioClip bal_howto;

    public AudioClip bal_screech;

    public AudioClip[] bal_praises = new AudioClip[5];

    public AudioClip[] bal_problems = new AudioClip[3];

    public AudioClip me_lookout;

    private float endDelay;

    public TMP_Text hintTMPText;

    private int problem;

    private int audioInQueue;

    private float num1;

    private float num2;

    private float num3;

    private int sign;

    private float solution;

    public string notebookType;

    private string[] endlessText = new string[2]
    {
        "READY FOR ANOTHER CHALLANGE?",
        "WOWIE! YOU GOT AN ANOTHER NOTEBOOK."
    };

    public string impossibleQuestion;

    public string[] hints = new string[10];

    public bool questionInProgress;

    public string[] answers = new string[10];

    public string[] questions = new string[10];

    public bool[] opinion = new bool[10];

    public string TheFrustratedGamer_BBTUDVideoAt3Min39Sec;
    public string TheFrustratedGamer_BBTUDVideoAt3Min39SecB;
    public string TheFrustratedGamer_BBTUDVideoAt12Min16Sec;
    public string DontDebugThis_DoTheCode;
    public string DontDebugThis_DoTheCode2;

    private string[] tips = new string[]
    {
        "There are openable lockers in this game, and one of them has an apple :)",
        "Dr. Reflex's Hammer is the only item that has infinite uses.",
        "There is an area of vending machines in the second and third floor.",
        "You can save your progress anytime, if you're in a rush.",
        "Going to a different floor can throw characters off.",
        "Never play this at 3 AM. It's really scary. This is your warning.",
        "Do not lock Baldi from that one room with the red item.",
        "Principal can catch you for only 30 seconds before he gives up.",
        "You can always drop your items in order to save space.",
        "Are subjects are too hard? Disable them in settings.",
        "Captions can help you figure out where the characters at.",
        "Distance from Baldi: {DISTANCE}"
    };

    private bool impossibleMode;

    private bool mode42;

    private int problemsWrong;

    //private bool BaldiInvisible;

    private AudioClip[] audioQueue = new AudioClip[20];

    public AudioSource baldiAudio;

    private string fourty2;
    
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
        // English question update
        if (notebookType == "English")
        {
            answers[1] = VersionInput.currentVersion.ToString();
        }
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

    private void NewProblem()
    {
        if ((notebooks == 2 & impossibleMode & this.playerAnswer.text.ToUpper() == TheFrustratedGamer_BBTUDVideoAt3Min39Sec.ToUpper())
        || (notebooks == 2 & impossibleMode & this.playerAnswer.text.ToUpper() == TheFrustratedGamer_BBTUDVideoAt3Min39SecB.ToUpper()))
        {
            StartCoroutine(CheatText(DontDebugThis_DoTheCode));
        }
        if (notebookType.ToUpper() == "SCIENCE" & impossibleMode & this.playerAnswer.text.ToUpper() == TheFrustratedGamer_BBTUDVideoAt12Min16Sec)
        {
            StartCoroutine(CheatText(DontDebugThis_DoTheCode2));
        }
        playerAnswer.text = string.Empty;
        problem++;
        gc.SetLearnMusicStage(problem);
        if (problem <= 3)
        {
            QueueAudio(bal_problems[problem - 1]);
            if (problemsWrong > 0 || (gc.mode == "story" && (problem <= 2 || notebooks <= 1)) || (gc.mode == "endless" && (problem <= 2 || notebooks != 2)) || gc.mode == "event")////problem 2, notebooks 1
            {
                sign = Mathf.RoundToInt(Random.Range(0f, questions.Length - 1));;
                if (sign == 9 && notebookType == "Geography")
                {
                    questionText.fontSizeMin = 8f;
                    hintTMPText.fontSizeMin = 4.9f;
                }
                questionText.text = "SOLVE " + notebookType.ToUpper() + " Q" + problem + ": \n" + questions[sign];
                hintTMPText.isRightToLeftText = false;
                hintTMPText.text = "<u>HINT:</u>\n" + hints[sign];
            }
            else
            {
                impossibleMode = true;
                QueueAudio(gc.GetRandomBuzz(true));
                questionText.text = "SOLVE " + notebookType.ToUpper() + " Q" + problem + ": \n" + impossibleQuestion + " " + RandomCharacters() + "?";
                questionText2.text = "SOLVE " + notebookType.ToUpper() + " Q" + problem + ": \n" + impossibleQuestion + " " + RandomCharacters() + "?";
                questionText3.text = "SOLVE " + notebookType.ToUpper() + " Q" + problem + ": \n" + impossibleQuestion + " " + RandomCharacters() + "?";
                hintTMPText.text = "<u>HINT:</u>\nEROOR WHILE LOADING HINT";
                hintTMPText.isRightToLeftText = true;
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
                questionText.text = "YOU PASSED " + notebookType.ToUpper() + ". I AM SO PROUD OF YOU!";
                if (notebooks >= 2)
                {
                    questionText2.text = "CAN YOU READ THIS, PLAYER?";
                    questionText3.text = "I BET YOU CAN'T READ THIS PROPERLY!";
                }
            }
            else if (gc.mode == "story" & problemsWrong <= 0)
            {
                questionText.text = "YOU PASSED " + notebookType.ToUpper() + " BUT I'M STILL MAD!";
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
                questionText.text = "YOU FAILED " + notebookType.ToUpper() + "! WHY?";
                questionText2.text = string.Empty;
                questionText3.text = string.Empty;
                gc.failedNotebooks++;
            }
            else
            {
                int num2 = Mathf.RoundToInt(Random.Range(0f, 1f));
                string[] hintText = {
                   "READY OR NOT, HERE I COME!",
                   "DO YOU KNOW " + notebookType.ToUpper() + "?"
                };
                questionText.text = hintText[num2];
                questionText2.text = string.Empty;
                questionText3.text = string.Empty;
            }
            int num3 = Mathf.RoundToInt(Random.Range(0f, tips.Length - 1f));
            float distance;
            if (!gc.spoopMode)
            {
                distance = (gc.tutorBaldi.transform.position - gc.playerTransform.position).magnitude;
            }
            else
            {
                distance = (gc.baldiTransform.position - gc.playerTransform.position).magnitude;
            }
            hintTMPText.isRightToLeftText = false;
            hintTMPText.text = "<u>TIP:</u>\n" + tips[num3].Replace("{DISTANCE}", distance.ToString());
        }
    }

    private string RandomCharacters()
    {
        string[] Alphabet = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        return Alphabet[(Random.Range(0, 26))] + Alphabet[(Random.Range(0, 26))] + Alphabet[(Random.Range(0, 26))] + Alphabet[(Random.Range(0, 26))] + Alphabet[(Random.Range(0, 26))];
    }

    private bool sixsixsix;

    public void CheckAnswer()
    {
        if (this.playerAnswer.text == "31718")
        {
            StartCoroutine(CheatText("THIS IS WHERE IT ALL BEGAN"));
            if (!MultiplayerManager.isActive) SceneManager.LoadSceneAsync("TestRoom");
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
            if ((((!opinion[sign] && this.playerAnswer.text.ToUpper().Trim() == answers[sign].ToUpper()) || (opinion[sign] && playerAnswer.text != string.Empty)) && !impossibleMode) || (fourty2 == "true" & this.playerAnswer.text == "42")
            || (notebooks == 2 & impossibleMode & this.playerAnswer.text.ToUpper() == TheFrustratedGamer_BBTUDVideoAt3Min39Sec.ToUpper()) || (notebooks == 2 & impossibleMode & this.playerAnswer.text.ToUpper() == TheFrustratedGamer_BBTUDVideoAt3Min39SecB.ToUpper())
            || (notebookType.ToUpper() == "SCIENCE" & impossibleMode & this.playerAnswer.text.ToUpper() == TheFrustratedGamer_BBTUDVideoAt12Min16Sec))
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
        if (this.problemsWrong <= 0 & gc.spoopMode)
        {
            this.baldiScript.GetAngry(-1f);
            if (MultiplayerManager.isActive)
            {
                gc.quarter.SetActive(true);
                ClientSend.SendActionToOtherPlayer("quarter");
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

    public void ButtonClear()
    {
        playerAnswer.text = string.Empty;
    }

    public void Backspace()
    {
        if (playerAnswer.text.Length > 0)
            playerAnswer.text = playerAnswer.text.Substring(0, playerAnswer.text.Length - 1);
    }

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
