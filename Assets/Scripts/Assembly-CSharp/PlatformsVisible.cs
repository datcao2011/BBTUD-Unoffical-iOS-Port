using UnityEngine;
using Platforms;

public class PlatformsVisible : MonoBehaviour
{
    [Header("Visible Buttons")]
    [Tooltip("Visible to all platforms")]
    public bool anyPlatform = true;

    [Header("Specific Platforms")]
    public bool windows = true;

    public bool macOSX = true;

    public bool linux = true;

    public bool android = true;

    public bool iOS = true;

    public bool xboxOne = true;

    private void Update()
    {
        if (anyPlatform)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (windows && CurrentDevice.currentPlatform == PFPlatform.Windows)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (macOSX && CurrentDevice.currentPlatform == PFPlatform.MacOS)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (linux && CurrentDevice.currentPlatform == PFPlatform.Linux)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (android && CurrentDevice.currentPlatform == PFPlatform.Android)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (iOS && CurrentDevice.currentPlatform == PFPlatform.iPhone)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (xboxOne && CurrentDevice.currentPlatform == PFPlatform.XboxOne)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}