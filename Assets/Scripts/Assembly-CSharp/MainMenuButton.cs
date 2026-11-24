using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    private SpriteRenderer thisSprite;

    public Sprite hoveredSprite;

    private Sprite normalSprite;

    public OnClickedEvent onClick
    {
        get
        {
            return m_onClick;
        }
    }

    [SerializeField]
    private OnClickedEvent m_onClick = new OnClickedEvent();

    private GameObject MainCanvas;

    private void Awake()
    {
        thisSprite = GetComponentInChildren<SpriteRenderer>();
        normalSprite = thisSprite.sprite;
        MainCanvas = GameObject.Find("Main Canvas");
    }

    private void Update()
    {
        if (!MainCanvas.activeSelf)
        {
            return;
        }
        Vector2 position;

        if (InputManager.usingTouch)
        {
            if (Input.touchCount > 0)
            {
                position = Input.GetTouch(0).position;
            }
            else
            {
                position = new Vector2(0f, 0f);
            }
        }
#if UNITY_EDITOR
        else if (VirtualCursor.IsEnabled)
        {
            position = VirtualCursor.Position;
        }
#endif
        else
        {
            position = Input.mousePosition;
        }

        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform == transform)
        {

            thisSprite.sprite = hoveredSprite;
            if (Input.GetMouseButtonDown(0))
            {
                m_onClick.Invoke();
            }
        }
        else
        {
            thisSprite.sprite = normalSprite;
        }
    }

    [System.Serializable]
    public class OnClickedEvent : UnityEvent
    {

    }
}
