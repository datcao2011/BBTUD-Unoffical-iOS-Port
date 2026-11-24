using UnityEngine;
using Platforms;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class FarmFlagScript : MonoBehaviour
{
    public GameObject winObject;

    public AudioSourceLoop MainMusic;

    public AudioSource MelodyMusic;

    public FarmScript farm;

    public bool winned;

    private void Start()
    {
        MainMusic.LoopEnd = Settings.GetBool("CopyrightMusic", true) ? 0f : 16f;
        Settings.OnSettingChanged += OnCopyrightMusicChange;
    }

    private void OnCopyrightMusicChange(string key, object val)
    {
        if (key == "CopyrightMusic")
        {
            MainMusic.LoopEnd = (bool)val ? 0f : 16f;
        }
    }

    private void OnDestroy()
    {
        Settings.OnSettingChanged -= OnCopyrightMusicChange;
    }

    private void Update()
    {
        if (winned)
        {
            if (!MultiplayerManager.isActive) Time.timeScale = 0f;
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Cancel") || OnTap())
            {
                SceneManager.LoadScene("Starter");
            }
        }
    }

    public bool OnTap()
    {
        if (!Input.touchSupported)
        {
            return false;
        }
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).phase == TouchPhase.Began;
        }
        return false;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.transform.name == "Player")
        {
            if (!winned) StartCoroutine(FadeMusicUp());
            winned = true;
            if (!MultiplayerManager.isActive) Time.timeScale = 0f;
            winObject.SetActive(true);
        }
    }

    private IEnumerator FadeMusicUp()
    {
        while (MelodyMusic.volume < 1f)
        {
            MelodyMusic.volume += Time.unscaledDeltaTime * 2f;
            yield return null;
        }
        MelodyMusic.volume = 1f;
        yield break;
    }
}