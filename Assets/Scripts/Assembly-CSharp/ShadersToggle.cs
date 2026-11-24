using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadersToggle : MonoBehaviour
{
    private Toggle toggle;

    public string KeyName;

    // Start is called before the first frame update
    private void Start()
    {
        toggle = GetComponent<Toggle>();
        if (KeyName == "OcclusionCulling")
        {
            toggle.isOn = Settings.GetBool(KeyName, GameOptimizer.OptimizationDefault);
        }
        else
        {
            toggle.isOn = Settings.GetBool(KeyName, toggle.isOn);
        }
        toggle.onValueChanged.AddListener(ValueChanged);
    }

    private void ValueChanged(bool val)
    {
        Settings.SetBool(KeyName, val);
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
