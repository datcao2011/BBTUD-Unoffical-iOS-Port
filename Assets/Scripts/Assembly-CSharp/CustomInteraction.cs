using UnityEngine;
using UnityEngine.Events;

public class CustomInteraction : MonoBehaviour
{
    [SerializeField]
    private GameObject clickCursor;

    private GameObject click;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float Distance;

#nullable enable
    public UnityEvent? OnInteract;
#nullable disable

    private void Update()
    {
        if (Vector3.Distance(player.position, transform.position) < Distance)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit raycastHit;
            bool isHovering = Physics.Raycast(ray, out raycastHit) && (raycastHit.transform == transform);
            if (isHovering)
            {
                if (click == null)
                {
                    click = Instantiate(clickCursor);
                    Destroy(click, 10.25f);
                }
                if (InteractScript.Intracting && Time.timeScale != 0f)
                {
                    OnInteract?.Invoke();
                }
            }
            else if (click != null)
            {
                Destroy(click);
            }
        }
        else if (click != null)
        {
            Destroy(click);
        }
    }
}