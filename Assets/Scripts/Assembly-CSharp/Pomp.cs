using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro;

// Token: 0x02000027 RID: 39
public class Pomp : NPC
{
    public bool stop;

    public TMP_Text timerText;

    public float time;

    [HideInInspector]
    public bool angry;

    public bool playerInClassroom;

    private float timeSeconds;

    private float timeMinutes;

    private bool mention;

    public TMP_Text worldText;

    public SpriteRenderer sprite;

    public Sprite angrySprite;

    public AudioSource stompSound;

    private Sprite normalSprite;

    private bool count;

    [HideInInspector]
    public bool screaming;

    private bool wanderingInClass;

    public float Cooldown;

    private int wanderPoints;

    public AudioClip aud_map;

    public AudioClip aud_there;

    // Token: 0x060000B8 RID: 184 RVA: 0x00006780 File Offset: 0x00004B80
    private void Start()
    {
        regularSpeed = speed;
        this.audioDevice = base.GetComponent<AudioSource>();
        this.agent = base.GetComponent<NavMeshAgent>();
        normalSprite = sprite.sprite;
        this.Wander();
        targetPlayer = true;
        timeToMove = waitTime;
        timerText.gameObject.SetActive(false);
    }

    // Token: 0x060000B9 RID: 185 RVA: 0x000067AC File Offset: 0x00004BAC
    protected override void Update()
    {
        base.Update();
        if (this.timeToMove > 0f)
        {
            this.timeToMove -= 1f * Time.deltaTime;
        }
        else if (!stop)
        {
            Move();
        }
        timeMinutes = Mathf.Floor(time / 60);
        timeSeconds = (float)Mathf.CeilToInt(time % 60);
        if (ps.onClassroom & count & !angry)
        {
            count = false;
            timerText.gameObject.SetActive(false);
            keepWandering = false;
            audioQueue.Queue(inTime);
            audioQueue.Queue(aud_dismiss);
            stop = false;
        }
        if (!forcedToStop) agent.isStopped = stop;
        if ((timeSeconds > 59f || timeSeconds < 1f) & count & !mention & !meeting)
        {
            mention = true;
            if (time < 1f)
            {
                if (!angry)
                {
                    StartCoroutine(MinutesLeft(Mathf.RoundToInt(timeMinutes) + 1, true));
                    angry = true;
                    stop = true;
                    sprite.sprite = angrySprite;
                    targetPlayer = true;
                    playerInClassroom = true;
                    agent.Warp(transform.position + new Vector3(0f, 31f, 0f));
                }
            }
            else
            {
                StartCoroutine(MinutesLeft(Mathf.RoundToInt(timeMinutes) + 1, false));
            }
        }
        else if (!audioDevice.isPlaying & audioQueue.audioInQueue <= 0)
        {
            mention = false;
        }
        if (time > 0f & count)
        {
            time -= Time.deltaTime;
            timerText.gameObject.SetActive(true);
        }
        else if (count)
        {
            count = false;
            timerText.gameObject.SetActive(false);
            time = 0f;
        }
        worldText.text = GetTimer();
        timerText.text = "Class: " + GetTimer();
    }

    private IEnumerator MinutesLeft(int timeLeft, bool timesUp)
    {
        audioDevice.PlayOneShotWC(aud_Numbers[timeLeft]);
        yield return new WaitForSecondsRealtime(0.75f);
        audioDevice.PlayOneShotWC(aud_MinutesLeft);
        if (timesUp)
            audioQueue.Queue(aud_TimesUp);
        yield break;
    }

    // Token: 0x060000BA RID: 186 RVA: 0x000068E0 File Offset: 0x00004CE0
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (this.moveFrames > 0f)
        {
            this.moveFrames -= 1f;
            if (!inFlood)
                base.Speed = this.speed;
            else
                base.Speed = speed / 1.5f;
        }
        else if (!angry || !stop)
        {
            base.Speed = 10f;
        }
        if (metPlayer && !meeting && !count && !targetPlayer)
        {
            if (Cooldown > 0f)
            {
                Cooldown -= Time.deltaTime;
            }
            else
            {
                Cooldown = 0f;
                metPlayer = false;
                targetPlayer = true;
            }
        }
        if (ForceRun > 0f)
        {
            ForceRun -= Time.deltaTime;
        }
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        bool IsAiming = IsInSameFloor() && !GameManager.player.isInvisible && GameManager.player.NoFacultyAlert <= 0f && Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player";
        if (IsAiming & targetPlayer & !IsFlatten)
        {
            if (!db && !angry)
            {
                stompSound.PlayOneShotWC(aud_there);
            }
            this.db = true;
            this.TargetPlayer();
        }
        else if (!wandering)
        {
            this.db = false;
        }
        if (IsAiming & !IsFlatten & (base.transform.position - player.position).magnitude <= 15f)
        {
            if (!metPlayer)
            {
                metPlayer = true;
                Cooldown = 90f;
                if (ps.PlayerHasPass())
                {
                    targetPlayer = false;
                    gameController.UsePass();
                    return;
                }
                StartCoroutine(MeetClass());
            }
        }
    }

    public void Scream()
    {
        targetPlayer = false;
        audioDevice.PlayOneShotWC(aud_Scream);
        StartCoroutine(AngryTimer());
    }

    private void ResetWanderClass()
    {
        wanderingInClass = false;
        wanderPoints = 0;
    }

    protected override void OnFlatten()
    {
        db = false;
        stompSound.pitch = audioDevice.pitch;
    }

    protected override void OnUnflatten()
    {
        stompSound.pitch = audioDevice.pitch;
    }

    public Transform[] wanderTargets;

    private void WanderClass()
    {
        wanderingInClass = true;
        ClearTargetsAndSetTarget(wanderTargets[Mathf.RoundToInt(Random.Range(0f, wanderTargets.Length - 1))].position);
        wanderPoints++;
        if (wanderPoints > 8 & !keepWandering)
        {
            Wander();
        }
    }

    private bool meeting;

    private IEnumerator MeetClass()
    {
        meeting = true;
        targetPlayer = false;
        stop = true;
        audioQueue.Queue(aud_reminder);
        int num = Mathf.RoundToInt((float)Random.Range(4f, 9f));
        audioQueue.Queue(aud_Numbers[num]);
        yield return new WaitForSecondsRealtime(aud_reminder.length + 0.75f);
        audioDevice.PlayOneShotWC(aud_Minutes);
        audioQueue.Queue(aud_map);
        yield return new WaitForSecondsRealtime(1.5f);
        WanderClass();
        keepWandering = true;
        time = (float)num * 60f;
        count = true;
        stop = false;
        yield return new WaitForSeconds(1.2f);
        meeting = false;
        yield break;
    }

    public void Teleport(Vector3 pos)
    {
        agent.Warp(pos);
    }

    private bool keepWandering;

    private IEnumerator AngryTimer()
    {
        WanderClass();
        keepWandering = true;
        while (audioDevice.isPlaying || audioQueue.audioInQueue > 0)
        {
            yield return null;
        }
        angry = false;
        base.Speed = speed;
        sprite.sprite = normalSprite;
        audioQueue.Queue(aud_dismiss);
        targetPlayer = false;
        stop = false;
        keepWandering = false;
        screaming = false;
        Wander();
        yield break;
    }

    private float regularSpeed;

    private string GetTimer()
    {
        string result;
        string minutes;
        string seconds;
        float currentMinutes = timeMinutes;
        float currentSeconds = timeSeconds;
        if (currentSeconds > 59f)
        {
            currentSeconds = 0f;
            currentMinutes += 1f;
        }
        minutes = currentMinutes.ToString();
        if (currentSeconds < 10f)
        {
            seconds = "0" + currentSeconds;
        }
        else
        {
            seconds = currentSeconds.ToString();
        }
        result = minutes + ":" + seconds;
        return result;
    }

    // Token: 0x060000BB RID: 187 RVA: 0x000069B4 File Offset: 0x00004DB4
    private void Wander()
    {
        ResetWanderClass();
        this.wanderer.GetNewTarget();
        SetTarget(this.wanderTarget.position);
        wandering = true;
    }

    private void SetDestination(Vector3 location)
    {
        SetTarget(location);
        wandering = false;
    }

    // Token: 0x060000BC RID: 188 RVA: 0x000069EE File Offset: 0x00004DEE
    public void TargetPlayer()
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi && !MultiplayerManager.instance.isMainPlayer)
        {
            MultiplayerManager.instance.client.SendAsUDP(5, Photon.Realtime.ReceiverGroup.Others, "Mrs Pomp", SchoolSaves.Utils.ToFloat(player.position));
            wandering = false;
            return;
        }
        ClearTargetsAndSetTarget(this.player.position);
        wandering = false;
    }

    public void TargetAnotherPlayer(Vector3 Position)
    {
        ClearTargetsAndSetTarget(Position);
        wandering = false;
        ForceRun = 2f;
    }

    private float ForceRun = 0f;

    // Token: 0x060000BD RID: 189 RVA: 0x00006A20 File Offset: 0x00004E20

    public bool wandering;

    private void Move()
    {
        stompAnim.SetTrigger("stomp");
        stompSound.PlayOneShot(stomp);
        if (base.transform.position == this.previous && ForceRun <= 0f)
        {
            if (!wanderingInClass)
                this.Wander();
            else
                WanderClass();
        }
        this.moveFrames = 10f;
        if (db || angry || ForceRun > 0f)
        {
            timeToMove = playerTargetWaitTime - (angry ? 0.1f : 0f);
            speed = regularSpeed + 40f;
        }
        else
        {
            timeToMove = waitTime;
            speed = regularSpeed;
        }
        previous = base.transform.position;
    }

    public bool stopTimer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Pomp Class" & !wanderingInClass)
        {
            WanderClass();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.name == "Flood")
        {
            inFlood = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "Flood")
        {
            inFlood = false;
        }
    }

    // Token: 0x0400012E RID: 302
    public bool db;

    public bool targetPlayer;

    public bool metPlayer;

    public Animator stompAnim;

    // Token: 0x04000130 RID: 304
    public float speed;

    public Transform classRoom;

    public AudioClip stomp;

    public AudioClip aud_Minutes;

    public AudioClip aud_Scream;

    public AudioClip aud_MinutesLeft;

    public AudioClip inTime;

    public AudioClip[] aud_Numbers = new AudioClip[10];

    // Token: 0x04000131 RID: 305
    public float timeToMove;

    public float playerTargetWaitTime;

    // Token: 0x04000134 RID: 308
    public float waitTime;

    // Token: 0x04000136 RID: 310
    private float moveFrames;

    private bool inFlood;

    public PlayerScript ps;

    // Token: 0x0400013F RID: 319
    public Transform player;

    // Token: 0x04000140 RID: 320
    public Transform wanderTarget;

    // Token: 0x04000141 RID: 321
    public AILocationSelectorScript wanderer;

    // Token: 0x04000142 RID: 322
    private AudioSource audioDevice;

    // Token: 0x04000143 RID: 323
    public AudioClip aud_dismiss;

    public AudioClip aud_TimesUp;

    // Token: 0x04000144 RID: 324
    public AudioClip aud_reminder;

    public AudioQueueScript audioQueue;

    public GameControllerScript gameController;

    // Token: 0x04000147 RID: 327
    private Vector3 previous;

    // Token: 0x04000148 RID: 328
    private NavMeshAgent agent;
}
