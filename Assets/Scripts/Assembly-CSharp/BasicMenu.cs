using UnityEngine.SceneManagement;
using UnityEngine;

public class BasicMenu : MonoBehaviour
{
    public GameObject Menu;
    private bool IsShown;

    // Update is called once per frame
    private void Update()
    {
        if (!IsShown && SceneManager.sceneCount >= 2)
        {
            IsShown = true;
        }
        else if (IsShown && SceneManager.sceneCount < 2)
        {
            IsShown = false;
            Menu.SetActive(true);
        }
        if (IsShown)
        {
            Menu.SetActive(false);
        }
    }
}
