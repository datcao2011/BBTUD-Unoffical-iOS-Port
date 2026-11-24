using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinOrLoseScreen : MonoBehaviour
{
    public TMP_Text progressText;
    public GameObject Button;

    public int[] lowPrizeIndex;

    public int[] mediumPrizeIndex;

    public int[] highPrizeIndex;
    
    public bool win;

    public Image[] itemPrizes;

    public Sprite starLit;

    public Image[] starImages;

    public Sprite[] items;

    public TMP_Text coinsText;

    private int low;

    private int medium;

    private int high;

    private void Start()
    {
        low = lowPrizeIndex[Random.Range(0, lowPrizeIndex.Length)];
        medium = mediumPrizeIndex[Random.Range(0, mediumPrizeIndex.Length)];
        high = highPrizeIndex[Random.Range(0, highPrizeIndex.Length)];
        itemPrizes[0].sprite = items[low - 1];
        itemPrizes[1].sprite = items[medium - 1];
        itemPrizes[2].sprite = items[high - 1];
        CheckRewards();
    }

    private void CheckRewards()
    {
        int receiveCoins;
        PlayerPrefs.SetInt("ItemsCollected", PlayerPrefs.GetInt("ItemsCollected") + 1);
        PlayerPrefs.SetInt("ItemCollected" + (PlayerPrefs.GetInt("ItemsCollected", 0) - 1).ToString(), low);
        receiveCoins = Random.Range(3, 10);
        if (win)
        {
            starImages[0].sprite = starLit;
            PlayerPrefs.SetInt("ItemsCollected", PlayerPrefs.GetInt("ItemsCollected") + 1);
            PlayerPrefs.SetInt("ItemCollected" + (PlayerPrefs.GetInt("ItemsCollected", 0) - 1).ToString(), medium);
            receiveCoins += Random.Range(10, 21);
        }
        if (PlayerPrefs.GetInt("HAS_PWins") >= 3)
        {
            starImages[1].sprite = starLit;
            PlayerPrefs.SetInt("ItemsCollected", PlayerPrefs.GetInt("ItemsCollected") + 1);
            PlayerPrefs.SetInt("ItemCollected" + (PlayerPrefs.GetInt("ItemsCollected", 0) - 1).ToString(), high);
            receiveCoins += Random.Range(100, 151);
        }
        coinsText.text = "+" + receiveCoins + " coins.";
        int currentCoins = CoinSaveSystem.Load();
        CoinSaveSystem.Save(currentCoins + receiveCoins);
    }

    public void MainMenu()
    {
        Button.SetActive(false);
        progressText.gameObject.SetActive(true);
        progressText.text = "Loading";
        SceneManager.LoadSceneAsync("Starter");
    }
}