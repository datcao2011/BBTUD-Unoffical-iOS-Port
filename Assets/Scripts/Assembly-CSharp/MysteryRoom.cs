using UnityEngine;

public class MysteryRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Walls;

    [SerializeField]
    private GameObject[] WallsToReshow;

    [SerializeField]
    private GameObject FrontDoor;

    [SerializeField]
    private GameObject Item;

    [SerializeField]
    private GameControllerScript gc;

    private bool Active;

    private bool PlayerInside;

    private string[] itemObjectNames = new string[26]
    {
        "Nothing",
        "Pickup_EnergyFlavoredZestyBar",
        "Pickup_YellowDoorLock",
        "Pickup_Key",
        "Pickup_BSODA",
        "Pickup_Quarter",
        "Pickup_Tape",
        "Pickup_AlarmClock",
        "Pickup_WD-3D",
        "Pickup_SafetyScissors",
        "Pickup_Boots",
        "Pickup_Apple",
        "Pickup_AppleGod",
        "Pickup_Teleporter",
        "Pickup_CoinDoor",
        "Pickup_Pencil",
        "Pickup_Health",
        "Pickup_Whistle",
        "Pickup_MathPass",
        "Pickup_InvisibleItem",
        "Pickup_Hook",
        "Pickup_Portal",
        "Pickup_Nametag",
        "Pickup_Chalk",
        "Pickup_Hammer",
        "Pickup_Banana"
    };

    public void ReappearItem()
    {
        foreach (GameObject Wall in Walls)
        {
            Wall.SetActive(false);
        }
        FrontDoor.SetActive(true);
        int[] ItemIds = new int[] { 11, 20, 13, 24 };
        int ItemId = ItemIds[Random.Range(0, ItemIds.Length)];
        Item.GetComponentInChildren<SpriteRenderer>().sprite = gc.itemSprites[ItemId];
        Item.name = itemObjectNames[ItemId];
        Item.SetActive(true);
        Active = true;
        gameObject.SetActive(true);
    }

    public void EventEnded()
    {
        FrontDoor.SetActive(false);
        foreach (GameObject Wall in WallsToReshow)
        {
            Wall.SetActive(true);
        }
        Active = false;
        if (!PlayerInside)
        {
            gameObject.SetActive(false); // Now we hide this room altogether.
            foreach (GameObject Wall in Walls) // We can also show the walls again.
            {
                Wall.SetActive(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Player")
        {
            PlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            PlayerInside = false;
            if (!Active)
            {
                gameObject.SetActive(false); // Now we hide this room altogether.
                foreach (GameObject Wall in Walls) // We can also show the walls again.
                {
                    Wall.SetActive(true);
                }
            }
        }
    }
}