using UnityEngine;
using System.Collections;

public class AudioTest : MonoBehaviour
{
    // public AudioManager audMan;

    public AudioClip[] testNum = new AudioClip[10];

    public AudioClip countdown;

    private float timer;

    private void Start()
    {
        StartCoroutine(soundTest());
    }

    private void Update()
    {
        if (timer > 0f)
        {
            if (Time.deltaTime == 0f)
                timer -= Time.unscaledDeltaTime;
            else
                timer -= Time.deltaTime;
        }
    }

    private IEnumerator soundTest()
    {
        // timer = 10f;
        // audMan.PlaySound(countdown);
        // while (timer > 0f)
        // {
        //     yield return null;
        // }
        // audMan.PlayMusic(testNum[0], 0.2f, true);
        yield break;
    }
}