using UnityEngine;

public class BB_Candle : MonoBehaviour
{
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit raycastHit;
        bool IsHovering = Candle.activeSelf && Physics.Raycast(ray, out raycastHit) && raycastHit.transform == transform;
        if (InteractScript.Intracting)
        {
            if (IsHovering)
            {
                BirthdayBash.Instance.Glitch();
            }
        }
        if (IsHovering)
        {
            if (click == null)
                click = Object.Instantiate(clickCursor);
        }
        else
        {
            if (click != null)
                Object.Destroy(click);
        }
    }

    public GameObject Candle;
    public GameObject clickCursor;
    private GameObject click;
}