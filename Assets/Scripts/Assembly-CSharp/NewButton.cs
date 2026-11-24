using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Platforms;

[AddComponentMenu("UI/New Button")]
public class NewButton : Button
{
    private bool highlighted;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        highlighted = true;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        highlighted = false;
    }

    private void Update()
    {
        if (GamePadController.isAnyGamepadConnected)
        {
            highlighted = IsHighlighted();
        }
    }

    public bool isHighlighted
    {
        get
        {
            return highlighted;
        }
    }
}