using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrReflexHand : MonoBehaviour
{
    public DrReflex Reflex;
    public bool IsRight;
    public LayerMask Mask;

    // Update is called once per frame
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit hit;
        if (InteractScript.Intracting && Physics.Raycast(ray, out hit, float.PositiveInfinity, Mask, QueryTriggerInteraction.Ignore)) // This should make sure that any triggers isn't blocking the player.
        {
            if (hit.transform == transform)
            {
                Reflex.Answer(IsRight);
            }
        }
    }
}
