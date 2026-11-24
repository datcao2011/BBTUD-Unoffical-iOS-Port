using UnityEngine;

public class DisableOnLoad : MonoBehaviour
{
    private void Update()
    {
        gameObject.SetActive(false);
    }
}