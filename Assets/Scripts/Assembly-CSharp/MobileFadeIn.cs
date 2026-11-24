using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileFadeIn : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    private float Duration = 0f;

    private void Update()
    {
        if (InputManager.usingTouch)
        {
            if (Input.touchCount > 0)
            {
                anim.SetBool("b", true);
                Duration = 5f;
            }
            if (Duration > 0f)
            {
                Duration -= Time.deltaTime;
                if (Duration <= 0f)
                {
                    Duration = 0f;
                    anim.SetBool("b", false);
                }
            }
        }
    }
}
