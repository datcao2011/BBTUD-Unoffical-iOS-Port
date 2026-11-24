using UnityEngine;
using TMPro;

public class CampingScore : MonoBehaviour
{
    public TMP_Text text;

    private void Start()
    {
        text.text = "High Score:\n" + Settings.GetInt("HighCampScore");
    }
}