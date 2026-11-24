using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorIncidator : MonoBehaviour
{
    private SpriteRenderer render;
    private float timer;
    public float Y = 2f;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        render.enabled = false;
        render.allowOcclusionWhenDynamic = false;
    }

    // Update is called once per frame
    void Update()
    {
        render.enabled = timer > 0f;
        timer -= (timer > 0f) ? Time.unscaledDeltaTime : 0f;
    }

    public void Hide()
    {
        timer = 0f;
    }

    public void Show(Vector3 position)
    {
        position.y = Y;
        transform.position = position;
        timer = 30f;
    }
}
