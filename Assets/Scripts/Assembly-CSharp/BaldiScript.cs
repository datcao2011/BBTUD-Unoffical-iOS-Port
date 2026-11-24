using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Platforms;
using SchoolSaves;

// Token: 0x02000027 RID: 39
public class BaldiScript : NPC
{
    // public bool slapping;

    private bool rulerBroken;

    public bool rumble;

    public float baldiPriority;

    public GameObject HAS_NormalBaldi;
    public GameObject HAS_CountdownBaldi;
    public GameObject sprite;

    public Slider baldiSlider;

    public Transform cameraLocation;

    private PlatPlayer inputPlayer;

    private bool slapping;

    // Token: 0x060000B8 RID: 184 RVA: 0x00006780 File Offset: 0x00004B80
    private void Start()
    {
        this.baldiAudio = base.GetComponent<AudioSource>();
        this.agent = base.GetComponent<NavMeshAgent>();
        this.timeToMove = this.baseTime;
        this.Wander();
        inputPlayer = PlatTools.GetPlatformPlayer();
        rumble = Settings.GetBool("Vibrate");
        baldiAnimator.updateMode = AnimatorUpdateMode.Normal;
        vibrationDistance = 50f;
    }

    // Token: 0x060000B9 RID: 185 RVA: 0x000067AC File Offset: 0x00004BAC
    protected override void Update()
    {
        base.Update();
        if (ps.gameOver)
        {
            baldiAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        if (ps.appleEat)
        {
            SetTarget(base.transform.position);
            currentPriority = 999999f;
            return;
        }
        else if (MultiplayerManager.isActive && !MultiplayerManager.instance.isAIBaldi)
        {
            MyAgent.SetDestination(cameraLocation.position);
        }
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi && !MultiplayerManager.instance.PauseMovement)
        {
            if (inputPlayer.GetButton("Interact"))
            {
                slapping = true;
            }
            else
            {
                slapping = false;
            }
        }
        if (sprite.activeSelf)
        {
            if (baldiSlider != null && baldiSlider.isActiveAndEnabled)
            {
                baldiSlider.value = baldiSlider.maxValue - timeToMove;
                baldiSlider.transform.parent.GetChild(4).GetComponent<TMPro.TMP_Text>().color = gameController.colored.Evaluate(baldiSlider.normalizedValue);
                baldiSlider.fillRect.GetComponent<Image>().color = gameController.colored.Evaluate(baldiSlider.normalizedValue);
                baldiSlider.transform.parent.GetChild(4).GetComponent<TMPro.TMP_Text>().text = $"{Mathf.FloorToInt(100 * baldiSlider.normalizedValue)}% [{timeToMove:0.0}]";
            }
            if (this.timeToMove > 0f)
            {
                this.timeToMove -= 1f * Time.deltaTime;
            }
            else
            {
                if (MultiplayerManager.isActive)
                {
                    if ((MultiplayerManager.instance.isBaldi && slapping) || (MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer))
                    {
                        Move();
                    }
                }
                else
                {
                    this.Move();
                }
            }
        }
        if (this.coolDown > 0f)
        {
            this.coolDown -= 1f * Time.deltaTime;
        }
        if (this.baldiTempAnger > 0f)
        {
            this.baldiTempAnger -= 0.02f * Time.deltaTime;
        }
        else
        {
            this.baldiTempAnger = 0f;
        }
        if (this.antiHearingTime > 0f)
        {
            this.antiHearingTime -= Time.deltaTime;
        }
        else
        {
            this.antiHearing = false;
        }
        if (this.endless)
        {
            if (this.timeToAnger > 0f)
            {
                this.timeToAnger -= 1f * Time.deltaTime;
            }
            else
            {
                this.timeToAnger = this.angerFrequency;
                this.GetAngry(this.angerRate);
                this.angerRate += this.angerRateRate;
            }
        }
        if (gameController.HASManager)
        {
            if (gameController.HASManager.hiderOrSeeker == "Seeker")
            {
                if (db)
                {
                    if (HAS_NormalBaldi.activeSelf)
                    {
                        HAS_NormalBaldi.SetActive(false);
                        sprite.SetActive(true);
                        timeToMove = 0.5f;
                    }
                    FleeFromPlayer();
                }
                else
                {
                    if (wandering)
                    {
                        if ((base.transform.position - Destination).magnitude <= 5f)
                        {
                            HAS_NormalBaldi.SetActive(true);
                            sprite.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                if (gameController.HASManager.seekTimer > 0f)
                {
                    if ((base.transform.position - Destination).magnitude <= 5f)
                    {
                        HAS_CountdownBaldi.SetActive(true);
                        sprite.SetActive(false);
                    }
                }
                else if (db)
                {
                    TargetPlayer();
                }
            }
        }
    }

    // Token: 0x060000BA RID: 186 RVA: 0x000068E0 File Offset: 0x00004CE0
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (this.moveFrames > 0f)
        {
            this.moveFrames -= 1f;
            if (!inFlood)
                Speed = this.speed;
            else
                Speed = speed * 0.3f;
            if (moveFrames <= 0)
            {
                PreviousVelocity = agent.velocity;
            }
        }
        else
        {
            Speed = 0f;
        }
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        if (IsInSameFloor() && !ps.isInvisible && Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
        {
            this.db = true;
            if (!gameController.HASManager) this.TargetPlayer();
        }
        else
        {
            this.db = false;
        }
    }

    // Token: 0x060000BB RID: 187 RVA: 0x000069B4 File Offset: 0x00004DB4
    public void Wander()
    {
        if (gameController.scene != InputManager.IsSchoolScene())
        {
            TargetPlayer();
            return;
        }
        if (!gameController.HASManager)
        {
            WanderAround(WanderType.All);
        }
        else
        {
            if (gameController.HASManager.hiderOrSeeker == "Hider")
            {
                if (gameController.HASManager.seekTimer > 0f)
                {
                    return;
                }
                HAS_CountdownBaldi.SetActive(false);
                sprite.SetActive(true);
                WanderAround(WanderType.All);
            }
            else
            {
                WanderAround(WanderType.Rooms);
            }
        }
        this.coolDown = 1f;
        wandering = true;
        this.currentPriority = 0f;
        if (!antiHearing & antiHearingTime <= 0f)
            BaldiInidcator.TriggerLost();
    }

    // Token: 0x060000BC RID: 188 RVA: 0x000069EE File Offset: 0x00004DEE
    public void TargetPlayer()
    {
        if (!ps.appleEat)
        {
            BaldiInidcator.TriggerFound();
            if (!MultiplayerManager.isActive)
            {
                ClearTargetsAndSetTarget(player.position);
            }
            else if (MultiplayerManager.instance.isAIBaldi)
            {
                if (MultiplayerManager.instance.isMainPlayer)
                {
                    ClearTargetsAndSetTarget(player.position);
                }
                else
                {
                    MultiplayerManager.instance.client.SendAsUDP(5, Photon.Realtime.ReceiverGroup.Others, "Baldi", player.position.ToFloat());
                }
            }
            this.coolDown = 1f;
            wandering = false;
            this.currentPriority = 0f;
        }
    }


    public void TargetAnotherPlayer(Vector3 Position)
    {
        if (!ps.appleEat)
        {
            if (MultiplayerManager.isActive || MultiplayerManager.instance.isAIBaldi) ClearTargetsAndSetTarget(Position);
            this.coolDown = 1f;
            wandering = false;
            this.currentPriority = 0f;
        }
    }

    public void FleeFromPlayer()
    {
        FleeFromTarget(player.position);
        this.coolDown = 1f;
        wandering = false;
        this.currentPriority = 0f;
    }

    public void ResetTime()
    {
        timeToMove = baseTime;
        moveFrames = 0;
        if (baldiSlider != null && baldiSlider.isActiveAndEnabled)
        {
            baldiSlider.maxValue = baseTime;
        }
    }

    // Token: 0x060000BD RID: 189 RVA: 0x00006A20 File Offset: 0x00004E20

    private bool breakingRuler;

    public bool IsAnimRulerBroken;

    public bool wandering;

    [HideInInspector]
    public Vector3 PreviousVelocity;

    private void Move()
    {
        if ((base.transform.position == this.previous || (Vector3.Distance(base.transform.position, previous) <= 2f && Vector3.Distance(base.transform.position, agent.destination) <= 2f)) & this.coolDown < 0f)
        {
            this.Wander();
        }
        this.moveFrames = 10f;
        this.timeToMove = this.baldiWait - this.baldiTempAnger;
        if (baldiSlider != null && baldiSlider.isActiveAndEnabled)
        {
            baldiSlider.maxValue = this.baldiWait - this.baldiTempAnger;
        }
        this.previous = base.transform.position;
        if (!rulerBroken)
        {
            // We don't want the caption to appear for Baldi if they're Baldi. That'll be annoying.
            // Also, HIDE & SEEK! We can't give away Baldi's Position.
            if ((MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi) || (gameController.HASManager != null && gameController.HASManager.hiderOrSeeker == "Seeker"))
            {
                baldiAudio.PlayOneShot(slap);
            }
            else
            {
                baldiAudio.PlayOneShotWC(slap);
            }
            if (MultiplayerManager.isActive && (MultiplayerManager.instance.isBaldi || (MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer))) Multiplayer.ClientSend.PlaySoundClip(gameObject.name, this.slap.name);
            this.baldiAnimator.SetTrigger("slap");
            IsAnimRulerBroken = false;
        }
        else if (!breakingRuler)
        {
            baldiAnimator.SetTrigger("brokenSlap");
            if (MultiplayerManager.isActive && (MultiplayerManager.instance.isBaldi || (MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer))) Multiplayer.ClientSend.PlaySoundClip(gameObject.name, "BAL_SlapBroken");
            IsAnimRulerBroken = true;
        }
        else
        {
            breakingRuler = false;
            baldiAnimator.SetTrigger("brokenSlap");
            if (MultiplayerManager.isActive && (MultiplayerManager.instance.isBaldi || (MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer))) Multiplayer.ClientSend.PlaySoundClip(gameObject.name, ruleBroke.name);
            baldiAudio.PlayOneShotWC(ruleBroke);
            IsAnimRulerBroken = true;
        }
        if (this.rumble && !(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi))
        {
            float num = Vector3.Distance(base.transform.position, this.player.position);
            if (num < this.vibrationDistance)
            {
                float motorLevel = 1f - num / this.vibrationDistance;
                if (motorLevel > 5f)
                {
                    motorLevel = 4.5f;
                }
                if (CurrentDevice.currentPlatform == PFPlatform.Windows && GamePadController.isAnyGamepadConnected) CurrentDevice.VibrateController(motorLevel, motorLevel, 0.2f * Time.timeScale);
#if UNITY_IOS || UNITY_ANDROID
                Handheld.Vibrate();
#endif
            }
        }
    }

    // Token: 0x060000BE RID: 190 RVA: 0x00006AAC File Offset: 0x00004EAC
    public void GetAngry(float value, bool MPSendEvent = true)
    {
        if (NullManager.Instance != null)
        {
            NullManager.Instance.NULL.GetAngry(value);
            return;
        }
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi)
        {
            if (MPSendEvent) Multiplayer.ClientSend.SendActionToOtherPlayer("anger:" + value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (!MultiplayerManager.instance.isAIBaldi) return;
        }
        this.baldiAnger += value * angerMultiplier;
        if (this.baldiAnger < 0.5f)
        {
            this.baldiAnger = 0.5f;
        }
        this.baldiWait = -3f * this.baldiAnger / (this.baldiAnger + 2f / this.baldiSpeedScale) + 3f;
    }

    public float tempAngerMultiplier = 1f;

    public float angerMultiplier = 1f;

    // Token: 0x060000BF RID: 191 RVA: 0x00006B0E File Offset: 0x00004F0E
    public void GetTempAngry(float value, bool MPSendEvent = true)
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi)
        {
            if (MPSendEvent) Multiplayer.ClientSend.SendActionToOtherPlayer("tempanger:" + value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (!MultiplayerManager.instance.isAIBaldi) return;
        }
        this.baldiTempAnger += value * tempAngerMultiplier;
    }

    public void SetLocation(Vector3 location)
    {
        if (!MultiplayerManager.isActive || !MultiplayerManager.instance.isBaldi)
        {
            SetTarget(location);
        }
        else
        {
            SetTarget(cameraLocation.position);
        }
    }

    // Token: 0x060000C0 RID: 192 RVA: 0x00006B1E File Offset: 0x00004F1E
    public void Hear(Vector3 soundLocation, float priority)
    {
        if (!isActiveAndEnabled) return;
        if (gameController.HASManager)
        {
            if (gameController.HASManager.seekTimer > 0f) return;
            if (gameController.HASManager.hiderOrSeeker == "Seeker") return;
        }
        if (!this.antiHearing && priority >= this.currentPriority)
        {
            BaldiInidcator.TriggerHear();
            wandering = false;
            if (!MultiplayerManager.isActive)
                ClearTargetsAndSetTarget(soundLocation);
            else if (MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer)
            {
                ClearTargetsAndSetTarget(soundLocation);
            }
            else if (!MultiplayerManager.instance.isBaldi)
                Multiplayer.ClientSend.SendActionToOtherPlayer("hear:" + soundLocation.x.ToString("0") + "," + soundLocation.y.ToString("0") + "," + soundLocation.z.ToString("0"));
            this.currentPriority = priority;
        }
        else
        {
            BaldiInidcator.TriggerConfused();
        }
    }

    // Token: 0x060000C1 RID: 193 RVA: 0x00006B4B File Offset: 0x00004F4B
    public void ActivateAntiHearing(float t)
    {
        BaldiInidcator.TriggerConfused();
        this.antiHearing = true;
        this.antiHearingTime = t;
        this.Wander();
    }

    public void BreakRuler(float rulerBrokenTime)
    {
        rulerBroken = true;
        breakingRuler = true;
        stopTimer = false;
        //console.log("Time set.");
        StartCoroutine(BrokenRulerTime(rulerBrokenTime));
    }

    public bool stopTimer;

    public void SetPriority(float priority, float duration)
    {
        baldiPriority = currentPriority;
        currentPriority = priority;
        StartCoroutine(Priority(duration, priority));
    }

    private IEnumerator Priority(float t, float priority)
    {
        float time = t;
        while (time > 0f)
        {
            currentPriority = priority;
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        currentPriority = baldiPriority;
        yield break;
    }

    private IEnumerator BrokenRulerTime(float t)
    {
        float time = t;
        while (time > 0f & !stopTimer)
        {
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        stopTimer = false;
        breakingRuler = false;
        rulerBroken = false;
        yield break;
    }

    public void SpoopyJumpscare()
    {
        if (MultiplayerManager.isActive) return;
        baldi.SetActive(true);
        spoopyText.SetActive(true);
        if (gameController.notebooks < 2)
        {
            gameController.notebooks = 2;
        }
        gameController.debugMode = false;
        StartCoroutine(timer());
    }

    private IEnumerator timer()
    {
        float time = 0.1f;
        while (time > 0f)
        {
            time -= Time.unscaledDeltaTime;
            yield return null;
        }
        if (!agent.Warp(player.position))
        {
            player.position = base.transform.position;
        }
        ps.gameOver = true;
        ps.cameraScript.GM_PositionHeight = FlattenDuration > 0f ? 1f : 5f;
        FindObjectOfType<CameraScript>().GetComponent<Camera>().nearClipPlane = 0.1f;
        yield break;
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

    public GameObject spoopyText;

    // Token: 0x0400012F RID: 303
    public float baseTime;

    // Token: 0x04000130 RID: 304
    public float speed;

    public AudioClip ruleBroke;

    // Token: 0x04000131 RID: 305
    public float timeToMove;

    // Token: 0x04000132 RID: 306
    public float baldiAnger;

    // Token: 0x04000133 RID: 307
    public float baldiTempAnger;

    public float vibrationDistance;

    // Token: 0x04000134 RID: 308
    public float baldiWait;

    // Token: 0x04000135 RID: 309
    public float baldiSpeedScale;

    // Token: 0x04000136 RID: 310
    private float moveFrames;

    // Token: 0x04000137 RID: 311
    public float currentPriority;

    // Token: 0x04000138 RID: 312
    public bool antiHearing;

    // Token: 0x04000139 RID: 313
    public float antiHearingTime;

    // Token: 0x0400013A RID: 314
    public float angerRate;

    // Token: 0x0400013B RID: 315
    public float angerRateRate;

    // Token: 0x0400013C RID: 316
    public float angerFrequency;

    // Token: 0x0400013D RID: 317
    public float timeToAnger;

    // Token: 0x0400013E RID: 318
    public bool endless;

    private bool inFlood;

    public PlayerScript ps;

    // Token: 0x0400013F RID: 319
    public Transform player;

    // Token: 0x04000140 RID: 320
    public Transform wanderTarget;

    // Token: 0x04000141 RID: 321
    public AILocationSelectorScript wanderer;

    // Token: 0x04000142 RID: 322
    private AudioSource baldiAudio;

    // Token: 0x04000143 RID: 323
    public AudioClip slap;

    // Token: 0x04000144 RID: 324
    public AudioClip[] speech = new AudioClip[3];

    // Token: 0x04000145 RID: 325
    public Animator baldiAnimator;

    public BaldiInidcatorScript BaldiInidcator;

    public GameControllerScript gameController;

    // Token: 0x04000146 RID: 326
    public float coolDown;

    // Token: 0x04000147 RID: 327
    private Vector3 previous;

    // Token: 0x04000148 RID: 328
    public NavMeshAgent agent;

    public GameObject baldi;
}
