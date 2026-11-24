using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MBMovement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private float initialDistance;
    private Vector3 initialScale;
    private RectTransform rectTransform;
    private bool IsSelected;
    private bool IsClicked;
    private float SelectDelay;
    public bool Scalable = true;
    private MaskableGraphic Background;
    public bool Moveable = true;

    private Vector2 initialPos;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Background = GetComponent<MaskableGraphic>();
        if (Background == null)
        {
            Background = GetComponentInChildren<MaskableGraphic>();
        }
        SetColor(Color.white);
    }

    private void SetColor(Color color)
    {
        if (Background != null)
        {
            color.a = Background.color.a;
            Background.color = color;
        }
    }

    //Detect current clicks on the GameObject (the one with the script attached)
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //Output the name of the GameObject that is being clicked
        IsClicked = true;
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        SetColor(Color.white);
        if (IsSelected)
        {
            Vector2 Pos = rectTransform.anchoredPosition;
            Vector2 Sca = rectTransform.localScale;
            Settings.SetString($"MobileC_{rectTransform.name}", $"{Pos.x.ToString(CultureInfo.InvariantCulture)},{Pos.y.ToString(CultureInfo.InvariantCulture)}|{Sca.x.ToString(CultureInfo.InvariantCulture)},{Sca.y.ToString(CultureInfo.InvariantCulture)}");
        }
        IsClicked = false;
        IsSelected = false;
        SelectDelay = 0f;
    }

    private void Update()
    {
        if (IsClicked && SelectDelay < 1f && Input.touchCount == 1)
        {
            SelectDelay += Time.unscaledDeltaTime;
            return;
        }
        else if (IsClicked && SelectDelay >= 1f && !IsSelected)
        {
            IsSelected = true;
            SetColor(Color.blue);
            initialPos = rectTransform.position;

            if (Input.touchCount >= 2)
            {
                var touchZero = Input.GetTouch(0);
                var touchOne = Input.GetTouch(1);
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = rectTransform.localScale;
            }
        }
        if (!IsSelected)
        {
            return;
        }
        if (Moveable)
        {
            if (Input.touchCount >= 1 && Input.touchCount <= 2)
            {
                if (Input.touchCount == 1)
                {
                    var touchZero = Input.GetTouch(0);
                    if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled)
                    {
                        return;
                    }
                    Vector2 Pos1 = touchZero.position;
                    rectTransform.position = Pos1;
                }
                else
                {
                    var touchZero = Input.GetTouch(0);
                    if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled)
                    {
                        return;
                    }
                    Vector2 Pos1 = touchZero.position;
                    var touchOne = Input.GetTouch(1);
                    if (touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
                    {
                        rectTransform.position = Pos1;
                        return;
                    }
                    Vector2 Pos2 = touchZero.position;
                    Vector2 Difference = Vector3.Lerp(Pos2, Pos1, 0.5f);
                    rectTransform.position = Difference;
                }
            }
        }
        if (Scalable)
        {
            if (Input.touchCount == 2)
            {
                var touchZero = Input.GetTouch(0);
                var touchOne = Input.GetTouch(1);

                // if one of the touches Ended or Canceled do nothing
                if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled
                   || touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
                {
                    return;
                }

                // It is enough to check whether one of them began since we
                // already excluded the Ended and Canceled phase in the line before
                if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
                {
                    // track the initial values
                    initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                    initialScale = rectTransform.localScale;
                }
                // else now is any other case where touchZero and/or touchOne are in one of the states
                // of Stationary or Moved
                else
                {
                    // otherwise get the current distance
                    var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                    // A little emergency brake ;)
                    if (Mathf.Approximately(initialDistance, 0)) return;

                    // get the scale factor of the current distance relative to the inital one
                    var factor = currentDistance / initialDistance;

                    // apply the scale
                    // instead of a continuous addition rather always base the 
                    // calculation on the initial and current value only
                    rectTransform.localScale = initialScale * factor;
                }
            }
        }
    }
}