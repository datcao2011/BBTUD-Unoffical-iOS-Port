using UnityEngine;
using System.Collections;

public class DangerousTeleporter : MonoBehaviour
{
    public AILocationSelectorScript teleporter;

    public GameControllerScript gc;

    public Transform teleportTarget;

    public Transform player;

    public AudioSource audioDevice;

    public AudioClip teleportSound;

    public ElevatorScript Elevator;

    public GameOptimizer Optimizer;

    public void UseTeleporter()
    {
        StartCoroutine(Teleporter());
    }

    private void Teleport()
    {
        teleporter.GetNewTarget();
        if (gc.HASManager && gc.HASManager.hiderOrSeeker == "Hider" && teleportTarget.position.y >= 20f)
        {
#if DEBUG
            Debug.LogError("NOPE! YOU'RE NOT TELEPORTING UP THERE!");
#endif
            return;
        }
        if (Elevator != null && Elevator.unavailable && teleportTarget.position.y >= 20f)
        {
#if DEBUG
            Debug.LogError("NOPE! YOU'RE NOT TELEPORTING UP THERE!");
#endif
            teleportTarget.position = teleporter.rooms[1].position;
        }
        Vector3 pos = new Vector3(teleportTarget.position.x, teleportTarget.position.y + 4f, teleportTarget.position.z);
        if (Optimizer != null && Optimizer.isActiveAndEnabled) // Update the floor activation since it'll glitch
        {
            Optimizer.ForceUpdate(teleportTarget.position.y); 
        }
        gc.player.transform.position = pos;
        gc.player.SetPosition(pos);
        audioDevice.PlayOneShot(teleportSound);
    }

    private IEnumerator Teleporter()
    {
        int teleports = UnityEngine.Random.Range(12, 16);
        int teleportCount = 0;
        float baseTime = 0.2f;
        float currentTime = baseTime;
        float increaseFactor = 1.1f;
        gc.SetNoclip(true);
        while (teleportCount < teleports)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0f)
            {
                this.Teleport();
                int num = teleportCount;
                teleportCount = num + 1;
                baseTime *= increaseFactor;
                currentTime = baseTime;
            }
            yield return null;
        }
        gc.SetNoclip(false);
        yield break;
    }
}