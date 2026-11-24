using UnityEngine;
using UnityEngine.UI;
using Platforms;
using TMPro;

public class FarmWinPrizeScript : MonoBehaviour
{
    public int[] lowPrizeIndex;

    public int[] mediumPrizeIndex;

    public int[] highPrizeIndex;

    public Image[] itemPrizes;

    public Sprite starLit;

    public Image[] starImages;

    public TMP_Text exitText;

    public TMP_Text timeText;

    public TMP_Text bestText;

    public TMP_Text coinsText;

    public float highTime = 100f;

    private int low;

    private int medium;

    private int high;

    private void Start()
    {
        low = lowPrizeIndex[Random.Range(0, lowPrizeIndex.Length)];
        medium = mediumPrizeIndex[Random.Range(0, mediumPrizeIndex.Length)];
        high = highPrizeIndex[Random.Range(0, highPrizeIndex.Length)];
        itemPrizes[0].sprite = FindObjectOfType<GameControllerScript>().itemSprites[low - 1];
        itemPrizes[1].sprite = FindObjectOfType<GameControllerScript>().itemSprites[medium - 1];
        itemPrizes[2].sprite = FindObjectOfType<GameControllerScript>().itemSprites[high - 1];
        CheckRewards();
    }

    private void Update()
    {
        if (InputManager.isUsingController == true)
            exitText.text = "Press B to exit";
        else if (InputManager.usingTouch)
            exitText.text = "Tap to exit";
        else
            exitText.text = "Press Q to exit";
    }

    private void CheckRewards()
    {
        int receiveCoins;
        Settings.SetInt("ItemsCollected", Settings.GetInt("ItemsCollected") + 1);
        Settings.SetInt("ItemCollected" + (Settings.GetInt("ItemsCollected", 0) - 1).ToString(), low);
        receiveCoins = Random.Range(3, 10);
        FarmScript farm = FindObjectOfType<FarmScript>();
        if (farm.timeInFarm < highTime)
        {
            starImages[0].sprite = starLit;
            Settings.SetInt("ItemsCollected", Settings.GetInt("ItemsCollected") + 1);
            Settings.SetInt("ItemCollected" + (Settings.GetInt("ItemsCollected", 0) - 1).ToString(), medium);
            receiveCoins += Random.Range(10, 21);
        }
        if (Settings.GetFloat("FarmBestTime") > farm.timeInFarm || Settings.GetFloat("FarmBestTime") == 0f)
        {
            starImages[1].sprite = starLit;
            if (!CheatScript.cheated)
            {
                Settings.SetFloat("FarmBestTime", farm.timeInFarm);
                Settings.SetInt("ItemsCollected", Settings.GetInt("ItemsCollected") + 1);
                Settings.SetInt("ItemCollected" + (Settings.GetInt("ItemsCollected", 0) - 1).ToString(), high);
                receiveCoins += Random.Range(100, 151);
            }
        }
        if (!CheatScript.cheated && Settings.HasKey("Account"))
        {
            if (!AccountManager.hasInstance())
            {
                Debug.LogError($"There was no account instance available to sync scores. If you care about your stats, please screenshot and send this to SWA:\nBEST={farm.timeInFarm}\nMAZE ID={FindObjectOfType<FarmGenerator>().MazeID}\nWHEN={System.DateTime.Now}\nUSER={Settings.GetString("Account")}");
                InputManager.DisplayEvent("Uh oh! Your scores couldn't be submitted. If your scores value enough, open your logs.", Color.red, 10f, (eve) =>
                {
                    eve.Disappear();
                    FindObjectOfType<ConsoleTraceInGame>().show = true;
                }, "Open Logs");
            }
            else
            {
                AccountManager.SubmitFarmScore(farm.timeInFarm, FindObjectOfType<FarmGenerator>().MazeID);
            }
        }
        float bestTime = Settings.GetFloat("FarmBestTime");
        timeText.text = farm.timeInFarm.ToString("0");
        bestText.text = bestTime.ToString("0");
        coinsText.text = "+" + receiveCoins + " coins.";
        int currentCoins = CoinSaveSystem.Load();
        CoinSaveSystem.Save(currentCoins + receiveCoins);
    }
}