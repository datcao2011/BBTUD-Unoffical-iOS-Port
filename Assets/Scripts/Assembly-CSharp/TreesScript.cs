using System;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class TreesScript : MonoBehaviour
{
	// Token: 0x0600007B RID: 123 RVA: 0x00004D48 File Offset: 0x00002F48
	private void Start()
	{
		for (int i = 0; i < this.treeCount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.tree);
			gameObject.transform.SetParent(transform.parent);
			gameObject.GetComponent<TreeScript>().trees = base.gameObject.GetComponent<TreesScript>();
		}
	}

	// Token: 0x040000BC RID: 188
	public int rangeX;

	// Token: 0x040000BD RID: 189
	public int rangeZ;

	// Token: 0x040000BE RID: 190
	public int minX;

	// Token: 0x040000BF RID: 191
	public int minZ;

	// Token: 0x040000C0 RID: 192
	public int treeCount;

	// Token: 0x040000C1 RID: 193
	public GameObject tree;
}
