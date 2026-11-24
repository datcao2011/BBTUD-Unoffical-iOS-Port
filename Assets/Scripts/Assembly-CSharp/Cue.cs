using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Cue : MonoBehaviour
{
    public bool IsAnimation;
    public bool IsAudio;

    public AudioSource aud;
    public Animator Anim;
    public string Trigger;

    [Header("Message Cue")]
    public bool IsMessage;
    public string Message;
    public float Duration = 5f;

    [Header("Set Pitch")]
    public bool SetPitch;
    public float Pitch;
    public CutscenePrizeScript Prize;

    private void Awake()
    {
        if (aud == null) aud = GetComponent<AudioSource>();
        if (IsAudio) aud.Play();
        if (IsAnimation) Anim.SetTrigger(Trigger);
        if (SetPitch) Prize.Magnitude = Pitch - 1f;
    }

    private void OnEnable()
    {
        if (IsMessage)
        {
            HudScreenTextScript Text = FindObjectOfType<HudScreenTextScript>();
            Text.ShowHudText(Duration, Regex.Unescape(Message));
            Text.hudText.alignment = TMPro.TextAlignmentOptions.Top;
        }
    }
}