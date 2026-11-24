using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilingLighting : MonoBehaviour
{
    public Color lightingColor;

    // Start is called before the first frame update
    void Start()
    {
        SetColors();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColors()
    {
        foreach (Transform t in transform)
        {
            MeshRenderer renderer = t.GetComponent<MeshRenderer>();
            if (!renderer) continue;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                lightingColor.a = renderer.sharedMaterial.color.a;
                var tempMaterial = new Material(renderer.sharedMaterial)
                {
                    color = lightingColor
                };
                renderer.sharedMaterial = tempMaterial;
            }
#endif
#if UNITY_EDITOR
            else
            {
#endif
                t.GetComponent<MeshRenderer>().material.color = lightingColor;
#if UNITY_EDITOR
            }
#endif
        }

    }

    public void SetDefaultColors()
    {
        foreach (Transform t in transform)
        {
            MeshRenderer renderer = t.GetComponent<MeshRenderer>();
            if (!renderer) continue;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                lightingColor.a = renderer.sharedMaterial.color.a;
                var tempMaterial = new Material(renderer.sharedMaterial)
                {
                    color = Color.white
                };
                renderer.sharedMaterial = tempMaterial;
            }
#endif
#if UNITY_EDITOR
            else
            {
#endif
                t.GetComponent<MeshRenderer>().material.color = lightingColor;
#if UNITY_EDITOR
            }
#endif
        }
    }
}
