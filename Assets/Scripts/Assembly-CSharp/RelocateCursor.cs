using UnityEngine;
using Platforms;
using System.Collections;
using TMPro;

public class RelocateCursor : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.ClearDeveloperConsole();
        }
    }
}