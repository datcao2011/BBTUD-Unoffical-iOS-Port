using UnityEngine.UI;
using UnityEngine;

namespace Platforms
{
    public class PlatformsErrorMessage : MonoBehaviour
    {
        private Text text;

        private float deleteTimer;
        private bool isActive;

        public void StartWork(string message, float time = 15f) // Called from Platforms Scripts
        {
            text.text = message;
            deleteTimer = time;
            isActive = true;
        }

        private void Awake()
        {
            text = transform.GetChild(2).GetComponent<Text>(); // Get text component
        }

        private void Update()
        {
            if (!isActive) return; // If isn't active, don't run code

            if (deleteTimer > 0f) // If the delete timer is more than 0
            {
                deleteTimer -= Time.unscaledDeltaTime; // The timer goes down
            }
            else
            {
                DestroyImmediate(transform.parent.parent.gameObject); // Delete this gameobject immediately
            }
        }

        public void Exit()
        {
            DestroyImmediate(transform.parent.parent.gameObject); // Delete this gameobject immediately
        }
    }
}