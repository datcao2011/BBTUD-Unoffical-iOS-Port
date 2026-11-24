using UnityEngine;

public class NPCGumScript : MonoBehaviour
{
    private Transform t;

    private void Start()
    {
        Object.Destroy(gameObject, 10f);
    }

    private void Update()
    {
        if (t != null)
        {
            Vector3 pos = t.position;
            pos.y = transform.position.y;
            transform.position = pos;
        }
    }

    public void SetTransform(Transform _set)
    {
        t = _set;
    }
}
