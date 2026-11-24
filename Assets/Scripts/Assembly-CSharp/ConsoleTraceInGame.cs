// NOTE: For an actively-maintained version of this script, see https://github.com/mminer/consolation.
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>
public class ConsoleTraceInGame : MonoBehaviour
{
    public Version version = new Version(1, 0, 0);

    /// <summary>
    /// The hotkey to show and hide the console window.
    /// </summary>
    public KeyCode toggleKey;

    public KeyCode toggleKey1;

    public KeyCode toggleMainKey;

    // /// <summary>
    // /// Whether to open the window by shaking the device (mobile-only).
    // /// </summary>
    // public bool shakeToOpen = false;
    public static bool instanceOpen;

    //public float shakeAcceleration = 10f;

    float textFieldWidth;

    public List<Log> logs = new List<Log>();
    Vector2 scrollPosition;
    public bool show
    {
        get { return shown; }
        set
        {
            shown = value;
        }
    }
    public static bool shown { get; protected set; }
    bool collapse;

    // Visual elements:

    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
    {
        { LogType.Assert, Color.white },
        { LogType.Error, Color.red },
        { LogType.Exception, Color.red },
        { LogType.Log, Color.white },
        { LogType.Warning, Color.yellow },
    };

    const int margin = 20;

    Rect windowRect;
    Rect titleBarRect = new Rect(0, 0, 10000, 20);
    GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
    GUIContent closeLabel = new GUIContent("Close", "Closes the log details.");
    GUIContent sendLabel = new GUIContent("Send", "Send command to the console.");
    string commandText = "";
    //bool isWebGL;

    void OnEnable()
    {
        if (instanceOpen)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instanceOpen = true;
        Application.logMessageReceived += HandleLog;
        //isWebGL = Platforms.CurrentDevice.currentPlatform == Platforms.PFPlatform.WebGL;
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_WEBGL
        EnableMobileLogs = Settings.GetBool("OpenLogs");
#endif
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void Start()
    {
        if (Debug.isDebugBuild && !Application.isEditor)
        {
            HandleLog("This is currently a development build. Errors/exceptions are moved to unity debug console on the bottom left. It will only appear once an error was logged.", "Console Window", LogType.Warning);
        }
    }

    public void RefreshWindowSize()
    {
        textFieldWidth = Screen.width - (margin * 6);
        windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleMainKey) /*|| (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration)*/)
        {
            // if (!isWebGL)
            // {
            textFieldWidth = Screen.width - (margin * 6);
            windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
            show = !show;
            //}
        }
    }

    public static bool EnableMobileLogs;

    void OnGUI()
    {
        if (!show)
        {
            if (EnableMobileLogs)
            {
                GUILayout.Space(Screen.height / 2);
                if (GUILayout.Button("Open Logs"))
                {
                    show = true;
                    textFieldWidth = Screen.width - (margin * 6);
                    windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
                }
            }
            return;
        }
        windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
    }

    /// <summary>
    /// A window that displayss the recorded logs.
    /// </summary>
    /// <param name="windowID">Window ID.</param>
    void ConsoleWindow(int windowID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Iterate through the recorded logs.
        for (int i = 0; i < logs.Count; i++)
        {
            var log = logs[i];

            // Combine identical messages if collapse option is chosen.
            if (collapse)
            {
                var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

                if (messageSameAsPrevious)
                {
                    continue;
                }
            }

            GUI.contentColor = logTypeColors[log.type];
            GUILayout.Label(log.message);
            if (log.type == LogType.Exception)
            {
                GUILayout.Label("   -> " + log.stackTrace);
            }
        }

        GUILayout.EndScrollView();
        GUI.contentColor = Color.white;

        GUILayout.BeginHorizontal();
        var boldtext = new GUIStyle(GUI.skin.label);
        boldtext.fontStyle = FontStyle.Bold;
        GUILayout.Label(InputManager.Stats, boldtext);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();


        commandText = GUILayout.TextField(commandText, 250, GUILayout.Width(textFieldWidth));

        if (GUILayout.Button(sendLabel))
        {
            HandleLog(commandText, "No Stack Trace :)", LogType.Assert);
            GameSystem.SendCommand(commandText);
            commandText = "";
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(clearLabel))
        {
            logs.Clear();
#if UNITY_EDITOR
            var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
#endif
        }
        if (GUILayout.Button(closeLabel))
        {
            show = false;
        }

        collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();

        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(titleBarRect);
    }

    /// <summary>
    /// Records a log from the log callback.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="stackTrace">Trace of where the message came from.</param>
    /// <param name="type">Type of message (error, exception, warning, assert).</param>
    void HandleLog(string message, string stackTrace, LogType type)
    {
        if (Debug.isDebugBuild && !Application.isEditor)
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                return;
            }
        }

        logs.Add(new Log()
        {
            message = message,
            stackTrace = stackTrace,
            type = type,
        });
    }

    public struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }
}