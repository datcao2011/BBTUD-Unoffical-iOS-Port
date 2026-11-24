using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public void SetEnabled(bool value)
    {
        foreach (WindElement element in GetComponentsInChildren<WindElement>())
        {
            element.SetEnabled(value);
        }
    }

    public void SetYOffset(float offsetY)
    {
        foreach (WindElement element in GetComponentsInChildren<WindElement>())
        {
            element.SetYOffset(offsetY);
        }
    }
}
