using TMPro;
using UnityEngine;

public class DisconnectScreen : MonoBehaviour
{
    public GameObject LoadingObject;
    public GameObject button;
    public TMP_Text Text;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Text.text = "No internet connection";
        }
    }

    private void FixedUpdate()
    {
        if (!LoadingObject.activeSelf)
        {
            Text.text = Application.internetReachability == NetworkReachability.NotReachable ? "No internet connection" : "Disconnected from server";
        }
    }

    public void LoadMainMenu()
    {
        LoadingObject.SetActive(true);
        button.SetActive(false);
        FindObjectOfType<SceneLoader>().LoadSceneAsync(Application.internetReachability == NetworkReachability.NotReachable ? "MainMenu" : "MM_Connect");
    }
}