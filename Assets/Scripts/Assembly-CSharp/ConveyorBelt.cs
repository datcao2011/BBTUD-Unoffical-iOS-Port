using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private PlayerScript player;
    public MeshRenderer meshRenderer;
    private float value;

    // Start is called before the first frame update
    private void Start()
    {
        player = FindObjectOfType<PlayerScript>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (player.transform.position.y >= 20f)
        {
            value += Time.deltaTime * 0.75f;
            meshRenderer.material.mainTextureOffset = new Vector2(0f, value);
        }
        else if (value != 0f)
        {
            value = 0f;
            meshRenderer.material.mainTextureOffset = new Vector2(0f, value);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Player" && !other.isTrigger)
        {
            player.windVelocity = transform.forward * 7f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            player.windVelocity = Vector3.zero;
        }
    }
}
