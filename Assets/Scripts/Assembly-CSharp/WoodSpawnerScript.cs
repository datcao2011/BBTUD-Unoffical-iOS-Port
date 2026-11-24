using UnityEngine;

public class WoodSpawnerScript : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < this.woodCount; i++)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.firewood);
            gameObject.transform.SetParent(transform.parent);
            WoodPickupScript script = gameObject.GetComponent<WoodPickupScript>();
            script.woodSpawner = this;
            script.player = FindObjectOfType<PlayerScript>().transform;
            script.camp = FindObjectOfType<CampingScript>();
        }
    }

    // Token: 0x040000C5 RID: 197
    public int rangeX;

    // Token: 0x040000C6 RID: 198
    public int rangeZ;

    // Token: 0x040000C7 RID: 199
    public int woodCount;

    // Token: 0x040000C8 RID: 200
    public GameObject firewood;
}