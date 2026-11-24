using UnityEngine;

public class BB_Button : MonoBehaviour
{
    private void Awake()
    {
        Text.text = "0";
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit raycastHit;
        bool IsHovering = Manager.Wall.activeSelf && Vector3.Distance(player.position, transform.position) < 7f & Physics.Raycast(ray, out raycastHit) && raycastHit.transform == transform;
        if (InteractScript.Intracting)
        {
            if (IsHovering)
            {
                Num++;
                if (Num >= 10) Num = 0;
                Text.text = Num.ToString();
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

    public Transform player;
    public TMPro.TMP_Text Text;
    public BB_ButtonManager Manager;
    public int Num;
    public GameObject clickCursor;
    private GameObject click;
}