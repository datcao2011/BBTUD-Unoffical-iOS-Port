using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Guy : NPC
{
    private NavMeshAgent agent;

    public Transform wanderTarget;

    public Transform gameOverTransform;

    public RandomEvent events;

    public Transform player;

    private Vector3 startingPosition;

    public AudioSource audioDevice;

    public AudioClip shootSound;

    public AILocationSelectorScript wanderer;

    public FlashlightScript flashlight;

    private LockDownDoor[] lockDownDoors;
    private bool active;

    private void Start()
    {
        lockDownDoors = FindObjectsOfType<LockDownDoor>();
        agent = GetComponent<NavMeshAgent>();
        startingPosition = base.transform.position;
        Wander();
    }

    public void Summon()
    {
        active = true;
        gameObject.SetActive(true);
        if (player.position.y >= 40f)
        {
            position = GameObject.Find("ElevatorCollider (1) (up)").transform.position;
            player.GetComponent<PlayerScript>().GravityMultiplier = 5f;
        }
        else if (player.position.y >= 20f)
        {
            position = GameObject.Find("ElevatorCollider (1)").transform.position;
            player.GetComponent<PlayerScript>().GravityMultiplier = 5f;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!events.codeRed && gameObject.activeSelf && active)
        {
            active = false;
            GameManager.cheatingConsole.SelectTMPText(GameManager.enableTexts[4]);
            GameManager.cheatingConsole.SetCharacterState(4, false);
            base.transform.position = startingPosition;
            player.GetComponent<PlayerScript>().GravityMultiplier = 1f;
        }
        if (player.GetComponent<PlayerScript>().gameOver == true)
        {
            audioDevice.Stop();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        if (Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) & (raycastHit.transform.tag == "Player") & IsInSameFloor() & SeesPlayer & !GameManager.player.isInvisible)
        {
            this.TargetPlayer();
        }
        if (!shot && agent.velocity.magnitude < 1f)
        {
            Wander();
        }
    }

    private void LateUpdate()
    {
        Vector3 form = base.transform.position + Camera.main.transform.rotation * -Vector3.forward;
        base.transform.LookAt(form, Camera.main.transform.rotation * Vector3.up);
        base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
    }

    private void Wander()
    {
        if (IsNearToLockDownDoor()) return;
        base.Speed = 10f;
        wanderer.GetNewTarget();
        agent.SetDestination(wanderTarget.position);
    }

    public void SetTo(Vector3 position)
    {
        base.Speed = 15f;
        base.SetTarget(position);
    }

    private bool IsNearToLockDownDoor()
    {
        if (transform.position.y >= 20f) return false;
        foreach (LockDownDoor door in lockDownDoors)
        {
            if (((base.transform.position - door.transform.position).magnitude <= 50f && !door.broken))
            {
                return true;
            }
        }
        return false;
    }

    private void TargetPlayer()
    {
        base.Speed = 20f;
        base.SetTarget(player.position);
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        if (Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) & raycastHit.transform.tag == "Player" & (base.transform.position - player.position).magnitude <= 120f & IsInSameFloor() & !GameManager.player.isInvisible)
        {
            if (!shot) StartCoroutine(Shoot());
        }
    }

    public bool SeesPlayer
    {
        get
        {
            if (flashlight.on)
            {
                return true;
            }
            return (base.transform.position - player.position).magnitude <= 25f;
        }
    }

    private bool shot;

    private IEnumerator Shoot()
    {
        audioDevice.PlayOneShot(shootSound, 1f);
        shot = true;
        StartCoroutine(reload());
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        yield return new WaitForSeconds((float)Mathf.RoundToInt((base.transform.position - player.position).magnitude) / 100f);
        if (Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) & raycastHit.transform.tag == "Player" & !GameManager.player.isInvisible)
        {
            player.GetComponent<PlayerScript>().TakeDamage(Random.Range(50f, 90f));
            if (player.GetComponent<PlayerScript>().health <= 0f)
            {
                player.GetComponent<PlayerScript>().cameraScript.gameOverCharacterTarget = gameOverTransform;
            }
        }
        yield break;
    }

    private IEnumerator reload()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        agent.isStopped = false;
        yield return new WaitForSeconds(1f);
        shot = false;
        yield break;
    }

}
