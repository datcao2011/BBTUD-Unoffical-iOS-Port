// GameOverScript
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Platforms;

public class GameOverScript : MonoBehaviour
{
    private RawImage image;

    private float delay;

    public Texture[] images = new Texture[10];

    public Texture rare;

    private float chance;

    private AudioSource audioDevice;

    public SceneLoader loader;

    public GameObject loadingScreen;

    private void Start()
    {
        Time.timeScale = Settings.GetFloat("TimeSpeed");
        image = base.GetComponent<RawImage>();
        audioDevice = base.GetComponent<AudioSource>();
        delay = 5f;
        chance = Random.Range(1f, 99f);
        if (chance < 98f)
        {
            int num = Mathf.RoundToInt(Random.Range(0f, images.Length - 1f));
            image.texture = images[num];
        }
        else
        {
            image.texture = rare;
        }
        if (System.DateTime.Now.Hour == 3)
        {
            image.texture = rare;
            image.transform.localScale = new Vector3(0.6954246f, 2.412767f, 0.534942f);
        }
    }

    private void Update()
    {
        delay -= 1f * Time.deltaTime;
        if (delay <= 0f)
        {
            if (System.DateTime.Now.Hour == 3)
            {
                threeAMJumpscare.PlacefaceJumpscare = true;
                loader.LoadScene("THREEAM");
                return;
            }
            if (chance < 98f)
            {
                loader.LoadScene("Starter");
            }
            else
            {
                image.transform.localScale = new Vector3(3.136597f, 2.412767f, 2.412767f);
                image.color = Color.red;
                if (!audioDevice.isPlaying)
                {
                    audioDevice.Play();
                }
                if (delay <= -5f)
                {
                    ApplicationConfigure.closeApplication();
                }
            }
        }
    }
}