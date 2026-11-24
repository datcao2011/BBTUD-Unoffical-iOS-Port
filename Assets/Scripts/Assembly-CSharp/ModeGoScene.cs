using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ModeGoScene : MonoBehaviour
{

    public float delay;

    private bool startDelay;

    private bool delayDone;

    private string scene;

    public Scene sceneLoader;

    public Slider slider;

    public TMP_Text textProgress;

    public SceneLoader loader;

    private void Start()
    {
        delay = 1.5f;
        if (System.DateTime.Now.Hour == 3 && GameObject.Find("Baldi Intro"))
        {
            GameObject.Find("Baldi Intro").SetActive(false);
            GameObject.Find("Settings Music").SetActive(false);
        }
    }

    private void Update()
    {
        if (startDelay)
        {
            delay -= Time.deltaTime;
            if (System.DateTime.Now.Hour == 3)
                SceneManager.LoadScene("THREEAM");
        }
        if (delay <= 0f & !delayDone)
        {
            delayDone = true;
            loader.LoadSceneAsync(scene);
        }
    }

    public void GoCampingScene()
    {
        scene = "BusCamping";
        startDelay = true;
    }

    public void GoFarmingScene()
    {
        scene = "FarmCamping";
        startDelay = true;
    }

    private IEnumerator LoadAsyncData()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progress;
            float progressText = progress * 100f;
            textProgress.text = progressText.ToString("0") + "%";
            yield return null;
        }
        textProgress.text = "Done!";
        yield break;
    }

}
