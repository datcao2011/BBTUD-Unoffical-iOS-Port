using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MysteryScript : MonoBehaviour
{
    private bool opened;

    public AudioClip openSound;

    public Animator animator;

    public Sprite[] prizes;

    public Image prize;

    public GameObject buttonObject;

    public string currentScene = "MysteryRoom";

    private int randomizer;

    private void Start()
    {
        randomizer = Mathf.RoundToInt(Random.Range(1f, 25f));
        if (Random.Range(1f, 100f) >= 98f)
        {
            randomizer = 12;
        }
        else
        {
            while (randomizer == 12)
            {
                randomizer = Mathf.RoundToInt(Random.Range(1f, 25f));
            }
        }
        prize.sprite = prizes[randomizer - 1];
    }

    private void Update()
    {
        if (!opened && (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Use") || Input.touchCount > 0))
        {
            opened = true;
            AudioSource audioDevice = gameObject.AddComponent<AudioSource>();
            AudioSourceSpeed speed = audioDevice.AddComponent<AudioSourceSpeed>();
            audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
            audioDevice.clip = openSound;
            audioDevice.Play();
            Destroy(speed, openSound.length);
            Destroy(audioDevice, openSound.length + Time.unscaledDeltaTime);
            animator.SetTrigger("Open");
        }
    }

    public void UnloadMysteryRoom()
    {
        Settings.SetInt("ItemsCollected", Settings.GetInt("ItemsCollected") + 1);
        Settings.SetInt("ItemCollected" + (Settings.GetInt("ItemsCollected", 0) - 1).ToString(), randomizer);
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);
    }

    public static void RemoveItemCollected(int itemID)
    {
        bool continues = false;
        for (int i = 0; i < Settings.GetInt("ItemsCollected"); i++)
        {
            if (!continues)
            {
                if (Settings.GetInt("ItemCollected" + i, 0) == itemID)
                {
                    continues = true;
                    Settings.DeleteKey("ItemCollected" + i);
                }
            }
        }
    }
}