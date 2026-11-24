using UnityEngine;
using UnityEngine.UI;

public class VolumeListenerScript : MonoBehaviour
{
    private void Start()
    {
        this.slider.value = Settings.GetFloat("AudioVolume", 1f);
        staticSlider = slider;
    }

    private void Update()
    {
        Settings.SetFloat("AudioVolume", slider.value);
    }

    public Slider slider;

    public static Slider staticSlider;

    public static void SetVolume(float v)
    {
        Settings.SetFloat("AudioVolume", v);
        staticSlider.value = v;
    }
}
