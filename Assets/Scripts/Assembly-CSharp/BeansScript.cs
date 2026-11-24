using SchoolSaves;
using UnityEngine;
using UnityEngine.AI;

public class BeansScript : NPC
{
    private NavMeshAgent agent;

    public AILocationSelectorScript wanderer;

    public Transform wanderTarget;

    public Transform player;

    public PlayerScript ps;

    private bool spitting;

    private float coolDown;

    private AudioSource audioDevice;

    private float goSpitting;

    [SerializeField]
    private AudioQueueScript audioDevice2;

    [SerializeField]
    private AudioClip[] aud_aboutToSpit = new AudioClip[5];

    [SerializeField]
    private AudioClip[] aud_idle = new AudioClip[5];

    [SerializeField]
    private AudioClip[] aud_watchOut = new AudioClip[5];

    [SerializeField]
    private AudioClip[] aud_sorry = new AudioClip[5];

    [SerializeField]
    private AudioClip[] aud_sorryNPC = new AudioClip[5];

    [SerializeField]
    private AudioClip aud_spit;

    [SerializeField]
    private AudioClip aud_blowing;

    public GameObject gumSpit;

    private GameObject gum;

    public Animator anim;

    public Transform gumPosition;

    private bool gonnaSpit;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioDevice = GetComponent<AudioSource>();
        Wander();
    }

    protected override void Update()
    {
        base.Update();
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer) return;
        if (!gonnaSpit)
        {
            if (gum == null & spitting)
            {
                coolDown = 30f;
                spitting = false;
                agent.isStopped = false;
                Speed = 35f;
                Wander();
            }
        }
        if (coolDown > 0f)
        {
            coolDown -= Time.deltaTime;
        }
        if (goSpitting > 0f)
        {
            goSpitting -= Time.deltaTime;
        }
        else if (gonnaSpit)
        {
            Spit();
        }
    }

    private bool _IsLookingAtPlayer;

    private Vector3 MP_PreviousPosition;
    private Vector3 MP_Velocity;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (MultiplayerManager.isActive)
        {
            if (!MultiplayerManager.instance.isMainPlayer)
            {
                MP_Velocity = (MP_CurrentPos - MP_PreviousPosition) * 35f;
                if (MP_Velocity.magnitude < 0.2f)
                {
                    anim.SetBool("walking", false);
                }
                else
                {
                    anim.SetBool("walking", true);
                }
                MP_PreviousPosition = MP_CurrentPos;
                return;
            }
        }
        if (!spitting)
        {
            if (agent.velocity.magnitude < 0.1f & !wantsElevate & !usingElevate)
            {
                Wander();
            }
            if (agent.velocity.magnitude < 0.2f & !wantsElevate & !usingElevate)
            {
                anim.SetBool("walking", false);
            }
            else
            {
                anim.SetBool("walking", true);
            }
        }
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        _IsLookingAtPlayer = IsInSameFloor() && coolDown <= 0f && !GameManager.player.isInvisible && GameManager.player.NoFacultyAlert <= 0f && Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player";
        if (_IsLookingAtPlayer & !spitting)
        {
            GoSpit();
        }
    }

    private void LateUpdate()
    {
        if (_IsLookingAtPlayer) transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    private void Wander()
    {
        WanderAround(WanderType.Halls);
        if (Random.Range(0f, 199f) >= 180f && !audioDevice.isPlaying)
        {
            int num = Mathf.RoundToInt(Random.Range(0f, 4f));
            audioDevice.PlayOneShotWC(aud_idle[num]);
        }
    }

    private void GoSpit()
    {
        spitting = true;
        agent.isStopped = true;
        Speed = 0f;
        anim.SetBool("walking", false);
        int num = Mathf.RoundToInt(Random.Range(0f, aud_aboutToSpit.Length - 1f));
        audioDevice2.ClearQueue();
        audioDevice.Stop();
        audioDevice.PlayOneShotWC(aud_aboutToSpit[num]);
        audioDevice2.Queue(aud_blowing);
        anim.SetTrigger("gum");
        goSpitting = 3.5f;
        gonnaSpit = true;
    }

    private void Spit()
    {
        gum = Object.Instantiate(gumSpit, gumPosition.position, transform.rotation);
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            float[] Position = gum.transform.position.ToFloat();
            float[] Rotation = gum.transform.rotation.eulerAngles.ToFloat();
            string Pos = Position[0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Position[1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Position[2].ToString(System.Globalization.CultureInfo.InvariantCulture);
            string Rot = Rotation[0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Rotation[1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Rotation[2].ToString(System.Globalization.CultureInfo.InvariantCulture);
            Multiplayer.ClientSend.SendActionToOtherPlayer("beans_spawngum:" + Pos + ":" + Rot);
        }
        audioDevice.PlayOneShotWC(aud_spit);
        int num = Mathf.RoundToInt(Random.Range(0f, 4f));
        audioDevice2.Queue(aud_watchOut[num]);
        anim.SetTrigger("spit");
        gonnaSpit = false;
    }

    public void SetDestination(Vector3 position)
    {
        SetTarget(position);
    }

    public void Teleport(Vector3 position)
    {
        agent.Warp(position);
    }

    public void SorryForPlayer()
    {
        int num = Mathf.RoundToInt(Random.Range(0f, 4f));
        audioDevice2.Queue(aud_sorry[num]);
    }

    public void SorryForNPC()
    {
        int num = Mathf.RoundToInt(Random.Range(0f, 4f));
        audioDevice2.Queue(aud_sorryNPC[num]);
    }
}