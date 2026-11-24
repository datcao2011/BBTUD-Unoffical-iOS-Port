using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    private string[] TipTexts = new string[]
    {
        "There is one type of apple that makes you OVERPOWERED :D",
        "Save your progress by using the save feature on supported modes.",
        "You can switch your HUD to classic or remastered in settings.",
        "This game is intended to expand the classic schoolhouse in a ultra way.",
        "You can disable YCTP problems on settings. Be aware, Baldi may be faster.",
        "School events can be tracked on a faculty room.",
        "Some characters have more features than the original game.",
        "Achievements in this game are fun to get. Try to get them all!",
        "Using collected items can help you win, but it counts as cheating!",
        "If you're on multiplayer, make sure to be fast because time doesn't stop for you.",
        "There is a way to cheat. Beat the game without cheats, cheater!",
        "You are cool if you read this :D",
        "If you're on multiplayer, look around you. Baldi might be hiding.",
        "If you're on multiplayer, some things might be removed or added.",
        "Turning on captions can help you figure out where's the sound coming from.",
        "If you're on multiplayer, you can give items to other players by dropping them."
    };

    public Gradient backgroundColors;

    public RawImage background;

    public RawImage secondBackground;

    public float delay;

    [SerializeField]
    private Slider uselessSlider;

    public float delayHints;

    public TMP_Text tipText;

    private void OnEnable() // This should fix some existing issues
    {
        if (Settings.GetFloat("TimeSpeed") == 0f)
        {
            Settings.SetFloat("TimeSpeed", 1f);
        }
        Time.timeScale = Settings.GetFloat("TimeSpeed");
        delayHints = 4f;
        GenerateTipText();
    }

    private void GenerateTipText()
    {
        StopAllCoroutines();
        int num = Mathf.RoundToInt(UnityEngine.Random.Range(0f, TipTexts.Length - 1));
        string tip = TipTexts[num];
        StartCoroutine(TipTextGeneration("<u>TIP: </u>" + tip));
    }

    private int PreferredFrameLimit
    {
        get
        {
#if UNITY_STANDALONE_WIN
            return 160;
#elif UNITY_STANDALONE
            return 120;
#else
            return Platforms.CurrentDevice.currentPlatform == Platforms.PFPlatform.Android ? 30 : 60;
#endif
        }
    }

    private System.Collections.IEnumerator TipTextGeneration(string tip)
    {
        int MaxFrames = tip.Length;
        float time = 0f;
        float MaxTime = (float)MaxFrames / (Platforms.CurrentDevice.currentPlatform == Platforms.PFPlatform.Android ? 30 : 60);
        int Stage = 0;
        delayHints += MaxTime;
        bool Stop = false;
        bool StageUnderscore = true;
        while (!Stop)
        {
            time += Time.deltaTime;

            int LastStage = Stage;
            Stage = Mathf.FloorToInt(time / (0.2f * (60f / PreferredFrameLimit)));
            if (Stage != LastStage)
            {
                StageUnderscore = !StageUnderscore;
            }

            string FinalText = tip.Substring(0, Mathf.RoundToInt((time >= MaxTime ? MaxTime : time) * (Platforms.CurrentDevice.currentPlatform == Platforms.PFPlatform.Android ? 30 : 60)));

            if (StageUnderscore && time < MaxTime) FinalText += "<color=white>_";
            tipText.text = FinalText;
            if (time >= MaxTime)
            {
                Stop = true;
            }
            yield return null;
        }
        tipText.text = tip; // Final text
        yield break;
    }

    private void Update()
    {
        delayHints -= Time.deltaTime;
        delay -= Time.deltaTime;
        uselessSlider.value += Time.unscaledDeltaTime * 1f;
        if (uselessSlider.value >= uselessSlider.maxValue)
            uselessSlider.value = uselessSlider.minValue;
        Color newColor = backgroundColors.Evaluate(uselessSlider.normalizedValue);
        if (secondBackground != null)
        {
            secondBackground.color = newColor;
        }
        background.color = newColor;
        if (delayHints <= 0f)
        {
            delayHints = 5f;
            GenerateTipText();
        }
    }
}