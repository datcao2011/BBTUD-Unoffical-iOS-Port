using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManagerSlider : MonoBehaviour
{
    public AudioMixer gameMixer;
    public TMPro.TMP_Text PercentageText;
    private Slider slider;
    public string VolumeValue;
    private string PercentageTemplate;

    private void Start()
    {
        //console.log("Audio Manager has started.");
        PercentageTemplate = PercentageText.text.Replace("100.0%", "{VOL}");
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnVolumeChanged);
        slider.value = Settings.GetFloat("Volume_" + VolumeValue, 1f);
    }

    public void OnVolumeChanged(float val)
    {
        if (VolumeValue == "MasterVolume")
        {
            AudioListener.volume = val; // Just in case.
        }
        else if (val <= 0f)
        {
            gameMixer.SetFloat(VolumeValue, -80f);
        }
        else gameMixer.SetFloat(VolumeValue, Mathf.Log10(val) * 20);
        PercentageText.text = PercentageTemplate.Replace("{VOL}", (val * 100f).ToString("0.0") + "%");
        Settings.SetFloat("Volume_" + VolumeValue, val);
    }
}