using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeAwards : MonoBehaviour
{
    public GameObject MobileControls;

    public AudioSource source;

    public TMP_Text Text;

    public GameControllerScript gc;

    // Start is called before the first frame update
    private void Start()
    {
        Camera.main.fieldOfView = 60f;
        GameObject.Find("Player").transform.position = new Vector3(-754f, 4f, -55f);
        GameObject.Find("Player").GetComponent<PlayerScript>().SetHudOff();
        Camera.main.fieldOfView = 60;
        GameObject.Find("GameController").GetComponent<GameControllerScript>().SetAllCharacters(false);
        GameObject.Find("GameController").GetComponent<GameControllerScript>().EnableExitMode(false);
        GameObject.Find("GameController").GetComponent<AudioSource>().mute = true;
        GameObject.Find("GameController").GetComponent<AudioSource>().Stop();
        GameObject.Find("GameController").GetComponent<GameControllerScript>().StopNoise();
        GameObject.Find("GameController").GetComponent<FogScript>().stopFog = true;
        RenderSettings.fog = false;
        MobileControls.SetActive(false);
        float TimeTaken = Time.timeSinceLevelLoad;
        int Coins = 0;
        float Range;
        switch (Settings.GetString("ModeType"))
        {
            case "Stealthy":
                Range = Random.Range(400f, 800f);
                if (TimeTaken <= 300f)
                {
                    Range *= Random.Range(1.5f, 2f);
                }
                Coins = (int)Range;
                break;
            case "Grapple":
                Range = Random.Range(200f, 400f);
                if (TimeTaken <= 300f)
                {
                    Range *= Random.Range(1.5f, 2f);
                }
                Coins = (int)Range;
                break;
            case "Speedy":
                Range = Random.Range(1200f, 1500f);
                if (TimeTaken <= 300f)
                {
                    Range *= Random.Range(1.5f, 2f);
                }
                Coins = (int)Range;
                break;
        }
        CoinSaveSystem.Save(CoinSaveSystem.Load() + Coins);
        Text.text = Text.text.Replace("+32", "+" + Coins);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!source.isPlaying)
        {
            SceneManager.LoadScene("Starter");
        }
    }
}
