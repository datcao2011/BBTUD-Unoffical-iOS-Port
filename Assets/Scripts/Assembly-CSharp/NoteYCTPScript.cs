using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Platforms;

public class NoteYCTPScript : MonoBehaviour
{
    public GameControllerScript gc;

    public TMP_Text questionText;

    public TMP_Text hintTMPText;

    private int problem;

    public string[] questions = new string[10];

    private NullEndingScript NullEnding;

    private void Start()
    {
        gc.ActivateLearningGame();
        if (gc.HudVersion == "2")
        {
            transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        NullEnding = FindObjectOfType<NullEndingScript>();
#if UNITY_ANDROID
        if (NullEnding)
        {
            if (NullScript.EasyMode)
            {
                questions = new string[1] { "Congrats, I guess..." };
            }
        }
#endif
        NewProblem();
    }

    private void NewProblem()
    {
        if (problem >= questions.Length) { ExitGame(); return; }
        questionText.text = questions[problem];
        hintTMPText.text = "...";
        problem++;
    }

    // Token: 0x060000A4 RID: 164 RVA: 0x000061FC File Offset: 0x000045FC
    private void ExitGame()
    {
        this.gc.DeactivateLearningGame(base.gameObject);
        if (NullEnding)
        {
#if UNITY_ANDROID
            if (NullScript.EasyMode)
            {
                if (AchievementManager.Instance != null)
                {
                    AchievementManager.Instance.Earn(Achievements.undefined);
                }
                SceneManager.LoadScene("Starter");
            }
            else
            {
                NullEnding.InitEnding();
            }
#else
            NullEnding.InitEnding();
#endif
        }
    }

    public void ButtonOK()
    {
        NewProblem();
    }
}