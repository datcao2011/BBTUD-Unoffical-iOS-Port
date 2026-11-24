using UnityEngine;

public static class MobileTouchScript
{
    private static float YRotation;

    private static float XRotation;

    public static float XAxis
    {
        get
        {
            return XRotation;
        }
        set
        {
            XRotation = value;
        }
    }

    public static float YAxis
    {
        get
        {
            return YRotation;
        }
        set
        {
            YRotation = value;
        }
    }
}