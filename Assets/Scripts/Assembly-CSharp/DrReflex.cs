using System.Collections;
using Multiplayer;
using Photon.Pun;
using Photon.Realtime;
using SchoolSaves;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class DrReflex : NPC
{
    public Animator[] AngleAnimators;
    [Header("Audio (Voicelines)")]
    public AudioClip[] IdleSounds;
    public AudioClip[] PlayerFailsTest;
    public AudioClip SeenPlayer_Start;
    public AudioClip SeenPlayer_Loop;
    public AudioClip SeenPlayer_End;
    public AudioClip PlayerStartTest;
    public AudioClip PlayerTooSlow1;
    public AudioClip PlayerTooSlow2;
    public AudioClip PlayerWrong1;
    public AudioClip PlayerWrong2;
    public AudioClip PlayerRetryPrompt;
    public AudioClip PlayerQuicklyRight;
    public AudioClip[] PlayerRight;
    public AudioClip[] PlayerRight2;
    public AudioClip PlayerLeaves;
    public AudioClip[] AngryAtPlayer;
    public AudioClip CalmAtPlayer;
    public AudioClip SFX_Boink;
    public AudioClip SFX_Hammer;
    public AudioQueueScript MainAudioMan;

    [Header("Angle Turning (from 1st Prize)")]
    public float turnSpeed;

    public float angleDiff;

    public float currentSpeed;

    private float coolDown;

    [Header("Normal Variables")]
    private AudioSource audioDevice;
    public bool HasSlowRotation = true;
    public GameObject[] Sprites;
    public Animator BaseAnim;
    public bool Angry;
    public bool WantsTheHammer;
    private bool HasTheHammer;
    public Transform WheresTheHammer;

    public bool ChasingPlayer;
    public int PlayerAttempts;
    private int ThinkStage;
    private bool IsTesting;
    private float TestingDelay;
    private float TestingGO;
    private float TestCooldown;
    private bool AnswerIsRight;

    private float HammeringDelay;

    private Transform player;
    private GameControllerScript gc;

    private Vector3 LastSeen;

    public GameObject Hands;

    [Header("Clinic/Unique Mechs")]
    public ClinicRoom Clinic;
    public ClinicHammerTrigger HammerManager;
    public bool ReturningHammer;
    public bool HasHammer;
    public bool ChasingTheHammer;
    [Tooltip("This is Dr. Reflex's Height from the beta tester version.")]
    public bool PreHeightChange;

    private void Start()
    {
        currentSpeed = 10f;
        audioDevice = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerScript>().transform;
        gc = FindObjectOfType<GameControllerScript>();
        Wander();
        if (PreHeightChange)
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (Transform child in Sprites[i].transform)
                {
                    child.localPosition = new Vector3(child.localPosition.x, 2.654f, child.localPosition.z);
                    child.localScale = new Vector3(3f, 3f, 3f);
                }
            }
            Sprites[2].transform.localPosition = new Vector3(Sprites[2].transform.localPosition.x, 2.654f, Sprites[2].transform.localPosition.z);
            Sprites[2].transform.localScale = new Vector3(3f, 3f, 3f);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (coolDown > 0f && !WantsTheHammer)
        {
            coolDown -= 1f * Time.deltaTime;
        }
        if (WantsTheHammer || (HasHammer && ReturningHammer && coolDown <= 0f))
        {
            SetTarget(WheresTheHammer.position);
        }
        if (HasSlowRotation)
        {
            Vector3 eulerAngles = base.transform.eulerAngles;
            float y = eulerAngles.y;
            Vector3 steeringTarget = MyAgent.steeringTarget;
            float x = steeringTarget.x;
            Vector3 position = base.transform.position;
            float y2 = x - position.x;
            Vector3 steeringTarget2 = MyAgent.steeringTarget;
            float z = steeringTarget2.z;
            Vector3 position2 = base.transform.position;
            angleDiff = Mathf.DeltaAngle(y, Mathf.Atan2(y2, z - position2.z) * 57.29578f);
            if (Mathf.Abs(angleDiff) < 5f)
            {
                Transform transform = base.transform;
                Vector3 steeringTarget3 = MyAgent.steeringTarget;
                float x2 = steeringTarget3.x;
                Vector3 position3 = base.transform.position;
                float y3 = position3.y;
                Vector3 steeringTarget4 = MyAgent.steeringTarget;
                transform.LookAt(new Vector3(x2, y3, steeringTarget4.z));
                base.Speed = currentSpeed;
                if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
                {
                    SetActiveSprite(1);
                }
            }
            else
            {
                base.transform.Rotate(new Vector3(0f, turnSpeed * Mathf.Sign(angleDiff) * Time.deltaTime, 0f));
                base.Speed = 0f;
                if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
                {
                    SetActiveSprite(0);
                }
            }
        }
        else if (IsTesting)
        {
            base.Speed = 0f;
            // Delay
            if (TestingDelay > 0f)
            {
                TestingDelay -= Time.deltaTime;
                if (TestingDelay <= 0f) // FAST!
                {
                    Audio_PlayOneShotWC(PlayerStartTest);
                    AnswerIsRight = Mathf.RoundToInt(Random.Range(0f, 1f)) == 1;
                    BaseAnim.SetBool("Right", AnswerIsRight);
                    BaseAnim.SetTrigger("GO");
                    TestingGO = 0.6f;
                    if (PlayerAttempts > 1)
                    {
                        TestingGO = 0.75f;
                    }
                    if (PlayerAttempts >= 4)
                    {
                        TestingGO = 1.1f;
                    }
                    Hands.SetActive(true);
                    // Since the rigidbody can be detected for a second
                    TurnOffCollision(true);
                }
            }
            else if (TestingGO > 0f) // Testing started
            {
                TestingGO -= Time.deltaTime;
                // Pause animation
                if (PlayerAttempts > 1 && PlayerAttempts < 4 && TestingGO <= 0.3f)
                {
                    BaseAnim.speed = 0f;
                }
                if (PlayerAttempts >= 4 && TestingGO <= 0.7f)
                {
                    BaseAnim.speed = 0f;
                }

                if (TestingGO <= 0f) // Too slow...
                {
                    Hands.SetActive(false);
                    BaseAnim.speed = 1f;
                    // Reset collision
                    TurnOffCollision(false);
                    if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
                    {
                        audioDevice.Stop();
                        MainAudioMan.ClearQueue();
                        if (gc.CM != null && gc.CM.isActiveAndEnabled)
                        {
                            gc.CM.DestroyAllCaptions(false);
                        }
                    }
                    Audio_Queue(PlayerFailsTest[Mathf.RoundToInt(Random.Range(0f, PlayerFailsTest.Length - 1))]);
                    Audio_Queue((Mathf.RoundToInt(Random.Range(0f, 1f)) == 1) ? PlayerTooSlow1 : PlayerTooSlow2);
                    Audio_Queue(PlayerRetryPrompt);
                    TestingDelay = 7f;
                    PlayerAttempts++;
                }
            }
        }
        else
        {
            base.Speed = currentSpeed;
            if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
            {
                SetActiveSprite(2);
            }
            if (ThinkStage == 0 && !audioDevice.isPlaying)
            {
                audioDevice.clip = SeenPlayer_Loop;
                audioDevice.loop = true;
                audioDevice.Play();
                ThinkStage = 1;
            }
        }

        if (TestCooldown > 0f)
        {
            TestCooldown -= Time.deltaTime;
        }
    }

    public void CarryHammer()
    {
        WantsTheHammer = false;
        HasTheHammer = true;
        ClearTargetsAndSetTarget(LastSeen);
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            ClientSend.SendCharacterState("Doctor Reflex", 4);
        }
    }

    private void TurnOffCollision(bool Val)
    {
        Transform t = Hands.transform;
        t.SetParent(Val ? transform.parent : Sprites[2].transform, true);
        if (!Val) t.localRotation = Quaternion.identity;
    }

    private int HammerAudio;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (MultiplayerManager.isActive && MultiplayerManager.instance.HasMoreThan2Players)
        {
            if (MultiplayerManager.instance.isMainPlayer) MP_FixedUpdate(); else if (!MultiplayerManager.instance.isBaldi) MP_FixedUpdateOthers();
            return;
        }
        if (MultiplayerManager.isActive)
        {
            if (MultiplayerManager.instance.isMainPlayer) MP_FixedUpdate();
            return;
        }
        if (!IsTesting && !Angry && coolDown <= 0f && MyAgent.velocity.magnitude <= 1f && !wantsElevate && !usingElevate && (base.transform.position - MyAgent.destination).magnitude < 5f)
        {
            Wander();
        }
        if (!ChasingPlayer && !IsTesting && !Angry)
        {
            Vector3 direction = player.position - base.transform.position;
            RaycastHit raycastHit;
            if (Mathf.Abs(angleDiff) < 5f && IsInSameFloor() && !GameManager.player.isInvisible && GameManager.player.NoFacultyAlert <= 0f && !ReturningHammer && GetAngle() == 0 && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore))
            {
                if (raycastHit.transform.tag == "Player")
                {
                    if (GameManager.HasHammer()) // Player has his hammer
                    {
                        ChaseAndGetBackTHEHAMMER();
                    }
                    else if (!gc.HASManager && TestCooldown <= 0f) // Dr. Reflex will ignore the player while playing Hide & Seek.
                    {
                        TimeToThink(); // Now, we get the player to THIIIIIIIIIIIINK!
                    }
                }
            }
            // Hammer Check
            if (GameManager.HammerReflex != null)
            {
                direction = GameManager.HammerReflex.transform.position - transform.position;
                if (Mathf.Abs(angleDiff) < 5f && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore))
                {
                    if (raycastHit.transform.name == "Pickup_HammerReflex") // The player dropped the Hammer for him
                    {
                        ChaseAndGetBackTHEHAMMER();
                    }
                }
            }
        }
        else if (Angry)
        {
            if (HammeringDelay > 0f)
            {
                currentSpeed = 0f;
                HammeringDelay -= Time.deltaTime;
                Anim_SetBool("BAM", true);
                if (HammerAudio < 4)
                {
                    HammerAudio++;
                }
                else
                {
                    Audio_PlayOneShot(SFX_Hammer);
                    HammerAudio = 0;
                }
                return;
            }
            Anim_SetBool("BAM", false);
            if (WantsTheHammer) // He doesn't have the hammer yet
            {
                Vector3 direction = player.position - base.transform.position;
                RaycastHit raycastHit;
                if (IsInSameFloor() && !GameManager.player.isInvisible && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
                {
                    LastSeen = player.position; // Dr. Reflex will remember where he's last seen.
                }
            }
            if (HasTheHammer)
            {
                currentSpeed = 30f;
                Anim_SetBool("HasHammer", true);
                if (coolDown <= 0f && MyAgent.velocity.magnitude <= 1f & (base.transform.position - MyAgent.destination).magnitude < 5f & !wantsElevate & !usingElevate)
                {
                    WanderAround(WanderType.Rooms);
                    coolDown = 1f;
                    // Angry Idle Sounds
                    int num = Mathf.RoundToInt(Random.Range(0f, 2f));
                    if (num == 0)
                    {
                        int num2 = Mathf.RoundToInt(Random.Range(0f, AngryAtPlayer.Length - 1));
                        Audio_Queue(AngryAtPlayer[num2]);
                    }
                }
                Vector3 direction = player.position - base.transform.position;
                RaycastHit raycastHit;
                if (IsInSameFloor() && !GameManager.player.isInvisible && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
                {
                    TargetPlayer();
                }
            }
        }
        else if (IsTesting)
        {
            if (Vector3.Distance(player.position, transform.position) > 10f && TestingDelay <= 0f)
            {
                IStepAway();
            }
        }
        else
        {
            Vector3 direction = player.position - base.transform.position;
            RaycastHit raycastHit;
            if (Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore))
            {
                if (!ChasingTheHammer)
                {
                    if (raycastHit.transform.tag == "Player" && IsInSameFloor() && !GameManager.player.isInvisible)
                    {
                        TargetPlayer();
                    }
                }
                else
                {
                    if (raycastHit.transform.tag == "Player" && IsInSameFloor() && GameManager.HasHammer() && !GameManager.player.isInvisible)
                    {
                        TargetPlayer();
                    }
                }
            }
            if (Vector3.Distance(player.position, transform.position) <= 7f && !ChasingTheHammer)
            {
                ReachedPlayer();
            }
            if (ChasingTheHammer && GameManager.HammerReflex != null)
            {
                direction = GameManager.HammerReflex.transform.position - transform.position;
                if (Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore))
                {
                    if (raycastHit.transform.name == "Pickup_HammerReflex")
                    {
                        coolDown = 1f;
                        ClearTargetsAndSetTarget(raycastHit.transform.position);
                        if (Vector3.Distance(raycastHit.transform.position, transform.position) <= 7f && FlattenDuration <= 0f)
                        {
                            Destroy(raycastHit.transform.gameObject, 0f);
                            GameManager.HammerReflex = GameObject.Find("Pickup_HammerReflex"); // If Dr. Reflex can find any.
                            HasHammer = true;
                            ReturningHammer = true;
                            ChasingTheHammer = false;
                            ChasingPlayer = false;
                            coolDown = 0f;
                        }
                    }
                }
            }
        }
    }

    private void MP_FixedUpdate()
    {
        if (!IsTesting && !Angry && coolDown <= 0f && MyAgent.velocity.magnitude <= 1f && !wantsElevate && !usingElevate && (base.transform.position - MyAgent.destination).magnitude < 5f)
        {
            Wander();
        }
        if (!ChasingPlayer && !IsTesting && !Angry)
        {
            Vector3 direction = player.position - base.transform.position;
            RaycastHit raycastHit;
            if (Mathf.Abs(angleDiff) < 5f && TestCooldown <= 0f && IsInSameFloor() && !GameManager.player.isInvisible && GameManager.player.NoFacultyAlert <= 0f && !ReturningHammer && GetAngle() == 0 && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore))
            {
                if (raycastHit.transform.tag == "Player" && targetPlayer == null)
                {
                    TimeToThink();
                    targetPlayer = player;
                }
            }
        }
        else if (Angry)
        {
            if (HammeringDelay > 0f)
            {
                currentSpeed = 0f;
                HammeringDelay -= Time.deltaTime;
                if (!Anim_GetBool("BAM")) Anim_SetBool("BAM", true);
                if (HammerAudio < 4)
                {
                    HammerAudio++;
                }
                else
                {
                    Audio_PlayOneShot(SFX_Hammer);
                    HammerAudio = 0;
                }
                return;
            }
            if (Anim_GetBool("BAM")) Anim_SetBool("BAM", false);
            if (HasTheHammer)
            {
                currentSpeed = 30f;
                if (!Anim_GetBool("HasHammer")) Anim_SetBool("HasHammer", true);
                if (coolDown <= 0f && MyAgent.velocity.magnitude <= 1f & (base.transform.position - MyAgent.destination).magnitude < 5f & !wantsElevate & !usingElevate)
                {
                    WanderAround(WanderType.Rooms);
                    coolDown = 1f;
                    // Angry Idle Sounds
                    int num = Mathf.RoundToInt(Random.Range(0f, 2f));
                    if (num == 0)
                    {
                        int num2 = Mathf.RoundToInt(Random.Range(0f, AngryAtPlayer.Length - 1));
                        Audio_Queue(AngryAtPlayer[num2]);
                    }
                }
                if (targetPlayer == player || targetPlayer == null)
                {
                    Vector3 direction = player.position - base.transform.position;
                    RaycastHit raycastHit;
                    if (IsInSameFloor() && !GameManager.player.isInvisible && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
                    {
                        MultiplayerManager.instance.client.SendAsUDP(5, Photon.Realtime.ReceiverGroup.All, "Doctor Reflex", player.position.ToFloat());
                    }
                }
            }
        }
        else if (IsTesting)
        {
            if (targetPlayer != null && Vector3.Distance(targetPlayer.position, transform.position) > 10f && TestingDelay <= 0f)
            {
                IStepAway();
            }
        }
        else if (targetPlayer != null)
        {
            if (targetPlayer == player)
            {
                Vector3 direction = targetPlayer.position - base.transform.position;
                RaycastHit raycastHit;
                if (Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore))
                {
                    if (raycastHit.transform.tag == "Player" && IsInSameFloor() && !GameManager.player.isInvisible)
                    {
                        MultiplayerManager.instance.client.SendAsUDP(5, Photon.Realtime.ReceiverGroup.All, "Doctor Reflex", player.position.ToFloat());
                    }
                }
            }
            if (Vector3.Distance(targetPlayer.position, transform.position) <= 7f && !ChasingTheHammer)
            {
                ReachedPlayer();
            }
        }
    }

    private void MP_FixedUpdateOthers()
    {
        if (Angry)
        {
            if (HasTheHammer && targetPlayer == player)
            {
                Vector3 direction = player.position - base.transform.position;
                RaycastHit raycastHit;
                if (IsInSameFloor() && !GameManager.player.isInvisible && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
                {
                    MultiplayerManager.instance.client.SendAsUDP(5, Photon.Realtime.ReceiverGroup.All, "Doctor Reflex", player.position.ToFloat());
                }
            }
        }
        else if (!IsTesting)
        {
            Vector3 direction = player.position - base.transform.position;
            RaycastHit raycastHit;
            if (TestCooldown <= 0f && IsInSameFloor() && !GameManager.player.isInvisible && GameManager.player.NoFacultyAlert <= 0f && !ReturningHammer && GetAngle() == 0 && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
            {
                MultiplayerManager.instance.client.SendAsUDP(5, Photon.Realtime.ReceiverGroup.All, "Doctor Reflex", player.position.ToFloat());
            }
        }
    }

    public void MP_CharacterState(int state)
    {
        if (state == -3)
        {
            SetActiveSprite(0);
        }
        if (state == -2)
        {
            SetActiveSprite(1);
        }
        if (state == -1)
        {
            SetActiveSprite(2);
        }
        // Character States now
        if (state == 0 && !IsTesting && !Angry) // THINKKK...
        {
            ChasingPlayer = true;
            ChasingTheHammer = false;
            HasSlowRotation = false;
            WantsTheHammer = false;
            HasTheHammer = false;
            PlayerAttempts = 0;
            ThinkStage = 0;
            audioDevice.Stop();
            audioDevice.PlayOneShotWC(SeenPlayer_Start);
            HasSlowRotation = false;
        }
        if (state == 1) // Stop thinking...
        {
            ThinkStage = -1;
            ChasingPlayer = false;
            ChasingTheHammer = false;
            HasSlowRotation = false;
            WantsTheHammer = false;
            HasTheHammer = false;
            audioDevice.Stop();
            PlayerAttempts = 0;
            ThinkStage = 2;
            audioDevice.clip = null;
            if (gc.CM != null && gc.CM.isActiveAndEnabled)
            {
                gc.CM.DeleteClipCaption(gameObject, SeenPlayer_Loop.name);
            }
            audioDevice.loop = false;
            IsTesting = true;
            TestingDelay = SeenPlayer_End.length + 1f;
        }
        if (state == 2) // Wander
        {
            Wander();
        }
        if (state == 3) // Uh oh...
        {
            TurnOffCollision(false);
            Hands.SetActive(false);
            coolDown = 1f;
            PlayerAttempts = 0;
            BaseAnim.speed = 1f;
            HasSlowRotation = false;
            WantsTheHammer = false;
            HasTheHammer = false;
            IsTesting = false;
            Angry = true;
        }
        if (state == 4) // HE HAS THE HAMMER
        {
            coolDown = 1f;
            PlayerAttempts = 0;
            BaseAnim.speed = 1f;
            HasSlowRotation = false;
            WantsTheHammer = false;
            HasTheHammer = true;
            IsTesting = false;
            Angry = true;
        }
        if (state == 5) // Squish
        {
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
            {
                HammeringDelay = 1f;
                Anim_SetBool("BAM", true);
            }
        }
        if (state == 6)
        {
            TestCooldown = 35f; // We have to make sure the game doesn't bug.
            Invoke("HappyDelay", 1f);
        }
        if (state == 8) // Test completed
        {
            TestCooldown = 30f;
            bool IsFirstTry = PlayerAttempts == 0;
            PlayerAttempts = 0;
            TestingGO = 0f;
            TestingDelay = 0f;
            Wander();
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
            {
                audioDevice.Stop();
                MainAudioMan.ClearQueue();
                int Num = Mathf.RoundToInt(Random.Range(0f, PlayerRight.Length - (IsFirstTry ? 0 : 1)));
                Audio_Queue((Num == PlayerRight.Length) ? PlayerQuicklyRight : PlayerRight[Num]);
                Audio_Queue(PlayerRight2[Mathf.RoundToInt(Random.Range(0f, PlayerRight2.Length - 1))]);
                Audio_PlayOneShot(SFX_Boink);
            }
        }
    }

    private void TargetPlayer()
    {
        coolDown = 1f;
        targetPlayer = player;
        ClearTargetsAndSetTarget(player.position);
    }

    public void TargetAnotherPlayer(Transform thisOne, bool isMainPlayer)
    {
        if (TestCooldown > 0f) return; // Make sure Dr. Reflex doesn't call any player with a cooldown.
        if (!isMainPlayer)
        {
            if (targetPlayer == null)
            {
                targetPlayer = thisOne;
            }
            return;
        }
        if (!ChasingPlayer && !IsTesting && !Angry)
        {
            TimeToThink();
            targetPlayer = thisOne;
        }
        else if (thisOne != targetPlayer) { return; }
        ClearTargetsAndSetTarget(thisOne.position);
    }

    private Transform targetPlayer;

    private void TimeToThink()
    {
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer && !ChasingPlayer)
        {
            ClientSend.SendCharacterState("Doctor Reflex", 0);
        }
        ChasingPlayer = true;
        ChasingTheHammer = false;
        HasSlowRotation = false;
        WantsTheHammer = false;
        HasTheHammer = false;
        PlayerAttempts = 0;
        ThinkStage = 0;
        audioDevice.Stop();
        MainAudioMan.ClearQueue();
        audioDevice.PlayOneShotWC(SeenPlayer_Start);

        currentSpeed = 30f;
        SetActiveSprite(2);
        Anim_SetTrigger("CallRun");
        Anim_SetBool("HasHammer", false);
        Anim_SetBool("Right", false);
        Anim_SetBool("Running", true);
    }

    public void Hear(Vector3 Location)
    {
        if (!isActiveAndEnabled) return;
        if ((transform.position - Location).magnitude > 200) return;
        if (GameManager.baldiScrpt.antiHearing) return;
        ClearTargetsAndSetTarget(Location);
    }

    private void ChaseAndGetBackTHEHAMMER()
    {
        ChasingPlayer = true;
        ChasingTheHammer = true;
        HasSlowRotation = false;
        WantsTheHammer = false;
        HasTheHammer = false;
        PlayerAttempts = 0;
        ThinkStage = -1;
        audioDevice.Stop();

        currentSpeed = 30f;
        SetActiveSprite(2);
        Anim_SetTrigger("CallRun");
        Anim_SetBool("HasHammer", false);
        Anim_SetBool("Right", false);
        Anim_SetBool("Running", true);
    }

    private void ReachedPlayer()
    {
        SetActiveSprite(2);
        ChasingPlayer = false;
        ChasingTheHammer = false;
        HasSlowRotation = false;
        WantsTheHammer = false;
        HasTheHammer = false;
        audioDevice.Stop();
        PlayerAttempts = 0;
        ThinkStage = 2;
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            ClientSend.SendCharacterState("Doctor Reflex", 1);
        }
        audioDevice.clip = null;
        audioDevice.loop = false;
        if (gc.CM != null && gc.CM.isActiveAndEnabled)
        {
            gc.CM.DeleteClipCaption(gameObject, SeenPlayer_Loop.name);
        }
        Audio_Queue(SeenPlayer_End);
        currentSpeed = 0f;
        IsTesting = true;
        TestingDelay = SeenPlayer_End.length + 1f;
        Anim_SetBool("HasHammer", false);
        Anim_SetBool("Running", false);
        Anim_SetTrigger("GoIdle_Test");
    }

    private void Wander()
    {
        WanderAround(WanderType.Halls);
        if (ChasingPlayer)
        {
            ThinkStage = 0;
            audioDevice.Stop();
            MainAudioMan.ClearQueue();
            audioDevice.clip = null;
            audioDevice.loop = false;
            if (gc.CM != null && gc.CM.isActiveAndEnabled)
            {
                gc.CM.DeleteClipCaption(gameObject, SeenPlayer_Loop.name);
            }
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
            {
                ClientSend.SendCharacterState("Doctor Reflex", 2);
            }
        }
        int num = Mathf.RoundToInt(Random.Range(0f, 9f));
        if (num == 0 & coolDown <= 0f)
        {
            int num2 = Mathf.RoundToInt(Random.Range(0f, IdleSounds.Length - 1));
            Audio_Queue(IdleSounds[num2]);
        }
        coolDown = 1f;
        IsTesting = false;
        ChasingPlayer = false;
        HasSlowRotation = true;
        ChasingTheHammer = false;
        WantsTheHammer = false;
        HasTheHammer = false;
        currentSpeed = 10f;
        targetPlayer = null;
        Anim_SetBool("HasHammer", false);
        Anim_SetBool("Running", false);
        Anim_SetTrigger("GoIdle");
    }

    private int GetAngle()
    {
        for (int i = 0; i < Sprites.Length; i++)
        {
            if (Sprites[i].activeSelf && i != 2) return Sprites[i].GetComponent<DrReflexSpriteScript>().angle;
        }
        return 0;
    }

    public void Answer(bool Right)
    {
        // Reset collision
        TurnOffCollision(false);

        Hands.SetActive(false);
        if (!IsTesting) return;
        if (Right != AnswerIsRight)
        {
            audioDevice.Stop();
            MainAudioMan.ClearQueue();
            Audio_Queue(PlayerFailsTest[Mathf.RoundToInt(Random.Range(0f, PlayerFailsTest.Length - 1))]);
            Audio_Queue((Mathf.RoundToInt(Random.Range(0f, 1f)) == 1) ? PlayerWrong1 : PlayerWrong2);
            Audio_Queue(PlayerRetryPrompt);
            TestingDelay = 7f;
            PlayerAttempts++;
            BaseAnim.speed = 1f;
        }
        else
        {
            audioDevice.Stop();
            MainAudioMan.ClearQueue();
            bool IsFirstTry = PlayerAttempts == 0;
            int Num = Mathf.RoundToInt(Random.Range(0f, PlayerRight.Length - (IsFirstTry ? 0 : 1)));
            Audio_Queue((Num == PlayerRight.Length) ? PlayerQuicklyRight : PlayerRight[Num]);
            Audio_Queue(PlayerRight2[Mathf.RoundToInt(Random.Range(0f, PlayerRight2.Length - 1))]);
            TestCooldown = 30f;
            PlayerAttempts = 0;
            BaseAnim.speed = 1f;
            if (MultiplayerManager.isActive)
            {
                ClientSend.SendCharacterState("Doctor Reflex", 8);
            }
            Wander();
        }
        Audio_PlayOneShot(SFX_Boink);
    }

    public void IStepAway()
    {
        // Reset collision
        TurnOffCollision(false);
        Hands.SetActive(false);
        coolDown = 1f;
        PlayerAttempts = 0;
        BaseAnim.speed = 1f;
        HammerAudio = 0;
        HammeringDelay = 0f;
        HasSlowRotation = false;
        WantsTheHammer = false;
        HasTheHammer = false;
        IsTesting = false;
        Angry = true;
        MainAudioMan.ClearQueue();
        audioDevice.Stop();
        if (gc.CM != null && gc.CM.isActiveAndEnabled)
        {
            gc.CM.DestroyAllCaptions(false);
        }
        Audio_PlayOneShotWC(PlayerLeaves);
        SetActiveSprite(2);
        Anim_SetBool("Running", false);
        Anim_SetTrigger("UhOh");
        LastSeen = player.position;
        Invoke("GETTHEHAMMER", PlayerLeaves.length);
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            ClientSend.SendCharacterState("Doctor Reflex", 3);
        }
    }

    public void GetInstantAngry()
    {
        if (MultiplayerManager.isActive) return;
        // Reset collision
        TurnOffCollision(false);
        Hands.SetActive(false);
        coolDown = 1f;
        PlayerAttempts = 0;
        BaseAnim.speed = 1f;
        bool ShouldRun = HasSlowRotation;
        HasSlowRotation = false;
        WantsTheHammer = false;
        HasTheHammer = true;
        ReturningHammer = false;
        HasHammer = false;
        IsTesting = false;
        Angry = true;
        MainAudioMan.ClearQueue();
        audioDevice.Stop();
        SetActiveSprite(2);
        Anim_SetTrigger("UhOh");
        if (ShouldRun)
        {
            Anim_SetBool("Running", true);
        }
        Anim_SetBool("HasHammer", true);
    }

    private void GETTHEHAMMER()
    {
        Anim_SetBool("Running", true);
        WantsTheHammer = true;
        currentSpeed = 30f;
    }

    private int _LastSprite;

    private void SetActiveSprite(int sprite)
    {
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            if (_LastSprite != sprite)
                ClientSend.SendCharacterState("Doctor Reflex", sprite - 3);
            _LastSprite = sprite;
        }
        if (sprite <= 1)
        {
            if (!Sprites[sprite].activeSelf)
            {
                Sprites[sprite].SetActive(true);
                for (int i = 0; i < Sprites.Length; i++)
                {
                    if (i != sprite) Sprites[i].SetActive(false);
                }
            }
            return;
        }
        Sprites[0].SetActive(false);
        Sprites[1].SetActive(false);
        Sprites[2].SetActive(true);
    }

    public void PutBackHammer()
    {
        Angry = false;
        WantsTheHammer = false;
        HasTheHammer = false;
        ChasingPlayer = false;
        IsTesting = false;
        Anim_SetBool("BAM", false);
        Anim_SetBool("Running", false);
        Anim_SetBool("HasHammer", false);
        Anim_SetTrigger("GoIdle");
        coolDown = 0.2f;
        HasHammer = false;
        ReturningHammer = false;
        ChasingTheHammer = false;
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            ClientSend.SendCharacterState("Doctor Reflex", 2);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (ChasingTheHammer && ChasingPlayer && other.name == "Player" && GameManager.HasHammer() && !IsFlatten)
        {
            GameManager.TakeAwayHammer();
            HasHammer = true;
            ReturningHammer = true;
            ChasingTheHammer = false;
            ChasingPlayer = false;
            HasSlowRotation = false;
            coolDown = 0f;
            // Audio queue to be aware that the hammer has been taken off the player
            int num2 = Mathf.RoundToInt(Random.Range(0f, IdleSounds.Length - 1));
            Audio_Queue(IdleSounds[num2]);
        }
        if (!HasTheHammer) return;
        if (other.GetComponent<NPC>() && other.GetComponent<NPC>().FlattenDuration <= 0f && FlattenDuration <= 0f)
        {
            other.GetComponent<NPC>().FlattenDuration = 30f;
            HammeringDelay = 1f;
            Anim_SetBool("BAM", true);
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
            {
                ClientSend.SendActionToOtherPlayer("squish:" + other.name);
            }
        }
        if (other.name == "Its a Bully" && other.GetComponent<BullyScript>() && other.GetComponent<BullyScript>().FlattenDuration <= 0f && FlattenDuration <= 0f)
        {
            other.GetComponent<BullyScript>().SetFlatDuration(30f);
            HammeringDelay = 1f;
            Anim_SetBool("BAM", true);
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
            {
                ClientSend.SendActionToOtherPlayer("squish:Its a Bully");
            }
        }
        if (MultiplayerManager.isActive)
        {
            if (!MultiplayerManager.instance.isBaldi && other.GetComponent<PlayerScript>() && !other.GetComponent<PlayerScript>().IsFlat && !other.isTrigger)
            {
                other.GetComponent<PlayerScript>().SetFlatDuration(30f);
                HammeringDelay = 1f;
                if (targetPlayer == player || targetPlayer == null)
                {
                    TestCooldown = 35f; // We have to make sure the game doesn't bug.
                    Invoke("HappyDelay", 1f);
                    ClientSend.SendCharacterState("Doctor Reflex", 6);
                }
                Anim_SetBool("BAM", true);
                ClientSend.SendActionToOtherPlayer("squish_player:" + PhotonNetwork.LocalPlayer.ActorNumber);
                ClientSend.SendCharacterState("Doctor Reflex", 5);
            }
        }
        else if (other.GetComponent<PlayerScript>() && !other.GetComponent<PlayerScript>().IsFlat && !other.isTrigger)
        {
            if (!IsFlatten) other.GetComponent<PlayerScript>().SetFlatDuration(30f);
            HammeringDelay = 1f;
            if (!IsFlatten) Invoke("HappyDelay", 1f);
            Anim_SetBool("BAM", true);
        }
    }

    // Hammer Sounds whenever he hits a trigger
    private void OnTriggerEnter(Collider other)
    {
        if (!HasTheHammer) return;
        if (other.tag == "NPC") return;
        if (other.name == "Player") return;
        if (other.name == "Its a Bully") return;
        if (!other.isTrigger) return;
        if (HammeringDelay > 0f) return;
        Anim_SetTrigger("SWAH");
        Audio_PlayOneShot(SFX_Hammer);
    }

    private void HappyDelay()
    {
        Angry = false;
        WantsTheHammer = false;
        HasTheHammer = false;
        ChasingPlayer = false;
        IsTesting = false;
        Anim_SetBool("BAM", false);
        Anim_SetBool("Running", false);
        Anim_SetBool("HasHammer", false);
        Anim_SetTrigger("GoIdle");
        audioDevice.PlayOneShotWC(CalmAtPlayer);
        coolDown = CalmAtPlayer.length;
        Invoke("HappyDelay2", CalmAtPlayer.length);
        HasHammer = true;
        ReturningHammer = true;
        ChasingTheHammer = false;
        TestCooldown = 35f;
    }

    private void HappyDelay2()
    {
        currentSpeed = 10f;
        HasSlowRotation = true;
        base.Speed = currentSpeed;
    }

    public void Audio_PlayOneShotWC(AudioClip audio, bool AllowSoundPlayingIfNoMain = false) => Audio_PlayOneShot(audio, AllowSoundPlayingIfNoMain);

    public void Audio_PlayOneShot(AudioClip audio, bool AllowSoundPlayingIfNoMain = false)
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer && !AllowSoundPlayingIfNoMain) return;
        audioDevice.PlayOneShotWC(audio);
        if (MultiplayerManager.isActive) ClientSend.PlaySoundClip(gameObject.name, audio.name);
    }


    public void Audio_Queue(AudioClip audio)
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer) return;
        MainAudioMan.Queue(audio);
    }

    public void Anim_SetBool(string name, bool value)
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer) return;
        BaseAnim.SetBool(name, value);
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            object[] Params = new object[] {
                "Doctor Reflex",
                1,
                name,
                value
            };
            Photon.Realtime.RaiseEventOptions raiseEventOptions = new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(6, Params, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
        }
    }

    public bool Anim_GetBool(string name)
    {
        return BaseAnim.GetBool(name);
    }

    public void Anim_SetTrigger(string name)
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer) return;
        BaseAnim.SetTrigger(name);
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            object[] Params = new object[] {
                "Doctor Reflex",
                0,
                name,
                null
            };
            Photon.Realtime.RaiseEventOptions raiseEventOptions = new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(6, Params, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
        }
    }
}