using UnityEngine;

namespace Platforms
{
    public class InformationNotice : MonoBehaviour
    {
        [TextArea(1, 20)]
        public string information = "There's no editor information here.";
    }
}