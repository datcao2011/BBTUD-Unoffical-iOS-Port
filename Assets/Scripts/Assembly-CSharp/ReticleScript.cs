using UnityEngine;

public static class ReticleScript
{
    public static Vector2 Position()
    {
        return new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f);
    }
}