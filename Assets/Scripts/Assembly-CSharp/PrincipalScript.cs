// PrincipalScript
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using SchoolSaves;

public class PrincipalScript : NPC
{
    public bool seesRuleBreak;

    public Transform player;

    public Transform bully;

    public bool bullySeen;

    public PlayerScript playerScript;

    public BaldiInidcatorScript baldiInidcator;

    public BullyScript bullyScript;

    public BaldiScript baldiScript;

    public Transform wanderTarget;

    public AILocationSelectorScript wanderer;

    public DoorScript officeDoor;

    [SerializeField]
    private bool MultipleDoors;
    [SerializeField]
    private DoorScript[] moreOfficeDoors;

    public float coolDown;

    public float timeSeenRuleBreak;

    public float bullyDetention;

    public bool angry;

    public bool inOffice;

    public int detentions;

    private int[] lockTime = new int[11]
    {
        15,
        20,
        25,
        30,
        35,
        40,
        45,
        50,
        55,
        60,
        99
    };

    private int bullyTime;

    public AudioClip[] audTimes = new AudioClip[11];

    public AudioClip[] audScolds = new AudioClip[3];

    public AudioClip audDetention;

    public AudioClip audNoEating;

    public AudioClip audNoDrinking;

    public AudioClip audNoBullying;

    public AudioClip audNoFaculty;

    public AudioClip audNoLockers;

    public AudioClip audNoRunning;

    public AudioClip audNoStabbing;

    public AudioClip audNoEscaping;

    public AudioClip audImComing;

    public AudioClip aud_Whistle;

    public AudioClip aud_Delay;

    public AudioClip audAfterHours;

    private NavMeshAgent agent;

    private AudioQueueScript audioDevice;

    public GameObject bullyInDetention;

    public bool bullyCapture;

    public RandomEvent eventScript;

    public Transform partyLocation;

    private void Start()
    {
        agent = base.GetComponent<NavMeshAgent>();
        audioDevice = base.GetComponent<AudioQueueScript>();
    }

    protected override void OnPartyHaveToAttend()
    {
        AttendParty();
        angry = false;
    }

    protected override void Update()
    {
        base.Update();
        if (eventScript.principalBreak)
        {
            ClearTargetsAndSetTarget(partyLocation.position);
            return;
        }
        if (eventScript.party)
        {
            return;
        }
        if (coolDown > 0f)
        {
            coolDown -= 1f * Time.deltaTime;
        }
        if (HearWhistle > 0f)
        {
            HearWhistle -= 1f * Time.deltaTime;
        }
        if (bullySeen && currentPriority < 12f)
        {
            SetTarget(bully.position);
        }
        if (seesRuleBreak)
        {
            timeSeenRuleBreak += 1f * Time.deltaTime;
            if ((double)timeSeenRuleBreak >= 0.5 & !angry)
            {
                angry = true;
                seesRuleBreak = false;
                timeSeenRuleBreak = 0f;
                TargetPlayer();
                CorrectPlayer();
                chasing = true;
                StartCoroutine(ChaseTimer());
            }
        }
        else
        {
            timeSeenRuleBreak = 0f;
        }
        if (bullyDetention > 0f)
        {
            bullyInDetention.SetActive(true);
            bullyDetention -= 1f * Time.deltaTime;
        }
        if (bullyDetention < 0f)
        {
            bullyDetention = 0f;
        }
        if (bullyDetention == 0f)
        {
            bullyInDetention.SetActive(false);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if ((!MultiplayerManager.isActive && !eventScript.party) || (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi))
        {
            if (!angry)
            {
                Vector3 direction = player.position - base.transform.position;
                RaycastHit raycastHit;
                if (!playerScript.gc.HASManager)
                {
                    if (IsInSameFloor() && !GameManager.player.isInvisible && GameManager.player.NoFacultyAlert <= 0f && playerScript.guilt > 0f && !inOffice && !angry && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
                    {
                        seesRuleBreak = true;
                    }
                    else
                    {
                        seesRuleBreak = false;
                        if (agent.velocity.magnitude <= 1f & coolDown <= 0f & !wantsElevate & !usingElevate)
                        {
                            Wander();
                        }
                    }
                }
                else
                {
                    seesRuleBreak = false;
                    if (agent.velocity.magnitude <= 1f & coolDown <= 0f & !wantsElevate & !usingElevate)
                    {
                        Wander();
                    }
                }
                if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
                {
                    direction = bully.position - base.transform.position;
                    if (bullyScript.guilt > 0f && !inOffice && !angry && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3) && (raycastHit.transform.name == "Its a Bully"))
                    {
                        bullyCapture = true;
                        TargetBully();
                    }
                }
            }
            else
            {
                Vector3 direction = player.position - base.transform.position;
                RaycastHit raycastHit;
                if (Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && IsInSameFloor() && raycastHit.transform.tag == "Player" && !GameManager.player.isInvisible)
                {
                    TargetPlayer();
                }
                else if (agent.velocity.magnitude <= 1f & coolDown <= 0f & !wantsElevate & !usingElevate)
                {
                    LookForPlayer();
                }
            }
        }
        if ((player.GetComponent<Collider>().isTrigger || GameManager.player.isInvisible) && angry && Time.timeScale != 0f)
        {
            Wander();
        }
    }

    private void Wander()
    {
        if (agent.isStopped && !forcedToStop)
        {
            agent.isStopped = false;
        }
        if (stabbedByPlayer)
        {
            stabbedByPlayer = false;
            TargetPlayer();
            return;
        }
        if (HearWhistle > 0f && (base.transform.position - MyAgent.destination).magnitude < 5f)
        {
            HearWhistle = 0f;
        }
        else if (HearWhistle > 0f) return; // Still need to focus on the whistle.
        WanderAround(WanderType.All);
        currentPriority = 2f;
        coolDown = 1f;
        Speed = 20f;
        agent.acceleration = 1000f;
        FrameDependentSpeed = false;
        if (angry)
        {
            angry = false;
            WanderAround(WanderType.All);
            audioDevice.PlaySound(aud_Whistle);
            return;
        }
        if (Random.Range(0f, 10f) <= 1f)
        {
            audioDevice.PlaySound(aud_Whistle);
        }
    }

    private void LookForPlayer()
    {
        WanderAround(WanderType.All);
        currentPriority = 10f;
        coolDown = 1f;
    }

    private float HearWhistle;

    public void Hear(Vector3 soundLocation, float priority, bool PlaySound = false)
    {
        if (priority >= currentPriority)
        {
            ClearTargetsAndSetTarget(soundLocation);
            currentPriority = priority;
            if (PlaySound)
            {
                HearWhistle = 30f;
                base.Speed = 150f;
                agent.acceleration = 50000f;
                FrameDependentSpeed = true;
                audioDevice.PlaySound(audImComing);
                if (!MultiplayerManager.isActive)
                {
                    if (eventScript.principalBreak || eventScript.party)
                    {
                        coolDown = 1f;
                        // Because they'll still have delay when the event ends.
                        eventScript.principalBreak = false; 
                        eventScript.party = false;
                        eventScript.StopEventNOW();
                    }
                }
            }
        }
    }

    public float currentPriority;

    public bool chasing;

    private bool stabbedByPlayer;

    private IEnumerator ChaseTimer()
    {
        float time = 30f;
        while (time > 0f & chasing)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        if (chasing)
        {
            stabbedByPlayer = false;
            Wander();
            chasing = false;
        }
        yield break;
    }

    private void TargetPlayer()
    {
        if (!MultiplayerManager.isActive)
        {
            ClearTargetsAndSetTarget(player.position);
        }
        else if (MultiplayerManager.instance.isMainPlayer)
        {
            ClearTargetsAndSetTarget(player.position);
        }
        else
        {
            MultiplayerManager.instance.client.SendAsUDP(5, Photon.Realtime.ReceiverGroup.Others, "Principal", player.position.ToFloat());
        }
        currentPriority = 8f;
        coolDown = 1f;
        if (!stabbedByPlayer)
        {
            base.Speed = 20f;
            agent.acceleration = 1000f;
            FrameDependentSpeed = false;
        }
        HearWhistle = 0f;
    }

    public void TargetAnotherPlayer(Vector3 Position)
    {
        ClearTargetsAndSetTarget(Position);
        coolDown = 1f;
        if (!stabbedByPlayer)
        {
            base.Speed = 20f;
            agent.acceleration = 1000f;
            FrameDependentSpeed = false;
        }
        HearWhistle = 0f;
    }

    private void TargetBully()
    {
        if (!bullySeen)
        {
            ClearTargetsAndSetTarget(bully.position);
            audioDevice.Queue(audNoBullying);
            currentPriority = 10f;
            base.Speed = 20f;
            agent.acceleration = 1000f;
            FrameDependentSpeed = false;
            bullySeen = true;
        }
    }

    private void CorrectPlayer()
    {
        audioDevice.ClearQueue();
        AudioClip Clip = audNoRunning;
        if (playerScript.guiltType == "faculty")
        {
            Clip = audNoFaculty;
        }
        else if (playerScript.guiltType == "running")
        {
            Clip = audNoRunning;
        }
        else if (playerScript.guiltType == "drink")
        {
            Clip = audNoDrinking;
        }
        else if (playerScript.guiltType == "escape")
        {
            Clip = audNoEscaping;
        }
        else if (playerScript.guiltType == "eat")
        {
            Clip = audNoEating;
        }
        else if (playerScript.guiltType == "lockers")
        {
            Clip = audNoLockers;
        }
        else if (playerScript.guiltType == "stabbing")
        {
            Clip = audNoStabbing;
        }
        else if (playerScript.guiltType == "sneaking")
        {
            Clip = audAfterHours;
        }
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer)
        {
            if (!MultiplayerManager.instance.isBaldi)
            {
                GetComponent<AudioSource>().Stop();
                GetComponent<AudioSource>().PlayOneShotWC(Clip);
            }
        }
        else
        {
            audioDevice.Queue(Clip);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.name == "Its a Bully" && (FlattenDuration <= 0f || (GetComponent<BullyScript>() && GetComponent<BullyScript>().FlattenDuration > 0f)))
        {
            if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer)
            {
                return;
            }
            if (bullySeen)
            {
                bullyTrouble();
            }
        }
        if (other.name == "Office Trigger")
        {
            inOffice = true;
        }
        if (other.tag == "Player" & !IsFlatten & angry & !other.isTrigger)
        {
            if (playerScript.guiltType == "sneaking")
            {
                playerScript.cameraScript.gameOverCharacterTarget = agent.transform;
                playerScript.gameOver = true;
                return;
            }
            if (inOffice)
            {
                officeDoor.CloseDoor();
                if (MultipleDoors)
                {
                    foreach (DoorScript door in moreOfficeDoors)
                    {
                        door.CloseDoor();
                    }
                }
                agent.SetDestination(PrincipalWarpPosition);
            }
            else
            {
                agent.Warp(PrincipalWarpPosition);
                agent.isStopped = true;
            }
            chasing = false;
            if (player.transform.position.y >= 40f) playerScript.TakeDamage(player.transform.position.y / 18f * 30f);
            if (playerScript.health <= 0f)
            {
                playerScript.cameraScript.gameOverCharacterTarget = agent.transform;
            }
            if (!inOffice)
            {
                other.transform.position = DetentionVictimPosition;
            }
            Transform transform = other.transform;
            Vector3 position = PrincipalWarpPosition;
            float x = position.x;
            Vector3 position2 = other.transform.position;
            float y = position2.y;
            Vector3 position3 = PrincipalWarpPosition;
            if (!inOffice)
            {
                transform.LookAt(new Vector3(x, y, position3.z));
                playerScript.SetRotation(transform.rotation); // Because they won't be able to rotate properly.
            }
            inOffice = true;
            if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer)
            {
                if (MultiplayerManager.instance.isBaldi) return;
                officeDoor.LockDoor((float)lockTime[detentions]);
                if (MultipleDoors)
                {
                    foreach (DoorScript door in moreOfficeDoors)
                    {
                        door.LockDoor((float)lockTime[detentions]);
                    }
                }
                if (baldiScript.isActiveAndEnabled)
                {
                    float priority = 5f;
                    if (!baldiScript.antiHearing && priority >= baldiScript.currentPriority)
                    {
                        baldiScript.Hear(base.transform.position, 5f);
                        baldiInidcator.TriggerHear();
                    }
                    else
                    {
                        baldiInidcator.TriggerConfused();
                    }
                }
                if (baldiScript.gameController.DoctorReflex != null && baldiScript.gameController.DoctorReflex.Angry && !baldiScript.gameController.DoctorReflex.WantsTheHammer)
                {
                    baldiScript.gameController.DoctorReflex.Hear(transform.position); // Hear like Baldi
                }
                coolDown = 5f;
                angry = false;
                detentions++;
                if (detentions > 10)
                {
                    AchievementManager.Instance.Earn(Achievements.Detention99);
                    detentions = 10;
                }
                Multiplayer.ClientSend.SendCharacterState("Principal", 0);
                Invoke("MP_DetentionAudio", 0.25f);
                return;
            }
            audioDevice.Queue(aud_Delay);
            audioDevice.Queue(audTimes[detentions]);
            audioDevice.Queue(audDetention);
            int num = Mathf.RoundToInt(Random.Range(0f, 3f));
            audioDevice.Queue(audScolds[num]);
            officeDoor.LockDoor((float)lockTime[detentions]);
            if (MultipleDoors)
            {
                foreach (DoorScript door in moreOfficeDoors)
                {
                    door.LockDoor((float)lockTime[detentions]);
                }
            }
            if (baldiScript.isActiveAndEnabled)
            {
                float priority = 5f;
                if (!baldiScript.antiHearing && priority >= baldiScript.currentPriority)
                {
                    baldiScript.Hear(base.transform.position, 5f);
                    baldiInidcator.TriggerHear();
                }
                else
                {
                    baldiInidcator.TriggerConfused();
                }
            }
            coolDown = 5f;
            angry = false;
            detentions++;
            if (detentions > 10)
            {
                AchievementManager.Instance.Earn(Achievements.Detention99);
                detentions = 10;
            }
        }
    }

    public void MP_TeleportEvent()
    {
        if (inOffice)
        {
            officeDoor.CloseDoor();
            if (MultipleDoors)
            {
                foreach (DoorScript door in moreOfficeDoors)
                {
                    door.CloseDoor();
                }
            }
            agent.SetDestination(PrincipalWarpPosition);
        }
        else
        {
            agent.Warp(PrincipalWarpPosition);
            agent.isStopped = true;
        }
        inOffice = true;
        chasing = false;
        coolDown = 5f;
        angry = false;
        Invoke("MP_AudDelay", 2f);
    }

    private void MP_DetentionAudio()
    {
        GetComponent<AudioSource>().PlayOneShotWC(audTimes[detentions - 1]);
    }

    private void MP_AudDelay()
    {
        audioDevice.Queue(audDetention);
        int num = Mathf.RoundToInt(Random.Range(0f, 3f));
        audioDevice.Queue(audScolds[num]);
    }

    public Vector3 PrincipalWarpPosition = new Vector3(10f, 0f, 170f);
    public Vector3 DetentionVictimPosition = new Vector3(10f, 4f, 160f);

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Office Trigger")
        {
            inOffice = false;
        }
        if (other.name == "Its a Bully")
        {
            if (!bullyCapture)
            {
                bullySeen = false;
            }
        }
    }

    public void bullyTrouble()
    {
        bullyScript.inDetention = true;
        bullyScript.Reset();
        bullyCapture = false;
        agent.Warp(PrincipalWarpPosition);
        agent.isStopped = true;
        coolDown = 5f;
        if (detentionBullyTimes >= 11)
        {
            detentionBullyTimes = 10;
        }
        audioDevice.Queue(aud_Delay);
        audioDevice.Queue(audTimes[detentionBullyTimes]);
        audioDevice.Queue(audDetention);
        int num = Mathf.RoundToInt(Random.Range(0f, 3f));
        audioDevice.Queue(audScolds[num]);
        bullyDetention = (float)lockTime[detentionBullyTimes];
        detentionBullyTimes++;
    }

    public void Stabbed()
    {
        base.Speed = 0f;
        coolDown = 15f;
        stabbedByPlayer = true;
    }

    public int detentionBullyTimes;
}