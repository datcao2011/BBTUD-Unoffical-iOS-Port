// SweepScript
using UnityEngine;
using UnityEngine.AI;

public class SweepScript : NPC
{
    public Transform wanderTarget;

    public AILocationSelectorScript wanderer;

    public float coolDown;

    public float waitTime;

    public int wanders;

    public bool active;

    private Vector3 origin;

    public AudioClip aud_Sweep;

    public AudioClip aud_Intro;

    public AudioClip aud_Party;

    private NavMeshAgent agent;

    private AudioSource audioDevice;

    public RandomEvent eventSt;

    private int wanderLocation;

    private AudioQueueScript audMan;

    public bool onParty;

    private void Start()
    {
        agent = base.GetComponent<NavMeshAgent>();
        audMan = GetComponent<AudioQueueScript>();
        audioDevice = base.GetComponent<AudioSource>();
        origin = base.transform.position;
        waitTime = Random.Range(120f, 180f);
        MP_PreviousPosition = origin;
    }

    protected override void Update()
    {
        base.Update();
        if (!eventSt.party)
        {
            if (coolDown > 0f)
            {
                coolDown -= 1f * Time.deltaTime;
            }
            if (waitTime > 0f)
            {
                if (onCloset)
                    waitTime -= Time.deltaTime;
            }
            else if (!active)
            {
                active = true;
                wanders = 0;
                Wander();
                wanderLocation = Mathf.RoundToInt(Random.Range(0f, 1f));
                audMan.Queue(aud_Intro);
                if (wanderLocation == 0)
                    audMan.Queue(aud_Sweep);
            }
        }
        else
        {
            if (!onParty)
            {
                onParty = true;
                audMan.Queue(aud_Party);
            }
        }
        if (onParty & !eventSt.party)
        {
            onParty = false;
            GoHome();
        }
        if (!active & !onParty)
        {
            ClearTargetsAndSetTarget(origin);
        }
    }

    private Vector3 MP_PreviousPosition;
    private Vector3 MP_Velocity;

    public Vector3 GetVelocity()
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer)
        {
            return MP_Velocity;
        }
        return agent.velocity;
    }

    protected override void OnPartyHaveToAttend()
    {
        AttendParty();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (((double)agent.velocity.magnitude <= 0.1 || (agent.velocity.magnitude <= 5f && Vector3.Distance(transform.position, Destination) <= 2f)) & coolDown <= 0f & wanders < 5 & active & !eventSt.party & !wantsElevate & !usingElevate)
        {
            Wander();
        }
        else if (wanders >= 5 & !eventSt.party)
        {
            GoHome();
        }
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer)
        {
            MP_Velocity = (MP_CurrentPos - MP_PreviousPosition) * 50f;
            MP_PreviousPosition = MP_CurrentPos;
        }
    }

    private void Wander()
    {
        if (Mathf.RoundToInt(wanderLocation) == 0)
        {
            WanderAround(WanderType.Rooms);
            if (wanders > 0 & wanders < 5)
                audMan.Queue(aud_Sweep);
        }
        else
            WanderAround(WanderType.Halls);
        coolDown = 1f;
        wanders++;
    }

    private void GoHome()
    {
        ClearTargetsAndSetTarget(origin);
        waitTime = Random.Range(120f, 180f);
        wanders = 0;
        active = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "SweepClosetTrigger")
        {
            onCloset = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "SweepClosetTrigger")
        {
            onCloset = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!(other.tag == "NPC") && !(other.tag == "Player" && !other.isTrigger))
        {
            return;
        }
        if (OnPartyHavingFun) return; // We're sweeping the party instead.
        if (other.tag == "NPC")
        {
            other.GetComponent<BsodaEffectScript>().stuckTimes++;
        }
        GottaSweep();
    }

    public void GottaSweep()
    {
        //float supriseChances = Random.Range(1f, 99f);
        if (!onCloset || Vector3.Distance(GameManager.playerTransform.position, transform.position) <= 40f)
        {
            audioDevice.PlayOneShotWC(aud_Sweep);
        }
        else
        {
            audioDevice.PlayOneShot(aud_Sweep);
        }
    }

    private bool onCloset;
}