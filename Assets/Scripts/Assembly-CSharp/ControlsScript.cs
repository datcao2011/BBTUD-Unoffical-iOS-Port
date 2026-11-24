using UnityEngine;

public class ControlsScript : MonoBehaviour
{
    public GameObject pc;

    public GameObject mobile;

    private void Awake()
    {
        if (InputManager.usingTouch)
        {
            mobile.SetActive(true);
            pc.SetActive(false);
        }
        else
        {
            pc.SetActive(true);
            mobile.SetActive(false);
        }
    }

    private void Update()
    {
        if (InputManager.usingTouch)
        {
            mobile.SetActive(true);
            pc.SetActive(false);
        }
        else
        {
            pc.SetActive(true);
            mobile.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (pc != null) pc.SetActive(false);
    }
}