using UnityEngine;
using UnityEngine.UI;
using Platforms;
using TMPro;

public class CampingPrizeScript : MonoBehaviour
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

    private int low;

    private int medium;

    private int high;

    private void Start()
    {
        low = lowPrizeIndex[Random.Range(0, lowPrizeIndex.Length - 1)];
        medium = mediumPrizeIndex[Random.Range(0, mediumPrizeIndex.Length - 1)];
        high = highPrizeIndex[Random.Range(0, highPrizeIndex.Length - 1)];
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
        CampingScript camp = FindObjectOfType<CampingScript>();
        if (camp.camping.score > 10000f)
        {
            starImages[0].sprite = starLit;
            Settings.SetInt("ItemsCollected", Settings.GetInt("ItemsCollected") + 1);
            Settings.SetInt("ItemCollected" + (Settings.GetInt("ItemsCollected", 0) - 1).ToString(), medium);
            receiveCoins += Random.Range(10, 21);
        }
        int finalScore = Mathf.RoundToInt(camp.camping.score);
        if (finalScore > Settings.GetInt("HighCampScore"))
        {
            starImages[1].sprite = starLit;
            if (!CheatScript.cheated)
            {
                Settings.SetInt("HighCampScore", finalScore);
                Settings.SetInt("ItemsCollected", Settings.GetInt("ItemsCollected") + 1);
                Settings.SetInt("ItemCollected" + (Settings.GetInt("ItemsCollected", 0) - 1).ToString(), high);
                receiveCoins += Random.Range(100, 151);
            }
        }
        if (!CheatScript.cheated && Settings.HasKey("Account"))
        {
            if (!AccountManager.hasInstance())
            {
                Debug.LogError($"There was no account instance available to sync scores. If you care about your stats, please screenshot and send this to SWA:\nHIGH={finalScore}\nTIME REMAIN={camp.camping.timer}\nWHEN={System.DateTime.Now}\nUSER={Settings.GetString("Account")}");
                InputManager.DisplayEvent("Uh oh! Your scores couldn't be submitted. If your scores value enough, open your logs.", Color.red, 10f, (eve) =>
                {
                    eve.Disappear();
                    FindObjectOfType<ConsoleTraceInGame>().show = true;
                }, "Open Logs");
            }
            else
            {
                AccountManager.SubmitCampingScore(finalScore, camp.camping.timer);
            }
        }
        float bestTime = Settings.GetInt("HighCampScore");
        timeText.text = finalScore.ToString();
        bestText.text = Mathf.RoundToInt(bestTime).ToString();
        coinsText.text = "+" + receiveCoins + " coins.";
        int currentCoins = CoinSaveSystem.Load();
        CoinSaveSystem.Save(currentCoins + receiveCoins);
    }
}