using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaldiTV : MonoBehaviour
{
    [SerializeField]
    private Animator Anim;

    [SerializeField]
    private RectTransform Base;

    public bool IsBroadcasting;

    public bool IsPlaying;

    public AudioSource Audio;

    [SerializeField]
    private AudioClip Static;

    // Start is called before the first frame update
    private void Start()
    {
        if (!IsBroadcasting)
        {
            Base.anchoredPosition = new Vector2(64.29501f, 64.29525f);
            Anim.SetInteger("anim", 3);
        }
        if (Settings.GetInt("TVHud") == 2)
        {
            gameObject.SetActive(false);
        }
        if (NullManager.Instance != null)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsBroadcasting && IsPlaying && !Audio.isPlaying)
        {
            IsPlaying = false;
            IsBroadcasting = false;
            StartCoroutine(AscendAndStatic());
        }
    }

    public void DescendTV()
    {
        if (IsBroadcasting) return;
        Anim.SetInteger("anim", 3);
        StartCoroutine(Descend());
    }

    public void DescendWithAlert(bool Instant)
    {
        if (IsBroadcasting) return;
        Anim.SetInteger("anim", 1);
        if (Instant)
        {
            IsBroadcasting = true;
            Base.anchoredPosition = new Vector2(64.29501f, -64.29525f);
        }
        else
            StartCoroutine(Descend());
    }

    public void PlayAudioBeforeAscend(AudioClip clip, float Volume = 1f, float DelayStatic = 0.25f)
    {
        IsPlaying = false;
        Audio.clip = Static;
        Audio.loop = true;
        Audio.Play();
        Anim.SetInteger("anim", 0);
        StartCoroutine(AudioStaticDelay(clip, Volume, DelayStatic));
    }

    private IEnumerator AudioStaticDelay(AudioClip ClipToPlay, float Volume, float Delay)
    {
        while (Delay > 0f)
        {
            Delay -= Time.deltaTime;
            yield return null;
        }
        Anim.SetInteger("anim", 2);
        if (Audio.isPlaying) Audio.Stop();
        Audio.clip = null;
        Audio.PlayOneShot(ClipToPlay, Volume);
        IsPlaying = true;
        CaptionsManager CM = FindObjectOfType<CaptionsManager>();
        if (CM)
        {
            CM.CreateCustomCaption(Audio, ClipToPlay.name, ClipToPlay.length, 0.25f);
        }
        yield break;
    }

    private IEnumerator Descend()
    {
        IsBroadcasting = true;
        float Value = 64.29525f;
        while (Value > -64.29525f)
        {
            Value -= Time.deltaTime * 64.29525f * 1f; // Time.deltaTime + The Position to go to + The speed of a second.
            if (Value < -64.29525f) { Value = -64.29525f; }
            Base.anchoredPosition = new Vector2(64.29501f, Value);
            yield return null;
        }
        Base.anchoredPosition = new Vector2(64.29501f, -64.29525f);
        yield break;
    }

    private IEnumerator AscendAndStatic()
    {
        Audio.clip = Static;
        Audio.loop = true;
        Audio.Play();
        Anim.SetInteger("anim", 0);
        yield return new WaitForSeconds(0.25f);
        Audio.Stop();
        Anim.SetInteger("anim", 3);
        StartCoroutine(Ascend());
    }

    private IEnumerator Ascend()
    {
        IsBroadcasting = false;
        float Value = -64.29525f;
        while (Value < 64.29525f)
        {
            Value += Time.deltaTime * 64.29525f * 1f; // Time.deltaTime + The Position to go to + The speed of a second.
            if (Value > 64.29525f) { Value = 64.29525f; }
            Base.anchoredPosition = new Vector2(64.29501f, Value);
            yield return null;
        }
        Base.anchoredPosition = new Vector2(64.29501f, 64.29525f);
        yield break;
    }
}
