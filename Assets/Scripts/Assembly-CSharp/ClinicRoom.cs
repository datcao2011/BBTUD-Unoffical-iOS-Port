using UnityEngine;

public class ClinicRoom : MonoBehaviour
{
    public bool IsPlayerInside;
    public DrReflex DoctorReflex;
    public PlayerScript player;
    private int Times;
    public DoorScript Door;
    public FacultyTriggerScript FacultyTrigger;

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Player" && !other.isTrigger) IsPlayerInside = true;
        if (other.name == "Doctor Reflex" && DoctorReflex.HasHammer && !MultiplayerManager.isActive) // Won't get angry if they are already flat
        {
            if (IsPlayerInside && !player.IsFlat) // Is player inside trying to steal the hammer back?
            {
                if (player.NoFacultyAlert > 0f) return; // Also, ignore if the player has a faculty nametag. Tricks him.
                DoctorReflex.GetInstantAngry();
                if (Times < 5) Times++;
                if (Times == 1) // First warning, faculty gets involved. Principal will catch the player next time if they're in the room again (Treats as a faculty room)
                {
                    FacultyTrigger.gameObject.SetActive(true);
                }
                if (Times >= 2) // Now, Dr. Reflex will involve and call the Principal to get them off his Room.
                {
                    // Basically, Dr. Reflex uses his Principal Whistle.
                    DoctorReflex.GetComponent<AudioSource>().PlayOneShot(player.gc.aud_Whistle);
                    if (player.gc.principalNPC.isActiveAndEnabled) player.gc.principalNPC.Hear(player.transform.position, 12f, true);
                }
                if (Times >= 3) // Then, Dr. Reflex will lock his door for 5 minutes (10 minutes if 4th time and 15 minutes if all other times which is the max).
                {
                    Door.LockDoor(300f * (Times - 2));
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Doctor Reflex" && !MultiplayerManager.isActive && DoctorReflex.HasHammer && !DoctorReflex.Angry && !IsPlayerInside && !DoctorReflex.HasSlowRotation) // If Dr. Reflex returning the Hammer from the player.
        {
            if (Times < 3) Times++;
            if (Times == 1) // First warning, faculty gets involved. Principal will catch the player next time if they're in the room again (Treats as a faculty room)
            {
                FacultyTrigger.gameObject.SetActive(true);
            }
            if (Times >= 2) // Now, Dr. Reflex will lock his door for 10 minutes (15 minutes if 3th time which is the max).
            {
                Door.LockDoor(300f * Times);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player") IsPlayerInside = false;
    }
}