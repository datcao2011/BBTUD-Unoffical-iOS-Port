using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFogScript : MonoBehaviour
{
    public float CurrentDensity;
    public float ToDensity = 0.3f;
    private float CurrentTime;
    // Update is called once per frame
    void Update()
    {
        CurrentTime += Time.unscaledDeltaTime;
        RenderSettings.skybox = null;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.white;
        RenderSettings.fogDensity = Mathf.Clamp(Mathf.InverseLerp(0f, 0.5f, CurrentTime), CurrentDensity, ToDensity);
    }
}
