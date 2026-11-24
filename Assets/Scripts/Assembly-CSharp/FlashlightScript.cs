using UnityEngine;
using Platforms;

public class FlashlightScript : MonoBehaviour
{
    private Light flashLight;

    public bool on;

    public AudioClip aud_on;

    public AudioClip aud_off;
    private PlatPlayer input;

    private void Start()
    {
        input = PlatTools.GetPlatformPlayer();
        flashLight = GetComponent<Light>();
        flashLight.enabled = false;

        if (Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
        {
            flashLight.renderMode = LightRenderMode.ForceVertex;
        }
    }

    private void Update()
    {
        if (input.GetButtonDown("Flashlight Toggle") & Time.timeScale != 0f & !MultiplayerManager.isActive)
        {
            flashLight.enabled = !flashLight.enabled;
            on = flashLight.enabled;
			AudioClip aud;
			if (flashLight.enabled)
			{
				aud = aud_on;
			}
			else
			{
				aud = aud_off;
			}
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
			audioSource.PlayOneShot(aud, 1f);
			Destroy(audioSource, aud.length);
        }
    }
}
