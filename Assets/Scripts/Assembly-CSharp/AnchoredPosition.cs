using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchoredPosition : MonoBehaviour {

	public Vector2 anchoredPosition;

	private void FixedUpdate()
	{
		transform.position = anchoredPosition;
	}
}
