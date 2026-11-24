using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class AudioIntroAndLoop : MonoBehaviour
{
    public AudioClip Loop;
    private AudioSource audioDevice;

    private void Start()
    {
        audioDevice = base.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!audioDevice.isPlaying || audioDevice.time >= (audioDevice.clip.length - (Time.unscaledDeltaTime * 2)))
        {
            audioDevice.clip = Loop;
            audioDevice.loop = true;
            audioDevice.Play();
            Destroy(this); // Yeah, this is no longer needed anymore.
        }
    }
}