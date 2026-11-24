using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindElement : MonoBehaviour
{
    private MeshRenderer renderer1;
    private MeshRenderer renderer2;
    public Vector3 WindVelocity { get; private set; }
    private PlayerScript ps;
    public bool isActive;

    private void Start()
    {
        ps = FindObjectOfType<PlayerScript>();
        renderer1 = GetComponent<MeshRenderer>();
        if (!GetComponent<MeshRenderer>())
        {
            Debug.LogError("For some reason, we can't get the mesh renderer D:");
        }
        renderer2 = transform.GetChild(0).GetComponent<MeshRenderer>();
        SetEnabled(false);
    }

    private void Update()
    {
        if (isActive)
        {
            WindVelocity = transform.forward * 6.5f;
        }
        else
        {
            WindVelocity = Vector3.zero;
        }
    }

    public void SetEnabled(bool value)
    {
        renderer1.enabled = value;
        renderer2.enabled = value;
        isActive = value;
        if (!value && ps.windVelocity != Vector3.zero)
        {
            ps.windVelocity = Vector3.zero;
        }
    }

    public void SetYOffset(float offset)
    {
        if (!renderer1.enabled && !renderer2.enabled)
        {
            return;
        }
        renderer1.material.mainTextureOffset = new Vector2(0f, offset);
        renderer2.material.mainTextureOffset = new Vector2(0f, offset);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && renderer1.enabled)
        {
            ps.windVelocity = transform.forward * 6.5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            ps.windVelocity = Vector3.zero;
        }
    }
}
