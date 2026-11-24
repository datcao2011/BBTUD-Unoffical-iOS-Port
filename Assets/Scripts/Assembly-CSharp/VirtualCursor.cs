#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualCursor : MonoBehaviour
{
    private void Awake()
    {

    }

    public static Vector2 Position { get; private set; }

    public static bool IsEnabled { get; private set; }
}
#endif