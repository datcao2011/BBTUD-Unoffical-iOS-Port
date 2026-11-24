using Multiplayer;
using UnityEngine;

public class GumScript : MonoBehaviour
{
    public float speed;

    public GameObject NPCGummed;

    public AudioClip Splat;

    private float lifeSpan;

    private Rigidbody rb;

    private bool inBsoda;

    private Vector3 otherVelocity;

    private bool BeansExited;

    private void Start()
    {
        rb = base.GetComponent<Rigidbody>();
        rb.velocity = base.transform.forward * speed;
        lifeSpan = 15f;
    }

    private void Update()
    {
        if (!inBsoda)
            rb.velocity = base.transform.forward * speed;
        else
            rb.velocity = otherVelocity;
        lifeSpan -= Time.deltaTime;
        if (lifeSpan < 0f)
        {
            Object.Destroy(base.gameObject, 0f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "BSODA")
        {
            inBsoda = true;
            otherVelocity = ((Component)other).GetComponent<Rigidbody>().velocity;
        }
        if (other.name == "Grappling Hook")
        {
            Object.Destroy(base.gameObject, 0f);
            // Reference: https://www.youtube.com/watch?v=L8AEdH40BnA
        }
        if (!MultiplayerManager.isActive)
        {
            if (other.tag == "Player" && !other.isTrigger)
            {
                other.transform.GetComponent<PlayerScript>().Gummed(5f, Splat);
                GameObject.Find("Beans").GetComponent<BeansScript>().SorryForPlayer();
                Object.Destroy(base.gameObject, 0f);
            }
            else if (other.tag == "NPC" && other.transform.GetComponent<BsodaEffectScript>() && (other.name != "Beans" || BeansExited))
            {
                other.transform.GetComponent<BsodaEffectScript>().Gummed();
                GameObject Gum = Object.Instantiate(NPCGummed, new Vector3(other.transform.position.x, 20f * GetFloor(other.transform.position), other.transform.position.z), NPCGummed.transform.rotation);
                if (Gum.GetComponent<NPCGumScript>())
                {
                    Gum.GetComponent<NPCGumScript>().SetTransform(other.transform);
                }
                GameObject.Find("Beans").GetComponent<BeansScript>().SorryForNPC();
                Object.Destroy(base.gameObject, 0f);
            }
        }
        else
        {
            if (!MultiplayerManager.instance.isBaldi && other.tag == "Player")
            {
                other.transform.GetComponent<PlayerScript>().Gummed(10f, Splat);
                if (MultiplayerManager.instance.isMainPlayer) GameObject.Find("Beans").GetComponent<BeansScript>().SorryForPlayer();
                ClientSend.SendActionToOtherPlayer("gumhit:player:" + Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber);
                Object.Destroy(base.gameObject, 0f);
            }
            if (MultiplayerManager.instance.isMainPlayer)
            {
                if (other.tag == "NPC" && other.transform.GetComponent<BsodaEffectScript>() && (other.name != "Beans" || BeansExited))
                {
                    if ((other.name == "Baldi" && MultiplayerManager.instance.isAIBaldi) || other.name != "Baldi")
                    {
                        other.transform.GetComponent<BsodaEffectScript>().Gummed();
                        GameObject Gum = Instantiate(NPCGummed, new Vector3(other.transform.position.x, 20f * GetFloor(other.transform.position), other.transform.position.z), NPCGummed.transform.rotation);
                        if (Gum.GetComponent<NPCGumScript>())
                        {
                            Gum.GetComponent<NPCGumScript>().SetTransform(other.transform);
                        }
                        GameObject.Find("Beans").GetComponent<BeansScript>().SorryForNPC();
                        Object.Destroy(base.gameObject, 0f);
                        ClientSend.SendActionToOtherPlayer("gumhit:npc:" + other.transform.name);
                    }
                }
            }
            else if (MultiplayerManager.instance.isBaldi)
            {
                if (other.tag == "NPC" && other.transform.GetComponent<BsodaEffectScript>() && other.name == "Baldi")
                {
                    FindObjectOfType<PlayerScript>().Gummed(10f, Splat);
                    other.transform.GetComponent<BsodaEffectScript>().Gummed();
                    Destroy(base.gameObject, 0f);
                    ClientSend.SendActionToOtherPlayer("gumhit:npc:" + other.transform.name);
                }
            }
        }
    }

    private float GetFloor(Vector3 pos)
    {
        if (pos.y >= 40f) return 2;
        if (pos.y >= 20f) return 1;
        return 0;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "BSODA")
        {
            inBsoda = false;
        }
        if (other.name == "Beans")
        {
            BeansExited = true;
        }
    }
}