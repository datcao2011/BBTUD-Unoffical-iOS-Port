using UnityEngine;

public class KickstarterSoundtrack : MonoBehaviour
{
    public AudioClip[] music;

    public AudioClip previousMusic;
    public AudioClip currentMusic;
    public AudioClip nextMusic;

    private AudioSource audioDevice;
    [SerializeField]
    private bool playOnAwake;

    private void Awake()
    {
        audioDevice = gameObject.AddComponent<AudioSource>();
        audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.AMusic : null;
        audioDevice.gameObject.AddComponent<AudioSourceSpeed>();
        if (playOnAwake)
        {
            StartMusic();
        }
    }

    public void StartMusic()
    {
        currentMusic = music[Mathf.RoundToInt(Random.Range(0f, music.Length - 1f))];
        nextMusic = music[Mathf.RoundToInt(Random.Range(0f, music.Length - 1f))];
        audioDevice.clip = currentMusic;
        audioDevice.Play();
    }

    private void Update()
    {
        if (!audioDevice.isPlaying)
        {
            PlayNext();
        }
    }

    public void PlayNext()
    {
        audioDevice.clip = nextMusic;
        audioDevice.Play();
        previousMusic = currentMusic;
        currentMusic = nextMusic;
        nextMusic = music[Mathf.RoundToInt(Random.Range(0f, music.Length - 1f))];
    }
}