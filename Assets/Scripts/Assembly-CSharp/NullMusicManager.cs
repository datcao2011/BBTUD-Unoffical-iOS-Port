using UnityEngine;
using UnityEngine.Audio;
using MidiPlayerTK;
using System.Collections.Generic;
using System.Threading;

public class NullMusicManager : MonoBehaviour
{
    public AudioClip IntroStage2;

    public AudioClip DrumLoop;
    public AudioClip MainLoop;

    public AudioSource IntroSource;
    public AudioSource LoopSource;
    public AudioSource DrumSource;

    public AudioMixer Mixer;
    [SerializeField]
    private MidiFilePlayer MIDI;

    public float DrumOnlyDuration = 0f;
    private bool HasStarted = false;
    private bool IsStarting;
    private float TimeToLoop = 0f;

    public float Speed;

    [Header("Rhythm Music Attributes")]
    public float CurrentRhythmTime;
    public float CurrentRhythmSpeed = 1f;
    private float GameSpeed = 1f;
    public NullRotationManager SchoolRhythm;
    public GameControllerScript GameManager;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            MIDI.MPTK_CorePlayer = false;
        }
        MIDI.MPTK_Load();
        MIDI.OnEventNotesMidi.AddListener(OnBeat);
        GameSpeed = Settings.GetFloat("TimeSpeed", 1f);
        Settings.OnSettingChanged += OnSettingChanged;
    }

    private void OnSettingChanged(string key, object value)
    {
        if (key == "TimeSpeed")
        {
            GameSpeed = (float)value;
        }
    }

    private void OnDestroy()
    {
        Settings.OnSettingChanged -= OnSettingChanged;
    }

    private void Update()
    {
        if (HasStarted)
        {
            if (DrumOnlyDuration > 0f)
            {
                DrumOnlyDuration -= Time.deltaTime;
                if (DrumOnlyDuration <= 0f)
                {
                    LoopSource.mute = false;
                    DrumSource.mute = true;
                }
            }
            CurrentRhythmTime += Time.unscaledDeltaTime * CurrentRhythmSpeed * GameSpeed;
            if (CurrentRhythmTime >= 110.169f)
            {
                CurrentRhythmTime -= 110.169f;
                if (MIDI.MPTK_IsPlaying)
                {
                    MIDI.MPTK_Position = CurrentRhythmTime;
                }
                else
                {
                    MIDI.MPTK_RePlay();
                }
            }
        }
        // Debug
        if (IsStarting)
        {
            if (TimeToLoop > 0f)
            {
                TimeToLoop -= Time.unscaledDeltaTime;
            }
            else
            {
                IsStarting = false;
                LoopTime();
            }
        }
    }

    private float LastBeat;

    public bool DisableOnBeatText;

    private void OnBeat(List<MPTKEvent> Beats)
    {
        if (Time.timeScale != 0f)
        {
            bool HasBeat = false;
            foreach (MPTKEvent Beat in Beats)
            {
                if (Beat.Meta == MPTKMeta.TextEvent) // Drum event
                {
                    float Current = Mathf.Clamp((Beat.RealTime - LastBeat) / 1000, 0f, 99f);
                    float B = 1f - Current;
                    bool IsNegative = SchoolRhythm.NegativeVal == -1f;
                    SchoolRhythm.NegativeVal = IsNegative ? 1f : -1f;
                    SchoolRhythm.Intensity = (B - 0.24f) / CurrentRhythmSpeed;
                    SchoolRhythm.Multiplier = 0.12f / B * CurrentRhythmSpeed;
                    LastBeat = Beat.RealTime;
                    UpdateHudElements(0);
                }
                if (!HasBeat)
                {
                    if (NullManager.Instance.health == 1)
                    {
                        if (Beat.Channel == 1)
                        {
                            HasBeat = true;
                            UpdateHudElements(1);
                        }
                    }
                    else if (NullManager.Instance.health > 2 && NullManager.Instance.health <= 7)
                    {
                        if (Beat.Channel == 13)
                        {
                            HasBeat = true;
                            UpdateHudElements(1);
                        }
                    }
                }
                if (Beat.Channel == 2)
                {
                    UpdateHudElements(2);
                }
            }
        }
    }

    private bool B;
    private bool C;

    private void UpdateHudElements(int type)
    {
        if (DisableOnBeatText) return;
        switch (type)
        {
            default:
                GameManager.notebookCount.text = Trim(LastBeat * 10) + "/" + Trim(LastBeat * 100) + " Notebooks";
                if (NullManager.Instance.health > 5 || NullManager.Instance.health == 1)
                {
                    if (B)
                    {
                        GameManager.warning.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, 0f);
                        GameManager.warning.GetComponent<RectTransform>().anchoredPosition += new Vector2(109.4f, 0f);
                    }
                    else
                    {
                        GameManager.warning.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 180f, 0f);
                        GameManager.warning.GetComponent<RectTransform>().anchoredPosition += new Vector2(-109.4f, 0f);
                    }
                    B = !B;

                    if (NullManager.Instance.health <= 8)
                    {
                        GameManager.notebookCount.isRightToLeftText = !GameManager.notebookCount.isRightToLeftText;
                    }
                    if (NullManager.Instance.health == 1)
                    {
                        GameManager.systemTime.timeText.isRightToLeftText = !GameManager.systemTime.timeText.isRightToLeftText;
                    }
                }
                else
                {
                    if (B)
                    {
                        GameManager.warning.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, 0f);
                        GameManager.warning.GetComponent<RectTransform>().anchoredPosition += new Vector2(109.4f, 0f);
                    }
                    B = false;
                }
                break;
            case 1:
                GameManager.exitCount.isRightToLeftText = !GameManager.exitCount.isRightToLeftText;
                break;
            case 2:
                if (NullManager.Instance.health == 1)
                {
                    if (!GameManager.player.heldObject)
                    {
                        GameManager.itemText.text = "QUIT GAME";
                        GameManager.warning.text = "GET OUT!";
                    }
                    else
                    {
                        if (C)
                        {
                            GameManager.itemText.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, 0f);
                            GameManager.itemText.GetComponent<RectTransform>().anchoredPosition += new Vector2(109.4f, 0f);
                        }
                        C = false;
                        GameManager.itemText.isRightToLeftText = false;
                        return;
                    }
                    if (C)
                    {
                        GameManager.itemText.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, 0f);
                        GameManager.itemText.GetComponent<RectTransform>().anchoredPosition += new Vector2(109.4f, 0f);
                    }
                    else
                    {
                        GameManager.itemText.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 180f, 0f);
                        GameManager.itemText.GetComponent<RectTransform>().anchoredPosition += new Vector2(-109.4f, 0f);
                    }
                    C = !C;
                }
                else if (NullManager.Instance.health <= 7)
                {
                    GameManager.warning.text = Vector3.Distance(GameManager.playerTransform.position, NullManager.Instance.NULL.position).ToString("0") + "m";
                }
                break;
        }
    }

    private string Trim(float Number)
    {
        string str = Number.ToString("0");
        return str.Substring(str.Length - 1);
    }

    public void InitiateStart()
    {
        IntroSource.clip = IntroStage2;
        IntroSource.loop = false;
        IntroSource.Play();
        Destroy(IntroSource.gameObject, IntroStage2.length);
        // Load Music loops     
        IsStarting = true;
        TimeToLoop = 7.868f / Settings.GetFloat("TimeSpeed", 1f);
        // Prepare the music to play
        if (DrumLoop.loadState == AudioDataLoadState.Unloaded) DrumLoop.LoadAudioData();
        if (MainLoop.loadState == AudioDataLoadState.Unloaded) MainLoop.LoadAudioData();

        GameManager.OverrideStaminaDesign = true;
    }

    private void LoopTime()
    {
        LoopSource.clip = MainLoop;
        LoopSource.loop = true;
        LoopSource.mute = false;
        LoopSource.Play();
        DrumSource.clip = DrumLoop;
        DrumSource.loop = true;
        DrumSource.mute = true;
        DrumSource.Play();
        HasStarted = true;
        MIDI.MPTK_Play(true);
        MIDI.MPTK_Speed = GameSpeed;
        // Pre-load the speed up audios now
        if (Pace2.loadState == AudioDataLoadState.Unloaded) Pace2.LoadAudioData();
        if (Pace3.loadState == AudioDataLoadState.Unloaded) Pace3.LoadAudioData();
        if (PaceHang.loadState == AudioDataLoadState.Unloaded) PaceHang.LoadAudioData();
        if (Pace2_Pause.loadState == AudioDataLoadState.Unloaded) Pace2_Pause.LoadAudioData();
        if (Pace3_Pause.loadState == AudioDataLoadState.Unloaded) Pace3_Pause.LoadAudioData();
        if (PaceHang_Pause.loadState == AudioDataLoadState.Unloaded) PaceHang_Pause.LoadAudioData();
    }

    public void Drums(float Duration)
    {
        if (!HasStarted) return;
        DrumOnlyDuration = Duration;
        LoopSource.mute = Duration > 0f;
        DrumSource.mute = Duration <= 0f;
    }

    private int CurrentIncrease;

    [Header("Sped Up Music")]
    public AudioClip Pace2;
    public AudioClip Pace2_Pause;
    public AudioClip Pace3;
    public AudioClip Pace3_Pause;
    public AudioClip PaceHang;
    public AudioClip PaceHang_Pause;

    public void IncreaseSpeed()
    {
        CurrentIncrease++;
        if (CurrentIncrease == 1)
        {
            float CurrentTime = LoopSource.time;
            LoopSource.clip = Pace2;
            DrumSource.clip = Pace2_Pause;
            CurrentRhythmSpeed = MainLoop.length / Pace2.length;
            MIDI.MPTK_Speed = CurrentRhythmSpeed * GameSpeed;
            LoopSource.Play();
            DrumSource.Play();
            // NOTE: WebGL has an issue of instantly setting time when changing audio clips, and possibly cause a crash. Not worth the risk!
#if !UNITY_WEBGL
            LoopSource.time = (float)System.Math.Truncate(CurrentTime / CurrentRhythmSpeed * 1000) / 1000f;
            DrumSource.time = (float)System.Math.Truncate(CurrentTime / CurrentRhythmSpeed * 1000) / 1000f;
#else
            MIDI.MPTK_RePlay();
            CurrentRhythmTime = 0f;
#endif
        }
        if (CurrentIncrease == 2)
        {
            float CurrentTime = LoopSource.time;
            LoopSource.clip = Pace3;
            DrumSource.clip = Pace3_Pause;
            CurrentRhythmSpeed = MainLoop.length / Pace3.length;
            MIDI.MPTK_Speed = CurrentRhythmSpeed * GameSpeed;
            LoopSource.Play();
            DrumSource.Play();

#if !UNITY_WEBGL
            LoopSource.time = (float)System.Math.Truncate(CurrentTime / (Pace2.length / Pace3.length) * 1000) / 1000f;
            DrumSource.time = (float)System.Math.Truncate(CurrentTime / (Pace2.length / Pace3.length) * 1000) / 1000f;
#else
            MIDI.MPTK_RePlay();
            CurrentRhythmTime = 0f;
#endif
        }
        if (CurrentIncrease == 3)
        {
            float CurrentTime = LoopSource.time;
            LoopSource.clip = PaceHang;
            DrumSource.clip = PaceHang_Pause;
            CurrentRhythmSpeed = MainLoop.length / PaceHang.length;
            MIDI.MPTK_Speed = CurrentRhythmSpeed * GameSpeed;
            LoopSource.Play();
            DrumSource.Play();

#if !UNITY_WEBGL
            // Android devices have an issue of MIDI not looping sometimes. We'll make sure to replay it so we can still have that final shake effect.
            if (MIDI.MPTK_IsPlaying)
            {
                LoopSource.time = (float)System.Math.Truncate(CurrentTime / (Pace3.length / PaceHang.length) * 1000) / 1000f;
                DrumSource.time = (float)System.Math.Truncate(CurrentTime / (Pace3.length / PaceHang.length) * 1000) / 1000f;
            }
            else
            {
                MIDI.MPTK_RePlay();
                CurrentRhythmTime = 0f;
            }
#else
            MIDI.MPTK_RePlay();
            CurrentRhythmTime = 0f;
#endif
        }
    }
}