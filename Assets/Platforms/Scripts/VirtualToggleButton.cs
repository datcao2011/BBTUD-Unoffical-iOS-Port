using UnityEngine;
using UnityEngine.EventSystems;

namespace Platforms
{
    public class VirtualToggleButton : MonoBehaviour, IPointerDownHandler
    {
        public string buttonName;

        [HideInInspector]
        public bool isToggled;

        private void Start()
        {

        }

        public virtual void OnPointerDown(PointerEventData data)
        {
            isToggled = !isToggled;
            MobileInput.SetTriggerButton(buttonName, isToggled);
        }
    }
}