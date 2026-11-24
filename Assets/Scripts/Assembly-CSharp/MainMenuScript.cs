using UnityEngine;
using UnityEngine.EventSystems;
using Platforms;
using System.Collections;

public class MainMenuScript : MonoBehaviour
{
    private EventSystem eventSystem;

    [SerializeField]
    private GameObject gameobject;

    public GameObject BouncingText;

    public CanvasGroup canvasGroup;

    public AudioSource MusicAudio;

    public AudioClip mus_NoMelodyTheme;

    private void Awake()
    {
        System.DateTime now = System.DateTime.Now;
        if (now.Hour == 3)
        {
            MusicAudio.clip = mus_NoMelodyTheme;
            MusicAudio.Play();
        }
    }

    private void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        // if (InputManager.isUsingController == true)
        //     eventSystem.SetSelectedGameObject(gameobject);
        System.DateTime now = System.DateTime.Now;
        if (now.Hour == 3)
        {
            AchievementManager.Instance.Earn(Achievements.ThreeAM);
        }
    }

    private void Update()
    {
    }

    public void GoToShop()
    {
        StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        AudioSource[] audioDevices = FindObjectsOfType<AudioSource>();
        float value = 0f;
        Debug.Log("Visiting shop...");
        while (value < 1f)
        {
            value += 2.5f * Time.unscaledDeltaTime;
            if (value >= 1f) value = 1f;
            canvasGroup.alpha = value;
            foreach (AudioSource audioSource in audioDevices)
            {
                audioSource.volume = 1f - value;
            }
            yield return null;
        }
        yield return new WaitForSecondsRealtime(3f);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Shop");
        yield break;
    }

    private float resetTime;
}