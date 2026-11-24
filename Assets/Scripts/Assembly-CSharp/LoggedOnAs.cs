// YouWonScript
using UnityEngine;

public class LoggedOnAs : MonoBehaviour
{
    private void Start()
    {
        Camera.main.fieldOfView = 60f;
        GameObject.Find("Player").transform.position = new Vector3(-754f, 4f, -55f);
        GameObject.Find("Player").GetComponent<PlayerScript>().SetHudOff();
        Camera.main.fieldOfView = 60;
        GameObject.Find("GameController").GetComponent<GameControllerScript>().SetAllCharacters(false);
        GameObject.Find("GameController").GetComponent<GameControllerScript>().EnableExitMode();
        GameObject.Find("GameController").GetComponent<AudioSource>().mute = true;
        GameObject.Find("GameController").GetComponent<AudioSource>().Stop();
        GameObject.Find("GameController").GetComponent<GameControllerScript>().StopNoise();
        GameObject.Find("GameController").GetComponent<FogScript>().stopFog = true;
        RenderSettings.fog = false;
    }

    private void Update()
    {
        GameObject.Find("Player").GetComponent<PlayerScript>().stamina = 0f;
        GameObject.Find("Player").GetComponent<PlayerScript>().walkSpeed = 10f;
    }
}