// BsodaSparyScript
using UnityEngine;

public class BsodaSparyScript : MonoBehaviour
{
    public float speed;

    public float lifeSpan = 30f;

    private Rigidbody rb;

    private void Start()
    {
        rb = base.GetComponent<Rigidbody>();
        rb.velocity = base.transform.forward * speed;
    }

    private void Update()
    {
        rb.velocity = base.transform.forward * speed;
        lifeSpan -= Time.deltaTime;
        if (lifeSpan < 0f)
        {
            Object.Destroy(base.gameObject, 0f);
        }
    }
}