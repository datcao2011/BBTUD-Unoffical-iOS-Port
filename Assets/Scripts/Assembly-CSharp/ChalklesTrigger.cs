using UnityEngine;

public class ChalklesTrigger : MonoBehaviour
{
    private Chalkles chalkles;

    public Vector3 chalklesPosition;

    public Vector3 chalklesRotation;

    public DoorScript door;

    private void Start()
    {
        chalkles = FindObjectOfType<Chalkles>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Player" && chalkles != null && chalkles.isActiveAndEnabled)
        {
            if (other.GetComponent<PlayerScript>().isInvisible || other.GetComponent<PlayerScript>().NoFacultyAlert > 0f)
            {
                return;
            }
            chalkles.OnClassroomEntered(chalklesPosition, Quaternion.Euler(chalklesRotation), door);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "Player" && chalkles != null && chalkles.isActiveAndEnabled)
        {
            chalkles.OnClassroomExit();
        }
    }
}