using UnityEngine;

public class ModeDetector : MonoBehaviour
{
    public GameObject customMenu;

    public BaldiScript baldi;

    public GameObject BBManager;

    public GameObject NULLManager;

    private void Awake()
    {
        if (!MultiplayerManager.isActive && Settings.GetString("ModeType", "Normal") == "NULL")
        {
            GameObject.Find("NoYCTPNotebook2").SetActive(false);
            NULLManager.SetActive(true);
            // FindObjectOfType<GameControllerScript>().gameObject.AddComponent<DisableSaveSystem>();
            // if (InputManager.usingTouch && GameObject.Find("SaveButton")) GameObject.Find("SaveButton").SetActive(false);
        }
    }

    private void Start()
    {
        if (MultiplayerManager.isActive) return;
        string currentType = Settings.GetString("ModeType", "Normal");
        if (currentType == "Custom")
        {
            customMenu.SetActive(true);
            CheatScript.cheated = true;
            GameObject.Find("NoYCTPNotebook").SetActive(false);
            GameObject.Find("NoYCTPNotebook2").SetActive(false);
        }
        else if (currentType == "Random")
        {
            CheatScript.cheated = true;
            FindObjectOfType<GameControllerScript>().gameObject.AddComponent<DisableSaveSystem>();
            GameControllerScript gc = FindObjectOfType<GameControllerScript>();

            if (InputManager.usingTouch && GameObject.Find("SaveButton")) GameObject.Find("SaveButton").SetActive(false);

            baldi = gc.baldiScrpt;
            baldi.angerMultiplier = Random.Range(1f, 3f);
            baldi.tempAngerMultiplier = Random.Range(1f, 3f);
            baldi.speed = Random.Range(70f, 80f);
            int Max = FindObjectsOfType<NotebookScript>().Length;
            gc.maxNotebooks = Mathf.RoundToInt(Random.Range(3f, Mathf.Min(Max, 8f)));
            gc.UpdateNotebookCount();
            PickupScript[] pickups = FindObjectsOfType<PickupScript>();
            int max = pickups.Length;
            int min = Mathf.RoundToInt(Random.Range(0f, max - 1));
            for (int i = 0; i < max; i++)
            {
                if (i <= min)
                    pickups[i].gameObject.SetActive(true);
                else
                    pickups[i].gameObject.SetActive(false);
            }

            NotebookScript[] notebooks = FindObjectsOfType<NotebookScript>();

            for (int i = 0; i < notebooks.Length - gc.maxNotebooks; i++)
            {
                int randomizer = Mathf.RoundToInt(Random.Range(0f, Mathf.Min(Max, 7f)));
                while (!notebooks[randomizer].isActiveAndEnabled || notebooks[randomizer].notebookType == "Math" || notebooks[randomizer].notebookType == "Questions")
                {
                    randomizer = Mathf.RoundToInt(Random.Range(0f, Mathf.Min(Max, 7f)));
                }

                notebooks[randomizer].gameObject.SetActive(false);
            }
        }
        else if (currentType == "BB")
        {
            // FindObjectOfType<GameControllerScript>().gameObject.AddComponent<DisableSaveSystem>();
            GameObject.Find("NoYCTPNotebook2").SetActive(false);
            // if (InputManager.usingTouch && GameObject.Find("SaveButton")) GameObject.Find("SaveButton").SetActive(false);
            BBManager.SetActive(true);
        }
        else if (currentType == "Normal" & Settings.GetString("CurrentMode") == "story")
        {
            GameObject.Find("NoYCTPNotebook").SetActive(false);
            GameObject.Find("NoYCTPNotebook2").SetActive(false);
        }
        else if (currentType == "Stealthy")
        {
            FindObjectOfType<GameControllerScript>().gameObject.AddComponent<DisableSaveSystem>();
            if (InputManager.usingTouch && GameObject.Find("SaveButton")) GameObject.Find("SaveButton").SetActive(false);
            baldi.gameController.spoopMode = true;
            baldi.gameController.baldiTutor.SetActive(false);
            baldi.gameController.schoolMusic.mute = true;
            baldi.gameController.learnMusic.mute = true;
            baldi.gameController.player.ResetGuilt("sneaking", float.PositiveInfinity);
            baldi.gameController.mrsPomp.GetComponent<Pomp>().timerText.text = "";
            ElevatorScript.U(true);
            GameObject.Find("NoYCTPNotebook2").SetActive(false);
            FindObjectOfType<PlayerScript>().SetPosition(new Vector3(5f, 4f, 0f));
            FindObjectOfType<RandomEvent>().PauseRandomEvents();
            if (FindObjectOfType<ClinicRoom>())
            {
                FindObjectOfType<ClinicRoom>().enabled = false;
            }
            if (FindObjectOfType<ClinicHammerTrigger>())
            {
                FindObjectOfType<ClinicHammerTrigger>().enabled = false;
            }
            ChangeToStealthItems();
            for (int i = 0; i < 14; i++)
            {
                if (i != 2)
                {
                    baldi.gameController.cheatingConsole.SelectTMPText(baldi.gameController.enableTexts[i]);
                    FindObjectOfType<CheatScript>().SetCharacterState(i, false);
                    FindObjectOfType<CheatScript>().characters[i].AddComponent<DisableOnLoad>();
                }
            }
            for (int i = 0; i < 18; i++)
            {
                FindObjectOfType<CheatScript>().CloneCharacter(2);
            }
            for (int i = 0; i < FindObjectsOfType<SwingingDoorScript>().Length; i++)
            {
                FindObjectsOfType<SwingingDoorScript>()[i].SkipRequirement = true;
            }
            SwingingDoorDarkScript SWD = FindObjectOfType<SwingingDoorDarkScript>();
            if (SWD != null) SWD.SkipRequirement = true;
        }
        else if (currentType == "Grapple")
        {
            // FindObjectOfType<GameControllerScript>().gameObject.AddComponent<DisableSaveSystem>();
            baldi.gameController.GetItem(0, 20);
            baldi.gameController.player.walkSpeed = 0f;
            baldi.gameController.player.runSpeed = 0f;
            baldi.gameController.player.stamina = -5f;
            baldi.gameController.player.staminaRate = 0f;
            baldi.gameController.warning.color = new Color(150f / 255f, 10f / 255f, 0f, 1f);
            GameObject.Find("NoYCTPNotebook2").SetActive(false);
            for (int i = 0; i < 14; i++)
            {
                if (i == 11) // Bully
                {
                    baldi.gameController.cheatingConsole.SelectTMPText(baldi.gameController.enableTexts[i]);
                    FindObjectOfType<CheatScript>().SetCharacterState(i, false);
                    FindObjectOfType<CheatScript>().characters[i].AddComponent<DisableOnLoad>();
                }
            }
        }
        else if (currentType == "Speedy")
        {
            baldi.gameController.maxNotebooks = 9;
            baldi.gameController.UpdateNotebookCount();
            baldi.gameController.spoopMode = true;
            baldi.gameController.baldiTutor.SetActive(false);
            baldi.gameObject.SetActive(true);
            baldi.gameController.schoolMusic.mute = true;
            baldi.gameController.learnMusic.mute = true;
            baldi.gameController.player.walkSpeed = 70f;
            FindObjectOfType<PlayerScript>().SetPosition(new Vector3(5f, 4f, 0f));
            FindObjectOfType<RandomEvent>().PauseRandomEvents();
            baldi.gameController.player.runSpeed = 100f;
            baldi.GetAngry(99f);
            for (int i = 0; i < 14; i++)
            {
                if (i != 0)
                {
                    baldi.gameController.cheatingConsole.SelectTMPText(baldi.gameController.enableTexts[i]);
                    FindObjectOfType<CheatScript>().SetCharacterState(i, false);
                    FindObjectOfType<CheatScript>().characters[i].AddComponent<DisableOnLoad>();
                }
            }
            baldi.gameController.mrsPomp.GetComponent<Pomp>().timerText.text = "";
            for (int i = 0; i < FindObjectsOfType<SwingingDoorScript>().Length; i++)
            {
                FindObjectsOfType<SwingingDoorScript>()[i].SkipRequirement = true;
            }
            SwingingDoorDarkScript SWD = FindObjectOfType<SwingingDoorDarkScript>();
            if (SWD != null) SWD.SkipRequirement = true;
            if (FindObjectOfType<ClinicRoom>())
            {
                FindObjectOfType<ClinicRoom>().enabled = false;
            }
            if (FindObjectOfType<ClinicHammerTrigger>())
            {
                FindObjectOfType<ClinicHammerTrigger>().Hammer.SetActive(false);
                FindObjectOfType<ClinicHammerTrigger>().enabled = false;
            }
            FindObjectOfType<GameControllerScript>().gameObject.AddComponent<DisableSaveSystem>();
            if (InputManager.usingTouch && GameObject.Find("SaveButton")) GameObject.Find("SaveButton").SetActive(false);
        }
    }

    private void ChangeToStealthItems()
    {
        string[] Names = new string[] {
            "Pickup_YellowDoorLock",
            "Pickup_Teleporter",
            "Pickup_Pencil",
            "Pickup_Apple",
            "Pickup_AlarmClock",
            "Pickup_CoinDoor",
            "Pickup_Health",
            "Pickup_EnergyFlavoredZestyBar",
            "Pickup_EnergyFlavoredZestyBar",
            "Pickup_EnergyFlavoredZestyBar",
            "Pickup_EnergyFlavoredZestyBar",
            "Pickup_Key",
            "Pickup_SafetyScissors",
            "Pickup_SafetyScissors",
            "Pickup_SafetyScissors",
            "Pickup_Tape",
            "Pickup_WD-3D",
            "Pickup_WD-3D",
            "Pickup_Quarter",
            "Pickup_Boots",
            "Pickup_BSODA",
            "Pickup_BSODA",
            "Pickup_BSODA",
            "Pickup_Apple",
            "Pickup_Apple",
            "Pickup_AppleGod",
            "Pickup_Whistle",
            "Pickup_HammerReflexOrigin"
        };
        foreach (string name in Names)
        {
            ChangeItem(GameObject.Find(name));
        }
    }

    public void ChangeItem(GameObject item)
    {
        if (item == null) return;
        int RandomChance = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f));
        if (RandomChance == 0)
        {
            item.name = "Pickup_Chalk";
            item.GetComponentInChildren<SpriteRenderer>().sprite = FindObjectOfType<GameControllerScript>().itemSprites[23];
        }
        else
        {
            item.name = "Pickup_InvisibleItem";
            item.GetComponentInChildren<SpriteRenderer>().sprite = FindObjectOfType<GameControllerScript>().itemSprites[19];
        }
    }
}