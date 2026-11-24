using UnityEngine;
using UnityEngine.EventSystems;

namespace Platforms
{
    public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public string buttonName;

        [HideInInspector]
        public bool isPressed;

        private void Start()
        {

        }

        public virtual void OnPointerDown(PointerEventData data)
        {
            MobileInput.SetTriggerButton(buttonName, true);
            isPressed = true;
        }

        public virtual void OnPointerUp(PointerEventData data)
        {
            MobileInput.SetTriggerButton(buttonName, false);
            isPressed = false;
        }
    }
}