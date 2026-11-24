using UnityEngine;

// THIS SCRIPT IS TO HELP BEGINNERS
namespace Platforms
{
    public class MobileInputController : MonoBehaviour
    {
        public void SetPositiveAxis(string name)
        {
            MobileInput.SetAxis(name, 1f);
        }

        public void SetNegativeAxis(string name)
        {
            MobileInput.SetAxis(name, -1f);
        }
    }
}