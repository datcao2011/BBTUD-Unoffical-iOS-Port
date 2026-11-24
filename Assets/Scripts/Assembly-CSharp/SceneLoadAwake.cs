using UnityEngine;
using System.Collections;

public class SceneLoadAwake : MonoBehaviour
{

    public string sceneName;

    public bool delay;

    public float delayTime;

    private void Awake()
    {
        if (Settings.GetBool("SkipUpdatesAndGaps"))
        {
            delay = false;
        }
        if (delay)
        {
            StartCoroutine(loadDelay(delayTime));
        }
        else
        {
            GetComponent<SceneLoader>().LoadSceneAsync(sceneName);
        }
    }

    private IEnumerator loadDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<SceneLoader>().LoadSceneAsync(sceneName);
        yield break;
    }
}
