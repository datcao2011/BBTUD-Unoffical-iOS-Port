using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JohnnyStore : MonoBehaviour
{
    public AudioSource JohnnyVoice;
    public Animator JohnnyAnimator;
    public AudioClip[] NoOffers;
    public AudioClip[] Phrases;
    public AudioClip[] Thanks;
     public AudioClip[] Purchased;
    public AudioClip[] AboutToPurchase;

    public bool HasOffers;

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        JohnnyAnimator.SetBool("frown", !HasOffers);
        JohnnyAnimator.SetBool("talk", JohnnyVoice.isPlaying);
    }

    public void OnHover()
    {
        if (!HasOffers)
        {
            PlayAudio(GetRandom(NoOffers));
        }
        else
        {
            PlayAudio(GetRandom(Phrases));
        }
    }

    public void IsLeaving()
    {
        if (HasOffers)
        {
            PlayAudio(GetRandom(Thanks));
        }
    }

    public void ItemPhrase()
    {
        PlayAudio(GetRandom(AboutToPurchase));
    }

    public void ItemPurchase()
    {
        PlayAudio(GetRandom(Purchased));
    }


    private AudioClip GetRandom(AudioClip[] Clip)
    {
        return Clip[Mathf.RoundToInt(Random.Range(0f, Clip.Length - 1))];
    }

    private void PlayAudio(AudioClip VoiceClip)
    {
        JohnnyVoice.Stop();
        JohnnyVoice.PlayOneShot(VoiceClip);
    }
}
