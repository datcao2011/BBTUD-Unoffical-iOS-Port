using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class AudioSourceSpeed : MonoBehaviour
{
    private AudioSource audioSource;
    private float speed;

    private void OnEnable()
    {
        audioSource = base.GetComponent<AudioSource>();
        if (audioSource.spatialBlend >= 1f & audioSource.dopplerLevel > 0f)
            audioSource.dopplerLevel = 0f;
        speed = Settings.GetFloat("TimeSpeed", 1f);
        audioSource.pitch = speed;

        Settings.OnSettingChanged += OnSettingChange;
    }

    private void OnDisable()
    {
        Settings.OnSettingChanged -= OnSettingChange;
    }

    private void OnSettingChange(string Setting, object Value)
    {
        if (Setting == "TimeSpeed")
        {
            float Speed = (float)Value;
            if (speed != Speed)
            {
                audioSource.pitch = Speed;
            }
            speed = Speed;
        }
    }
}