using System;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private PlayerScript player;
    private CameraScript cm;

    private bool pickedUp;

    public bool Thrown;

    public ProjectileSpawner ps;

    private BsodaSparyScript Spary;

    private Rigidbody rb;

    private Vector3 InitPoint;

    private float TimeBeforeReset;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
        cm = Camera.main.GetComponent<CameraScript>();
        Spary = GetComponent<BsodaSparyScript>();
        Spary.enabled = false;
        Spary.lifeSpan = float.PositiveInfinity;
        InitPoint = new Vector3(transform.position.x, 4f, transform.position.z);
    }

    private void Update()
    {
        if (pickedUp)
        {
            if (InteractScript.Intracting && Time.timeScale != 0f)
            {
                TimeBeforeReset = 10f;
                pickedUp = false;
                Thrown = true;
                GetComponentInChildren<PickupAnimationScript>().enabled = false;
                player.heldObject = false;
                player.gc.itemText.text = "Nothing";
                Spary.enabled = true;
                Spary.speed = 100f;
            }
        }
        else if (Thrown)
        {
            if (TimeBeforeReset > 0f)
            {
                TimeBeforeReset -= Time.deltaTime;
            }
            else
            {
                TimeBeforeReset = 0f;
                Thrown = false;
                pickedUp = false;
                Spary.enabled = false;
                Spary.lifeSpan = float.PositiveInfinity;
                GetComponentInChildren<PickupAnimationScript>().enabled = true;
                rb.velocity = Vector3.zero;
                rb.position = InitPoint;
                transform.position = InitPoint;
                rb.rotation = Quaternion.identity;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !pickedUp & !Thrown)
        {
            if (!this.player.heldObject)
            {
                this.player.heldObject = true;
                this.pickedUp = true;
                player.gc.itemText.text = name.Split('_')[0];
            }
        }
    }

    private void LateUpdate()
    {
        if (this.pickedUp && !Thrown)
        {
            base.transform.position = this.player.transform.position + this.player.transform.forward * (3f * cm.GetLookBehindValue()) + Vector3.up * -2f;
            base.transform.rotation = this.cm.transform.rotation;
        }
    }
}
