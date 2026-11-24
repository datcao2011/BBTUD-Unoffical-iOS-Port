using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(TilingLighting))]
public class TilingLightingTesting : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TilingLighting script = (TilingLighting)target;
        if (GUILayout.Button("Test Colors"))
        {
            script.SetColors();
        }
        if (GUILayout.Button("Reset Colors"))
        {
            script.SetDefaultColors();
        }
    }
}
#else
public class TilingLightingTesting
{

}
#endif