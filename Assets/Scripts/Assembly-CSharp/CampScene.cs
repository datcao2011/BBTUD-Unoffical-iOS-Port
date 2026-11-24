using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CampScene : MonoBehaviour
{
    public GameObject LoadingBus;
    
    public LoadingBusScreen Screen;

    private void OnTriggerEnter(Collider other)
    {
        LoadingBus.SetActive(true);
        Screen.LoadScene("CampScene");
        GameObject.Find("MobileControls").SetActive(false);
    }
}
