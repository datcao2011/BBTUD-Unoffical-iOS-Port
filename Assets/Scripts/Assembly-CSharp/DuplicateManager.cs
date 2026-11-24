using UnityEngine;
using UnityEngine.SceneManagement;

public class DuplicateManager : MonoBehaviour
{
    public string sceneName = "OptionsMenu";

    // Start is called before the first frame update
    private void Awake()
    {
        if (SceneManager.sceneCount >= 2)
        {
            DestroyImmediate(gameObject);
        }
        if (SceneManager.sceneCount >= 3)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
