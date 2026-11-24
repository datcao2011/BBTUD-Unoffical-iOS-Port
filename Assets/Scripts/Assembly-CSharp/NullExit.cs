using UnityEngine;

public class NullExit : MonoBehaviour
{
    public GameControllerScript gc;
    private Transform player;
    private NullScript NULL;
    private EntranceScript ec;

    private void Start()
    {
        NULL = NullManager.Instance.NULL;
        player = gc.playerTransform;
        ec = GetComponent<EntranceScript>();
    }

    private void Update()
    {
        if (gc.exitsReached == 3 && !ec.exitTaken && Vector3.Distance(player.position, transform.position) <= 100f && NULL.NullRunFrom == 1)
        {
            NULL.GetToExit(transform.position);
        }
    }
}