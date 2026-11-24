#if UNITY_EDITOR
using UnityEditor;
using Platforms.Console.Logger;
using UnityEngine;
using System;
public class CommandWindow : EditorWindow
{
    [MenuItem("Window/General/Send Command")]
    public static void ShowWindow()
    {
        GetWindow<CommandWindow>("Send Command");
    }

    private string commandText = "";

    private void OnGUI()
    {
        GUILayout.Label("Type a command to the console.");
        commandText = EditorGUILayout.TextField(commandText);
        if (GUILayout.Button("Send to Console"))
        {
            GameSystem.SendCommand(commandText);
            commandText = "";

        }
    }
}
#endif