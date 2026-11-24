// To use this script, download https://assetstore.unity.com/packages/tools/input-management/joystick-pack-107631 to your project
// then once installed, update '#if false' to '#if true'
#if true
using UnityEngine;

namespace Platforms.Extensions.JSP
{
    public class JoystickAxisScript : MonoBehaviour
    {
        [Tooltip("Axis name is based on your platforms input manager.")]
        public string xAxisName;
        [Tooltip("Axis name is based on your platforms input manager.")]
        public string yAxisName;

        private Joystick joystick;

        private void Start()
        {
            joystick = GetComponent<Joystick>();
            if (!joystick)
            {
                joystick = gameObject.AddComponent<Joystick>();
            }
        }

        private void Update()
        {
            MobileInput.SetAxis(xAxisName, joystick.Horizontal);
            MobileInput.SetAxis(yAxisName, joystick.Vertical);
        }
    }
}
#endif