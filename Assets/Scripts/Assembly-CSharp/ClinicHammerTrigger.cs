using UnityEngine;

public class ClinicHammerTrigger : MonoBehaviour
{
    public GameObject Hammer;

    public GameControllerScript gc;

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Doctor Reflex")
        {
            DrReflex DoctorReflex = other.GetComponent<DrReflex>();
            if (DoctorReflex.WantsTheHammer)
            {
                if (gc.HasHammer())
                {
                    gc.LoseItem(gc.CurrentHammerSlot);
                }
                if (GameObject.Find("Pickup_HammerReflex"))
                {
                    gc.MagicallyTeleportTheHammerBack();
                }
                Hammer.SetActive(false);
                DoctorReflex.CarryHammer();
            }
            if (DoctorReflex.ReturningHammer)
            {
                Hammer.SetActive(true);
                DoctorReflex.PutBackHammer();
            }
        }
    }

    public void GiveHammerBack()
    {
        Hammer.SetActive(true);
    }
}