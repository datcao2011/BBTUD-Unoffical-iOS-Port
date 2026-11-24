using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class AdvancedOptionHelp : MonoBehaviour
{
    public TMP_Text helpText;

    public void Button_OnClick(string help)
    {
        helpText.text = Regex.Unescape(help);
    }
}