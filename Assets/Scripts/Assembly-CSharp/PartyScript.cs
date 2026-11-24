using UnityEngine;

public class PartyScript : MonoBehaviour
{
    public PartyFunLocation[] Locations;

    public RandomEvent RandomEventManager;

    public GameObject PartyBalloons;

    private bool PartyActive;

    private void Update()
    {
        if (!PartyActive && RandomEventManager.party)
        {
            PartyBalloons.SetActive(true);
        }
        else if (PartyActive && !RandomEventManager.party)
        {
            PartyBalloons.SetActive(false);
        }
        PartyActive = RandomEventManager.party;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (RandomEventManager.party && other.tag == "NPC")
        {
            other.GetComponent<NPC>().OnPartyHavingFun = true;
            other.GetComponent<NPC>().SetTarget(RandomLocation().transform.position);
        }
    }

    public PartyFunLocation RandomLocation()
    {
        return Locations[Random.Range(0, Locations.Length)];
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "NPC")
        {
            other.GetComponent<NPC>().OnPartyHavingFun = false;
        }
    }
}