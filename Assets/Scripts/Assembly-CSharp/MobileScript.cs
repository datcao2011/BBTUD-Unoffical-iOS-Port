using UnityEngine;
using TouchControlsKit;

public class MobileScript : MonoBehaviour
{
    private TCKTouchpad pad;

    float sensitivity;

    private void Start()
    {
        if (!GetComponent<TCKTouchpad>())
        {
            pad = gameObject.AddComponent<TCKTouchpad>();
        }
        else
        {
            pad = GetComponent<TCKTouchpad>();
        }
        pad.sensitivity = 5f;
    }

    private void Update()
    {
        MobileTouchScript.XAxis = pad.axisX.value;
        MobileTouchScript.YAxis = pad.axisY.value;
    }

    private void OnEnable()
    {
        GameControllerScript GC = FindObjectOfType<GameControllerScript>();
        if (GC == null) return;
        GC.MoveSlider();
        if (Settings.GetInt("TypeHud", 1) == 1)
        {
            if (GameObject.Find("MobileItemSlot0") == null) return;
            #region Item Slots
            RectTransform element = GameObject.Find("MobileItemSlot0").GetComponent<RectTransform>();
            element.anchoredPosition = new Vector2(-278, -9);
            element = GameObject.Find("MobileItemSlot1").GetComponent<RectTransform>();
            element.anchoredPosition = new Vector2(-225, -9);
            element = GameObject.Find("MobileItemSlot2").GetComponent<RectTransform>();
            element.anchoredPosition = new Vector2(-170, -9);
            element = GameObject.Find("MobileItemSlot3").GetComponent<RectTransform>();
            element.anchoredPosition = new Vector2(-115, -9);
            element = GameObject.Find("MobileItemSlot4").GetComponent<RectTransform>();
            element.anchoredPosition = new Vector2(-60, -9);
            element = GameObject.Find("MobileItemSlot5").GetComponent<RectTransform>();
            element.anchoredPosition = new Vector2(-5, -9);
            #endregion
        }
    }

    private void OnDisable()
    {
        GameControllerScript GC = FindObjectOfType<GameControllerScript>();
        if (GC != null/* && !gameObject.activeInHierarchy*/) GC.MoveSlider();
    }
}
