using UnityEngine;
using System.Collections;

public class FogScript : MonoBehaviour
{
    public GameControllerScript gc;

    public RandomEvent events;

    public FogControllerScript fog;

    public ParticleSystem fogParticle;

    public Color normalColor;

    public AudioSource fogDevice;

    public AudioClip aud_fogStopped;

    public NotebookScript[] OrderOfNotebooks;
    
    public GameObject[] OrderOfCharacters;

    private bool started;

    private bool blinding;

    public float timer;

    private void Start()
    {
        fogParticle.Stop();
        fog.ChangeFogColor(normalColor);
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else if (started)
        {
            started = false;
            timer = 0f;
            var main = fogParticle.main;
            main.loop = false;
            fogDevice.Stop();
            fogDevice.PlayOneShot(aud_fogStopped);
            StartCoroutine(FadeFogOut());
        }
    }

    public bool stopFog;

    public bool FloodFog;

    private void LateUpdate()
    {
        if (events.floodScript != null && events.floodScript.isActiveAndEnabled && gc.cameraTransform.position.y < 3.8f)
        {
            if (!FloodFog)
            {
                fog.EnableFog(true);
                fog.ChangeFogColor(Color.cyan);
                fog.FogDensity(0.1f);
            }
            FloodFog = true;
            return;
        }
        else if (FloodFog)
        {
            FloodFog = false;
            if (!blinding)
            {
                fog.EnableFog(false);
                fog.FogDensity(0f);
            }
            else if (blinding)
            {
                fog.FogDensity(0.12f);
                fog.ChangeFogColor(Color.black);
            }
        }
        if (!events.codeRed && !stopFog && !blinding)
        {
            var main = fogParticle.main;
            if (gc.exitModeCheatActive)
            {
                fog.ChangeFogColor(gc.redColor);
                Color colorRed = gc.redColor;
                colorRed.a = 1;
                main.startColor = colorRed;
            }
            else
            {
                fog.ChangeFogColor(normalColor);
                Color colorRed = normalColor;
                colorRed.a = 1;
                main.startColor = colorRed;
            }
        }
    }

    public void StartFading(float time)
    {
        var main = fogParticle.main;
        timer = time;
        started = true;
        main.loop = true;
        fogParticle.Play();
        fogDevice.Play();
        StartCoroutine(FadeFogIn());
    }

    public void TheTest_BlindPlayer()
    {
        blinding = true;
        fog.EnableFog(true);
        fog.ChangeFogColor(Color.black);
        fog.FogDensity(0.2f);
    }

    public void TheTest_StopBlinding()
    {
        blinding = false;
        fog.EnableFog(false);
    }

    private IEnumerator FadeFogIn()
    {
        float fogDensity = 0.05f;
        float density = 0f;
        fog.FogDensity(0f);
        fog.EnableFog(true);
        while (fog.CurrentDensity() < fogDensity)
        {
            density += 0.008f * Time.deltaTime;
            fog.FogDensity(density);
            yield return null;
        }
        fog.FogDensity(0.05f);
        yield break;
    }
    private IEnumerator FadeFogOut()
    {
        float fogDensity = 0f;
        float density = 0.05f;
        fog.FogDensity(0.05f);
        while (fog.CurrentDensity() > fogDensity)
        {
            density -= 0.008f * Time.deltaTime;
            fog.FogDensity(density);
            yield return null;
        }
        fog.FogDensity(0f);
        fog.EnableFog(false);
        yield break;
    }
}