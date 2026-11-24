using UnityEngine;

public class FGScript : MonoBehaviour
{
    public GameObject player;

    private Vector3 regularPosition;

    public Material whiteSkybox;

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnEnable()
    {
        foreach (Camera camera in FindObjectsOfType<Camera>())
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.red;
        }
        regularPosition = player.transform.position;
        player.transform.position = new Vector3(10018.53f, 4, 10002.09f);
    }

    private void OnDisable()
    {
        foreach (Camera camera in FindObjectsOfType<Camera>())
        {
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.backgroundColor = Color.black;
        }
        player.transform.position = regularPosition;
        FindObjectOfType<RandomEvent>().FadeMusicUp(FindObjectOfType<RandomEvent>().gameController.schoolMusic);
        FindObjectOfType<RandomEvent>().ResumeRandomEvents();
        RenderSettings.skybox = whiteSkybox;
    }
}
