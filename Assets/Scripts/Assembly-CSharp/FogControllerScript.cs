using UnityEngine;

public class FogControllerScript : MonoBehaviour
{
    public void ChangeFogColor(Color color)
    {
        RenderSettings.fogColor = color;
    }

    public void EnableFog(bool enabled)
    {
        RenderSettings.fog = enabled;
    }

    public bool FogEnabled(bool enabled)
    {
        return RenderSettings.fog;
    }

    public void FogDensity(float value)
    {
        RenderSettings.fogDensity = value;
    }

    public float CurrentDensity()
    {
        return RenderSettings.fogDensity;
    }
}