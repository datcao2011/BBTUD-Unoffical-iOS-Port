using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBackOptions : MonoBehaviour
{
    public GameObject mainMenuLoader;

    public string scenename = "OptionsMenu";

    public void LoadBack()
    {
        if (SceneManager.GetActiveScene().name == scenename)
        {
            mainMenuLoader.SetActive(true);
        }
        else
        {
            SceneManager.UnloadSceneAsync(scenename);
        }
    }
}
