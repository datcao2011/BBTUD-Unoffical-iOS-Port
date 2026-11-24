// BsodaEffectScript
using UnityEngine;
using UnityEngine.AI;

public class BsodaEffectScript : MonoBehaviour
{
    private NavMeshAgent agent;

    private Vector3 otherVelocity;

    private Vector3 windVelocity;

    private bool inBsoda;

    private float failSave;

    private float stuckValue;

    private float stuckCooldown;

    private bool freed;

    private bool stucked;

    public int stuckTimes;

    private float freeTime;

    private bool AgentDestined;

    private void Start()
    {
        agent = base.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (SlidingOnBananaPeel && BananaPeel != null && BananaPeel.GetVelocity().magnitude >= 0.1f)
        {
            agent.velocity = BananaPeel.GetVelocity();
        }
        else if (SlidingOnBananaPeel)
        {
            if (BananaPeel != null)
            {
                BananaPeel.Unlink(transform);
                BananaPeel = null;
            }
            SlidingOnBananaPeel = false;
        }
        // BSODA & Gum
        if (inBsoda)
        {
            //Vector3 Velocity = otherVelocity;
            // To be honest, I don't think anyone likes this functionaility.
            /*if (onGum && inBsoda)
            {
                Velocity = otherVelocity * 0.1f;
            }
            else if (onGum)
            {
                Velocity = base.transform.forward * agent.speed * 0.1f;
            }*/
            if (AgentDestined)
            {
                agent.Move(otherVelocity * Time.deltaTime);
            }
            else agent.velocity = otherVelocity;
        }
        else if (onGum)
        {
            agent.velocity = base.transform.forward * agent.speed * 0.1f;
        }
        // Other
        if (failSave > 0f)
        {
            failSave -= Time.deltaTime;
        }
        else
        {
            inBsoda = false;
            AgentDestined = false;
            if (SlidingOnBananaPeel)
            {
                if (BananaPeel != null)
                {
                    BananaPeel.Unlink(transform);
                    BananaPeel = null;
                }
                SlidingOnBananaPeel = false;
            }
        }
        if (stuckValue >= 15f)
        {
            freed = true;
            inBsoda = false;
        }
        if (stuckCooldown < 5f & freed)
        {
            stuckCooldown += Time.deltaTime;
        }
        else if (freed)
        {
            stuckValue = 0f;
            freed = false;
            stuckCooldown = 0f;
        }
        if (stucked)
        {
            stuckValue += Time.deltaTime;
        }
        if (!inBsoda)
        {
            freeTime += Time.deltaTime;
        }
        if (freeTime >= 3f)
        {
            stuckTimes = 0;
        }
        if (stuckTimes > 5)
        {
            freed = true;
            stuckCooldown = 3f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "BSODA")
        {
            inBsoda = true;
            otherVelocity = ((Component)other).GetComponent<Rigidbody>().velocity;
            failSave = 1f;
            if ((name == "Baldi" || name == "NULL") && Vector3.Distance(transform.position, agent.destination) <= 1.6f) // BSODA fix
            {
                AgentDestined = true;
            }
        }
        else if (other.transform.name == "Gotta Sweep" & (other.GetComponent<SweepScript>()))
        {
            NPC MyNPC = GetComponent<NPC>();
            SweepScript GottaSweep = other.GetComponent<SweepScript>();
            if (!GottaSweep.OnPartyHavingFun && stuckValue < 15f && !freed && (MyNPC == null || GottaSweep.FlattenDuration <= 0f || (GottaSweep.FlattenDuration > 0f && MyNPC.FlattenDuration > 0f)))
            {
                freeTime = 0f;
                inBsoda = true;
                otherVelocity = base.transform.forward * agent.speed * 0.1f + GottaSweep.GetVelocity();
                failSave = 1f;
                stucked = true;
            }
        }
        else if (other.name == "Flood" && name != "Baldi")
        {
            inBsoda = true;
            failSave = 1f;
            otherVelocity = base.transform.forward * agent.speed * 0.3f;
        }
        else if (other.name == "A Banana Peel" && other.GetComponent<BANANAPEELAAAA>())
        {
            // if (MultiplayerManager.isActive)
            // {
            //     if (!(name == "Baldi" && ((MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer) || MultiplayerManager.instance.isBaldi)))
            //     {
            //         return;
            //     }
            //     if (name != "Baldi" && !MultiplayerManager.instance.isMainPlayer)
            //     {
            //         return;
            //     }
            //     if (MultiplayerManager.instance.isBaldi)
            //     {

            //     }
            // }
            SlidingOnBananaPeel = true;
            failSave = 1.2f;
            BananaPeel = other.GetComponent<BANANAPEELAAAA>();
            BananaPeel.Link(transform);
            Vector3 Velocity = agent.velocity.normalized;
            if (Velocity.magnitude <= 0 && name == "Baldi") Velocity = GetComponent<BaldiScript>().PreviousVelocity.normalized;
            if (Velocity.magnitude <= 0) Velocity = transform.forward;
            BananaPeel.Trip(Velocity * 28f);
        }
        else if (other.name == "Wind Box" && gameObject.name != "Cloudy Copter")
        {
            if (!other.GetComponent<WindElement>() || !other.GetComponent<WindElement>().isActive)
            {
                return;
            }
            inBsoda = true;
            otherVelocity = base.transform.forward * agent.speed + other.GetComponent<WindElement>().WindVelocity;
            failSave = 1f;
        }
    }
    private bool onGum;

    private bool SlidingOnBananaPeel;
    private BANANAPEELAAAA BananaPeel;

    public void Gummed()
    {
        onGum = true;
        StartCoroutine(GumTimer());
    }

    private System.Collections.IEnumerator GumTimer()
    {
        float timer = 10f;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        onGum = false;
        yield break;
    }

    private void OnTriggerExit()
    {
        inBsoda = false;
        stucked = false;
        AgentDestined = false;
        if (!freed)
        {
            stuckValue = 0f;
        }
    }
}