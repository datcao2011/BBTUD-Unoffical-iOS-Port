using UnityEngine;
using TMPro;

public class FPScript : MonoBehaviour
{
    private float deltaTime = 0.0f;

    public TMP_Text fpsText;

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string textS = string.Format("{1:0.}", msec, fps);
        string text = "FPS: " + textS;
        fpsText.text = text;
    }
}
