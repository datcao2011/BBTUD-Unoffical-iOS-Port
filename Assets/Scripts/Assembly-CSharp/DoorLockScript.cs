using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLockScript : MonoBehaviour {

    public AudioSource doorClose;

    public bool played;

    private void OnTriggerStay(Collider other)
    {
        if (!played && other.tag == "Player")
        {
            doorClose.Play();
            played = true;
            if (FindObjectOfType<TutorBaldi>()) 
            {
                FindObjectOfType<TutorBaldi>().SayHi();
            }
            if (other.transform.position.z < -8f)
            {
                other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, Mathf.Clamp(other.transform.position.z, -15f, 15f));
            }
        }
    }
}
