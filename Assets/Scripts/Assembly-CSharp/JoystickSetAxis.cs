using UnityEngine;

public class JoystickSetAxis : MonoBehaviour
{
    public static float forward;
    public static float strafe;

    private void Update()
    {
        strafe = GetComponent<Joystick>().Horizontal;
        forward = GetComponent<Joystick>().Vertical;
    }
}