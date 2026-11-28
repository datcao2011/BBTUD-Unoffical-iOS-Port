// PickupScript
using System.Globalization;
using SchoolSaves;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    public GameControllerScript gc;

    public Transform player;

    public GameObject clickCursor;

    public GameObject click;

    public bool pickupAfterLockerOpen;

    public OpenLockerScript lockerScript;

    private void Start()
    {
    }

    private void Update()
    {
        if (!pickupAfterLockerOpen)
        {
        }
        else if ((pickupAfterLockerOpen & !lockerScript.bDoorOpen) || (lockerScript.bDoorOpen & lockerScript.openTime > 2.85f))
        {
            if (click != null)
                Object.Destroy(click);
            return;
        }
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi) return;
        if (gc.player.IsFlat)
        {
            if (click != null) // Destroy the click cursor if available
                Object.Destroy(click);
            return;
        }
        if (Vector3.Distance(player.position, base.transform.position) >= 10f) // To save FPS, credit: https://gamebanana.com/tuts/15260
        {
            if (click != null) // Destroy the click cursor if available
                Object.Destroy(click);
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit raycastHit;
        bool IsHovering = Physics.Raycast(ray, out raycastHit) && Vector3.Distance(player.position, raycastHit.transform.position) < 10f; // Double check to make sure you can't pick up items far away.
        if (InteractScript.Intracting)
        {
            if (IsHovering)
            {
                if (raycastHit.transform.name == "Pickup_EnergyFlavoredZestyBar")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(1, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_YellowDoorLock")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(2, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Key")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(3, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_BSODA")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(4, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name.StartsWith("Pickup_Quarter"))
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(5, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Tape")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(6, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_AlarmClock")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(7, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_WD-3D")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(8, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_SafetyScissors")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(9, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Boots")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(10, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Apple")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(11, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_AppleGod")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(12, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Teleporter")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(13, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_CoinDoor")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(14, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Pencil")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(15, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Health")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(16, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Whistle")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(17, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_MathPass")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(18, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_InvisibleItem")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(19, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Hook")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(20, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Portal")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(21, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Nametag")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(22, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Chalk")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(23, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Hammer")
                {
                    Object.Destroy(click);
                    gc.IsGoingToCarryDRRHammer = true;
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(24, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Banana")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(25, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name.StartsWith("Pickup_HammerReflex")) // Doctor Reflex's Hammer
                {
                    Object.Destroy(click);
                    gc.IsGoingToCarryDRRHammer = true;
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(24, raycastHit.transform.position);
                }
                else if (raycastHit.transform.name == "Pickup_Present")
                {
                    Object.Destroy(click);
                    raycastHit.transform.gameObject.SetActive(false);
                    CollectItem(Mathf.RoundToInt(Random.Range(1f, 24f)), raycastHit.transform.position);
                }
                if (MultiplayerManager.isActive && raycastHit.transform.name.StartsWith("Pickup_"))
                {
                    string name = raycastHit.transform.name;
                    if (name.StartsWith("Pickup_Quarter") && name != "Pickup_Quarter")
                    {
                        return;
                    }
                    float[] Position = raycastHit.transform.position.ToFloat();
                    string Pos = Position[0].ToString(CultureInfo.InvariantCulture) + "," + Position[1].ToString(CultureInfo.InvariantCulture) + "," + Position[2].ToString(CultureInfo.InvariantCulture);
                    Multiplayer.ClientSend.SendActionToOtherPlayer("item:" + Pos + ":" + name);
                }
            }
        }
        if (IsHovering && isActiveAndEnabled)
        {
            if (raycastHit.transform.name.StartsWith("Pickup_"))
            {
                if (click == null)
                    click = Object.Instantiate(clickCursor);
            }
            else
            {
                if (click != null)
                    Object.Destroy(click);
            }
        }
    }

    private void CollectItem(int id, Vector3 Pos)
    {
        gc.CollectItem(id, Pos);
    }

    private void OnDisable()
    {
        if (click != null)
            Object.Destroy(click);
    }
}