using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingBusScreen : MonoBehaviour
{
    public TMP_Text progressText;

    public Image image;

    public void LoadScene(string scene)
    {
        StartCoroutine(sceneLoader(SceneManager.LoadSceneAsync(scene)));
    }

    private IEnumerator sceneLoader(AsyncOperation operation)
    {
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            image.fillAmount = 1f - progress;
            progressText.text = Mathf.CeilToInt(progress * 100f) + "%";
            yield return null;
        }
    }
}