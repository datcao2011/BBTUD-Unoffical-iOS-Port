using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Platforms;

public class PauseScript : MonoBehaviour
{

    public GameControllerScript gc;

    public GameObject mainText;

    public GameObject quitText;

    public GameObject restart;

    public GameObject PauseBG;

    public GameObject[] buttonSelect = new GameObject[4];

    private bool quitAYSshown;

    public bool restartActive;

    private EventSystem eventSystem;

    private void Start()
    {
        if (gc == null)
        {
            gc = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        }
        if (MultiplayerManager.isActive && GameObject.Find("RestartButton"))
        {
            GameObject.Find("QuitButton").GetComponent<RectTransform>().localPosition = GameObject.Find("RestartButton").GetComponent<RectTransform>().localPosition;
            GameObject.Find("RestartButton").SetActive(false);
        }
    }

    private void OnEnable()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) & !quitAYSshown & !restartActive & !SeeOptionsExists())
        {
            resume();
        }
        if (Input.GetKeyDown(KeyCode.Q) & !quitAYSshown & !restartActive & !SeeOptionsExists())
        {
            quitPause();
        }
        if (Input.GetKeyDown(KeyCode.O) & !quitAYSshown & !restartActive & !SeeOptionsExists())
        {
            showOptions();
        }
        if (Input.GetKeyDown(KeyCode.Y) & quitAYSshown & !restartActive)
        {
            quitGame();
        }
        else if (Input.GetKeyDown(KeyCode.N) & quitAYSshown & !restartActive)
        {
            notQuitGame();
        }
        if (Input.GetKeyDown(KeyCode.E) & !quitAYSshown & !restartActive & !SeeOptionsExists())
        {
            showRestartAYS();
        }
        if (Input.GetKeyDown(KeyCode.Y) & !quitAYSshown & restartActive)
        {
            restartGame();
        }
        else if (Input.GetKeyDown(KeyCode.N) & !quitAYSshown & restartActive)
        {
            hideRestartAYS();
        }
        // if (Input.GetKeyDown(KeyCode.Escape) & SeeOptionsExists())
        // {

        // }
    }

    public void resume()
    {
        gc.UnpauseGame();
    }

    public void quitPause()
    {
        mainText.SetActive(false);
        quitText.SetActive(true);
        quitAYSshown = true;
    }
    public void quitGame()
    {
        if (NullScript.Attempts > 0)
        {
            NullScript.Attempts--;
        }
        SceneManager.LoadScene("Starter");
    }
    public void notQuitGame()
    {
        mainText.SetActive(true);
        quitText.SetActive(false);
        quitAYSshown = false;
    }
    public void showOptions()
    {
        SceneManager.LoadSceneAsync("OptionsMenu", LoadSceneMode.Additive);
    }
    public bool SeeOptionsExists()
    {
        return SceneManager.sceneCount >= 2;
    }
    public void showRestartAYS()
    {
        if (MultiplayerManager.isActive)
        {
            InputManager.DisplayEvent("This action is disabled on multiplayer.", 5f);
            return;
        }
        restartActive = true;
        restart.SetActive(true);
        mainText.SetActive(false);
    }
    public void hideRestartAYS()
    {
        mainText.SetActive(true);
        restartActive = false;
        restart.SetActive(false);
    }
    public void restartGame()
    {
        if (MultiplayerManager.isActive)
        {
            InputManager.DisplayEvent("You can not restart while in a multiplayer game.", 5f);
            return;
        }
        gc.restartga();
        PauseBG.SetActive(false);
        restart.SetActive(false);
    }
}
