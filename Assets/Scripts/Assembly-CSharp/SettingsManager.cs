using Platforms;
using UnityEngine;

using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class SettingsManager : MonoBehaviour
{
    private float timeToVibrate;

    private int Times;
    [SerializeField]
    private Toggle vibrationToggle;
    [SerializeField]
    private Toggle threeDToggle;
    [SerializeField]
    private Toggle optimizer;
    [SerializeField]
    private Toggle debug;
    [SerializeField]
    private Toggle NoYCTP;
    [SerializeField]
    private Toggle IRLTimeDaylight;
    [SerializeField]
    private TMPro.TMP_Dropdown Distance;
    [SerializeField]
    private Toggle MainCaption;
    [SerializeField]
    private Toggle DoorCaption;
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_WEBGL
    [SerializeField]
    private ScrollRect AdvancedRect;
    [SerializeField]
    private Toggle LogToggle;
#endif

    private void Start()
    {
        if (Distance != null)
        {
            Distance.onValueChanged.AddListener(value =>
            {
                Settings.SetInt("Distance", value);
            });
            Distance.value = Settings.GetInt("Distance", GameOptimizer.CameraOptimizationDefault);
        }

        optimizer.onValueChanged.AddListener((value) =>
        {
            Settings.SetBool("Optimize", value);
        });
        optimizer.isOn = Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault);
        debug.onValueChanged.AddListener((value) =>
        {
            Settings.SetBool("Debug", value);
        });
        debug.isOn = Settings.GetBool("Debug");
        if (NoYCTP != null)
        {
            if (MultiplayerManager.isActive && MultiplayerManager.instance.CurrentSettings.YCTPUsage > 0)
            {
                NoYCTP.isOn = MultiplayerManager.instance.CurrentSettings.YCTPUsage >= 1 && MultiplayerManager.instance.CurrentSettings.YCTPUsage <= 2;
                if (MultiplayerManager.instance.CurrentSettings.YCTPUsage == 2)
                {
                    NoYCTP.graphic.color = new Color(0.2983713f, 0.3320849f, 0.4245283f, 1f);
                }
                NoYCTP.interactable = false;
            }
            else if (Settings.GetString("CurrentMode") == "endless" && SceneManager.GetActiveScene().name.StartsWith("School"))
            {
                NoYCTP.isOn = !Settings.GetBool("NoYCTP");
                NoYCTP.interactable = false;
            }
            else
            {
                NoYCTP.isOn = !Settings.GetBool("NoYCTP");
                NoYCTP.onValueChanged.AddListener((value) =>
                {
                    Settings.SetBool("NoYCTP", !value);
                });
            }
        }
        if (IRLTimeDaylight != null)
        {
            IRLTimeDaylight.isOn = Settings.GetBool("IRLDaylight", true);
            IRLTimeDaylight.onValueChanged.AddListener((value) =>
            {
                Settings.SetBool("IRLDaylight", value);
            });
            // #if UNITY_EDITOR || UNITY_ANDROID
#if UNITY_ANDROID
            Vector2 Size = AdvancedRect.content.sizeDelta;
            Size.y = 380.4182f;
            AdvancedRect.content.sizeDelta = Size;
            LogToggle.gameObject.SetActive(true);
            LogToggle.isOn = Settings.GetBool("OpenLogs");
            LogToggle.onValueChanged.AddListener((value) =>
            {
                Settings.SetBool("OpenLogs", value);
                ConsoleTraceInGame.EnableMobileLogs = value;
            });
#endif
#if UNITY_WEBGL
            if (Application.isMobilePlatform)
            {
                Vector2 Size = AdvancedRect.content.sizeDelta;
                Size.y = 380.4182f;
                AdvancedRect.content.sizeDelta = Size;
                LogToggle.gameObject.SetActive(true);
                LogToggle.isOn = Settings.GetBool("OpenLogs");
                LogToggle.onValueChanged.AddListener((value) =>
                {
                    Settings.SetBool("OpenLogs", value);
                    ConsoleTraceInGame.EnableMobileLogs = value;
                });
            }
#endif
        }
        Times = Settings.GetInt("VibrateSetupTimes");
        if (vibrationToggle.gameObject.activeSelf)
        {
            vibrationToggle.onValueChanged.AddListener((value) =>
            {
                Settings.SetBool("Vibrate", value);
            });
            if (Times == 0 & (InputManager.isUsingController == true || !Controller.isEnabled || InputManager.usingTouch))
            {
                Settings.SetInt("VibrateSetupTimes", 1);
                Times = 1;
                vibrationToggle.isOn = true;
            }
            else if (InputManager.isUsingController == true || !Controller.isEnabled || InputManager.usingTouch)
            {
                vibrationToggle.isOn = Settings.GetBool("Vibrate");
            }
        }
        threeDToggle.isOn = Settings.GetBool("3d movement");
        threeDToggle.onValueChanged.AddListener((value) =>
        {
            Settings.SetBool("3d movement", value);
        });

        // Look-behind mode (Toggle vs Hold) - runtime-cloned so no scene edit is required
        GameObject lbClone = Instantiate(threeDToggle.gameObject, threeDToggle.transform.parent);
        lbClone.name = "LookBehindToggle";
        Toggle lbToggle = lbClone.GetComponent<Toggle>();
        lbToggle.onValueChanged.RemoveAllListeners();
        lbToggle.isOn = Settings.GetBool("LookBehindToggle", true);
        lbToggle.onValueChanged.AddListener((value) =>
        {
            Settings.SetBool("LookBehindToggle", value);
        });
        TMPro.TMP_Text lbLabel = lbClone.GetComponentInChildren<TMPro.TMP_Text>();
        if (lbLabel != null) lbLabel.text = "Look Behind (Toggle)";
        RectTransform lbRT = lbClone.GetComponent<RectTransform>();
        RectTransform lbParent = threeDToggle.transform.parent.GetComponent<RectTransform>();
        float lbLowest = threeDToggle.GetComponent<RectTransform>().anchoredPosition.y - 45f;
        if (lbParent != null)
        {
            foreach (RectTransform sib in lbParent)
            {
                if (sib == lbRT) continue;
                float bottom = sib.anchoredPosition.y - sib.rect.height * sib.pivot.y;
                if (bottom < lbLowest) lbLowest = bottom;
            }
        }
        lbRT.anchoredPosition = new Vector2(lbRT.anchoredPosition.x, lbLowest - 6f - lbRT.rect.height * lbRT.pivot.y);
    }

    private void Update()
    {
        // if (timeToVibrate > 0f)
        // {
        //     timeToVibrate -= Time.deltaTime;
        //     CurrentDevice.Vibrate();
        // }
        if (Input.GetKeyDown(KeyCode.Escape) && (SceneManager.GetActiveScene().name != "OptionsMenu" && SceneManager.GetActiveScene().name != "MainMenu"))
        {
            SceneManager.UnloadSceneAsync("OptionsMenu");
        }
        if (DoorCaption != null)
        {
            DoorCaption.interactable = MainCaption.isOn;
        }
        if (vibrationToggle.gameObject.activeSelf)
        {
            if (InputManager.isUsingController == false & !InputManager.usingTouch)
            {
                if (vibrationToggle.interactable)
                {
                    vibrationToggle.onValueChanged.RemoveAllListeners();
                }
                vibrationToggle.interactable = false;
                if (Times == 0)
                    vibrationToggle.isOn = false;
            }
            else
            {
                if (Times == 0 & (InputManager.isUsingController == true || !Controller.isEnabled || InputManager.usingTouch))
                {
                    Settings.SetInt("VibrateSetupTimes", 1);
                    Times = 1;
                    vibrationToggle.isOn = true;
                }
                else if (!vibrationToggle.interactable && (InputManager.isUsingController == true || !Controller.isEnabled || InputManager.usingTouch))
                {
                    vibrationToggle.isOn = Settings.GetBool("Vibrate");
                    vibrationToggle.onValueChanged.AddListener((value) =>
                    {
                        Settings.SetBool("Vibrate", value);
                    });
                }
                vibrationToggle.interactable = true;
            }
        }
    }

    public void DeleteData()
    {
        Debug.Log("[Data] The data deletion was confirmed by the user.");
        if (AccountManager.hasInstance() && !string.IsNullOrEmpty(AccountManager.getSessionId()))
        {
            AccountManager.LogoutSession();
        }
#if !UNITY_WEBGL
        System.Threading.Thread.Sleep(3000);
        try
        {
            // Delete coin data
            if (System.IO.File.Exists(Application.persistentDataPath + "/Data/CoinData.bin")) System.IO.File.Delete(Application.persistentDataPath + "/Data/CoinData.bin");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Couldn't delete local coin data: " + ex);
        }
#endif
        PlayerPrefs.DeleteAll();
        Settings.Clear();
        Debug.Log("[Data] Data has been deleted.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameRestarter");
    }

    public void VibrateTest(float time)
    {
        timeToVibrate = time;
    }

    public void ConnectPad()
    {
        Controller.EnablePad();
    }
}