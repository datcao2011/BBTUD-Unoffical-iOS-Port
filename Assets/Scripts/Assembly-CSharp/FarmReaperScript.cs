using UnityEngine;

public class FarmReaperScript : MonoBehaviour
{
    [SerializeField]
    private Collider Player;

    private void Start()
    {
        Physics.IgnoreCollision(Player, GetComponent<Collider>());
        Physics.IgnoreLayerCollision(4, 9);
    }

    private void Update()
    {
        transform.position += new Vector3(0f, 0f, 2f * Time.deltaTime);
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject && !other.transform.name.Contains("Grass") && other.transform.name != "Player" && other.transform.tag != "NPC")
        {
            if (other.transform.parent != null && other.transform.parent.name.Contains("CornTexture"))
            {
                Destroy(other.transform.parent.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}