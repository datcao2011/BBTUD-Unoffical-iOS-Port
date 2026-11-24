using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableSaveSystem : MonoBehaviour
{
    private void Awake()
    {
        SaveSystemStopper.Enabled = true;
    }

    private void OnEnable()
    {
        SaveSystemStopper.Enabled = true;
    }

    private void OnDisable()
    {
        SaveSystemStopper.Enabled = false;
    }

    private void OnDestroy()
    {
        SaveSystemStopper.Enabled = false;
    }
}