using TMPro;
using UnityEngine;

public class ModVersionCheck : MonoBehaviour
{
    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = "V" + VersionInput.currentVersion.ToString().Replace(".0", "");
    }
}