// EndlessTextScript
using UnityEngine;
using TMPro;

public class EndlessTextScript : MonoBehaviour
{
    public TMP_Text text;

    public string Scene = "School";

    private void Start()
    {
        text.text = "Endless Mode:\nCollect as many notebooks as you can!\nHigh Score:\n" + Settings.GetInt("HighBooks_" + Scene) + " Notebooks";
    }

    private void Update()
    {
    }
}