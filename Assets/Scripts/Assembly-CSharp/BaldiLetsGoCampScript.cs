using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaldiLetsGoCampScript : MonoBehaviour
{
    public Animator baldiAnimator;

    public AudioSource baldiLetsGo;

    private void Update()
    {
        if (!baldiLetsGo.isPlaying)
        {
            baldiAnimator.SetBool("BaldiCampTalking", false);
        }
        else
        {
            baldiAnimator.SetBool("BaldiCampTalking", true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            Invoke("Delay", 0.1f);
    }

    private void Delay()
    {
        baldiLetsGo.Play();
    }
}
