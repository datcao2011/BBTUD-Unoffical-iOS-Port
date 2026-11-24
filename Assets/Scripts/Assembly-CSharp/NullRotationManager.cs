using System.Collections;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;

public class NullRotationManager : MonoBehaviour
{
    private MeshRenderer[] Renderers;
    [SerializeField]
    private Shader[] NullGlitchShaders;

    public float Intensity;
    public float Multiplier = 0.25f;
    public float NegativeVal = 1f;

    [Header("Glitch Start")]
    public bool GlitchStart;
    private float Glitch;
    private float GlitchIntensity;
    private float GlitchMultiplier = 1f;
    private float GlitchTimer = 0.005f;

    public float Result
    {
        get
        {
            if (GlitchStart) return GlitchIntensity;
            return Intensity * Multiplier * NegativeVal;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        //#if !UNITY_EDITOR
        if (!Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
            //#endif
            StartCoroutine(Add());
    }

    private IEnumerator Add()
    {
        // 3.0.5: highly not recommended for low-end devices, it'll lag spike and maybe crash your game. Remove the line below with caution.
        if (GameOptimizer.OptimizationDefault) yield break;

        Renderers = FindObjectsOfType<MeshRenderer>();
        for (int i = 0; i < Renderers.Length; i++)
        {
            MeshRenderer Renderer = Renderers[i];
            if (Renderer == null) continue;
            if (Renderer.GetComponent<MeshFilter>() && Renderer.GetComponent<MeshFilter>().mesh && Renderer.GetComponent<MeshFilter>().mesh.isReadable)
            {
                if (Renderer.tag == "OutsideWall" && Renderer.GetComponent<MeshCollider>())
                {
                    Renderer.GetComponent<MeshCollider>().enabled = false; // Disable it as it'll glitch players out
                }
                NullRotation rotation = Renderer.AddComponent<NullRotation>();
                rotation.Manager = this;
                rotation.enabled = Renderer.isVisible;
            }
            yield return null; // To reduce the lag at the start. We'll have to call it frame by frame so the game wouldn't freeze.
        }
        yield break;
    }

    private void Update()
    {
        if (Intensity > 0f)
        {
            Intensity -= Time.deltaTime;
            if (Intensity < 0f) Intensity = 0f;
        }
        if (GlitchStart)
        {
            if (Glitch <= 3.25f)
            {
                Glitch += Time.unscaledDeltaTime;
                if (Glitch >= 0.3f && Glitch < 0.918f)
                {
                    GlitchMultiplier = 1f + ((Glitch - 0.3f) * 1.618123f);
                    GlitchTimer = 0.05f + ((Glitch - 0.3f) * 0.08090615f);
                }
                else if (Glitch >= 0.918f && Glitch < 1.2f)
                {
                    GlitchMultiplier = 2f;
                    GlitchTimer = 0.1f;
                }
                else if (Glitch >= 1.2f && Glitch < 1.6f)
                {
                    GlitchMultiplier = 2.5f;
                }
                else if (Glitch >= 1.6f && Glitch < 1.9f)
                {
                    GlitchMultiplier = 3f;
                }
                // else if (Glitch >= 1.9f && Glitch < 2.7f)
                // {
                //     GlitchMultiplier = 4f;
                // }
                // else if (Glitch >= 2.7f && Glitch < 3f)
                // {
                //     GlitchMultiplier = 3.5f;
                // }
                // else if (Glitch >= 3f)
                // {
                //     GlitchMultiplier = 1f;
                // }
                if (B == 0)
                {
                    GlitchIntensity += Time.unscaledDeltaTime * GlitchMultiplier;
                    if (GlitchIntensity >= GlitchTimer * (1f + Mathf.Clamp(GlitchMultiplier * 0.2f, 0f, 10f)))
                    {
                        B = 1;
                    }
                }
                else if (B == 1)
                {
                    GlitchIntensity -= Time.unscaledDeltaTime * GlitchMultiplier;
                    if (GlitchIntensity <= 0f)
                    {
                        if (GlitchTimer >= 0.1f)
                        {
                            B = 2;
                        }
                        else
                        {
                            B = 0;
                        }
                    }
                }
                else if (B == 2)
                {
                    GlitchIntensity -= Time.unscaledDeltaTime * GlitchMultiplier;
                    if (GlitchIntensity <= GlitchTimer * (1f + Mathf.Clamp(GlitchMultiplier * 0.2f, 0f, 10f)) * -1f)
                    {
                        B = 3;
                    }
                }
                else if (B == 3)
                {
                    GlitchIntensity += Time.unscaledDeltaTime * GlitchMultiplier;
                    if (GlitchIntensity >= 0f)
                    {
                        B = 0;
                    }
                }
            }
            else
            {
                GlitchStart = false;
                NullManager.Instance.gc.warning.color = Color.red;
            }
        }
        for (int i = 0; i < MonoCache.allUpdate.Count; i++) MonoCache.allUpdate[i].Tick();
    }

    private int B;

    private float G(float B)
    {
        if (B < 0.1f)
        {
            return B;
        }
        return 0.1f - (B - 0.1f);
    }
}
