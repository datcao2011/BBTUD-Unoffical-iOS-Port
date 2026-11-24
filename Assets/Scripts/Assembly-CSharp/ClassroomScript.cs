using UnityEngine;

public class ClassroomScript : MonoBehaviour
{
    private DoorScript door;

    public Pomp mrsPomp;

    private bool closed;

    private bool onAngry;

    public float angryTimer;

    private void Start()
    {
        door = GetComponent<DoorScript>();
        mrsPomp = GameObject.Find("Mrs Pomp").GetComponent<Pomp>();
    }

    private void Update()
    {
        if (mrsPomp.angry && !mrsPomp.screaming)
        {
            angryTimer += Time.deltaTime;
            if (mrsPomp.playerInClassroom & angryTimer >= 60f)
            {
                mrsPomp.playerInClassroom = false;
                onAngry = false;
                angryTimer = 0f;
                float CurrentPos = 1;
                if (mrsPomp.player.position.y >= 20f) CurrentPos = 1.1f;
                else if (mrsPomp.player.position.y >= 40f) CurrentPos = 1.75f;
                mrsPomp.player.GetComponent<PlayerScript>().SetPosition(new Vector3(-132.9f, 4f, 274.4f));
                mrsPomp.player.GetComponent<PlayerScript>().TakeDamage(Random.Range(50f, 100f) * CurrentPos);
                if (mrsPomp.player.GetComponent<PlayerScript>().health <= 0f)
                {
                    mrsPomp.player.GetComponent<PlayerScript>().cameraScript.gameOverCharacterTarget = mrsPomp.transform;
                }
                mrsPomp.Teleport(new Vector3(-132.9f, 1.64f, 274.4f));
                mrsPomp.stop = false;
                mrsPomp.screaming = true;
                mrsPomp.Scream();
                door.LockDoor(19.273f);
            }
            if (!onAngry)
            {
                onAngry = true;
                closed = true;
            }
            if (door.bDoorOpen & !mrsPomp.screaming)
            {
                mrsPomp.stop = false;
                closed = false;
                if (!mrsPomp.playerInClassroom & !mrsPomp.screaming)
                {
                    mrsPomp.screaming = true;
                    onAngry = false;
                    angryTimer = 0f;
                    mrsPomp.player.GetComponent<PlayerScript>().TakeDamage(Random.Range(20f, 50f));
                    if (mrsPomp.player.GetComponent<PlayerScript>().health <= 0f)
                    {
                        mrsPomp.player.GetComponent<PlayerScript>().cameraScript.gameOverCharacterTarget = mrsPomp.transform;
                    }
                    mrsPomp.Scream();
                }
            }
            else if (mrsPomp.playerInClassroom & !closed)
            {
                closed = true;
                mrsPomp.playerInClassroom = false;
                if (!mrsPomp.ps.onClassroom)
                {
                    mrsPomp.player.position = new Vector3(-132.9f, 4f, 274.4f);
                }
                mrsPomp.stop = true;
                mrsPomp.Teleport(new Vector3(-106f, 1.64f, 274.4f));
            }
        }
    }
}