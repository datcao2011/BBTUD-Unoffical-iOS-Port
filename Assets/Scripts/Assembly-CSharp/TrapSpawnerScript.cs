using System;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class TrapSpawnerScript : MonoBehaviour
{
	// Token: 0x06000076 RID: 118 RVA: 0x00004C04 File Offset: 0x00002E04
	private void Start()
	{
		for (int i = 0; i < this.woodCount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.trap);
			gameObject.transform.SetParent(transform.parent);
			gameObject.GetComponent<TrapScript>().trapSpawner = this;
		}
	}

	// Token: 0x040000B6 RID: 182
	public int rangeX;

	// Token: 0x040000B7 RID: 183
	public int rangeZ;

	// Token: 0x040000B8 RID: 184
	public int woodCount;

	// Token: 0x040000B9 RID: 185
	public GameObject trap;
    
}
