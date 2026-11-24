// ClickableTest
using UnityEngine;

public class ClickableTest : MonoBehaviour
{
    public GameObject clickCursor;

    private void Start()
    {
    }

    private void Update()
    {
        if (InteractScript.Intracting)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.name == "MathNotebook")
            {
                base.gameObject.SetActive(false);
            }
            Ray rray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit rraycastHit;
            if (Physics.Raycast(rray, out rraycastHit) && rraycastHit.transform.name == "MathNotebook")
            {
                clickCursor.SetActive(true);
            }
            else
            {
                clickCursor.SetActive(false);
            }
        }
    }
}