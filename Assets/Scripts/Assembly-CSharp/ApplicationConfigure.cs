#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Platforms;

public class ApplicationConfigure : MonoBehaviour
{

    private static bool CanQuit;

    public static bool CanQuitApp()
    {
        return CanQuit;
    }

    public void openUrlTo(string url)
    {
        Application.OpenURL(url);
        console.log("Opened URL: " + url);
    }

    public void closeProgram()
    {
        closeApplication();
    }

    public static void closeApplication()
    {
        CanQuit = true;
#if !UNITY_WEBGL
        Application.Quit();
        if (CurrentDevice.currentPlatform == PFPlatform.Windows)
        {
            CurrentDevice.StopVibrateController();
        }
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        console.log("Application quitted.");
#else
        console.error("Quit isn't supported on WebGL. Restarting instead.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameRestarter");
#endif
    }

    public static void SetCanQuit(bool value)
    {
        CanQuit = value;
    }
}
