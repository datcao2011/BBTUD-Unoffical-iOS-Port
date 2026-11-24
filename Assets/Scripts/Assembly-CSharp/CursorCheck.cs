using UnityEngine;
using Platforms;
using System.Collections;

public class CursorCheck : MonoBehaviour
{
    public PlatPlayer platform;

    public Texture2D cursor;

    private Texture2D currentCursor;

    private void Awake()
    {
        StartCoroutine(GetMouse());
    }

    private IEnumerator GetMouse()
    {
        platform = PlatTools.GetPlatformPlayer();
        Cursor.lockState = CursorLockMode.Locked;
        float delay = 0.2f;
        while (delay > 0f)
        {
            delay -= Time.unscaledDeltaTime;
            yield return null;
        }
        Cursor.lockState = CursorLockMode.None;
        float delay2 = 0.1f;
        while (delay2 > 0f)
        {
            delay2 -= Time.unscaledDeltaTime;
            yield return null;
        }
        Vector2 MousePosition = platform.GetGlobalCursorPosition();
        int x = (int)MousePosition.x;
        int y = (int)MousePosition.y;
        CursorControllerScript.xPos = x;
        CursorControllerScript.yPos = y;
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Object.Destroy(this);
        yield break;
    }
}
