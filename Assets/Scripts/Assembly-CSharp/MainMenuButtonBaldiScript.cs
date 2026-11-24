using UnityEngine;

public class MainMenuButtonBaldiScript : MonoBehaviour
{
    public AudioClip jumpscareAudio;

    private AudioSource audioSource;

    private int playingTimes;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playingTimes = Settings.GetInt("PlayedTimes");
        if (playingTimes < 1)
        {
            Settings.SetInt("PlayedTimes", playingTimes + 1);
            Settings.SetFloat("AudioVolume", 1f);
            audioSource.clip = jumpscareAudio;
            audioSource.Play();
        }
    }
}