using UnityEngine;
using Platforms;

public class Hang : MonoBehaviour
{
    private void Update()
    {
        if (play)
        {
            if (VibrateDuration > 0f)
            {
                if (CurrentDevice.currentPlatform == PFPlatform.Windows) CurrentDevice.VibrateController(VibrateDuration, VibrateDuration);
                VibrateDuration -= Time.unscaledDeltaTime * 0.6f;
            }
            else if (play)
            {
                play = false;
                if (CurrentDevice.currentPlatform == PFPlatform.Windows) CurrentDevice.StopVibrateController();
            }
        }
    }

    public static void Play()
    {
        VibrateDuration = 0.8f;
        play = true;
    }

    public AudioClip CopyrightedHangSound;

    private static float VibrateDuration;

    private static bool play;
}