// StartButton
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class StartButton : MonoBehaviour
{
    private Button button;

    public GameObject eventBox;

    public GameObject Transition;

    private void Start()
    {
        if (DateTime.Now.Hour == 3 && GameObject.Find("Baldi Intro"))
        {
            GameObject.Find("Baldi Intro").SetActive(false);
            GameObject.Find("Settings Music").SetActive(false);
        }
    }

    // private void Update()
    // {
    // }

    public void StartGame()
    {
        try
        {
            if (DateTime.Now.Hour == 3)
            {
                SceneManager.LoadScene("THREEAM");
                return;
            }
            Transition.SetActive(true);
        }
        catch (TypeInitializationException ex)
        {
            InputManager.DisplayException(new Exception("We're sorry, but we're unable to access the System.DateTime API. Please check your logs.", ex), 15f);
            throw;
        }
        catch (Exception e)
        {
            InputManager.DisplayException(e, 15f);
            throw;
        }
    }


}