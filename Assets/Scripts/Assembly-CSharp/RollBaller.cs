using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollBaller : MonoBehaviour
{
    private Color[] savedColoredMaterials;

    private bool activateColors;

    private bool stop;

    private bool color;

    private float time;

    [HideInInspector]
    private MeshRenderer[] renderers;

    private void Start()
    {
        renderers = new MeshRenderer[FindObjectsOfType<MeshRenderer>().Length];
        renderers = FindObjectsOfType<MeshRenderer>();
        savedColoredMaterials = new Color[renderers.Length];
        for (int i = 0; i < savedColoredMaterials.Length; i++)
        {
            savedColoredMaterials[i] = renderers[i].material.color;
        }
    }

    private void Update()
    {
        if (!stop)
        {
            foreach (MeshRenderer render in FindObjectsOfType<MeshRenderer>())
            {
                render.material.mainTextureOffset += new Vector2(0.75f * Time.deltaTime, 0.75f * Time.deltaTime);
                time += Time.deltaTime;
                if (color & time >= 0.45f)
                {
                    time = 0f;
                    render.material.color = Random.ColorHSV();
                }
            }
        }
    }

    public void RevertEverything()
    {
        stop = true;
        for (int i = 0; i < savedColoredMaterials.Length; i++)
        {
            renderers[i].material.color = savedColoredMaterials[i];
            renderers[i].material.mainTextureOffset = new Vector2(0f, 0f);
        }
    }

    public void Colorize()
    {
        color = true;
    }
}
