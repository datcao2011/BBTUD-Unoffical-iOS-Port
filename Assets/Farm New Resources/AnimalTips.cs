using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalTips : MonoBehaviour
{
    private void Start()
    {
        if (Settings.HasKey("AnimalTips_HasShown"))
        {
            gameObject.SetActive(false);
            return;
        }
        Settings.SetInt("AnimalTips_HasShown", 1);
    }
}
