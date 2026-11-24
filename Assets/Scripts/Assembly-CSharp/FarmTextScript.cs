// EndlessTextScript
using UnityEngine;
using TMPro;

public class FarmTextScript : MonoBehaviour
{
    public TMP_Text text;

    public string playerPrefName = "FarmBestTime";
    public string title = "Best Time";

    private void Start()
    {
        float bestTime = Settings.GetFloat(playerPrefName);
        text.text = title + ":\n" + bestTime.ToString("0");
    }

    private void Update()
    {
    }
}