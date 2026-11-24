using UnityEngine;
using Platforms;

public class InteractScript : MonoBehaviour
{
    private void Awake()
    {
        player = PlatTools.GetPlatformPlayer();
    }

    private PlatPlayer player;

    private void Update()
    {
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi)
        {
            isInteracting = false;
            return;
        }
        isInteracting = InputManager.usingTouch ? MobileInput.GetMobileInputButtonDown("Interact") : player.GetButtonDown("Interact");
    }

    protected static bool isInteracting;


    public static bool Intracting
    {
        get
        {
            return isInteracting;
        }
    }
}