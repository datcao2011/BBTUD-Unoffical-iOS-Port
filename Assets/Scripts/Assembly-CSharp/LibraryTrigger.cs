using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LibraryTrigger : MonoBehaviour
{
    private bool InsideLibrary;
    [SerializeField]
    private AudioMixer MainAudio;
    private float Audio;

    [SerializeField]
    public GameControllerScript GameController;

    // Start is called before the first frame update
    private void Start()
    {
        Audio = 1f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (InsideLibrary)
        {
            if (Audio > 0f)
            {
                Audio -= Time.deltaTime * 2.5f;
                if (Audio <= 0f)
                {
                    Audio = 0f;
                    GameController.CM.ActivateIntensiveAudioSourceChecking = false;
                    if (GameController.CM.isActiveAndEnabled)
                    {
                        GameController.CM.DestroyAllCaptions(false);
                    }
                }
                MainAudio.SetFloat("MasterVolume", Mathf.Log10(Mathf.Lerp(0.0001f, 1f, Audio)) * 20);
            }
        }
        else
        {
            if (Audio < 1f)
            {
                Audio += Time.deltaTime * 2.5f;
                if (Audio > 1f) Audio = 1f;
                MainAudio.SetFloat("MasterVolume", Mathf.Log10(Mathf.Lerp(0.0001f, 1f, Audio)) * 20);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Player")
        {
            InsideLibrary = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            InsideLibrary = false;
            if (!GameController.player.gameOver)
            {
                GameController.CM.ActivateIntensiveAudioSourceChecking = true;
            }
        }
    }

    private void OnDisable()
    {
        Audio = 1f;
        MainAudio.SetFloat("MasterVolume", 0f);
        if (GameController != null && !GameController.player.gameOver && GameController.CM != null)
        {
            GameController.CM.ActivateIntensiveAudioSourceChecking = true;
        }
    }
}
