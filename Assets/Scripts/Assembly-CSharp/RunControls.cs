using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Platforms;

public class RunControls : MonoBehaviour
{
    public Image toggleRun;

    public VirtualToggleButton toggler;

    public bool running;

    private void LateUpdate()
    {
        running = toggler.isToggled;
        if (running)
        {
            toggleRun.color = Color.red;
        }
        else
        {
            toggleRun.color = Color.white;
        }
    }
}
