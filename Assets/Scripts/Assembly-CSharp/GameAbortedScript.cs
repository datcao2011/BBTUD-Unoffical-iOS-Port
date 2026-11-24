// NOTE: For an actively-maintained version of this script, see https://github.com/mminer/consolation.
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>
public class GameAbortedScript : MonoBehaviour
{
    public static GameAbortedScript instance;

    public CanvasGroup buttons;

    public static string errorCode = "SCENE_MANUAL_LOAD";

    public static string errorMsg = "The scene has been manually loaded. No problems have been found.";

    public GameObject button;

#if UNITY_WEBGL || UNITY_EDITOR
    public RectTransform rectTransform;
#endif

    public TMP_Text err_text;

    [Header("Remastered")]
    public GameObject Boink;
    public GameObject Music;
    public Sprite RemasteredImage;

    private void Start()
    {
        instance = this;
        InputManager.StopDisplayingMessages();
        StartCoroutine(ErrorLogging());
        Cursor.lockState = CursorLockMode.Confined;
#if UNITY_WEBGL
        GameObject.Find("Quit").SetActive(false);
        rectTransform.anchoredPosition = new Vector2(0f, 39f);
#endif
        if (UnityEngine.Random.Range(0f, 1f) <= 0.5f)
        {
            GameObject.Find("ErrorImage").GetComponent<Image>().sprite = RemasteredImage;
            Boink.SetActive(true);
            Music.SetActive(true);
            GetComponent<AudioSource>().enabled = false;
        }
    }

    public IEnumerator ErrorLogging()
    {
        err_text.text = "ERROR: The application has detected major problems ";
        yield return new WaitForSecondsRealtime(0.05f);
        err_text.text += "and the game's process has been abondoned to prevent fatal errors.\n\n";
        yield return new WaitForSecondsRealtime(0.25f);
        err_text.text += errorCode;
        yield return new WaitForSecondsRealtime(0.16f);
        err_text.text += "\n\n" + errorMsg;
        yield return new WaitForSecondsRealtime(0.35f);
#if UNITY_WEBGL
        err_text.text += "\n\nIf you see this screen several times, try refreshing your browser. If this happens aga";
#else
        err_text.text += "\n\nIf you see this screen several times, try re-downloading the game. If this happens aga";
#endif
        yield return new WaitForSecondsRealtime(0.03f);
        err_text.text += "in, contact SWA about this issue.";
#if !UNITY_WEBGL
        yield return new WaitForSecondsRealtime(2f);
        err_text.text += " This game will automatically cl";
        yield return new WaitForSecondsRealtime(0.03f);
        err_text.text += "ose in 2 minutes.";
        yield return new WaitForSecondsRealtime(120f);
        ApplicationConfigure.closeApplication();
#endif
        yield break;
    }

    private void Update()
    {
        if (FindObjectOfType<ConsoleTraceInGame>().show)
        {
            buttons.interactable = false;
        }
        else
        {
            buttons.interactable = true;
        }
        Cursor.visible = true;
    }


    public void ShowLog()
    {
        FindObjectOfType<ConsoleTraceInGame>().RefreshWindowSize();
        FindObjectOfType<ConsoleTraceInGame>().show = true;
    }

    public void RelaunchApplication()
    {
        if (FindObjectOfType<ConsoleTraceInGame>().show) return;
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameRestarter");
    }

    public void QuitApplication()
    {
        if (FindObjectOfType<ConsoleTraceInGame>().show) return;
        ApplicationConfigure.closeApplication();
    }
}