
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FarmScene : MonoBehaviour
{
    public GameObject LoadingBus;

    public LoadingBusScreen Screen;

    private void OnTriggerEnter(Collider other)
    {
        LoadingBus.SetActive(true);
        Screen.LoadScene(Settings.GetString("FarmMode") == "Barn" ? "Farm" : "FarmScene");
        GameObject.Find("MobileControls").SetActive(false);
    }
}
