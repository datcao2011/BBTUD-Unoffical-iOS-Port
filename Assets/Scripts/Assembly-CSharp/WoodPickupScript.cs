using System;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class WoodPickupScript : MonoBehaviour
{
    private void Start()
    {
        if (woodSpawner == null) return;
        base.transform.position = new Vector3((float)UnityEngine.Random.Range(-woodSpawner.rangeX, this.woodSpawner.rangeX), 4f, (float)UnityEngine.Random.Range(-this.woodSpawner.rangeZ, this.woodSpawner.rangeZ));
        while (Physics.CheckSphere(base.transform.position, this.separateDistance, 1024, QueryTriggerInteraction.Collide))
        {
            base.transform.position = new Vector3((float)UnityEngine.Random.Range(-woodSpawner.rangeX, this.woodSpawner.rangeX), 4f, (float)UnityEngine.Random.Range(-this.woodSpawner.rangeZ, this.woodSpawner.rangeZ));
        }
    }

    private void Update()
    {
        bool IsHovering = false;
        if (Vector3.Distance(player.position, base.transform.position) < 10f && Time.timeScale != 0f)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            IsHovering = Physics.Raycast(ray, out hit) && hit.transform == base.transform;
        }

        if (InteractScript.Intracting)
        {
            if (IsHovering)
            {
                Destroy(click);
                Destroy(gameObject);
                camp.AddSticks(1);
                IsHovering = false;
            }
        }
        if (IsHovering)
        {
            if (click == null)
            {
                click = Instantiate(clickCursor);
                Destroy(click, 13f);
            }
        }
        else
        {
            if (click != null)
                Destroy(click);
        }
    }

    public Transform player;

    public CampingScript camp;

    public GameObject clickCursor;

    public GameObject click;

    public WoodSpawnerScript woodSpawner;

    // Token: 0x040000C4 RID: 196
    public float separateDistance;
}
