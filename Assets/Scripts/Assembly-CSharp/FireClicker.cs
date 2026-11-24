using UnityEngine;

public class FireClicker : MonoBehaviour
{
    private void Update()
    {
        if (InteractScript.Intracting)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "FireClicker" & Vector3.Distance(player.position, base.transform.position) < 10f)
                {
                    fireS.WoodInFire();
                }
            }
        }
        Ray ray2 = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit hit2;
        if (Physics.Raycast(ray2, out hit2))
        {
            if (hit2.transform.name == "FireClicker" & Vector3.Distance(player.position, base.transform.position) < 10f)
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
    }

    public FireScript fireS;

    public Transform player;

    public GameObject clickCursor;

    public GameObject click;
}