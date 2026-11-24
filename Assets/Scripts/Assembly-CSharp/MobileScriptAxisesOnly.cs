using UnityEngine;
using TouchControlsKit;

public class MobileScriptAxisesOnly : MonoBehaviour
{
    private TCKTouchpad pad;

    float sensitivity;

    private void Start()
    {
        if (!GetComponent<TCKTouchpad>())
        {
            pad = gameObject.AddComponent<TCKTouchpad>();
        }
        else
        {
            pad = GetComponent<TCKTouchpad>();
        }
    }

    private void Update()
    {
        MobileTouchScript.XAxis = pad.axisX.value;
        MobileTouchScript.YAxis = pad.axisY.value;
    }
}
