using UnityEngine;

public class CrashTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        CrashGame.Crash();
    }
}