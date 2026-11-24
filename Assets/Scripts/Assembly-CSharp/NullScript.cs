using System.Collections;
using UnityEngine;
using Platforms;
using UnityEngine.AI;

public class NullScript : NPC
{
    public Transform player;

    public AudioClip GameOver;

    public AudioSource audioDevice;

    public bool IsHit;

    public float coolDown;

    public Sprite norm;

    public Sprite hit;

    public SpriteRenderer sr;

    public GameControllerScript gc;

    public bool HasBossStarted;

    public float baseTime;

    // Token: 0x04000130 RID: 304
    public float speed;

    // Token: 0x04000131 RID: 305
    public float timeToMove;

    // Token: 0x04000132 RID: 306
    public float baldiAnger;

    // Token: 0x04000133 RID: 307
    public float baldiTempAnger;

    // Token: 0x04000134 RID: 308
    public float baldiWait;

    // Token: 0x04000135 RID: 309
    public float baldiSpeedScale;

    // Token: 0x04000136 RID: 310
    private float moveFrames;

    // Token: 0x04000138 RID: 312
    public bool antiHearing;

    public float currentPriority;

    public bool wandering;

    public BaldiInidcatorScript BaldiInidcator;

    // Token: 0x04000139 RID: 313
    public float antiHearingTime;

    private bool inFlood;

    private Vector3 previous;

    public int NullRunFrom;

    public bool targetPlayer = true;

    private float TempSpeed;
    private float TempDuration;

    public Vector3 NULLRunTo = new Vector3(10f, 0f, 170f);

    //public RandomEvent SchoolEventSystem;

    [Header("NULL")]

    public AudioClip Null_Hit;
    public AudioClip Null_Regret;
    public AudioClip Null_Start;
    public AudioClip Null_Loop;
    public AudioClip Null_CheatingHaha;

    public AudioClip[] Null_Phrases;

    public Material shader;

    public bool IsMonologuing;

    private float TimeToSpeak = 10f;
    private float GameTimer;

    private AudioQueueScript Audio;

    public bool PreV3_0_3Changes;

#if UNITY_ANDROID
    public static bool EasyMode;
#endif

    private void Start()
    {
        TargetPlayer();
        FrameDependentSpeed = true;
        shader.SetColor("_Color", Color.white);
        shader.SetFloat("_Cutoff", 0.5f);
        Attempts++;
        Audio = gameObject.AddComponent<AudioQueueScript>();
        if (PreV3_0_3Changes)
        {
            speed = 150f;
        }
    }

    public void OnLastExit()
    {
        audioDevice.Stop();
        Destroy(Audio);
    }

    public void ReachedExit()
    {
        forcedToStop = true;
        IsMonologuing = true;
        FrameDependentSpeed = false;
        NullRunFrom = 4;
        FindObjectOfType<RandomEvent>().StopEventNOW();
        FindObjectOfType<RandomEvent>().PauseRandomEvents();
        NullManager.Instance.Music.gameObject.SetActive(true);
        audioDevice.PlayOneShotWC(Null_Start);
        ScheduleCaptions();
        gc.player.walkSpeed = 15f;
        gc.player.runSpeed = 15f;
        gc.player.staminaRate = 0f;
        gc.player.stamina = -5f;
        gc.LoseItem(0);
        gc.LoseItem(1);
        gc.LoseItem(2);
        gc.LoseItem(3);
        gc.LoseItem(4);
        gc.LoseItem(5);
        foreach (PickupScript Pickup in FindObjectsOfType<PickupScript>())
        {
            Pickup.gameObject.SetActive(false);
        }
        NullManager.Instance.ProjectileSpawnr.maxObjects = 99;
        GameManager.gameObject.AddComponent<DisableSaveSystem>();
        if (InputManager.usingTouch && GameObject.Find("SaveButton")) GameObject.Find("SaveButton").SetActive(false);
    }

    private void ScheduleCaptions()
    {
        if (gc.CM && gc.CM.isActiveAndEnabled)
        {
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_1", 2.8f, 0f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_2", 3.2f, 2.8f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_3", 3f, 6f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_4", 5.34f, 9f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_5", 4.16f, 14.34f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_6", 2.4f, 18.5f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_7", 4.1f, 20.9f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_8", 4.2f, 25f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_9", 4.78f, 29.2f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_10", 3.82f, 33.98f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_11", 1.64f, 37.8f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_12", 4f, 39.44f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_13", 2.04f, 43.44f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_14", 2.49f, 45.48f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_15", 1.41f, 47.97f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_16", 1.22f, 49.4f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_17", 3.72f, 50.6f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_18", 3.56f, 54.3f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_19", 4.8f, 57.88f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_20", 1.84f, 62.68f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_21", 2.08f, 64.52f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_22", 6.96f, 66.6f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_23", 4.16f, 73.56f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_24", 3.48f, 77.72f);
            gc.CM.CreateCustomCaption(audioDevice, "Null_PreBoss_Intro_25", 4.3f, 81.2f);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (HasBossStarted)
        {
            if (this.coolDown > 0f)
            {
                this.coolDown -= 1f * Time.deltaTime;
            }
            else if (targetPlayer)
            {
                TargetPlayer();
            }
            else
            {
                ClearTargetsAndSetTarget(NULLRunTo);
            }
            if (IsMonologuing && !audioDevice.loop && !audioDevice.isPlaying)
            {
                audioDevice.clip = Null_Loop;
                audioDevice.loop = true;
                audioDevice.Play();
            }
            if (!NullManager.Instance.BossFight && !forcedToStop)
            {
                TempDuration += Time.deltaTime;
                if (TempDuration >= 3f)
                {
                    TempSpeed = 150f;
                }
                else
                {
                    TempSpeed = 30f;
                }
                base.Speed = TempSpeed;
            }
        }
        else
        {
            if (NullRunFrom == 1)
            {
                ClearTargetsAndSetTarget(NULLRunTo);
            }
            // if (SchoolEventSystem.principalBreak)
            // {
            //     ClearTargetsAndSetTarget(NULLRunTo);
            // }
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
            if (this.timeToMove > 0f)
            {
                this.timeToMove -= 1f * Time.deltaTime;
            }
            else
            {
                this.Move();
            }
            if (TimeToSpeak > 0f)
            {
                TimeToSpeak -= Time.deltaTime;
            }
            else
            {
                TimeToSpeak = 10f;
                if (Random.Range(0f, 1f) <= 1f) // Test
                {
                    GenericPhrase();
                }
                if (DoesntHaveAnyItem() && gc.player.stamina <= 0f && Vector3.Distance(transform.position, player.position) <= 25f && baldiAnger >= 1.2f)
                {
                    NothingPhrase();
                }
            }
            GameTimer += Time.deltaTime;
        }
    }

    public void Hit()
    {
        audioDevice.Stop();
        audioDevice.clip = null;
        audioDevice.loop = false;
        IsMonologuing = false;
        if (gc.CM != null)
        {
            gc.CM.DestroyAllCaptions(false);
        }
        NullManager.Instance.KillAllowance = false;
        this.IsHit = true;
        forcedToStop = true;
        audioDevice.PlayOneShotWC(Null_Hit);
        NullManager.Instance.NullHit();
        NullManager.Instance.Music.Drums(Null_Hit.length);
        StartCoroutine(AHHH());
        StartCoroutine(HadHit());
    }

    private float HasHitTime = 0f;

    private IEnumerator HadHit()
    {
        if (NullManager.Instance.health == 9) yield break;
        gc.exitCount.text = NullManager.Instance.health + "/10 Hits";
        if (HasHitTime > 0f)
        {
            HasHitTime = Null_Hit.length;
            base.Speed += 5f;
            yield break;
        }
        HasHitTime = Null_Hit.length;
        while (HasHitTime > 0f)
        {
            HasHitTime -= Time.unscaledDeltaTime;
            yield return null;
        }
        shader.SetColor("_Color", Color.white);
        shader.SetFloat("_Cutoff", 0.5f);
        base.Speed += 5f;
        NullManager.Instance.KillAllowance = true;
        forcedToStop = false;
        yield break;
    }

    private IEnumerator AHHH()
    {
        float Duration = Null_Hit.length;
        float NextTint = 0f;
        while (Duration > 0f)
        {
            Duration -= Time.unscaledDeltaTime;
            if (NextTint > 0f)
            {
                NextTint -= Time.unscaledDeltaTime;
            }
            else
            {
                shader.SetColor("_Color", Random.ColorHSV());
                shader.SetFloat("_Cutoff", Random.Range(0f, 1f));
                NextTint = 0f;//0.03f;
            }
            yield return null;
        }
        yield break;
    }

    public void GetToExit(Vector3 Position)
    {
        NullRunFrom = 2;
        NULLRunTo = Position;
        HasBossStarted = true;
        base.Speed = 0f;
    }

    public void ActivateAntiHearing(float t)
    {
        BaldiInidcator.TriggerConfused();
        this.antiHearing = true;
        this.antiHearingTime = t;
        this.Wander();
    }

    private void Move()
    {
        if ((base.transform.position == this.previous || (Vector3.Distance(base.transform.position, previous) <= 2f && Vector3.Distance(base.transform.position, MyAgent.destination) <= 2f)) & this.coolDown < 0f)
        {
            this.Wander();
        }
        this.moveFrames = 10f;
        this.timeToMove = this.baldiWait - this.baldiTempAnger;
        this.previous = base.transform.position;
        if (this.gc.baldiScrpt.rumble && !(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi))
        {
            float num = Vector3.Distance(base.transform.position, this.player.position);
            if (num < 50f)
            {
                float motorLevel = 1f - num / 50f;
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

    public void Wander()
    {
        if (NullRunFrom == 1)
        {
            ClearTargetsAndSetTarget(NULLRunTo);
            return;
        }
        WanderAround(WanderType.All);
        this.coolDown = 1f;
        if (IsHearing)
        {
            DoorPhrase(0.01f);
        }
        wandering = true;
        IsHearing = false;
        this.currentPriority = 0f;
        if (!antiHearing && antiHearingTime <= 0f)
            BaldiInidcator.TriggerLost();
    }

    private bool DoesntHaveAnyItem()
    {
        return gc.item[0] == 0 && gc.item[1] == 0 && gc.item[2] == 0 && gc.item[3] == 0 && gc.item[4] == 0 && gc.item[5] == 0;
    }

    private bool IsHearing;

    private bool[] HasAudioPlayed = new bool[7];

    private float TimeSinceLastLook = 0f;

    private void DoorPhrase(float Chance)
    {
        if (!HasAudioPlayed[0] && !audioDevice.isPlaying && Random.Range(0f, 1f) <= Chance)
        {
            HasAudioPlayed[0] = true;
            PlaySoundCheck(Null_Phrases[0]);
        }
    }

    public static int Attempts = 0;

    private void GenericPhrase()
    {
        // Check if everything has already been played
        bool HasAlreadyPlayed = true;
        for (int i = 2; i < Null_Phrases.Length; i++)
        {
            if (!HasAudioPlayed[i])
            {
                HasAlreadyPlayed = false;
            }
        }
        if (HasAlreadyPlayed) return; // No need for this
        int RandomPhrase = Random.Range(2, Null_Phrases.Length);
        while (HasAudioPlayed[RandomPhrase])
        {
            RandomPhrase = Random.Range(2, Null_Phrases.Length);
        }
        switch (RandomPhrase)
        {
            case 2:
                if (TimeSinceLastLook > 0f && GameTimer >= 240f)
                {
                    HasAudioPlayed[RandomPhrase] = true;
                    PlaySoundCheck(Null_Phrases[RandomPhrase]);
                }
                break;
            case 3:
                if (Attempts >= 5 && GameTimer >= 60f)
                {
                    HasAudioPlayed[RandomPhrase] = true;
                    PlaySoundCheck(Null_Phrases[RandomPhrase]);
                }
                break;
            case 4:
                if (TimeSinceLastLook > 0f && GameTimer >= 60f)
                {
                    HasAudioPlayed[RandomPhrase] = true;
                    PlaySoundCheck(Null_Phrases[RandomPhrase]);
                }
                break;
            case 5:
                if (GameTimer >= 300f && TimeSinceLastLook >= 60f)
                {
                    HasAudioPlayed[RandomPhrase] = true;
                    PlaySoundCheck(Null_Phrases[RandomPhrase]);
                }
                break;
            case 6:
                if (GameTimer >= 300f && TimeSinceLastLook >= 60f)
                {
                    HasAudioPlayed[RandomPhrase] = true;
                    PlaySoundCheck(Null_Phrases[RandomPhrase]);
                }
                break;
        }
    }

    private void NothingPhrase()
    {
        if (!HasAudioPlayed[1] && !audioDevice.isPlaying && Random.Range(0f, 1f) <= 0.2f)
        {
            HasAudioPlayed[1] = true;
            PlaySoundCheck(Null_Phrases[1]);
        }
    }

    private void PlaySoundCheck(AudioClip clip)
    {
        if (NullRunFrom <= 0)
        {
            Audio.Queue(clip);
        }
    }

    public void Hear(Vector3 soundLocation, float priority)
    {
        if (!isActiveAndEnabled) return;
        if (NullManager.Instance.BossFight) return;
        if (!this.antiHearing && priority >= this.currentPriority)
        {
            BaldiInidcator.TriggerHear();
            wandering = false;
            IsHearing = true;
            ClearTargetsAndSetTarget(soundLocation);
            this.currentPriority = priority;
        }
        else
        {
            BaldiInidcator.TriggerConfused();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        if (targetPlayer && !gc.player.isInvisible && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
        {
            TimeSinceLastLook = 0f;
            this.TargetPlayer();
        }
        else
        {
            TimeSinceLastLook += Time.deltaTime;
        }
        if (!HasBossStarted)
        {
            if (this.moveFrames > 0f)
            {
                this.moveFrames -= 1f;
#if UNITY_ANDROID
                if (EasyMode)
                {
                    speed = 75f;
                }
#endif
                if (!inFlood)
                    base.Speed = this.speed;
                else
                    base.Speed = speed * 0.3f;
            }
            else
            {
                base.Speed = 0f;
            }
        }
    }

    public void GetAngry(float value)
    {
#if UNITY_ANDROID
        if (!EasyMode)
        {
#endif
        if (Time.unscaledDeltaTime >= 0.02f)
        {
            value *= Random.Range(1.1f, 1.5f); // Because NULL will be slower in low framerates. We have to speed up the anger.
        }
#if UNITY_ANDROID
        }
#endif
        this.baldiAnger += value * angerMultiplier;
        if (this.baldiAnger < 0.5f)
        {
            this.baldiAnger = 0.5f;
        }
        this.baldiWait = -3f * this.baldiAnger / (this.baldiAnger + 2f / this.baldiSpeedScale) + 3f;
    }

    public float angerMultiplier = 1f;

    private void TargetPlayer()
    {
        SetTarget(this.player.position);
        this.coolDown = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile" && other.GetComponent<ProjectileScript>().Thrown)
        {
            NullManager.Instance.ProjectileSpawnr.Projectiles.Remove(other.gameObject);
            NullManager.Instance.ProjectileSpawnr.objects--;
            Destroy(other.gameObject);
            Hit();
        }
        if (other.name == "Window Collider")
        {
            DoorPhrase(0.04f);
        }
        if (other.tag == "Player" && NullManager.Instance.KillAllowance && targetPlayer && !IsKilling)
        {
            IsKilling = true;
            Destroy(NullManager.Instance.Music.gameObject);
            gc.SetBuzzClip(null);
            gc.SetGameOverDelay(9.61f);
            gc.player.gameOver = true;
            gc.cameraScript.NoFade = true;
            gc.cameraScript.gameOverCharacterTarget = transform;
            gc.audioDevice.PlayOneShot(GameOver);
            StartCoroutine(KillSequence());
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

    private bool IsKilling;

    private IEnumerator KillSequence()
    {
        yield return new WaitForSecondsRealtime(0.1f); // The frame freezes when receiving the Mesh Renderers so we put a delay for enough time the Game Over screen renders.
        if (Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
        {
            StartCoroutine(WallRenderers_Optimized());
        }
        else
        {
            Renderers = FindObjectsOfType<MeshRenderer>();
            StartCoroutine(WallRenderers(0.1f));
        }
        float t = 8f;
        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime;
            yield return null;
        }
        StartCoroutine(ClassicFade());
        yield break;
    }

    private IEnumerator ClassicFade()
    {
        Camera camera = Camera.main;
        camera.backgroundColor = Color.black;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.nearClipPlane = 0.01f;
        float DelayB = 1f;
        while (DelayB > 0f)
        {
            DelayB -= Time.unscaledDeltaTime;
            camera.farClipPlane = DelayB * 400f;
            yield return null;
        }
        yield break;
    }

    private MeshRenderer[] Renderers;

    private IEnumerator WallRenderers(float speed)
    {
        float num = 0f;
        float rate = 1f;
        float Duration = 0f;
        while (Duration < 15f)
        {
            Duration += Time.unscaledDeltaTime;
            num += Time.unscaledDeltaTime * 0.25f * rate * speed;
            if (rate < 10f)
            {
                rate += Time.unscaledDeltaTime * speed;
            }
            if (rate > 10f)
            {
                rate = 10f;
            }
            this.BendMeshRenderers(num);
            yield return null;
        }
        yield break;
    }

    private IEnumerator WallRenderers_Optimized()
    {
        float Speed = 1f;
        Camera Cam = Camera.main;
        float CurrentFieldOfView = Cam.fieldOfView;
        while (CurrentFieldOfView < 1500f)
        {
            Speed += Time.unscaledDeltaTime * 2.5f;
            CurrentFieldOfView += Time.unscaledDeltaTime * Speed;
            Cam.fieldOfView = CurrentFieldOfView;
            yield return null;
        }
        yield break;
    }

    private void BendMeshRenderers(float intensity)
    {
        foreach (MeshRenderer meshRenderer in Renderers)
        {
            if (meshRenderer.name == "Black") continue;
            if (meshRenderer.GetComponent<MeshFilter>() && Vector3.Distance(gc.playerTransform.position, meshRenderer.transform.position) <= 80f)
            {
                Mesh mesh = meshRenderer.GetComponent<MeshFilter>().mesh;
                if (mesh.isReadable)
                {
                    Vector3[] vertices = mesh.vertices;
                    for (int j = 0; j < vertices.Length; j += 2)
                    {
                        vertices[j] += new Vector3(Random.Range(-1f, 1f) * intensity, Random.Range(-1f, 1f) * intensity, Random.Range(-1f, 1f) * intensity);
                    }
                    mesh.vertices = vertices;
                }
            }
        }
    }
}
