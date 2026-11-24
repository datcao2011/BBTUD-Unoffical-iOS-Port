// To use this script, download https://assetstore.unity.com/packages/tools/input-management/touch-controls-kit-lite-41395 to your project
// then once installed, update '#if false' to '#if true'
#if false
using UnityEngine;
using TouchControlsKit;

namespace Platforms
{
    public class TouchpadAxisScript : MonoBehaviour
    {
        private TCKTouchpad pad;


        [Tooltip("Axis name is based on your platforms input manager.")]
        public string xAxisName;
        [Tooltip("Axis name is based on your platforms input manager.")]
        public string yAxisName;

        private void Start()
        {
            pad = GetComponent<TCKTouchpad>();
            if (!pad)
            {
                pad = gameObject.AddComponent<TCKTouchpad>();
                pad.sensitivity = 1f;
            }
        }

        private void Update()
        {

            MobileInput.SetAxis(xAxisName, pad.axisX.value);
            MobileInput.SetAxis(yAxisName, pad.axisY.value);
        }
    }
}
#endif