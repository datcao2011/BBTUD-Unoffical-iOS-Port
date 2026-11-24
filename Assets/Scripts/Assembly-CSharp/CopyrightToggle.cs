using UnityEngine;
using UnityEngine.UI;

public class CopyrightToggle : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Toggle>().isOn = Settings.GetBool("CopyrightMusic", true);
        GetComponent<Toggle>().onValueChanged.AddListener((val) => {
            Settings.SetBool("CopyrightMusic", val);
        });
    }
}