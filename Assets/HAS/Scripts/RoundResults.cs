using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Events;


public class RoundResults : MonoBehaviour
{
    public static string textToInput;
    public static bool playerWin;
    public static bool roundMaxed;
    public TMP_Text screenText;
    public Animator anim;
    public GameObject WinScreen;
    public GameObject LoseScreen;

    public TMP_Text PleaseWait;
    public GameObject NextRoundButtons;
    public GameObject ContinueButton;

    private void Start()
    {
        CursorControllerScript.UnlockCursor();
        PlayerPrefs.DeleteKey("HAS_HOS");
        anim.SetBool("o", true);
        screenText.text = textToInput;
        if (roundMaxed)
        {
            NextRoundButtons.SetActive(false);
            ContinueButton.SetActive(true);
        }
    }

    public void MainMenu()
    {
        anim.SetBool("o", false);
        StartCoroutine(DelayFunction(() =>
        {
            StartCoroutine(LoadScene("Starter"));
            PleaseWait.gameObject.SetActive(true);
        }));
    }

    public void NextRound()
    {
        anim.SetBool("o", false);
        StartCoroutine(DelayFunction(() =>
        {
            StartCoroutine(LoadScene(Settings.GetString("ModeSelected", "School"))); // The scene (New or Old?)
            PleaseWait.gameObject.SetActive(true);
        }));
    }

    public void Continue()
    {
        anim.SetBool("o", false);
        StartCoroutine(DelayFunction(() =>
        {
            WinScreen.SetActive(playerWin);
            LoseScreen.SetActive(!playerWin);
        }));
    }

    public IEnumerator DelayFunction(UnityAction action)
    {
        yield return new WaitForSeconds(2f);
        action.Invoke();
        yield break;
    }

    public IEnumerator LoadScene(string sceneName)
    {
        PleaseWait.text = "Please Wait";
        if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(1f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        if (!Settings.GetBool("SkipUpdatesAndGaps")) operation.allowSceneActivation = false;
        float progress = Mathf.Clamp01(operation.progress / .9f);
        PleaseWait.text = "Loading (" + progress + "%)";
        while (progress < 1f)
        {
            progress = Mathf.Clamp01(operation.progress / .9f);
            PleaseWait.text = "Loading (" + Mathf.CeilToInt(progress * 100f) + "%)";
            yield return null;
        }
        if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(0.07f);
        PleaseWait.text = SceneLoader.GetRandomText();
        if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(0.08f);
        operation.allowSceneActivation = true;
        yield break;
    }
}
