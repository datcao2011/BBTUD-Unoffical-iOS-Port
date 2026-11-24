using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackMaterial : MonoBehaviour
{
    private MeshRenderer black;
    private Material Current;

    // Start is called before the first frame update
    private void Start()
    {
        black = GetComponent<MeshRenderer>();
    }

    private void LateUpdate()
    {
        if (Current != black.material)
        {
            Current = black.material;
            black.material.color = Color.black;
        }
    }
}
