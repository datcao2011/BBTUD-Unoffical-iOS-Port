using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystemStopper : MonoBehaviour
{
    public static bool isOn;
    public static bool Enabled;

    private void Update()
    {
        if (isOn)
        {
            if (!MultiplayerManager.isActive) Time.timeScale = 0f;
            CursorControllerScript.UnlockCursor();
        }
    }

    private void OnDestroy()
    {
        isOn = false;
    }
}