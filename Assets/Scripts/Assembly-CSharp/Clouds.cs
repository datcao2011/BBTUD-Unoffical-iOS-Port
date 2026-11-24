using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public ParticleSystem Particles;
    public ParticleSystemRenderer ParticleShader;
    public Collider tcollider;

    public float Duration;
    private bool NeedsToBeDestroyed;
    public float DestroyDuration;

    [Header("Shaders")]
    public Shader DiffuseShader;
    public Shader StandardShader;

    // Start is called before the first frame update
    private void Start()
    {
        DestroyDuration = 5f;
        Duration = Random.Range(50f, 60f);
        ParticleSystem.MainModule Main = Particles.main;
        Main.duration = Duration;
        Particles.Play();
    }

    private void OnEnable()
    {
#if !UNITY_EDITOR
        Settings.OnSettingChanged += OnValChange;
        OnValChange("1.4Shaders", Settings.GetBool("1.4Shaders"));
#endif
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR
        Settings.OnSettingChanged -= OnValChange;
#endif
    }

    private void OnValChange(string name, object val)
    {
        if (name == "1.4Shaders")
        {
            bool Value = (bool)val;
            ParticleShader.sharedMaterial.shader = Value ? DiffuseShader : StandardShader;
        }
    }

    public void ForceDestroy(bool Immediate)
    {
        Duration = 0f;
        if (Immediate) Destroy(gameObject);
    }

    public void ExcludeCollision(Collider Objec)
    {
        Physics.IgnoreCollision(Objec, tcollider);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Duration > 0f)
        {
            Duration -= Time.deltaTime;
        }
        else if (!NeedsToBeDestroyed)
        {
            NeedsToBeDestroyed = true;
        }

        if (NeedsToBeDestroyed)
        {
            if (DestroyDuration > 0f)
            {
                DestroyDuration -= Time.deltaTime;
                if (DestroyDuration <= 3f)
                {
                    tcollider.enabled = false;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
