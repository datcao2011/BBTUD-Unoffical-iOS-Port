using SchoolSaves;
using UnityEngine;
using UnityEngine.AI;

public class PlaytimeScript : NPC
{
    public bool db;

    public bool playerSeen;

    public bool disappointed;

    public int audVal;

    public Animator animator;

    public Transform player;

    public PlayerScript ps;

    public Transform wanderTarget;

    public AILocationSelectorScript wanderer;

    public float coolDown;

    public float playCool;

    public bool playerSpotted;

    public bool jumpRopeStarted;

    private NavMeshAgent agent;

    public AudioClip[] aud_Numbers = new AudioClip[10];

    public AudioClip[] aud_Random = new AudioClip[2];

    public AudioClip aud_Instrcutions;

    public AudioClip aud_Oops;

    public AudioClip aud_LetsPlay;

    public AudioClip aud_Congrats;

    public AudioClip aud_ReadyGo;

    public AudioClip aud_Sad;

    public AudioSource audioDevice;

    private bool stop1;

    private void Start()
    {
        agent = base.GetComponent<NavMeshAgent>();
        audioDevice = base.GetComponent<AudioSource>();
        Wander();
    }

    protected override void Update()
    {
        base.Update();
        if (coolDown > 0f)
        {
            coolDown -= 1f * Time.deltaTime;
        }
        if (playCool >= 0f)
        {
            playCool -= Time.deltaTime;
        }
        else if (animator.GetBool("disappointed"))
        {
            playCool = 0f;
            animator.SetBool("disappointed", false);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (MultiplayerManager.isActive && MultiplayerManager.instance.HasMoreThan2Players)
        {
            if (MultiplayerManager.instance.isMainPlayer) MP_FixedUpdate(); else if (!MultiplayerManager.instance.isBaldi) MP_FixedUpdateOthers();
            return;
        }
        if (!ps.jumpRope)
        {
            Vector3 direction = player.position - base.transform.position;
            RaycastHit raycastHit;
            if ((base.transform.position - player.position).magnitude <= 40f && IsInSameFloor() && playCool <= 0f && !GameManager.player.isInvisible && GameManager.player.NoFacultyAlert <= 0f && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
            {
                playerSeen = true;
                TargetPlayer();
            }
            else if (playerSeen & coolDown <= 0f)
            {
                playerSeen = false;
                Wander();
            }
            else if (agent.velocity.magnitude <= 1f & coolDown <= 0f & !wantsElevate & !usingElevate)
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

    private Photon.Realtime.Player MP_CurrentJumpropingPlayer;
    private bool MP_JumpRopeActive;

    public void MP_StartJumpRope(int ActorPlayer)
    {
        if (Photon.Pun.PhotonNetwork.InRoom && !MP_JumpRopeActive)
        {
            MP_CurrentJumpropingPlayer = Photon.Pun.PhotonNetwork.CurrentRoom.GetPlayer(ActorPlayer);
            if (MP_CurrentJumpropingPlayer != null)
            {
                MP_JumpRopeActive = true;
                if (MP_CurrentJumpropingPlayer.IsLocal && !ps.jumpRope)
                {
                    ps.ActivateJumpRope();
                }
            }
        }
    }

    public void MP_EndJumpRope(bool CutRope)
    {
        if (CutRope)
        {
            animator.SetBool("disappointed", true);
        }
        playCool = 15f;
        MP_JumpRopeActive = false;
        if (MP_CurrentJumpropingPlayer != null)
        {
            MP_CurrentJumpropingPlayer = null;
        }
    }

    private void MP_FixedUpdateOthers()
    {
        if (!MP_JumpRopeActive)
        {
            Vector3 direction = player.position - base.transform.position;
            RaycastHit raycastHit;
            if (Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore))
            {
                if (raycastHit.transform.tag == "Player" & (base.transform.position - player.position).magnitude <= 80f & IsInSameFloor() & playCool <= 0f & !GameManager.player.isInvisible)
                {
                    MultiplayerManager.instance.client.SendAsUDP(5, Photon.Realtime.ReceiverGroup.Others, "Playtime", player.position.ToFloat());
                }
            }
        }
    }

    private void MP_FixedUpdate()
    {
        if (!MP_JumpRopeActive)
        {
            Vector3 direction = player.position - base.transform.position;
            RaycastHit raycastHit;
            if (Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore))
            {
                if (raycastHit.transform.tag == "Player" & (base.transform.position - player.position).magnitude <= 80f & IsInSameFloor() & playCool <= 0f & !GameManager.player.isInvisible)
                {
                    playerSeen = true;
                    TargetPlayer();
                    return;
                }
                // if (raycastHit.transform != null & raycastHit.transform.tag == "PlayerModel" & (base.transform.position - raycastHit.transform.position).magnitude <= 80f & playCool <= 0f)
                // {
                //     playerSeen = true;
                //     TargetPlayer();
                //     SetTarget(raycastHit.transform.position);
                //     return;
                // }
            }
            if (playerSeen & coolDown <= 0f)
            {
                playerSeen = false;
                Wander();
            }
            else if (agent.velocity.magnitude <= 1f & coolDown <= 0f & !wantsElevate & !usingElevate)
            {
                Wander();
            }
            jumpRopeStarted = false;
        }
        else
        {
            if (MP_CurrentJumpropingPlayer == null || MP_CurrentJumpropingPlayer.Get(MP_CurrentJumpropingPlayer.ActorNumber) == null)
            {
                MP_EndJumpRope(false);
            }
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

    private void Wander()
    {
        WanderAround(WanderType.Halls);
        base.Speed = 15f;
        playerSpotted = false;
        if (!stop1)
        {
            audVal = Mathf.RoundToInt(Random.Range(0f, 1f));
            if (!audioDevice.isPlaying)
            {
                PlaySound(aud_Random[audVal]);
            }
            coolDown = 1f;
        }
        stop1 = false;
    }

    public void TargetAnotherPlayer(Vector3 Position)
    {
        ClearTargetsAndSetTarget(Position);
        base.Speed = 20f;
        coolDown = 0.2f;
        if (!playerSpotted)
        {
            playerSpotted = true;
            PlaySound(aud_LetsPlay);
        }
    }

    protected override void OnFlatten()
    {
        if (ps.jumpRope)
        {
            ps.DeactivateJumpRope();
            playCool = 45f;
        }
    }

    private void TargetPlayer()
    {
        animator.SetBool("disappointed", false);
        ClearTargetsAndSetTarget(player.position);
        base.Speed = 20f;
        coolDown = 0.2f;
        if (!playerSpotted)
        {
            playerSpotted = true;
            PlaySound(aud_LetsPlay);
        }
    }

    public void Disappoint()
    {
        animator.SetBool("disappointed", true);
        audioDevice.Stop();
        PlaySound_Other(aud_Sad);
        playCool = 15f;
    }

    public void PlaySound(AudioClip audio)
    {
        if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
        {
            audioDevice.PlayOneShotWC(audio);
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
            {
                Multiplayer.ClientSend.PlaySoundClip(gameObject.name, audio.name);
            }
        }
    }

    public void PlaySound_Other(AudioClip audio)
    {
        audioDevice.PlayOneShotWC(audio);
        if (MultiplayerManager.isActive)
        {
            Multiplayer.ClientSend.PlaySoundClip(gameObject.name, audio.name);
        }
    }
}