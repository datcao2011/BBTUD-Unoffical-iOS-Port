using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    private bool isDone;

    private AsyncOperation operation;

    private void Awake()
    {
        if (InputManager.instance != null) InputManager.instance.instanceCreated = false;
        ConsoleTraceInGame.instanceOpen = false;
        Transform[] transforms = new Transform[0];
        try
        {
            transforms = new Transform[GameObject.FindObjectsOfType<Transform>().Length];
            transforms = GameObject.FindObjectsOfType<Transform>();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
        

        foreach (Transform t in transforms)
        {
            try
            {
                if (t != transform && t != Camera.main.transform && !t.GetComponent<Photon.Pun.PhotonHandler>())
                {
                    DestroyImmediate(t.gameObject);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                continue;
            }
        }

        Cursor.lockState = CursorLockMode.None;

        Cursor.SetCursor(null, Vector3.zero, CursorMode.Auto);

        SplashScreen.Begin();
    }

    private void Start()
    {
        operation = SceneManager.LoadSceneAsync(0);
        operation.allowSceneActivation = false;
    }

    private void Update()
    {
        if (SplashScreen.isFinished & !isDone)
        {
            isDone = true;
            operation.allowSceneActivation = true;
        }
        else
        {
            SplashScreen.Draw();
        }
    }
}