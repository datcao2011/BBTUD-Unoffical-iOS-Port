// Script
using UnityEngine;

public class Script : MonoBehaviour
{
    public AudioSource audioDevice;

    public AudioClip SecondaryAudioNoise;

    private bool played;

    private void Start()
    {
    }

    private void Update()
    {
        if (!audioDevice.isPlaying & played)
        {
            if (gameObject.name == "Filename2" && FindObjectOfType<BB_SpoopBalloon>())
            {
                Settings.SetInt("NULL_FirstTime", 1);
            }
            ApplicationConfigure.closeApplication();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player" & !played)
        {
            if (SecondaryAudioNoise != null)
            {
                if (GameObject.Find("Hall_NewRoom") && GameObject.Find("Hall_NewRoom").GetComponentInChildren<SwingingDoorScript>() && GameObject.Find("Hall_NewRoom").GetComponentInChildren<SwingingDoorScript>().coinDoor)
                {
                    audioDevice.clip = SecondaryAudioNoise;
                }
                if (gameObject.name == "Filename2" && FindObjectOfType<BB_SpoopBalloon>())
                {
                    audioDevice.clip = SecondaryAudioNoise;
                }
            }
            audioDevice.Play();
            played = true;
        }
    }
}