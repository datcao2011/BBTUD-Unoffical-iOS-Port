using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200000D RID: 13
public class CutscenePrizeScript : MonoBehaviour
{
	private Vector3 LastPos;

	public float Magnitude;

	// Token: 0x0600002D RID: 45 RVA: 0x00002A30 File Offset: 0x00000E30
	private void Start()
	{
		LastPos = transform.position;
	}

	private void Update()
	{
		this.motorAudio.pitch = Magnitude + 1f;
	}

	public AudioSource motorAudio;
}
