using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpaceMobile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField]
    private Color jumpColor;

    [SerializeField]
    private CameraScript cameraScript;

    [SerializeField]
    private Image imagePlaceHolder;

    [SerializeField]
    private Sprite[] placeHolderSprite = new Sprite[2];

    public bool JumpButton;
    public GameObject ShowSprite;

    private void Start()
    {
        cameraScript = Camera.main.GetComponent<CameraScript>();
    }

    private void Update()
    {
        if (JumpButton)
        {
            ShowSprite.SetActive(cameraScript.ps.jumpRope);
            return;
        }
        placeHolderSprite[1] = InputManager.instance.jump;
        if (cameraScript.ps.hiding)
        {
            imagePlaceHolder.sprite = placeHolderSprite[1];
            imagePlaceHolder.color = jumpColor;
        }
        else
        {
            imagePlaceHolder.sprite = placeHolderSprite[0];
            imagePlaceHolder.color = Color.white;
        }
    }

    public void Press()
    {
        if (JumpButton)
        {
            cameraScript.MobilePressed();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Press();
        if (!JumpButton && cameraScript != null) cameraScript.ToggleLookBehind();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Press();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}