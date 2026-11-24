using Platforms;
using UnityEngine;


// Token: 0x02000008 RID: 8
public class CursorControllerScript : MonoBehaviour
{
    public static int xPos = 0, yPos = 0;

    public GameControllerScript gc;

    private Vector2 mousePosition;

    private static Vector2 previousMouseLocation;

    public bool unlockCursorOfGame;

    private static bool unlockCursor;

    public static PlatPlayer platform;

    private static bool locked;

    private void Awake()
    {
        gc = GetComponent<GameControllerScript>();
        platform = PlatTools.GetPlatformPlayer();
        unlockCursor = unlockCursorOfGame;
    }

    private void Update()
    {
        if (gc == null)
        {
            return;
        }
        if (!Application.isFocused)
        {
            return;
        }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (locked & !InputManager.usingTouch)
        {
           
            if (InputManager.isUsingController || Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (!unlockCursor)
            {
                Cursor.lockState = CursorLockMode.Confined;
                platform.SetGlobalCursorPosition(previousMouseLocation);
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
#else
#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_IOS
        if (!InputManager.usingTouch & !unlockCursor & locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (InputManager.isUsingController == true && locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
        }
#endif
#endif
    }

    // Token: 0x0600001D RID: 29 RVA: 0x000027D8 File Offset: 0x00000BD8
    public static void LockCursor()
    {
        if (InputManager.usingTouch) return;
        Cursor.lockState = CursorLockMode.Confined;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        if (!locked)
        {
            if (!Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault)) previousMouseLocation = platform.GetGlobalCursorPosition(true);
        }
#endif
        locked = true;
        if (!unlockCursor)
            Cursor.visible = false;
    }

    // Token: 0x0600001E RID: 30 RVA: 0x000027E6 File Offset: 0x00000BE6
    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        locked = false;
        Cursor.visible = true;
    }

    public void LockMouseNonStatic()
    {
        LockCursor();
    }

    public void UnlockMouseNonStatic()
    {
        UnlockCursor();
    }
}
