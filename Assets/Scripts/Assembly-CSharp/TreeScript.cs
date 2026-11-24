using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class TreeScript : MonoBehaviour
{
	// Token: 0x06000078 RID: 120 RVA: 0x00004C54 File Offset: 0x00002E54
	private void Start()
	{
		base.transform.position = new Vector3((float)UnityEngine.Random.Range(this.trees.minX, this.trees.rangeX), 20f, (float)UnityEngine.Random.Range(this.trees.minZ, this.trees.rangeZ));
		while (Physics.CheckSphere(base.transform.position, this.separateDistance, 1024, QueryTriggerInteraction.Collide))
		{
			base.transform.position = new Vector3((float)UnityEngine.Random.Range(this.trees.minX, this.trees.rangeX), 20f, (float)UnityEngine.Random.Range(this.trees.minZ, this.trees.rangeZ));
		}
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00004D3B File Offset: 0x00002F3B
	private void Update()
	{
	}

	// Token: 0x040000BA RID: 186
	public TreesScript trees;

	// Token: 0x040000BB RID: 187
	public float separateDistance;
}
