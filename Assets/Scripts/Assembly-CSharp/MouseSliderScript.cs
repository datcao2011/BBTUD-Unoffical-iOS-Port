using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000023 RID: 35
public class MouseSliderScript : MonoBehaviour
{
	// Token: 0x060000A9 RID: 169 RVA: 0x0000644C File Offset: 0x0000484C
	private void Start()
	{
		this.slider = base.GetComponent<Slider>();
		this.slider.value = Settings.GetFloat("TurnSensitivity", 1f);
		this.slider.onValueChanged.AddListener((value) =>
		{
			Settings.SetFloat("TurnSensitivity", value);
		});
	}

	// Token: 0x04000122 RID: 290
	public Slider slider;
}
