using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class BB_SpoopBalloon : MonoBehaviour
{
	// Token: 0x06000AE5 RID: 2789 RVA: 0x00028CDC File Offset: 0x00026EDC
	private void FixedUpdate()
	{
		this.rb.AddForce((this.player.position - base.transform.position).normalized * this.speed);
	}

	// Token: 0x04000818 RID: 2072
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x04000819 RID: 2073
	public Transform player;

	// Token: 0x0400081A RID: 2074
	public float speed;
}
