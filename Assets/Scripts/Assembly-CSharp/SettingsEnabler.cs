using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsEnabler : MonoBehaviour
{
    private Selectable interact;

    private void Update()
    {
        interact = GetComponent<Selectable>();
        if (SceneManager.GetActiveScene().name != "OptionsMenu" && SceneManager.GetActiveScene().name != "MainMenu")
        {
            interact.interactable = false;
        }
    }
}