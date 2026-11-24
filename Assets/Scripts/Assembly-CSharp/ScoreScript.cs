// ScoreScript
using UnityEngine;
using TMPro;

public class ScoreScript : MonoBehaviour
{
    public GameObject scoreText;

    public TMP_Text text;

    private void Start()
    {
        if (Settings.GetString("CurrentMode") == "endless")
        {
            scoreText.SetActive(true);
            text.text = "Score: " + Settings.GetInt("CurrentBooks") + " Notebooks \nHigh Score: " + Settings.GetInt("HighBooks_" + Settings.GetString("ModeSelected", "School")) + " Notebooks";
        }
    }

    private void Update()
    {
    }
}