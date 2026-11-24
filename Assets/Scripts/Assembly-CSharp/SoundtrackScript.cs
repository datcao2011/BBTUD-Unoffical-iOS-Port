using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundtrackScript : MonoBehaviour
{
    private int onSong;

    public Image imageSong;

    public AudioClip[] song;

    public Sprite[] songSprite;

    public string[] titleTexts;

    public string[] authorTexts;

    public TMP_Text text;

    public TMP_Text text1;

    public AudioSource audioSource;

    public Slider timeSlider;

    public TMP_Text timeText;

    private float timeSeconds;

    private float timeMinutes;


    private void Start()
    {
        onSong = 0;
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            timeSlider.value = audioSource.time;
        }
        timeMinutes = Mathf.Floor(audioSource.time / 60);
        timeSeconds = (float)Mathf.FloorToInt(audioSource.time % 60);
        string result;
        string minutes;
        string seconds;
        float currentMinutes = timeMinutes;
        float currentSeconds = timeSeconds;
        if (currentSeconds > 59f)
        {
            currentSeconds = 0f;
            currentMinutes += 1f;
        }
        minutes = currentMinutes.ToString();
        if (currentSeconds < 10f)
        {
            seconds = "0" + currentSeconds;
        }
        else
        {
            seconds = currentSeconds.ToString();
        }
        result = minutes + ":" + seconds;
        timeText.text = result;
        currentMinutes = Mathf.Floor(song[onSong].length / 60);
        currentSeconds = (float)Mathf.FloorToInt(song[onSong].length % 60);
        if (currentSeconds > 59f)
        {
            currentSeconds = 0f;
            currentMinutes += 1f;
        }
        minutes = currentMinutes.ToString();
        if (currentSeconds < 10f)
        {
            seconds = "0" + currentSeconds;
        }
        else
        {
            seconds = currentSeconds.ToString();
        }
        result = minutes + ":" + seconds;
        timeText.text += "/" + result;
    }

    private void OnEnable()
    {
        PlayCurrentSong();
    }

    public void GoForward()
    {
        settingTime = true;
        if (onSong >= 5)
            onSong = 0;
        else
            onSong++;
        PlayCurrentSong();
    }

    public void GoBackward()
    {
        settingTime = true;
        if (onSong <= 0)
            onSong = 5;
        else
            onSong--;
        PlayCurrentSong();
    }

    private void PlayCurrentSong()
    {
        audioSource.Stop();
        audioSource.time = 0f;
        text.text = titleTexts[onSong];
        text1.text = authorTexts[onSong];
        imageSong.sprite = songSprite[onSong];
        audioSource.clip = song[onSong];
        timeSlider.maxValue = song[onSong].length;
        timeSlider.value = 0f;
        audioSource.Play();
        settingTime = false;
    }

    public void TogglePlayOrPause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public void SetPlayTime(float time)
    {
        if (Input.GetMouseButton(0) & !settingTime)
            audioSource.time = time;
    }

    private bool settingTime;

}