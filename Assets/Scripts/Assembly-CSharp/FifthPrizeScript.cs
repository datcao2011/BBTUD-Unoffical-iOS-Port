using UnityEngine;
using UnityEngine.AI;

public class FifthPrizeScript : NPC
{
    public float debug;

    public bool WanderAnotherTarget;

    public float turnSpeed;

    public float str;

    public float angleDiff;

    public float normSpeed;

    public float runSpeed;

    public float currentSpeed;

    public float acceleration;

    public float speed;

    public float autoBrakeCool;

    public float crazyTime;

    public Quaternion targetRotation;

    public float coolDown;

    public bool playerSeen;

    public bool hugAnnounced;

    public AILocationSelectorScript wanderer;

    public Transform player;

    public Transform wanderTarget;

    public AudioClip[] aud_Found = new AudioClip[2];

    public AudioClip[] aud_Lost = new AudioClip[2];

    public AudioClip[] aud_Hug = new AudioClip[2];

    public AudioClip[] aud_Random = new AudioClip[2];

    public AudioSource audioDevice;

    public AudioSource motorAudio;

    private NavMeshAgent agent;

    public RandomEvent eventScript;

    private AudioQueueScript audMan;

    private bool stabbed;

    public float stabTime;

    public Transform playerTransformPosition;

    private void Start()
    {
        agent = base.GetComponent<NavMeshAgent>();
        audMan = base.GetComponent<AudioQueueScript>();
        coolDown = 1f;
        Wander();
    }

    protected override void Update()
    {
        base.Update();
        if (WanderAnotherTarget)
        {
            WanderAnotherTarget = false;
            Wander();
        }
        if (coolDown > 0f)
        {
            coolDown -= 1f * Time.deltaTime;
        }
        if (autoBrakeCool > 0f)
        {
            autoBrakeCool -= 1f * Time.deltaTime;
        }
        else
        {
            agent.autoBraking = true;
        }
        Vector3 eulerAngles = base.transform.eulerAngles;
        float y = eulerAngles.y;
        Vector3 steeringTarget = agent.steeringTarget;
        float x = steeringTarget.x;
        Vector3 position = base.transform.position;
        float y2 = x - position.x;
        Vector3 steeringTarget2 = agent.steeringTarget;
        float z = steeringTarget2.z;
        Vector3 position2 = base.transform.position;
        angleDiff = Mathf.DeltaAngle(y, Mathf.Atan2(y2, z - position2.z) * 57.29578f);
        if (crazyTime <= 0f & stabTime <= 0f)
        {
            stabbed = false;
            if (Mathf.Abs(angleDiff) < 5f)
            {
                Transform transform = base.transform;
                Vector3 steeringTarget3 = agent.steeringTarget;
                float x2 = steeringTarget3.x;
                Vector3 position3 = base.transform.position;
                float y3 = position3.y;
                Vector3 steeringTarget4 = agent.steeringTarget;
                transform.LookAt(new Vector3(x2, y3, steeringTarget4.z));
                base.Speed = currentSpeed;
            }
            else
            {
                base.transform.Rotate(new Vector3(0f, turnSpeed * Mathf.Sign(angleDiff) * Time.deltaTime, 0f));
                base.Speed = 0f;
            }
        }
        else if (crazyTime > 0f)
        {
            base.Speed = 0f;
            base.transform.Rotate(new Vector3(0f, 180f * Time.deltaTime, 0f));
            crazyTime -= Time.deltaTime;
        }
        else if (stabTime > 0f)
        {
            base.Speed = 0f;
            stabTime -= Time.deltaTime;
        }
        motorAudio.pitch = (agent.velocity.magnitude + 1f) * Time.timeScale;
    }

    protected override void OnPartyHaveToAttend()
    {
        AttendParty();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (eventScript.party)
        {
            playerSeen = false;
            if (eventScript.eventEndTimer > 20f && !OnPartyHavingFun)
            {
                currentSpeed = normSpeed;
            }
            else
            {
                currentSpeed = runSpeed;
            }
            return;
        }
        Vector3 direction = player.position - base.transform.position;
        RaycastHit raycastHit;
        if (direction.magnitude <= 80f && IsInSameFloor() && !GameManager.player.isInvisible && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
        {
            if (!playerSeen && !stabbed)
            {
                int num = Mathf.RoundToInt(Random.Range(0f, 1f));
                audMan.Queue(aud_Found[num]);
            }
            playerSeen = true;
            TargetPlayer();
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = normSpeed;
            if (playerSeen & coolDown <= 0f)
            {
                if (!stabbed)
                {
                    int num2 = Mathf.RoundToInt(Random.Range(0f, 1f));
                    audMan.Queue(aud_Lost[num2]);
                }
                playerSeen = false;
                Wander();
            }
            else if (agent.velocity.magnitude <= 1f & coolDown <= 0f & (base.transform.position - agent.destination).magnitude < 5f & !wantsElevate & !usingElevate)
            {
                Wander();
            }
        }
    }

    private void Wander()
    {
        WanderAround(WanderType.Halls);
        hugAnnounced = false;
        int num = Mathf.RoundToInt(Random.Range(0f, 9f));
        if (num == 0 & coolDown <= 0f & !stabbed)
        {
            int num2 = Mathf.RoundToInt(Random.Range(0f, 1f));
            audMan.Queue(aud_Random[num2]);
        }
        coolDown = 1f;
    }

    private void TargetPlayer()
    {
        SetTarget(player.position);
        coolDown = 0.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" & !other.isTrigger)
        {
            if (!hugAnnounced & !stabbed)
            {
                int num = Mathf.RoundToInt(Random.Range(0f, 1f));
                audMan.Queue(aud_Hug[num]);
                hugAnnounced = true;
            }
            agent.autoBraking = false;
        }
        if (other.name == "Window Collider" && playerSeen && agent.velocity.magnitude >= 15f && other.GetComponent<WindowScript>()) // Will only break windows when the player's seen instead.
        {
            other.GetComponent<WindowScript>().Break();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" & !other.isTrigger)
        {
            autoBrakeCool = 1f;
        }
    }

    public void GoCrazy()
    {
        crazyTime = 30f;
    }

    public void Stabbed()
    {
        stabTime = 30f;
        stabbed = true;
        audMan.Queue(aud_Lost[0]);
    }
}