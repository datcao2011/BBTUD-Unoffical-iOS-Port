using UnityEngine;
using UnityEngine.UI;
using Platforms;
using TMPro;
using System.Collections.Generic;

public class ApplicationSettings : MonoBehaviour
{
    public GameObject screenWarning;

    public TMP_Text timerText;

    public TMP_Dropdown fullScreenMode;

    public TMP_Dropdown resolulation;

    public TMP_InputField heightField;

    public TMP_InputField widthField;

    public TMP_InputField hzField;

    public TMP_Dropdown FPSDropdown;
    public Toggle VSyncToggle;

    public GameObject customResolution;

    public Resolution[] resolutions;

    private int windowX, windowY;
    private RefreshRate refreshRate;

    private bool warningAppeared;

    private float revertingTime = 15f;

    private FullScreenMode lastFullScreenMode;

    private int lastScreenWindowX, lastScreenWindowY;
    private RefreshRate lastRefreshRate;

    private void OnEnable()
    {
        CurrentDevice.CheckDevice(true);
        int[] Frames;
#if UNITY_ANDROID
        List<int> FrameRates = new List<int>
        {
            -1,
            15,
            24,
            30
        };
        FPSDropdown.ClearOptions();
        List<string> FrameOptions = new List<string>
        {
            "Device Optimized FPS",
            "15",
            "24",
            "30"
        };
        int FrameRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
        if (FrameRate >= 60)
        {
            FrameOptions.Add("60");
            FrameRates.Add(60);
        }
        if (FrameRate >= 90)
        {
            FrameOptions.Add("90");
            FrameRates.Add(90);
        }
        if (FrameRate >= 120)
        {
            FrameOptions.Add("120");
            FrameRates.Add(120);
        }
        if (FrameRate >= 144)
        {
            FrameOptions.Add("144");
            FrameRates.Add(144);
        }
        Frames = FrameRates.ToArray();
        FPSDropdown.AddOptions(FrameOptions);
        FPSDropdown.RefreshShownValue();
        VSyncToggle.gameObject.SetActive(false);
#elif UNITY_WEBGL
        Frames = new int[] { -1, 30, 60, 90, 120, 144 };
        FPSDropdown.ClearOptions();
        List<string> FrameOptions = new List<string>();
        FrameOptions.Add("VSynced or Unlimited");
        FrameOptions.Add("30");
        FrameOptions.Add("60");
        FrameOptions.Add("90");
        FrameOptions.Add("120");
        FrameOptions.Add("144");
        FPSDropdown.AddOptions(FrameOptions);
        FPSDropdown.RefreshShownValue();
        VSyncToggle.gameObject.SetActive(false);
#else
        Frames = new int[] { -1, 30, 60, 90, 120, 144, 240, 360, 480 };
#endif
        if (gameObject.scene.name != "MainMenu" && CurrentDevice.currentPlatform == PFPlatform.MacOS && fullScreenMode.options.Count < 4)
        {
            List<string> ExtraFrameOption = new List<string>()
            {
                "Maximized Window"
            };
            fullScreenMode.AddOptions(ExtraFrameOption);
        }
        RefreshResolutionSettings();
        if (gameObject.scene.name != "MainMenu")
        {
            VSyncToggle.onValueChanged.AddListener(val =>
            {
#if !UNITY_ANDROID && !UNITY_WEBGL
                InputManager.SetVSync(val);
                FPSDropdown.interactable = !Settings.GetBool("vsync");
                if (Settings.GetBool("vsync"))
                {
                    FPSDropdown.captionText.text = "VSynced";
                }
                else
                {
                    FPSDropdown.captionText.text = FPSDropdown.options[GetDropdownValue(Settings.GetInt("frames"))].text;
                }
#endif
            });
            FPSDropdown.onValueChanged.AddListener(val =>
            {
                InputManager.SetFrameRate(Frames[val]);
            });
        }
    }

    private int GetDropdownValue(int frame)
    {
        int[] Frames;
#if UNITY_ANDROID
        Frames = new int[] { -1, 15, 24, 30, 60, 90, 120, 144 };
#elif UNITY_WEBGL
        Frames = new int[] { -1, 30, 60, 90, 120, 144 };
#else
        Frames = new int[] { -1, 30, 60, 90, 120, 144, 240, 360, 480 };
#endif
        int result = 0;
        for (int i = 0; i < Frames.Length; i++)
        {
            if (Frames[i] == frame)
            {
                result = i;
                break;
            }
        }
        return result;
    }

    private FullScreenMode lastUpdateFullScreenMode = FullScreenMode.FullScreenWindow;
    private int lastUpdateHeight = 1280, lastUpdateWidth = 720;
    private double lastUpdateHz = 30;

    public void RefreshResolutionSettings()
    {
        if (gameObject.scene.name == "MainMenu") return;

#if !UNITY_ANDROID && !UNITY_WEBGL
        VSyncToggle.SetIsOnWithoutNotify(Settings.GetBool("vsync"));
#endif
        FPSDropdown.SetValueWithoutNotify(GetDropdownValue(Settings.GetInt("frames")));
#if !UNITY_ANDROID && !UNITY_WEBGL
        FPSDropdown.interactable = !Settings.GetBool("vsync");
        if (Settings.GetBool("vsync"))
        {
            FPSDropdown.captionText.text = "VSynced";
        }
#endif

        resolutions = Screen.resolutions;

        resolulation.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        bool resolutionMatched = false;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string RefreshRateRatio = (System.Math.Round(resolutions[i].refreshRateRatio.value * 100) / 100).ToString();
            string option = resolutions[i].width + " x " + resolutions[i].height + " (" + RefreshRateRatio + " Hz)";
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
                resolutionMatched = true;
            }
        }
        options.Add("Custom");
        resolulation.AddOptions(options);
        if (!resolutionMatched)
        {
            resolulation.value = options.Count - 1;
            heightField.text = Screen.height.ToString();
            widthField.text = Screen.width.ToString();
            hzField.text = Screen.currentResolution.refreshRateRatio.value.ToString();
        }
        else resolulation.value = currentResolutionIndex;
        resolulation.RefreshShownValue();

        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                fullScreenMode.value = 1;
                break;
            case FullScreenMode.Windowed:
                fullScreenMode.value = 2;
                break;
            case FullScreenMode.MaximizedWindow:
                fullScreenMode.value = (fullScreenMode.options.Count >= 4) ? 3 : 0;
                break;
            default:
                fullScreenMode.value = 0;
                break;
        }
        fullScreenMode.RefreshShownValue();

        lastUpdateHeight = Screen.height;
        lastUpdateWidth = Screen.width;
        lastUpdateHz = Screen.currentResolution.refreshRateRatio.value;
        lastUpdateFullScreenMode = Screen.fullScreenMode;
    }

    public void OnResolutionDropdownChange()
    {
        if (resolulation.value == resolulation.options.Count - 1)
        {
            customResolution.SetActive(true);
        }
        else
        {
            customResolution.SetActive(false);
        }
    }

    public void SetScreenResolution(int x, int y, RefreshRate refreshRate, FullScreenMode fullScreenMode)
    {
        lastScreenWindowX = Screen.width;
        lastScreenWindowY = Screen.height;
        lastFullScreenMode = Screen.fullScreenMode;
        lastRefreshRate = Screen.currentResolution.refreshRateRatio;
        Screen.SetResolution(x, y, fullScreenMode, refreshRate);
        if (!warningAppeared)
        {
            warningAppeared = true;
            screenWarning.SetActive(true);
        }
    }

    private void Update()
    {
        if (warningAppeared)
        {
            revertingTime -= Time.unscaledDeltaTime;
            timerText.text = "Reverting changes in " + Mathf.CeilToInt(revertingTime);
        }
        if (warningAppeared && revertingTime <= 0f)
        {
            warningAppeared = false;
            revertingTime = 15f;
            RevertScreenChanges();
        }


    }

    private void FixedUpdate()
    {
        if (Screen.fullScreenMode != lastUpdateFullScreenMode)
        {
            RefreshResolutionSettings();
        }
        if (Screen.height != lastUpdateHeight || Screen.width != lastUpdateWidth || Screen.currentResolution.refreshRateRatio.value != lastUpdateHz)
        {
            RefreshResolutionSettings();
        }
    }

    public void RevertScreenChanges()
    {
        warningAppeared = false;
        revertingTime = 15f;
        screenWarning.SetActive(false);
        Screen.SetResolution(lastScreenWindowX, lastScreenWindowY, lastFullScreenMode, lastRefreshRate);
        RefreshResolutionSettings();
    }

    public void SaveScreenChanges()
    {
        warningAppeared = false;
        revertingTime = 15f;
        screenWarning.SetActive(false);
        ChangeSettings();
    }

    public void SetResolutionIndex(int index)
    {

    }


    public void Save()
    {
        warningAppeared = true;
        if (resolulation.value == resolutions.Length)
        {
            windowY = int.Parse(heightField.text);
            windowX = int.Parse(widthField.text);
            double Num = double.Parse(hzField.text);
            uint numerator = (uint)System.Math.Round((decimal)Num, 6) * 1000000;
            uint denominator = 1000000;
            refreshRate = new RefreshRate() { numerator = numerator, denominator = denominator };
        }
        else
        {
            windowX = resolutions[resolulation.value].width;
            windowY = resolutions[resolulation.value].height;
            refreshRate = resolutions[resolulation.value].refreshRateRatio;
        }
        FullScreenMode screenMode;
        switch (fullScreenMode.options[fullScreenMode.value].text)
        {
            case "Exclusive Fullscreen":
                screenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case "Resizeable Window":
                screenMode = FullScreenMode.Windowed;
                break;
            case "Maximized Window":
                screenMode = FullScreenMode.MaximizedWindow;
                break;
            default:
                screenMode = FullScreenMode.FullScreenWindow;
                break;
        }
        SetScreenResolution(windowX, windowY, refreshRate, screenMode);

        screenWarning.SetActive(true);
    }

    private void ChangeSettings()
    {

    }
}