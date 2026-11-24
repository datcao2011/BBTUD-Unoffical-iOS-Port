using System;
using Multiplayer;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000028 RID: 40
public class CraftersScript : NPC
{

    public RandomEvent eventScript;

    public bool disabled;

    private float currentSpeed;

    private float capturingTime;

    private Sprite normalSprite;

    [SerializeField]
    private Vector3 BaldiPosition = new Vector3(5f, 1.64f, 125f);

    // Token: 0x060000C3 RID: 195 RVA: 0x00006B69 File Offset: 0x00004F69
    private void Start()
    {
        this.agent = base.GetComponent<NavMeshAgent>();
        currentSpeed = Speed;
        this.audioDevice = base.GetComponent<AudioSource>();
        normalSprite = spriteImage.sprite;
        this.sprite.SetActive(false);
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi)
        {
            sprite.SetActive(true);
        }
        if (NullManager.Instance != null)
        {
            baldiAgent = NullManager.Instance.NULL.getAgent();
        }
    }

    protected override void OnPartyHaveToAttend()
    {
        AttendParty();
    }

    // Token: 0x060000C4 RID: 196 RVA: 0x00006B90 File Offset: 0x00004F90
    protected override void Update()
    {
        base.Update();
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer)
        {
            if (MultiplayerManager.instance.isBaldi)
            {
                sprite.SetActive(true);
                if (angry)
                {
                    if (!audioDevice.isPlaying)
                    {
                        audioDevice.clip = this.aud_Loop;
                        audioDevice.loop = true;
                        audioDevice.Play();
                    }
                }
                return;
            }
            if (this.gettingAngry)
            {
                this.anger += Time.deltaTime;
                if (this.anger >= 1f & !this.angry & gc.scene == InputManager.IsSchoolScene() & gc.notebooks >= gc.maxNotebooks)
                {
                    this.angry = true;
                    this.audioDevice.PlayOneShotWC(this.aud_Intro);
                    this.spriteImage.sprite = this.angrySprite;
                    if (MultiplayerManager.isActive)
                    {
                        ClientSend.SendActionToOtherPlayer("crafters.summon");
                    }
                }
            }
            else if (this.anger > 0f)
            {
                this.anger -= Time.deltaTime;
            }
            if (angry)
            {
                sprite.SetActive(true);
                if (!audioDevice.isPlaying)
                {
                    audioDevice.clip = this.aud_Loop;
                    audioDevice.loop = true;
                    audioDevice.Play();
                }
            }
            else
            {
                if (((MP_Position - transform.position).magnitude <= 1f && ((base.transform.position - this.player.position).magnitude >= 60f)) || this.forceShowTime > 0f)
                {
                    this.sprite.SetActive(true);
                }
                else
                {
                    this.sprite.SetActive(false);
                }
            }
            return;
        }
        if (eventScript.party)
        {
            sprite.SetActive(true);
            if (!OnPartyHavingFun)
            {
                AttendParty();
            }
            return;
        }
        if (this.forceShowTime > 0f)
        {
            this.forceShowTime -= Time.deltaTime;
        }
        if (this.gettingAngry)
        {
            this.anger += Time.deltaTime;
            if ((this.anger >= 1f & !this.angry & gc.scene == InputManager.IsSchoolScene() & gc.notebooks >= gc.maxNotebooks) || (this.anger >= 10f & !this.angry & gc.scene == "CampScene") || (this.anger >= 20f & !this.angry & gc.scene == InputManager.IsSchoolScene() & gc.notebooks < 7))
            {
                this.angry = true;
                this.audioDevice.PlayOneShotWC(this.aud_Intro);
                this.spriteImage.sprite = this.angrySprite;
                if (MultiplayerManager.isActive)
                {
                    ClientSend.SendActionToOtherPlayer("crafters.summon");
                }
            }
        }
        else if (this.anger > 0f)
        {
            this.anger -= Time.deltaTime;
        }
        if (!this.angry)
        {
            if (((base.transform.position - this.agent.destination).magnitude <= 20f & ((base.transform.position - this.player.position).magnitude >= 60f || player.GetComponent<Collider>().isTrigger)) || this.forceShowTime > 0f || gc.scene == "CampScene")
            {
                this.sprite.SetActive(true);
            }
            else
            {
                this.sprite.SetActive(false);
            }
        }
        else
        {
            Speed = Speed + 60f * Time.deltaTime;
            this.TargetPlayer();
            if (!this.audioDevice.isPlaying)
            {
                this.audioDevice.clip = this.aud_Loop;
                audioDevice.loop = true;
                audioDevice.Play();
            }
        }
        if (gc.notebooks >= 5 & gc.notebooks < 7)
        {
            forceShowTime = 1.1f;
        }
    }

    public void GetAngryCreppy()
    {
        this.angry = true;
        audioDevice.clip = gc.aud_MachineLoop;
        audioDevice.loop = true;
        audioDevice.Play();
        this.spriteImage.sprite = this.angrySprite;
    }

    // Token: 0x060000C5 RID: 197 RVA: 0x00006D34 File Offset: 0x00005134
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (angry)
        {
            capturingTime += Time.fixedDeltaTime;
        }
        if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
        {
            if (gc.scene == InputManager.IsSchoolScene() && capturingTime >= 15f)
            {
                capturingTime = 0f;
                agent.Warp(base.transform.position - base.transform.forward * 10f);
            }
        }
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        bool CurrentlyAiming = this.sprite.activeSelf && !GameManager.player.isInvisible && GameManager.player.NoFacultyAlert <= 0f && this.craftersRenderer.isVisible && Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player";
        if (gc.scene == InputManager.IsSchoolScene())
        {
            if (CurrentlyAiming && IsInSameFloor() & !IsFlatten)
            {
                if (!MultiplayerManager.isActive || !MultiplayerManager.instance.isBaldi)
                    this.gettingAngry = true;
            }
            else
            {
                this.gettingAngry = false;
            }
            return;
        }
        if (gc.scene == "CampScene" & playerScript.detected)
        {
            if (!angry)
            {
                this.gettingAngry = true;
            }
        }
    }

    // Token: 0x060000C6 RID: 198 RVA: 0x00006DE9 File Offset: 0x000051E9
    public void GiveLocation(Vector3 location, bool flee)
    {
        if (!this.angry)
        {
            SetTarget(location);
            if (flee)
            {
                this.forceShowTime = 3f;
            }
        }
    }

    // Token: 0x060000C7 RID: 199 RVA: 0x00006E14 File Offset: 0x00005214
    private void TargetPlayer()
    {
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            if (gc.SpectatingDisplay.activeSelf)
            {
                if (gc.notebooks < gc.maxNotebooks)
                {
                    disabledCount = 6;
                }
                OnTriggerEnter(gc.playerColider);
                return;
            }
        }
        SetTarget(this.player.position);
    }

    protected override void OnFlatten()
    {
        if (gc.exitsReached < 1 || gc.baldiScrpt.position.y < 20f) // Won't become happy if Baldi is trapped upstairs.
        {
            BecomeHappy();
        }
    }

    // Token: 0x060000C8 RID: 200 RVA: 0x00006E30 File Offset: 0x00005230
    private void OnTriggerEnter(Collider other)
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer) return; // No events, crafters doesn't care about the other players.
        if (other.tag == "Player" & this.angry & !IsFlatten)
        {
            if (gc.scene == InputManager.IsSchoolScene())
            {
                if (!MultiplayerManager.isActive || !gc.SpectatingDisplay.activeSelf)
                {
                    this.player.position = new Vector3(5f, 4f, 80f);
                }
                if (baldiAgent.isActiveAndEnabled && !MultiplayerManager.isActive) this.baldiAgent.Warp(BaldiPosition);
                if (MultiplayerManager.isActive)
                {
                    if (baldiAgent.isActiveAndEnabled && MultiplayerManager.instance.isAIBaldi)
                    {
                        baldiAgent.Warp(new Vector3(5f, 1.64f, 125f));
                    }
                    Multiplayer.ClientSend.SendActionToOtherPlayer("teleport:5,1.64,125:crafters");
                }
                this.player.LookAt(new Vector3(5f, this.player.position.y, 125f));
            }
            else if (gc.scene == "CampScene")
            {
                this.player.position = new Vector3(0.01999986f, this.player.position.y, -10.31f);
                this.baldiAgent.Warp(new Vector3(0f, this.baldi.position.y, 6.29f));

                camp.clearSticks();
                this.player.LookAt(new Vector3(0f, this.player.position.y, 6.29f));
                if (!camp.Optimized)
                {
                    RenderSettings.skybox.SetFloat("_Exposure", 0.38f);
                    RenderSettings.fog = false;
                }
            }
            disabled = true;
            capturingTime = 0f;
            if (gc.notebooks >= gc.maxNotebooks || gc.scene == "CampScene")
            {
                if (gc.scene != InputManager.IsSchoolScene()) gameObject.SetActive(false);
                else
                {
                    gc.cheatingConsole.SelectTMPText(gc.enableTexts[10]);
                    gc.cheatingConsole.SetCharacterState(10, false);
                }
            }
            else
            {
                if (disabledCount > 5)
                    BecomeHappy();
                disabledCount++;
            }
        }
    }

    private void BecomeHappy()
    {
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            ClientSend.SendActionToOtherPlayer("crafters.happy");
        }
        anger = 0f;
        disabledCount = 0;
        angry = false;
        Speed = currentSpeed;
        spriteImage.sprite = normalSprite;
        audioDevice.Stop();
    }

    public void BecomeInstantAngry()
    {
        if (angry) BecomeHappy_NoEvent(); // Make sure he's happy before he becomes angry again...
        sprite.SetActive(true);
        angry = true;
        audioDevice.PlayOneShotWC(this.aud_Intro);
        spriteImage.sprite = this.angrySprite;
        if (MultiplayerManager.isActive)
        {
            ClientSend.SendActionToOtherPlayer("crafters.summon");
        }
    }

    public void MP_Angry()
    {
        if (angry) BecomeHappy_NoEvent(); // Make sure he's happy before he becomes angry again...
        sprite.SetActive(true);
        angry = true;
        audioDevice.PlayOneShotWC(this.aud_Intro);
        spriteImage.sprite = this.angrySprite;
    }

    public void BecomeHappy_NoEvent()
    {
        anger = 0f;
        disabledCount = 0;
        angry = false;
        Speed = currentSpeed;
        spriteImage.sprite = normalSprite;
        audioDevice.Stop();
    }

    // Token: 0x04000149 RID: 329
    public bool db;

    public int disabledCount;

    // Token: 0x0400014A RID: 330
    public bool angry;

    // No longer needed
    // public GameObject yourSelf;

    // Token: 0x0400014B RID: 331
    public bool gettingAngry;

    // Token: 0x0400014C RID: 332
    public float anger;

    public CampingScript camp;

    public PlayerScript playerScript;

    // Token: 0x0400014D RID: 333
    private float forceShowTime;

    // Token: 0x0400014E RID: 334
    public Transform player;

    // Token: 0x0400014F RID: 335
    public Transform playerCamera;

    // Token: 0x04000150 RID: 336
    public Transform baldi;

    // Token: 0x04000151 RID: 337
    public NavMeshAgent baldiAgent;

    // Token: 0x04000152 RID: 338
    public GameObject sprite;

    // Token: 0x04000153 RID: 339
    public GameControllerScript gc;

    // Token: 0x04000154 RID: 340
    private NavMeshAgent agent;

    // Token: 0x04000155 RID: 341
    public Renderer craftersRenderer;

    // Token: 0x04000156 RID: 342
    public SpriteRenderer spriteImage;

    // Token: 0x04000157 RID: 343
    public Sprite angrySprite;

    // Token: 0x04000158 RID: 344
    private AudioSource audioDevice;

    // Token: 0x04000159 RID: 345
    public AudioClip aud_Intro;

    // Token: 0x0400015A RID: 346
    public AudioClip aud_Loop;
}
