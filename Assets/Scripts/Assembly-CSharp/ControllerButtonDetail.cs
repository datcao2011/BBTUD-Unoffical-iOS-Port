using UnityEngine;
using Platforms;


public class ControllerButtonDetail : MonoBehaviour
{
    public GameObject childGameObject;

    private void Start()
    {
        childGameObject.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.isUsingController == true)
        {
            childGameObject.SetActive(true);
        }
        else
        {
            childGameObject.SetActive(false);
        }
    }
}