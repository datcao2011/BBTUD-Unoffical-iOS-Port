using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrapplingHook : MonoBehaviour
{
    public bool AttachedNowGrapple;
    public bool FindingToAttach;
    public bool Debug_StartGrappling;
    public AudioSource audioDevice;
    public AudioSource grapplingLoop;
    public Rigidbody rb;
    public AudioClip aud_Start;
    public AudioClip aud_Attach;
    public AudioClip aud_Break;
    private Vector3 WhereToVelocity;
    public float GrapplingSpeed = 1f;
    public float Grappling_1_Time = 0f;
    public GameObject Crack;
    public CameraScript Cam;
    public string[] ExcludedObjects;
    public Transform Cube;
    public Transform Stretch;

    // Start is called before the first frame update
    private void Start()
    {
        name = "Grappling Hook";
    }

    private float InitialDistance;
    private float BreakPressure;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (FindingToAttach)
        {
            rb.velocity = WhereToVelocity * GrapplingSpeed;
            Grappling_1_Time += Time.fixedDeltaTime;
            if (Grappling_1_Time >= 15f) // If this been like this for 15 seconds, we gotta break it in order to not softlock.
            {
                OnAttached();
                Break();
            }
            else if (Mathf.Abs(transform.position.y - Cam.playerPos.position.y) > 10) // It'll break instantly.
            {
                OnAttached();
                Break();
            }
        }
        if (AttachedNowGrapple)
        {
            Current.Speed += 15f * Time.fixedDeltaTime;
            BreakPressure = (transform.position - Cam.playerPos.position).magnitude - (InitialDistance - Current.Speed);
            grapplingLoop.pitch = (Current.Speed - 20f) / 100f + 1f;
            if ((transform.position - Cam.playerPos.position).magnitude <= 5f)
            {
                Disable();
            }
            else if (BreakPressure > 100f)
            {
                Break();
            }
            else if (Mathf.Abs(transform.position.y - Cam.playerPos.position.y) > 10) // It'll break instantly if different heights.
            {
                Break();
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 Pos = new Vector3(Cam.playerPos.position.x, transform.position.y, Cam.playerPos.position.z);
        Cube.LookAt(Pos);
        Vector3 lookRot = Cube.transform.eulerAngles;
        lookRot.y -= 180;
        Cube.transform.eulerAngles = lookRot;

        float ScaleValue = Vector3.Distance(transform.position, Pos);
        Stretch.localScale = new Vector3(0.1f, 0.1f, ScaleValue);
        Stretch.localPosition = new Vector3(0f, 0f, ScaleValue / -2f);
    }

    public void StartGrappling()
    {
        Grappling_1_Time = 0f;
        Cam.DenyRotationYMovement = true;
        audioDevice.PlayOneShot(aud_Start);
        FindingToAttach = true;
        AttachedNowGrapple = false;
        Transform Player = FindObjectOfType<PlayerScript>().transform;
        WhereToVelocity = Player.forward;
        Crack.SetActive(false);
        rb.isKinematic = false;
        Vector3 NewPosition = Player.position + (Player.forward * 2f);
        NewPosition.y = (Cam.ps.FlatDuration > 0f) ? 1.5f : Player.position.y;
        rb.position = NewPosition;
        Vector3 NewAngles = transform.eulerAngles;
        NewAngles.y = Player.eulerAngles.y;
        rb.rotation = Quaternion.Euler(NewAngles);
    }

    public void Break()
    {
        // Audio will be disabled, so we create a seperate one.
        GameObject gameObject = new GameObject("Break!");
        Destroy(gameObject, aud_Break.length);
        AudioSource audioDevice = gameObject.AddComponent<AudioSource>();
        audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
        gameObject.AddComponent<AudioSourceSpeed>();
        audioDevice.clip = aud_Break;
        audioDevice.Play();
        Disable();
    }

    public void Disable()
    {
        grapplingLoop.Stop();
        grapplingLoop.pitch = 1f;
        AttachedNowGrapple = false;
        FindingToAttach = false;
        if (Current != null)
        {
            Cam.ps.GrappleLocation.Remove(Current);
        }
        Cam.DenyRotationYMovement = false;
        Destroy(gameObject);
    }

    private void OnAttached()
    {
        FindingToAttach = false;
        audioDevice.PlayOneShot(aud_Attach);
        grapplingLoop.Play();
        AttachedNowGrapple = true;
        WhereToVelocity = Vector3.zero;
        rb.velocity = WhereToVelocity;
        Crack.SetActive(true);
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        Cam.DenyRotationYMovement = false;
        InitialDistance = (transform.position - Cam.playerPos.position).magnitude;
        Current = new GrapplingHookItem
        {
            Hook = transform,
            Speed = 20f
        };
        Cam.ps.GrappleLocation.Add(Current);
    }

    private GrapplingHookItem Current;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "NPC" && FindingToAttach)
        {
            bool IsExcluded = false;
            foreach (string ExcludedName in ExcludedObjects)
            {
                if (collision.gameObject.name == ExcludedName) IsExcluded = true;
            }
            if (!IsExcluded) OnAttached();
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("Entered trigger with " + other.gameObject.name);
    //     OnAttached();
    // }
}
