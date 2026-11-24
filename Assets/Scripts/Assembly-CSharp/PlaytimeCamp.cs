using UnityEngine;
using UnityEngine.AI;

public class PlaytimeCamp : NPC
{
    public bool db;

    public bool playerSeen;

    public bool disappointed;

    public int audVal;

    public Animator animator;

    public Transform player;

    public PlayerScript ps;

    public float coolDown;

    public float playCool;

    public bool playerSpotted;

    public bool jumpRopeStarted;

    private NavMeshAgent agent;

    public AudioClip[] aud_Random = new AudioClip[2];

    public AudioClip aud_Sad;

    public AudioSource audioDevice;

    private bool stop1;

    private int sticks;

    public GameObject stickSprite;

    private GameObject click;

    public GameObject clickPrefab;

    private void Start()
    {
        agent = base.GetComponent<NavMeshAgent>();
        audioDevice = base.GetComponent<AudioSource>();
        playCool = 2f;
        Wander();
        player = FindObjectOfType<PlayerScript>().transform;
    }

    //private bool onBlow;

    protected override void Update()
    {
        base.Update();
        // if (FindObjectOfType<CloudyCopterCampingScript>().blowing)
        // {
        //     onBlow = true;
        //     SetTarget(FindObjectOfType<CloudyCopterCampingScript>().transform.position);
        // }
        // else if (onBlow)
        // {
        //     onBlow = false;
        //     Wander();
        // }
        stickSprite.SetActive(sticks > 0);
        if (coolDown > 0f)
        {
            coolDown -= 1f * Time.deltaTime;
        }
        if (playCool >= 0f & !collectingSticks)
        {
            playCool -= Time.deltaTime;
        }
        Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit raycastHit;
        bool IsHovering = sticks > 0 & Vector3.Distance(player.position, base.transform.position) < 20f & Physics.Raycast(ray, out raycastHit) && raycastHit.transform == transform;
        if (InteractScript.Intracting & Time.timeScale != 0f)
        {
            if (IsHovering)
            {
                audioDevice.PlayOneShotWC(aud_Random[1]);
                FindObjectOfType<CampingScript>().AddSticks(sticks);
                sticks = 0;
            }
        }
        if (IsHovering)
        {
            if (click == null)
            {
                click = Instantiate(clickPrefab);
            }
        }
        else
        {
            if (click != null)
            {
                Destroy(click);
            }
        }
        foreach (WoodPickupScript wood in FindObjectsOfType<WoodPickupScript>())
        {
            if (Vector3.Distance(transform.position, wood.transform.position) < 4.5f & collectingSticks)
            {
                playCool = 2f;
                collectingSticks = false;
                wood.gameObject.SetActive(false);
                sticks++;
            }
        }
        // if (FindObjectOfType<CampingScript>().sticks <= 0 && FindObjectOfType<FireScript>().timer <= 30f & sticks > 0)
        // {
        //     agent.SetDestination(player.position - -player.forward * 5f);
        // }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!ps.jumpRope)
        {
            if (agent.velocity.magnitude <= 1f & coolDown <= 0f & !wantsElevate & !usingElevate)
            {
                Wander();
            }
            jumpRopeStarted = false;
        }
        else
        {
            if (!jumpRopeStarted)
            {
                agent.Warp(base.transform.position - base.transform.forward * 10f);
            }
            jumpRopeStarted = true;
            base.Speed = 0f;
            playCool = 15f;
            stop1 = true;
        }
    }

    private bool collectingSticks;

    private void Wander()
    {
        if (FindObjectOfType<CloudyCopterCampingScript>().blowing)
        {
            return;
        }
        if (playCool <= 0f || collectingSticks)
        {
            collectingSticks = true;
            WoodPickupScript[] firewoods = FindObjectsOfType<WoodPickupScript>();
            if (firewoods.Length <= 0)
            {
                playCool = 30f;
                collectingSticks = false;
                return;
            }
            SetTarget(firewoods[Random.Range(0, firewoods.Length)].transform.position);
        }
        else
        {
            SetTarget(transform.position - transform.forward * Random.Range(-90f, 90f));
        }
        base.Speed = 15f;
        if (!stop1)
        {
            audVal = Mathf.RoundToInt(Random.Range(0f, 1f));
            if (!audioDevice.isPlaying && Random.Range(0f, 1f) > 0.74f)
            {
                audioDevice.PlayOneShotWC(aud_Random[audVal]);
            }
            coolDown = 1f;
        }
        stop1 = false;
    }
}