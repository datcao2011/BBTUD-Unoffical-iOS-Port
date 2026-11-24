using UnityEngine;
using TMPro;
using System.Collections;

public class HudScreenTextScript : MonoBehaviour
{
    public TMP_Text hudText;

    public GameObject hudScreen;

    public void ShowHudText(float time, string text)
    {
        hudText.alignment = text.Contains("Bell Event") ? TextAlignmentOptions.Top : TextAlignmentOptions.Center;
        if (hudScreen.activeSelf)
        {
            hudText.text = text;
            StopAllCoroutines();
            StartCoroutine(delayStop(time));
            return;
        }
        hudText.text = text;
        hudScreen.SetActive(true);
        StartCoroutine(delayStop(time));
    }

    public void ShowHudText(string text)
    {
        hudText.text = text;
        hudScreen.SetActive(true);
    }

    public void HideHudText()
    {
        hudScreen.SetActive(false);
        hudText.text = string.Empty;
    }

    private IEnumerator delayStop(float time)
    {
        yield return new WaitForSeconds(time);
        HideHudText();
        yield break;
    }
}