using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBeats : MonoBehaviour
{
    public float Chance;

    public AudioSource Music;

    // Start is called before the first frame update
    private void OnEnable()
    {
        if (Random.Range(0f, 1f) <= Chance)
        {
            Music.mute = false;
        }
    }

    private void OnDisable()
    {
        Music.mute = true;
    }
}
