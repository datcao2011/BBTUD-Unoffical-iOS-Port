// ExitButtonScript
using UnityEngine;
using Platforms;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExitButtonScript : MonoBehaviour
{
    public CursorControllerScript cursorController;

    private Button button;

    private bool exited;
    private bool exitedTwice;

    public ApplicationConfigure app;

    public AudioClip audio2;

    private void Start()
    {
        cursorController.UnlockMouseNonStatic();
        button = base.GetComponent<Button>();
        button.onClick.AddListener(ExitGame);
        if (CurrentDevice.currentPlatform == PFPlatform.XboxOne || CurrentDevice.currentPlatform == PFPlatform.WebGL)
        {
            base.gameObject.SetActive(false);
        }
    }

    private void ExitGame()
    {
        if (exited) { if (!exitedTwice) { exitedTwice = true; ApplicationConfigure.closeApplication(); } return; }
        exited = true;
        StartCoroutine(exitDelay());
    }

    private IEnumerator exitDelay()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Credits");
        operation.allowSceneActivation = false;
        AudioSource audioToPlay = new GameObject("Thanks for playing!").AddComponent<AudioSource>();
        audioToPlay.clip = audio2;
        audioToPlay.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ACharacters : null;
        audioToPlay.Play();
        yield return new WaitForSecondsRealtime(audio2.length);
        operation.allowSceneActivation = true;
        yield break;
    }
}