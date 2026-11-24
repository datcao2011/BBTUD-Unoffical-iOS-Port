using UnityEngine;
using UnityEngine.UI;

public class RainbowBackground : MonoBehaviour
{
    public Graphic target;

    public Gradient rainbow;

    private float time;

    private void Awake()
    {
        if (!target) target = GetComponent<Graphic>();
    }

    private void Update()
    {
        time += Time.unscaledDeltaTime * 0.05f;
        if (time >= 1f)
        {
            time = 0f;
        }
        target.color = rainbow.Evaluate(time);
    }
}