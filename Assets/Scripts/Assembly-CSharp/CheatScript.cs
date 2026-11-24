using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CheatScript : MonoBehaviour
{
    private GameObject[] ObjectCloned = new GameObject[500];

    public Collider playerCollider;

    private float playerSpeed;

    private bool unlimitedStamina;

    private int clonedObjects;

    public bool ClonesInteract;

    public TMP_Text currentTextEdit;

    public TMP_InputField speedField;
    public TMP_Dropdown itemDropdown;

    public RandomEvent schoolEventSystem;

    public TMP_Text objectClones;

    public GameObject[] characters = new GameObject[7];

    public Button startButton;

    public Button endButton;

    public Button chooseButton;

    public Button rulerBreakageButton;

    private Vector3 UpUpPosition;
    private Vector3 UpPosition;
    private Vector3 DownPosition;

    public static bool cheated;
    public int slot;

    public Toggle FOV;

    public void SelectTMPText(TMP_Text text)
    {
        currentTextEdit = text;
    }

    private void CheckForUpdateOfSlot()
    {
        UpdateSlot(slot);
    }

    public void UpdateSlot(int id)
    {
        GameControllerScript gc = FindObjectOfType<GameControllerScript>();
        if (scene != InputManager.IsSchoolScene()) return;
        slot = id;
        itemDropdown.value = gc.item[id];
        itemDropdown.RefreshShownValue();
    }

    public void SetItem(int itemId)
    {
        cheated = true;
        GameControllerScript gc = FindObjectOfType<GameControllerScript>();
        gc.GetItem(slot, itemId);
    }

    public void AllowFutureClonesIntreaction(bool value)
    {
        ClonesInteract = value;
    }

    public void SetOptions(string[] names, Sprite[] sprites)
    {
        itemDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> Options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < names.Length; i++)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData(names[i], sprites[i]);
            Options.Add(data);
        }
        itemDropdown.AddOptions(Options);
        itemDropdown.RefreshShownValue();
    }

    private void Update()
    {
        if (objectClones != null && objectClones.isActiveAndEnabled) objectClones.text = "Clones: " + clonedObjects;
        if (scene != InputManager.IsSchoolScene()) return;
        if (MultiplayerManager.isActive)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        if (scene == InputManager.IsSchoolScene() && startButton.isActiveAndEnabled)
        {
            rulerBreakageButton.interactable = FindObjectOfType<BaldiScript>();
            if (schoolEventSystem.eventTimer > 0f)
            {
                startButton.interactable = true;
            }
            else
            {
                startButton.interactable = false;
            }
            if (schoolEventSystem.eventEndTimer > 0f)
            {
                endButton.interactable = true;
            }
            else
            {
                endButton.interactable = false;
            }
            if (schoolEventSystem.eventTimer > 0f)
            {
                chooseButton.interactable = true;
            }
            else
            {
                chooseButton.interactable = false;
            }
        }

        if (MultiplayerManager.isActive)
        {
            if (MultiplayerManager.instance.gc.cheatConsoleActive)
            {
                MultiplayerManager.instance.gc.cheatConsoleActive = false;
                DestroyImmediate(this);
            }
        }
    }

    public void SetCharacterState(int index)
    {
        cheated = true;
        if (characters[index].GetComponent<DisableOnLoad>())
        {
            Destroy(characters[index].GetComponent<DisableOnLoad>(), 0f);
        }
        GameObject game1Object = characters[index];
        if (game1Object.activeSelf)
        {
            game1Object.SetActive(false);
        }
        else
        {
            game1Object.SetActive(true);
        }
        if (game1Object.name == "Principal of the Thing")
        {
            if (!game1Object.activeSelf)
                currentTextEdit.text = "Enable Principal";
            else
                currentTextEdit.text = "Disable Principal";
        }
        else
        {
            if (!game1Object.activeSelf)
                currentTextEdit.text = "Enable " + game1Object.name;
            else
                currentTextEdit.text = "Disable " + game1Object.name;
        }
    }

    public void StartEvent()
    {
        if (MultiplayerManager.isActive) return;
        cheated = true;
        schoolEventSystem.eventTimer = 0f;
    }

    public void EndEvent()
    {
        if (MultiplayerManager.isActive) return;
        cheated = true;
        schoolEventSystem.StopEventNOW();
    }

    public void Cd()
    {
        cheated = true;
    }

    public void ChooseEvent(int index)
    {
        if (MultiplayerManager.isActive) return;
        cheated = true;
        FindObjectOfType<RandomEvent>().ChooseEvent(index);
    }

    public void SetCharacterState(int index, bool active)
    {
        GameObject game1Object = characters[index];
        characters[index].SetActive(active);
        if (game1Object.name == "Principal of the Thing")
        {
            if (!game1Object.activeSelf)
                currentTextEdit.text = "Enable Principal";
            else
                currentTextEdit.text = "Disable Principal";
        }
        else
        {
            if (!game1Object.activeSelf)
                currentTextEdit.text = "Enable " + game1Object.name;
            else
                currentTextEdit.text = "Disable " + game1Object.name;
        }
    }
    public void SceneLoad(int index)
    {
        SceneManager.LoadScene(index);
    }
    public void SceneLoad(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SetUnlimitedStamina(bool value)
    {
        if (MultiplayerManager.isActive) return;
        cheated = true;
        playerCollider.GetComponent<PlayerScript>().useStamina = !value;
    }
    public void SetUnlimitedItems(bool value)
    {
        if (MultiplayerManager.isActive) return;
        cheated = true;
        FindObjectOfType<GameControllerScript>().infiniteItems = value;
    }

    public void SetSpeed(string value)
    {
        if (MultiplayerManager.isActive) return;
        cheated = true;
        if (value == string.Empty)
            value = "1";
        float speed = float.Parse(value);
        playerCollider.GetComponent<PlayerScript>().walkSpeed = speed;
        playerCollider.GetComponent<PlayerScript>().runSpeed = speed + 6f;
        if (FOV.isOn)
        {
            Camera.main.fieldOfView = speed + 50f;
        }
    }

    public bool IsFOVEnabled
    {
        get
        {
            return FOV.isOn;
        }
    }

    public void CloneCharacter(int index)
    {
        if (MultiplayerManager.isActive) return;
        cheated = true;
        if (clonedObjects >= 499)
        {
            console.error("Cannot clone any more objects.");
            InputManager.DisplayException(new System.OverflowException("Cannot clone any more objects."), 1f);
            return;
        }
        GameObject game1Object = characters[index];
        GameObject clonedObject = Object.Instantiate(game1Object);
        clonedObject.name = ClonesInteract ? game1Object.name : (game1Object.name + "(C." + clonedObjects + ")");
        clonedObject.SetActive(true);
        ObjectCloned[clonedObjects] = clonedObject;
        clonedObjects++;
    }

    public IEnumerator DeleteAllClones()
    {
        cheated = true;
        console.log("Deleting all clones...");
        while (clonedObjects > 0)
        {
            DeleteAClone();
            yield return null;
        }
        clonedObjects = 0;
        console.log("Done!");
        yield break;
    }

    private void Awake()
    {
        cheated = false;
    }

    private void Start()
    {
        scene = SceneManager.GetActiveScene().name;
        FOV.isOn = Settings.GetBool("FOVToggle", true);
        FOV.onValueChanged.AddListener((value) =>
        {
            Settings.SetBool("FOVToggle", value);
            Camera.main.fieldOfView = value ? (playerCollider.GetComponent<PlayerScript>().walkSpeed + 50f) : 60f;
        });
        if (scene != InputManager.IsSchoolScene()) return;
        UpUpPosition = GameObject.Find("ElevatorCollider (1) (up)").transform.position;
        UpPosition = GameObject.Find("ElevatorCollider (1)").transform.position;
        DownPosition = GameObject.Find("ElevatorCollider").transform.position;
    }

    private void OnDestroy()
    {
        cheated = false;
    }

    string scene;

    public void ButtonDeleteClones()
    {
        StartCoroutine(DeleteAllClones());
    }

    public void NavigateUpOrDown(bool up)
    {
        cheated = true;
        if (up)
        {
            if (playerCollider.transform.position.y >= 20) playerCollider.transform.position = new Vector3(UpUpPosition.x, 44f, UpUpPosition.z);
            else
                playerCollider.transform.position = new Vector3(UpPosition.x, 24f, UpPosition.z);
        }
        else
        {
            if (playerCollider.transform.position.y >= 40) playerCollider.transform.position = new Vector3(UpPosition.x, 24f, UpPosition.z);
            else
                playerCollider.transform.position = new Vector3(DownPosition.x, 4f, DownPosition.z);
        }
    }


    private void DeleteAClone()
    {
        Object.Destroy(ObjectCloned[0]);
        DeleteClone();
    }

    private void DeleteClone()
    {
        for (int i = 1; i < clonedObjects; i++)
        {
            ObjectCloned[i - 1] = ObjectCloned[i];
        }
        clonedObjects--;
    }

    public void OnCheatEnable()
    {
        speedField.SetTextWithoutNotify(playerCollider.GetComponent<PlayerScript>().walkSpeed.ToString());
        CheckForUpdateOfSlot();
    }
}