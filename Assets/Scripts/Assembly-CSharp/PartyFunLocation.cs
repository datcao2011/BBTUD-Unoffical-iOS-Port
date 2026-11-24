using UnityEngine;

public class PartyFunLocation : MonoBehaviour
{
    public PartyScript Manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NPC" && other.GetComponent<NPC>().OnPartyHavingFun)
        {
            PartyFunLocation Location = Manager.RandomLocation();
            while (Location == this)
            {
                Location = Manager.RandomLocation();
            }
            other.GetComponent<NPC>().SetTarget(Location.transform.position);
        }
    }
}