using System.Collections.Generic;
using UnityEngine;

public class AudioSourceDelay : MonoBehaviour
{
    public float delay;

    public AudioClip clipLengthDelay;

    public AudioClip clip;

    public AudioSource toPlay;

    private void Awake()
    {
        if (clipLengthDelay != null)
        {
            delay = clipLengthDelay.length;
        }
        toPlay.clip = clip;
#if UNITY_WEBGL
        StartCoroutine(AudioDelay());
#else
        toPlay.PlayDelayed(delay);
#endif
    }

#if UNITY_WEBGL
    private System.Collections.IEnumerator AudioDelay()
    {
        while (delay > 0f)
        {
            delay -= Time.unscaledDeltaTime;
            yield return null;
        }
        toPlay.Play();
        yield break;
    }
#endif
}
