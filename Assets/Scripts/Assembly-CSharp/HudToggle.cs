using System;
using UnityEngine;
using UnityEngine.UI;

public class HudToggle : MonoBehaviour
{
    public string Setting = "Hud";
    public bool IsTypeHud = false;
    public int DefaultVal = 1;

    private void Start()
    {
        if (IsTypeHud)
        {
            Dropdown.value = Settings.GetInt(Setting, DefaultVal);
            Dropdown.onValueChanged.AddListener(val =>
            {
                Settings.SetInt(Setting, val);
            });
            return;
        }
        hudEnabled = Settings.GetString(Setting, "true");
        if (hudEnabled == "true")
        {
            toggle.isOn = true;
        }
        else if (hudEnabled == "false")
        {
            toggle.isOn = false;
        }
        toggle.onValueChanged.AddListener(val =>
        {
            if (IsTypeHud) return;
            Settings.SetString(Setting, val ? "true" : "false");
        });
    }

    public Toggle toggle;

    public TMPro.TMP_Dropdown Dropdown;

    private string hudEnabled;
}
