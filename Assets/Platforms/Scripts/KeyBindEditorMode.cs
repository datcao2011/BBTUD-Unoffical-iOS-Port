using UnityEngine;
using UnityEditor;

namespace Platforms
{
#if UNITY_EDITOR
    [CustomEditor(typeof(KeybindManager))]
    public class KeyBindEditorMode : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            KeybindManager script = (KeybindManager)target;
            if (script.PlatformsInputManager != null)
            {
                if (script.GetComponent<Canvas>())
                {
                    EditorGUILayout.HelpBox("A canvas component needs to be removed in order to work.", MessageType.Error);
                    return;
                }
                if (script.Configuration == null)
                {
                    if (GUILayout.Button("Generate UI Configuration"))
                    {
                        script.GenerateUI();
                    }
                }
                else
                {
                    if (GUILayout.Button("Re-generate UI Configuration"))
                    {
                        script.GenerateUI();
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("To work on the keybind configuration, you'll need to put the Input Platforms Manager to work with it.", MessageType.Error);
            }
        }
    }
#else
    public class KeyBindEditorMode
    {

    }
#endif
}