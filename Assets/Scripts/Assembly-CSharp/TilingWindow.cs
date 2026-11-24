#if UNITY_EDITOR
using UnityEditor;
using Platforms.Console.Logger;
using UnityEngine;
using System;
public class TilingWindow : EditorWindow
{
    [MenuItem("Window/Tiling")]
    public static void ShowWindow()
    {
        GetWindow<TilingWindow>("Tiling");
    }

    private int Direction = 0;
    private float Spacing;
    private Vector3 PositionAddition;

    private void OnGUI()
    {
        Transform Selected = Selection.activeTransform;
        if (Selected == null) return;
        GUILayout.Label("Name: " + Selected.name);
        if (Direction != 0) EditorGUILayout.Vector3Field("Position", Selected.position); else EditorGUILayout.Vector3Field("Local Position", Selected.localPosition);
        Direction = EditorGUILayout.IntField("Direction", Direction);
        if (Direction != 0)
            Spacing = EditorGUILayout.FloatField("Spacing", Spacing);
        else
            PositionAddition = EditorGUILayout.Vector3Field("Addition", PositionAddition);
        //Returns true if any key was pressed.
        if (Event.current.isKey)
        {
            //The key that was pressed.
            KeyCode key = Event.current.keyCode;
            if (key == KeyCode.Alpha1)
            {
                Vector3 Position;
                switch (Direction)
                {
                    case 0:
                        Position = PositionAddition;
                        break;
                    case 1:
                        Position = Selected.forward;
                        break;
                    case -1:
                        Position = Selected.forward * -1;
                        break;
                    case 2:
                        Position = Selected.right;
                        break;
                    case -2:
                        Position = Selected.right * -1;
                        break;
                    case 3:
                        Position = Selected.up;
                        break;
                    case -3:
                        Position = Selected.up * -1;
                        break;
                    default:
                        Position = Selected.forward;
                        break;
                }
                GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject);
                GameObject b;
                if (prefabRoot != null)
                    b = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
                else
                    b = Instantiate(Selection.activeGameObject, Selected.parent, true);
                b.name = Selected.name;
                if (Direction != 0) b.transform.position += Position * Spacing; else b.transform.localPosition += PositionAddition;
                Selection.activeGameObject = b;
            }
        }
    }
}
#endif