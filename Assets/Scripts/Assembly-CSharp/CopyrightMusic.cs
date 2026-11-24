using UnityEngine;


public class CopyrightMusic : MonoBehaviour
{
    public AudioClip uncopyrightMusic;
    private AudioClip _origin;

    public bool playOnAwake = true;

    private void Awake()
    {
        _origin = GetComponent<AudioSource>().clip;
        if (!Settings.GetBool("CopyrightMusic", true))
        {
            GetComponent<AudioSource>().clip = uncopyrightMusic;
            if (playOnAwake)
            {
                GetComponent<AudioSource>().Play();
            }
        }
        Settings.OnSettingChanged += OnCopyrightMusicChange;
    }

    private void OnCopyrightMusicChange(string key, object val)
    {
        if (key == "CopyrightMusic")
        {
            bool IsPlaying = GetComponent<AudioSource>().isPlaying;
            GetComponent<AudioSource>().clip = (bool)val ? _origin : uncopyrightMusic;
            if (IsPlaying) GetComponent<AudioSource>().Play();
        }
    }

    private void OnDestroy()
    {
        Settings.OnSettingChanged -= OnCopyrightMusicChange;
    }
}