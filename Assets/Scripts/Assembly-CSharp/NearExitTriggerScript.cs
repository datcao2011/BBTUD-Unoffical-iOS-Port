// NearExitTriggerScript
using UnityEngine;
using UnityEngine.AI;

public class NearExitTriggerScript : MonoBehaviour
{
    public GameControllerScript gc;

    public EntranceScript es;

    private void OnTriggerEnter(Collider other)
    {
        if (gc.exitsReached < 3 & gc.finaleMode & other.tag == "Player")
        {
            if (!MultiplayerManager.isActive || !MultiplayerManager.instance.isBaldi)
            {
                CurrentlyNear();
            }
        }
        if (other.name == "NULL" && !es.exitTaken && NullManager.Instance.NULL.NullRunFrom == 2)
        {
            // Close Exit
            if (es.gameObject.name == "Entrance (0)")
            {
                es.Lower();
            }
            else
            {
                Transform T = ((es.gameObject.name == "Entrance (2)") ? transform.parent : transform.parent.GetChild(0)).Find("OutOf");
                foreach (Transform child in T)
                {
                    child.GetComponent<MeshRenderer>().material = gc.WhiteWall;
                }
                es.Lower();
            }
            foreach (NavMeshObstacle obstacle in FindObjectsOfType<NavMeshObstacle>())
            {
                obstacle.enabled = false;
            }
            // NULL
            gc.audioDevice.PlayOneShot(gc.aud_Switch, 0.8f);
            NullManager.Instance.NULL.ReachedExit();
            NullManager.Instance.ProjectileSpawnr.SpawnProjectile(transform.parent.position + (transform.parent.right * 40f));
            NullManager.Instance.ProjectileSpawnr.SpawnProjectile(transform.parent.position + (transform.parent.right * -40f));
        }
    }

    public void CurrentlyNear(bool SendEvent = true)
    {
        if (!es.exitTaken)
        {
            if (BirthdayBash.Instance != null && es.name == "Entrance (2)")
            {
                return;
            }
            if (MultiplayerManager.isActive && SendEvent)
            {
                if (Photon.Pun.PhotonNetwork.IsMasterClient && Photon.Pun.PhotonNetwork.CurrentRoom.IsVisible)
                {
                    Photon.Pun.PhotonNetwork.CurrentRoom.IsVisible = false;
                }
                Multiplayer.ClientSend.SendActionToOtherPlayer("exit:" + es.gameObject.name);
            }
            gc.ExitReached();
            if (Vector3.Distance(gc.baldiTransform.position, gc.playerTransform.position) <= 200f && SendEvent) { gc.baldiScrpt.Hear(transform.position, 5f); }
            if (es.gameObject.name == "Entrance (0)")
            {
                es.Lower();
            }
            else
            {
                es.CloseElevate();
            }
        }
    }
}