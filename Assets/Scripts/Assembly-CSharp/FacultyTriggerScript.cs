// FacultyTriggerScript
using UnityEngine;

public class FacultyTriggerScript : MonoBehaviour
{
    public PlayerScript ps;

    [SerializeField]
    private BoxCollider hitBox;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ps.ResetGuilt("faculty", 1f);
        }
    }
}