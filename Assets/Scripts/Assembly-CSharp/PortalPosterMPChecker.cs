using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPosterMPChecker : MonoBehaviour
{
    [SerializeField]
    private Material OutOf;

    private void Awake()
    {
        if (Settings.GetString("ModeType") == "Multiplayer")
        {
            GetComponent<MeshRenderer>().sharedMaterial = OutOf;
            transform.parent.name = "PortalMachine_OutOf";
        }
        Destroy(this);
    }
}
