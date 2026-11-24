using UnityEngine;
using UnityEngine.UI;
using Platforms;
using TMPro;

public class AnimalPrizes : MonoBehaviour
{
    public int[] lowPrizeIndex;

    public int[] mediumPrizeIndex;

    public int[] highPrizeIndex;

    public Image[] itemPrizes;

    public Sprite starLit;

    public Image[] starImages;

    public TMP_Text title;

    public TMP_Text exitText;

    public TMP_Text timeText;

    public TMP_Text bestText;

    public TMP_Text coinsText;

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
        PlayerPrefs.SetInt("ItemsCollected", PlayerPrefs.GetInt("ItemsCollected") + 1);
        PlayerPrefs.SetInt("ItemCollected" + (PlayerPrefs.GetInt("ItemsCollected", 0) - 1).ToString(), low);
        receiveCoins = Random.Range(3, 10);
        AnimalFarm farm = FindObjectOfType<AnimalFarm>();
        if (farm.round >= farm.maxRounds)
        {
            title.text = "You have won the game!";
            starImages[0].sprite = starLit;
            PlayerPrefs.SetInt("ItemsCollected", PlayerPrefs.GetInt("ItemsCollected") + 1);
            PlayerPrefs.SetInt("ItemCollected" + (PlayerPrefs.GetInt("ItemsCollected", 0) - 1).ToString(), medium);
            receiveCoins += Random.Range(10, 21);
        }
        if (PlayerPrefs.GetFloat("FarmBestScore") > farm.score || PlayerPrefs.GetFloat("FarmBestScore") == 0f)
        {
            starImages[1].sprite = starLit;
            if (!CheatScript.cheated)
            {
                PlayerPrefs.SetFloat("FarmBestScore", farm.score);
                PlayerPrefs.SetInt("ItemsCollected", PlayerPrefs.GetInt("ItemsCollected") + 1);
                PlayerPrefs.SetInt("ItemCollected" + (PlayerPrefs.GetInt("ItemsCollected", 0) - 1).ToString(), high);
                receiveCoins += Random.Range(100, 151);
            }
        }
        if (!CheatScript.cheated && Settings.HasKey("Account"))
        {
            if (!AccountManager.hasInstance())
            {
                Debug.LogError($"There was no account instance available to sync scores. If you care about your stats, please screenshot and send this to SWA:\nHIGH={farm.score}\nTIME={Time.timeSinceLevelLoad}\nWHEN={System.DateTime.Now}\nUSER={Settings.GetString("Account")}");
                InputManager.DisplayEvent("Uh oh! Your scores couldn't be submitted. If your scores value enough, open your logs.", Color.red, 10f, (eve) =>
                {
                    eve.Disappear();
                    FindObjectOfType<ConsoleTraceInGame>().show = true;
                }, "Open Logs");
            }
            else
            {
                AccountManager.SubmitFarmScore(farm.score, Time.timeSinceLevelLoad);
            }
        }
        float bestTime = PlayerPrefs.GetFloat("FarmBestScore");
        timeText.text = farm.score.ToString("0");
        bestText.text = bestTime.ToString("0");
        coinsText.text = "+" + receiveCoins + " coins.";
        int currentCoins = CoinSaveSystem.Load();
        CoinSaveSystem.Save(currentCoins + receiveCoins);
    }
}