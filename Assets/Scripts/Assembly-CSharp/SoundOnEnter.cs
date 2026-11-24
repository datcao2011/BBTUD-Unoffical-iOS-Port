using UnityEngine;

public class SoundOnEnter : MonoBehaviour
{
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}