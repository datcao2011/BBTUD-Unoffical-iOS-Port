using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Daylight : MonoBehaviour
{
    public Gradient timeGradient;
    private float secondsOfToday;
    private float seconds;

    public Material day;
    public Material night;
    private bool stopLight;

    [SerializeField]
    private bool WeirdSky;

    private void Start()
    {
        if (!Settings.GetBool("IRLDaylight", true))
        {
            Stop();
        }
    }

    private void OnDisable()
    {
        RenderSettings.skybox = day;
        if (RenderSettings.skybox.HasProperty("_Tint"))
            RenderSettings.skybox.SetColor("_Tint", new Color(0.502f, 0.502f, 0.502f, 0.502f));
        else if (RenderSettings.skybox.HasProperty("_SkyTint"))
            RenderSettings.skybox.SetColor("_SkyTint", new Color(0.502f, 0.502f, 0.502f, 0.502f));
        RenderSettings.skybox.SetFloat("_Exposure", 1f);
    }

    public void Stop()
    {
        RenderSettings.skybox = day;
        if (WeirdSky)
        {
            if (RenderSettings.skybox.HasProperty("_Tint"))
                RenderSettings.skybox.SetColor("_Tint", timeGradient.Evaluate(0f));
            else if (RenderSettings.skybox.HasProperty("_SkyTint"))
                RenderSettings.skybox.SetColor("_SkyTint", timeGradient.Evaluate(0f));
        }
        else
        {
            if (RenderSettings.skybox.HasProperty("_Tint"))
                RenderSettings.skybox.SetColor("_Tint", new Color(0.502f, 0.502f, 0.502f, 0.502f));
            else if (RenderSettings.skybox.HasProperty("_SkyTint"))
                RenderSettings.skybox.SetColor("_SkyTint", new Color(0.502f, 0.502f, 0.502f, 0.502f));
        }

        stopLight = true;
        RenderSettings.skybox.SetFloat("_Exposure", 1f);
    }

    private void Update()
    {
        if (!stopLight)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(DateTime.Today);
            secondsOfToday = (float)(timeSpan.TotalSeconds);

            if (secondsOfToday <= 43200)
            {
                if (secondsOfToday >= 18000)
                {
                    RenderSettings.skybox = day;
                }
                else
                {
                    RenderSettings.skybox = night;
                }
                seconds = secondsOfToday - 86400;
            }
            else
            {
                if (secondsOfToday < 75600)
                {
                    RenderSettings.skybox = day;
                }
                else
                {
                    RenderSettings.skybox = night;
                }
                seconds = 86400 - secondsOfToday;
            }

            Color selectedColor = timeGradient.Evaluate(seconds / 86400f);
            if (RenderSettings.skybox.HasProperty("_Tint"))
                RenderSettings.skybox.SetColor("_Tint", selectedColor);
            else if (RenderSettings.skybox.HasProperty("_SkyTint"))
                RenderSettings.skybox.SetColor("_SkyTint", selectedColor);
        }
    }
}