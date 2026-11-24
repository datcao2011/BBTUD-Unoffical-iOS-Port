using System;
using UnityEngine;

// Token: 0x02000005 RID: 5
public class BalloonCollision : MonoBehaviour
{
    // Token: 0x0600000C RID: 12 RVA: 0x000020F6 File Offset: 0x000002F6
    private void Start()
    {
        this.ChangeDirection();
        Physics.IgnoreCollision(Mine, Player, true);
    }

    // Token: 0x0600000D RID: 13 RVA: 0x00008CB4 File Offset: 0x00006EB4
    private void Update()
    {
        if (directionTime > 0f)
        {
            this.directionTime -= Time.deltaTime;
        }
        this.rb.velocity = this.direction * this.speed;
    }

    // Token: 0x0600000E RID: 14 RVA: 0x00008D10 File Offset: 0x00006F10
    private void ChangeDirection()
    {
        this.direction.x = UnityEngine.Random.Range(-1f, 1f);
        this.direction.z = UnityEngine.Random.Range(-1f, 1f);
        this.direction = this.direction.normalized;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.transform.tag == "Floor") return;
        if (this.directionTime <= 0f)
        {
            this.ChangeDirection();
            this.directionTime = 0.4f;
        }
    }

    // Token: 0x0400000C RID: 12
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Collider Player;
    
    [SerializeField]
    private Collider Mine;

    // Token: 0x0400000D RID: 13
    private float directionTime = 0.4f;

    // Token: 0x0400000E RID: 14
    private Vector3 direction;

    // Token: 0x0400000F RID: 15
    public float speed = 10f;
}