using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class AudioSourceLoop : MonoBehaviour
{
    private AudioSource audioSource;

    public float LoopStart;
    public float LoopEnd;

    private void Start()
    {
        audioSource = base.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (LoopEnd > 0f && audioSource.isPlaying && audioSource.time >= LoopEnd)
        {
            audioSource.time = audioSource.time - LoopEnd + LoopStart;
        }
    }
}
