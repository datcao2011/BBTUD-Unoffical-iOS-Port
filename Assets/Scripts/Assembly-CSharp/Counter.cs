using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    private void Start()
    {
        console.log(FindObjectsOfType<MeshRenderer>().Length);
    }
}
