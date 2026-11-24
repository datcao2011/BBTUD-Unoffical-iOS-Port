using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    private float time;
    [SerializeField]
    private float NextTime;
    [SerializeField]
    private Material Lit;
    [SerializeField]
    private Material Unlit;

    [SerializeField]
    private MeshRenderer Renderer;

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= NextTime)
        {
            Renderer.material = Unlit;
            if (time >= (NextTime + 0.07f))
            {
                time = 0f;
                Renderer.material = Lit;
            }
        }
    }
}
