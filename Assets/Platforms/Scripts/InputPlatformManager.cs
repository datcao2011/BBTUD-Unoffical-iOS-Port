// Importing namespaces
using UnityEngine;
using Platforms.Console.Logger;

// On namespace Platforms
namespace Platforms
{
    [DisallowMultipleComponent] // Make sure that no same components are in this GameObject
    [ExecuteInEditMode] // This is like running the functions in the game, but in the editor.
    public class InputPlatformManager : MonoBehaviour
    {
        // This will run when the game is on play mode.
        private void Awake()
        {
            if (Application.isPlaying)
            {
                if (FindObjectsOfType<InputPlatformManager>().Length >= 2)
                {
                    DestroyImmediate(gameObject);
                    return;
                }
            }
        }

        private void Start()
        {
            // Adding required components.
            if (!GetComponent<GetAxisScript>())
            {
                gameObject.AddComponent<GetAxisScript>();
            }
            if (!GetComponent<PlatformAxisInput>())
            {
                gameObject.AddComponent<PlatformAxisInput>();
            }

            /*
            THE FOLLOWING BELOW USES Application.isPlaying WHICH MEANS THE FUNCTION WILL RUN IN PLAY MODE
            */
            if (Application.isPlaying) // IF the application is playing
            {
                // INSTANCE CHECK
                if (instance != null & instance != this)
                {
                    Destroy(gameObject);
                    return;
                }
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
                // INSTANCE CHECK

                if (showErrorMessagesInGame) // Displaying Platforms error messages on Screen
                {
                    /* ONLY APPEARS IN EDITOR */ if (Application.isEditor) ConsoleLogger.Log("In-game Platforms Error Messages are enabled. If any error was caught in platforms, it will also display in the game.");
                    ConsoleLogger.exceptionReceived += CreateErrorMessage;
                }
                instance = this; // Make the instance this

                // Input check
                PlatformAxisInput Input = GetComponent<PlatformAxisInput>();
                foreach (PlatformAxises axis in Input.axises)
                {
                    axis.positiveDefaultKey = axis.positiveAxisKey;
                    axis.negativeDefaultKey = axis.negativeAxisKey;
                    axis.defaultControllerBumpers = axis.controllerBumpers;
                    axis.defaultControllerButtons = axis.controllerButtons;
                    axis.defaultControllerDPad = axis.controllerDPad;
                    axis.defaultControllerStick = axis.controllerStick;
                    axis.defaultControllerTrigger = axis.controllerAxisTrigger;
                    axis.defaultMouseButton = axis.mouseButton;
                    axis.defaultMouseScrollWheel = axis.mouseScrollWheel;
                }
            }
        }

        private static InputPlatformManager instance; // Instance for static purposes
        [Tooltip("Do not destroy the GameObject when switching scenes.")]
        public bool dontDestroyOnLoad; // Do not destroy when a new scene is loading
        [Tooltip("Show the platforms error messages on the game.")]
        public bool showErrorMessagesInGame = true; // See CreateErrorMessage function for details.

        private void OnDestroy() // WHEN THE GAMEOBJECT IS DESTROYED
        {
            if (!Application.isPlaying) return; // THE GAME MUST BE ON PLAY MODE WHEN GETTING DESTROYED
            ConsoleLogger.exceptionReceived -= CreateErrorMessage;
        }

        public void CreateErrorMessage(System.Exception exception) // (USED BY ConsoleLogger.exceptionReceived) Creates error message on the screen for debug purposes. You can turn this off by unchecking Show Error Messages In Game (in the Inspector).
        {
            if (Resources.Load("Error Message") == null)
            {
                ConsoleLogger.LogError($"The following file doesn't exist: {Application.dataPath.Replace(System.IO.Path.DirectorySeparatorChar, '/')}/Platforms/Resources/Error Message.prefab");
                return;
            }
            GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Error Message"));

            Canvas canvas = gameObject.GetComponentInChildren<Canvas>();
            PlatformsErrorMessage errorMessage = canvas.GetComponentInChildren<PlatformsErrorMessage>();
            errorMessage.StartWork(exception.Message);
        }

        public void CreateErrorMessage(System.Exception exception, float timer) // (FOR GLOBAL SCRIPTS) Creates error message on the screen for debug purposes. You can turn this off by unchecking Show Error Messages In Game (in the Inspector).
        {
            if (Resources.Load("Error Message") == null)
            {
                ConsoleLogger.LogError($"The following file doesn't exist: {Application.dataPath.Replace(System.IO.Path.DirectorySeparatorChar, '/')}/Platforms/Resources/Error Message.prefab");
                return;
            }
            GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Error Message"));

            Canvas canvas = gameObject.GetComponentInChildren<Canvas>();
            PlatformsErrorMessage errorMessage = canvas.GetComponentInChildren<PlatformsErrorMessage>();
            errorMessage.StartWork(exception.Message, timer);
        }
    }
}