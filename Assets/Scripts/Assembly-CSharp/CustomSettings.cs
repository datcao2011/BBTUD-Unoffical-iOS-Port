using UnityEngine;

public class CustomSettings : MonoBehaviour
{
    public bool[] characterActive = new bool[11]
    {
true,
true,
true,
true,
true,
true,
true,
true,
true,
true,
true
    };

    public NotebookScript[] notebooks = new NotebookScript[9];

    private PickupScript[] items;

    public bool randomEvents = true;

    public GameObject[] characters;

    public DisableOnLoad[] disableOnLoads = new DisableOnLoad[11];

    public AudioSource audioDevice;
    private void Awake()
    {
        items = new PickupScript[FindObjectsOfType<PickupScript>().Length];
        items = FindObjectsOfType<PickupScript>();
        itemField.text = FindObjectsOfType<PickupScript>().Length.ToString();
#if !UNITY_WEBGL
        AudioListener.pause = true;
        audioDevice.ignoreListenerPause = true;
#else
        audioDevice.mute = true;
#endif
        FindObjectOfType<GameControllerScript>().cheatConsoleActive = true;
        FindObjectOfType<GameControllerScript>().gameObject.AddComponent<DisableSaveSystem>();
        if (FindObjectOfType<RollTrigger>())
        {
            Destroy(FindObjectOfType<RollTrigger>().gameObject, 0f);
        }
        CursorControllerScript.UnlockCursor();
    }

    private void Update()
    {
        if (!MultiplayerManager.isActive) Time.timeScale = 0f;
        CheatScript.cheated = true;
        if (InputManager.usingTouch && GameObject.Find("SaveButton")) GameObject.Find("SaveButton").SetActive(false);
    }

    public void Play()
    {
#if !UNITY_WEBGL
        AudioListener.pause = false;
#endif
        FindObjectOfType<GameControllerScript>().cheatConsoleActive = false;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
#if !UNITY_WEBGL
        if (AudioListener.pause)
            AudioListener.pause = false; // Unpauses audio so it doesn't glitch for the rest of the game.
#endif
    }

    public void RandomEvents(bool value)
    {
        randomEvents = value;
        if (randomEvents)
        {
            FindObjectOfType<RandomEvent>().ResumeRandomEvents();
        }
        else
        {
            FindObjectOfType<RandomEvent>().PauseRandomEvents();
        }
    }

    public void SetCharacterState(int character)
    {
        characterActive[character] = !characterActive[character];
        if (characterActive[character])
        {
            Destroy(disableOnLoads[character], 0f);
            if (character != 0)
                characters[character].SetActive(true);
        }
        else
        {
            disableOnLoads[character] = characters[character].AddComponent<DisableOnLoad>();
        }
    }

    public TMPro.TMP_InputField field;

    public TMPro.TMP_InputField itemField;

    public void SetMaxNotebooks(string notebooks)
    {
        if (notebooks == string.Empty)
        {
            notebooks = "7";
        }
        if (notebooks.StartsWith("-"))
        {
            notebooks = "7";
            field.text = notebooks;
        }
        int books = int.Parse(notebooks);
        if (books < 2)
        {
            books = 2;
            field.text = "2";
        }
        if (books > 9)
        {
            books = 9;
            field.text = "9";
        }
        GameControllerScript gc = FindObjectOfType<GameControllerScript>();
        gc.maxNotebooks = books;
        gc.UpdateNotebookCount();
        for (int i = 0; i < this.notebooks.Length; i++)
        {
            if (books - 1 >= i)
            {
                this.notebooks[i].gameObject.SetActive(true);
            }
            else
            {
                this.notebooks[i].gameObject.SetActive(false);
            }
        }
    }

    public BaldiScript baldi;

    public void SetBaldiAnger(string text)
    {
        if (text == string.Empty)
        {
            text = "1";
        }
        if (text.StartsWith("-"))
        {
            text = "1";
        }
        if (text.EndsWith("."))
        {
            text = "1";
        }
        float anger = float.Parse(text);
        baldi.angerMultiplier = anger;
    }

    public void SetTempBaldiAnger(string text)
    {
        if (text == string.Empty)
        {
            text = "0.25";
        }
        if (text.StartsWith("-"))
        {
            text = "0.25";
        }
        if (text.EndsWith("."))
        {
            text = "0.25";
        }
        float anger = float.Parse(text);
        baldi.angerMultiplier = anger + 0.75f;
    }

    public void SetSpeed(string text)
    {
        if (text == string.Empty)
        {
            text = "70";
        }
        if (text.StartsWith("-"))
        {
            text = "70";
        }
        if (text.EndsWith("."))
        {
            text = "70";
        }
        float speed = float.Parse(text);
        baldi.speed = speed;
    }


    public void SetItemCount(string text)
    {
        if (text == string.Empty)
        {
            text = items.Length.ToString();
        }
        if (text.StartsWith("-"))
        {
            text = items.Length.ToString();
            itemField.text = text;
        }
        int items2 = int.Parse(text);
        if (items2 < 0)
        {
            items2 = items.Length;
            itemField.text = items2.ToString();
        }
        if (items2 > items.Length)
        {
            items2 = items.Length;
            itemField.text = items2.ToString();
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items2 - 1 >= i)
            {
                items[i].gameObject.SetActive(true);
            }
            else
            {
                items[i].gameObject.SetActive(false);
            }
        }
    }
}