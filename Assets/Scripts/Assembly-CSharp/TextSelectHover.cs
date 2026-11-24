using UnityEngine;
using TMPro;

public class TextSelectHover : MonoBehaviour
{
    public TMP_Text text;
    private Color regularColor;

    private void Start()
    {
        if (text != null) regularColor = text.color;
    }

    public void Underline()
    {
        text.fontStyle = FontStyles.Underline;
    }
    public void Normal()
    {
        text.color = regularColor;
        text.fontStyle = FontStyles.Normal;
    }
    public void RedColor()
    {
        text.color = Color.red;
    }

    private void OnDisable()
    {
        if (text != null && regularColor != default) Normal();
    }
}
