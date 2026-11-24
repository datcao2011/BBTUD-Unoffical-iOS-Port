using UnityEngine;
using System.Collections;

public class AudioWhenActive : MonoBehaviour
{
    public GameObject objectToActivate;

    public AudioClip toPlay;

    public Sprite normal;

    public Sprite slappedMode;

    private void Awake()
    {
        objectToActivate.SetActive(false);
    }

    private bool isPlayed;

    private void Update()
    {
        if (objectToActivate.activeSelf & !isPlayed)
        {
            isPlayed = true;
            objectToActivate.SetActive(false);
            GetComponent<AudioSource>().PlayOneShot(toPlay);
            RulerSlap();
        }
        else if (!objectToActivate.activeSelf)
        {
            isPlayed = false;
        }
    }

    public void RulerSlap()
    {
        base.StopAllCoroutines();
        base.StartCoroutine(slap());
    }

    private IEnumerator slap()
    {
        GetComponent<SpriteRenderer>().sprite = normal;
        yield return new WaitForSecondsRealtime(0.05f);
        GetComponent<SpriteRenderer>().sprite = slappedMode;
        yield return new WaitForSecondsRealtime(0.5f);
        GetComponent<SpriteRenderer>().sprite = normal;
        yield break;
    }
}
