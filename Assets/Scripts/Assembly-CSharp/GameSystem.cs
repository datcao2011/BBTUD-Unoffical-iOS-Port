using UnityEngine.SceneManagement;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class GameSystem
{
    private static string[] availableScenes = new string[]
    {
        "BusCamping",
        "CampScene",
        "Credits",
        "Farm",
        "FarmCamping",
        "FarmScene",
        "GameOver",
        "GameOver1",
        "GameRestarter",
        "Loading",
        "Logon",
        "MainMenu",
        "MysteryRoom *",
        "OptionsMenu",
        "RestartError",
        "SaveSystem *",
        "School",
        "School_2",
        "Starter",
        "TestRoom",
        "MM_Connect",
        "MM_Disconnected"
    };

    public static void AbortApplication(string message, string errorcode)
    {
        console.error("================================================================");
        console.error("AN MAJOR ERROR HAS OCCURRED IN THE GAME");
        console.error(" ");
        console.error(message);
        console.error(" ");
        console.error("This application's process has been abondoned by the error. Please contact support for more info.");
        console.error(" ");
        console.error("ERROR CODE: " + errorcode);
        console.error("If this error code continues, please contact support.");
        console.error("================================================================");
        GameAbortedScript.errorMsg = message;
        GameAbortedScript.errorCode = errorcode;
        SceneManager.LoadScene("RestartError");
    }

    public static void SendCommand(string command)
    {
        string[] temp = command.Split(' ');
        string baseCommand = temp[0];
        string[] args = shiftString(temp);

        switch (baseCommand)
        {
            case "console":
                ConsoleCommand(args);
                break;
            case "scene":
                SceneManage(args);
                break;
            case "window":
            case "screen":
                Debug.Log($"{Screen.width}x{Screen.height} {Screen.currentResolution.refreshRateRatio.value} Hz (FullScreenMode:{Screen.fullScreenMode} IsFullScreen:{Screen.fullScreen})");
                return;
            case "crash":
                UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.AccessViolation);
                break;
            // case "":
            //     Object.FindObjectOfType<ConsoleTraceInGame>().show = false;
            //     AbortApplication("Click Between Frames is illegitimate and will not be allowed for use in Baldi's Basics The Ultra Decompile\nPlease disable this mod to continue playing BBTUD.", "CBF detected, loser!");
            //     break;
            case "help":
                Debug.Log("Commands:\n\nconsole - Access to console commands\nscene - Scene Manager");
                break;
            case "access":
            case "set":
                break;
            case "easymode":
#if UNITY_ANDROID
                Debug.Log("Easy mode for NULL is enabled. WARNING: Ending will be modified.");
                NullScript.EasyMode = true;
#else
                Debug.LogError("Easy mode is only available on Android.");
#endif
                break;
            default:
                Debug.LogError("The command " + baseCommand + " does not exist. Try typing 'help' for commands.");
                break;
        }
        switch (command.Trim().ToLower())
        {
            case "access register alpha-access":
            case "access register beta-access":
                Debug.LogWarning("There is no beta register access.");
                break;
            case "access unregister beta-access":
            case "access unregister alpha-access":
            case "access unregister":
                System.IO.File.Delete(Application.persistentDataPath + "/config.ini");
                Debug.Log("All beta and alpha access unregistered. Please restart the game to apply changes.");
                break;
            case "set mp-preloadlevels true":
                Settings.SetBool("MP-PreloadLevels", true);
                Debug.Log("Multiplayer levels will be pre-loaded from now on.");
                break;
            case "set mp-preloadlevels false":
                Settings.SetBool("MP-PreloadLevels", false);
                Debug.Log("Multiplayer levels will be loaded at game start from now on.");
                break;
        }
    }

    private static bool HasSceneLoaded(string scene)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == scene)
            {
                return true;
            }
        }
        return false;
    }

    private static void SceneManage(string[] args)
    {
        if (MultiplayerManager.isActive)
        {
            Debug.LogError("Scene Manager isn't available.");
            return;
        }
        switch (args[0])
        {
            case "load":
                if (args.Length <= 1)
                {
                    Debug.LogError("Please type a scene to load.\n\nExample when unloading all loaded scenes and loading an new scene: 'scene load TestRoom'\nExample when loading an scene when the active scene still loaded: 'scene load OptionsMenu add'");
                }
                string scene = args[1];

                console.log("Attempting to load scene " + scene);

                if (args.Length >= 3 && args[2] == "add")
                {
                    Debug.Log("Loading scene " + scene + "...");
                    Debug.Log("Loaded scenes will not unload since you're adding a scene to the scene.");
                    AsyncOperation operation2 = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                    return;
                }

                Debug.Log("Loading scene " + scene + "...");
                Debug.Log("Scene is loading on the background. You can still play the game while the scene is loading.");
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

                break;
            case "unload":
                if (args.Length <= 1)
                {
                    Debug.LogError("Please type a scene to unload.");
                }
                string scene3 = string.Join(" ", shiftString(args));

                console.log("Attempting to unload scene " + scene3);

                if (!HasSceneLoaded(scene3))
                {
                    Debug.LogWarning("The scene '" + scene3 + "' is not loaded.");
                    return;
                }

                Debug.Log("Unloading scene " + scene3 + "...");
                AsyncOperation operation3 = SceneManager.UnloadSceneAsync(scene3);

                break;
            case "list":
                Debug.Log("=================== CURRENT LOADED SCENES ===================");
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Debug.Log(SceneManager.GetSceneAt(i).name);
                }
                Debug.Log("====================== AVAILABLE SCENES =====================");
                Debug.LogWarning("* Stable only if added a scene");
                for (int i = 0; i < availableScenes.Length; i++)
                {
                    Debug.Log(availableScenes[i]);
                }
                break;
            case "help":
                Debug.Log("Scene Commands:");
                Debug.Log("scene list - Lists loaded scenes and available scenes to load.");
                Debug.Log("scene load <scenename> [add]? - Load a scene. add (optional): Adding a scene to the scene instead of unloading all scenes and loading the new scene.");
                Debug.Log("scene unload <loadedscenename> - Unloads a loaded scene.");
                break;
            default:
                Debug.LogError("Unknown scene command. Try typing 'scene help'");
                break;
        }
    }

    private static void ConsoleCommand(string[] args)
    {
        switch (args[0])
        {
            case "log":
                if (args.Length <= 1)
                {
                    Debug.LogError("Please type a message to log to the console. Example: 'console log Hello world!'");
                }
                string message = string.Join(" ", shiftString(args));
                Debug.Log(message);
                break;
            default:
                Debug.LogError("Console commands currently available are\n\nconsole log <message>");
                break;
        }
    }

    private static string[] shiftString(string[] arr)
    {
        string[] newArr = new string[arr.Length - 1];
        // Shifting elements
        for (int i = 0; i < arr.Length - 1; i++)
        {
            newArr[i] = arr[i + 1];
        }
        return newArr;
    }
}