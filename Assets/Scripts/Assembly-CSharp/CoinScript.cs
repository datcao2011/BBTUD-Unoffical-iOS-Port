using UnityEngine;
using TMPro;
using System.IO;
using System.IO.Compression;

public class CoinScript : MonoBehaviour
{
    public GameObject box;

    public TMP_Text titleText;

    public TMP_Text text;

    public TMP_Text okButton;

    public TMP_Text cancelButton;

    public TMP_Text coinsText;

    private void Start()
    {
        int coins = CoinSaveSystem.Load();
        coinsText.text = "Coins: " + AbbreviateNumber(coins);
    }

    public int coins;

    private void LateUpdate()
    {
        // CoinData data = CoinSaveSystem.Load();
        // if (data == null)
        // {
        //     coinsText.text = "Error while loading coins, please restart the game.";
        // }
        // else
        // {
        //     coins = data.coins;
        //     coinsText.text = "Coins: " + coins;
        // }
    }

    public bool Buy(string name, int cost)
    {
        CreateTextBox("Go to shop?", "Purchase the mystery door on the shop?", "Yes/visit shop", "No");
        return false;
    }

    public void CreateTextBox(string title, string text, string okBox, string cancelBox)
    {
        box.SetActive(true);
        titleText.text = title;
        this.text.text = text;
        okButton.text = okBox;
        okButton.transform.parent.gameObject.SetActive(true);
        cancelButton.text = cancelBox;
        StartCoroutine(coinProcess("visit", 0));
    }

    private bool processing;

    private bool accept;

    private System.Collections.IEnumerator coinProcess(string name, int cost)
    {
        processing = true;
        while (processing)
        {
            yield return null;
        }
        if (accept)
        {
            accept = false;
            FindObjectOfType<MainMenuScript>().GoToShop();
        }
        yield break;
    }

    public void Accept()
    {
        accept = true;
        HideBox();
    }

    public void HideBox()
    {
        processing = false;
        box.SetActive(false);
    }

    public string AbbreviateNumber(float num)
    {
        if (num >= 1000000)
            return (Mathf.Floor(num / 100000) / 10).ToString("0.0") + "M";
        else if (num >= 1000)
            return (Mathf.Floor(num / 100) / 10).ToString("0.0") + "K";
        else
            return num.ToString("0");
    }
}
