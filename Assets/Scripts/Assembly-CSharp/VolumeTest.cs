using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class VolumeTest : MonoBehaviour
{

    public AudioClip audioToPlay;

    private AudioSource audioPlayer;

    private Button buttonToPress;
    public AudioMixerGroup AudioGroup;

    private void Start()
    {
        audioPlayer = gameObject.AddComponent<AudioSource>();
        gameObject.AddComponent<AudioSourceSpeed>();
        audioPlayer.clip = audioToPlay;
        audioPlayer.playOnAwake = false;
        buttonToPress = GetComponent<Button>();
        buttonToPress.onClick.AddListener(TestAudio);
        audioPlayer.outputAudioMixerGroup = AudioGroup;
    }

    private void Update()
    {
        buttonToPress.interactable = !audioPlayer.isPlaying;
        buttonToPress.GetComponentInChildren<TMP_Text>().text = audioPlayer.isPlaying ? "Playing audio." : "Test Audio";
    }

    public void TestAudio()
    {
        audioPlayer.Play();
    }
}
