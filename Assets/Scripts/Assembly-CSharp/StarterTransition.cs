using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class StarterTransition : MonoBehaviour
{
    public CanvasGroup BG;
    public CanvasGroup LoadingBG;
    public Slider _Slider;
    public TMP_Text Progress;

    private AsyncOperation Operation;

    public float TransitionTime;

    private void Awake()
    {
        if (Settings.GetBool("SkipUpdatesAndGaps"))
        {
            SceneManager.LoadScene("Starter");
            return;
        }
        Operation = SceneManager.LoadSceneAsync("Starter");
        Operation.allowSceneActivation = false;
        Time.timeScale = 0f;
        StartCoroutine(FadeAllAudioDown());
        StartCoroutine(FadeBGToRed());
    }

    private IEnumerator FadeBGToRed()
    {
        float time = 0f;
        while (time < TransitionTime)
        {
            time += Time.unscaledDeltaTime;
            if (time >= TransitionTime)
            {
                time = TransitionTime;
            }
            BG.alpha = Mathf.InverseLerp(0f, TransitionTime, time);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(0.2f);
        Operation.allowSceneActivation = true;
        yield return new WaitForSecondsRealtime(3f);
        time = 0f;
        UpdateProgress(Mathf.Clamp01(Operation.progress / .9f));
        while (time < TransitionTime)
        {
            time += Time.unscaledDeltaTime;
            UpdateProgress(Mathf.Clamp01(Operation.progress / .9f));
            if (time >= TransitionTime)
            {
                time = TransitionTime;
            }
            LoadingBG.alpha = Mathf.InverseLerp(0f, TransitionTime, time);
            yield return null;
        }
        while (Operation.progress < 0.9f)
        {
            UpdateProgress(Mathf.Clamp01(Operation.progress / .9f));
            yield return null;
        }
        yield break;
    }

    public void UpdateProgress(float progress)
    {
        _Slider.value = progress;
        Progress.text = (progress * 100f).ToString("0") + "%";
    }

    private IEnumerator FadeAllAudioDown()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        float time = TransitionTime + 0.2f;
        while (time > 0f)
        {
            time -= Time.unscaledDeltaTime;
            if (time <= 0f)
            {
                time = 0f;
            }
            foreach (AudioSource source in sources)
            {
                source.volume = Mathf.InverseLerp(0f, TransitionTime, time);
            }
            yield return null;
        }
        yield break;
    }
}