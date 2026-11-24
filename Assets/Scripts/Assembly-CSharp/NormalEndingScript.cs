// YouWonScript
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NormalEndingScript : MonoBehaviour
{
    public AudioSource audioDevice;

    public DoorScript door;

    private PlayerScript player;

    public Collider collide;

    private GameControllerScript gc;

    private void Start()
    {
        FindObjectOfType<Daylight>().Stop();
        Camera.main.fieldOfView = 60f;
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        player.transform.position = new Vector3(-784f, 4f, -55f);
        player.SetPosition(new Vector3(-784f, 4f, -55f));
        player.SetRotation(Quaternion.Euler(new Vector3(0f, 180f, 0f)));
        player.SetHudOff();
        gc = FindObjectOfType<GameControllerScript>();
        gc.SetAllCharacters(false);
        gc.EnableExitMode(false);
        gc.SetNoclip(false);
        gc.GetComponent<AudioSource>().mute = true;
        gc.GetComponent<AudioSource>().Stop();
        gc.StopNoise();
        gc.GetComponent<FogScript>().stopFog = true;
        RenderSettings.fog = false;
        RenderSettings.fogDensity = 0f;
        if (audioDevice == null)
        {
            audioDevice = gameObject.AddComponent<AudioSource>();
            audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.AAmbience : null;
        }
        audioDevice.PlayOneShot(gc.aud_Switch, 1f);
        GameObject.Find("Environment").SetActive(false);
        for (int i = 0; i < 6; i++)
        {
            gc.LoseItem(i);
        }
    }

    private void Update()
    {
        player.stamina = 100f;
        player.walkSpeed = 10f;
        player.runSpeed = 16f;
        if (door.bDoorOpen)
        {
            collide.enabled = true;
        }
    }

    private void LateUpdate()
    {
        gc.Can_UseItems = gc.itemSelected != 13;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            SceneManager.LoadScene("Starter");
        }
    }
}