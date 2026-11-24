using UnityEngine;

public class GameObjectMobileDetector : MonoBehaviour
{
    private Transform[] items;
    private bool enable;

    private void Awake()
    {
        items = new Transform[transform.childCount];
        int i = 0;
        foreach (Transform item in transform)
        {
            items[i] = item;
            i++;
        }
        if (InputManager.usingTouch && !enable)
        { EnableOrDisable(true); }
        else if (!InputManager.usingTouch && enable)
        { EnableOrDisable(false); }
    }

    private void Update()
    {
        if (InputManager.usingTouch && !enable)
        { EnableOrDisable(true); }
        else if (!InputManager.usingTouch && enable)
        { EnableOrDisable(false); }
    }

    private void EnableOrDisable(bool enabled)
    {
        enable = enabled;
        foreach (Transform item in items)
        {
            item.gameObject.SetActive(enabled);
        }
    }
}