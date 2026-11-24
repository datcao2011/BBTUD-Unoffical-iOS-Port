using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Platforms;

public class threeAMJumpscare : MonoBehaviour
{
    public AudioSource audioDevice;

    public bool test;

    public static bool PlacefaceJumpscare;

    public bool B;

    public GameObject G;

    private float Duration;

    private void Awake()
    {
        if (PlacefaceJumpscare && !B) { G.SetActive(true); transform.parent.gameObject.SetActive(false); return; }
    }

    private void Start()
    {
        if (PlacefaceJumpscare && !B) { return; }
        if (B)
        {
            int sysHour2 = DateTime.Now.Hour;
            if (sysHour2 != 3 & !test)
            {
                SceneManager.LoadScene("MainMenu");
                return;
            }
            AudioListener.volume = 2f;
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            if (CurrentDevice.currentPlatform == PFPlatform.Windows)
            {
                CurrentDevice.VibrateDeviceAndController(1f, 50f, 50f);
            }
#endif
            return;
        }
        int sysHour = DateTime.Now.Hour;
        if (sysHour != 3 & !test)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }
        AudioListener.volume = 2f;
        audioDevice.Play();
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (CurrentDevice.currentPlatform == PFPlatform.Windows)
        {
            CurrentDevice.VibrateDeviceAndController(1f, 50f, 50f);
        }
#endif
    }

    private void Update()
    {
        if (B)
        {
            Duration += Time.deltaTime;
            if (Duration >= 2.5f)
            {
                ApplicationConfigure.closeApplication();
                PlacefaceJumpscare = false;
            }
            return;
        }
        int sysHour = DateTime.Now.Hour;
        if (!audioDevice.isPlaying & (sysHour == 3 || test))
        {
            ApplicationConfigure.closeApplication();
        }
    }
}