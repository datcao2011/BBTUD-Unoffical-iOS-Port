using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HoverText : MonoBehaviour
{
    public NewButton[] buttons;

    public string[] texts;

    public TMP_Text text;

    private void Update()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            if (buttons[i].isHighlighted)
            {
                text.text = texts[i];
            }
        }
    }
}