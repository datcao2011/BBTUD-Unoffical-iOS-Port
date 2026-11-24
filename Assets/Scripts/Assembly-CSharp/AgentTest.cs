// AgentTest
using UnityEngine;
using UnityEngine.AI;

public class AgentTest : NPC
{
    public bool db;

    public Transform player;

    public Transform wanderTarget;

    public AILocationSelectorScript wanderer;

    public float coolDown;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = base.GetComponent<NavMeshAgent>();
        Wander();
        
    }

    protected override void Update()
    {
        if (coolDown > 0f)
        {
            coolDown -= 1f * Time.deltaTime;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Vector3 direction = player.position - base.transform.position;
        RaycastHit raycastHit;
        if (Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) & raycastHit.transform.tag == "Player" & !GameManager.player.isInvisible)
        {
            db = true;
            TargetPlayer();
        }
        else
        {
            db = false;
            if (agent.velocity.magnitude <= 1f & coolDown <= 0f & !wantsElevate & !usingElevate)
            {
                Wander();
            }
        }
    }

    private void Wander()
    {
        WanderAround(WanderType.All);
        coolDown = 1f;
    }

    private void TargetPlayer()
    {
        SetTarget(player.position);
        coolDown = 1f;
    }
}