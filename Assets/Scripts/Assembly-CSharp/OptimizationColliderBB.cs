using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizationColliderBB : MonoBehaviour
{
    private bool IsInside;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Settings.GetString("ModeType", "Normal") != "BB")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !IsInside)
        {
            IsInside = true;
            Camera.main.useOcclusionCulling = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            IsInside = false;
            Camera.main.useOcclusionCulling = Settings.GetBool("OcclusionCulling", GameOptimizer.OptimizationDefault);
        }
    }
}
