using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeSpeedSlider : MonoBehaviour
{
    // Token: 0x060000A9 RID: 169 RVA: 0x0000644C File Offset: 0x0000484C
    private void OnEnable()
    {
        this.slider = base.GetComponent<Slider>();
        if (Settings.GetFloat("TimeSpeed") < 0.1f)
        {
            Settings.SetFloat("TimeSpeed", 1f);
        }
        this.slider.value = Settings.GetFloat("TimeSpeed", 1f);
        this.slider.onValueChanged.AddListener((value) =>
        {
            if (!MultiplayerManager.isActive)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
                    Time.timeScale = value;
                Settings.SetFloat("TimeSpeed", value);
                float valueOf = value * 100f;
                speedValue.text = valueOf.ToString("0.0") + "%";
            }
        });
        if (!MultiplayerManager.isActive)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
                Time.timeScale = slider.value;
        }
        float valueOf = slider.value * 100f;
        speedValue.text = valueOf.ToString("0.0") + "%";
        if (MultiplayerManager.isActive)
        {
            slider.interactable = false;
        }
    }

    // Token: 0x060000AA RID: 170 RVA: 0x0000649D File Offset: 0x0000489D
    private void Update()
    {
        if (MultiplayerManager.isActive)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            slider.value = 1f;
        }
    }

    // Token: 0x04000122 RID: 290
    public Slider slider;

    public TMP_Text speedValue;
}