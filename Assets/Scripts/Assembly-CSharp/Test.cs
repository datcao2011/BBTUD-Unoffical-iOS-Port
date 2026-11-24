/*
NOTE: 
THIS SCRIPT MAY NOT FUNCTION CORRECTLY BECAUSE THIS SCRIPT
USES BBTUD SCRIPTS TO MAKE THIS HAPPEN.
*/

using System.Collections;
using UnityEngine;

public class Test : NPC
{
    public SpriteRenderer spriteRenderer; // I have two sprite renderers. The second renderer is a child object of this renderer. 

    private Transform player;
    private float normalSpeed = 20f;
    private float speed = 1000f;
    private bool activated;
    private bool activating;
    public bool blinding;
    [HideInInspector]
    public AudioSource audioSource;

    [HideInInspector]
    public Animator animator;

    public AudioClip audActivate;
    public AudioClip audLoop;
    public AudioClip audSighted;
    public AudioClip audBlindStart;
    public AudioClip audBlindLoop;

    public FogScript fog;

    private bool sightedPlayer;
    private bool wandering;
    private AILocationSelectorScript AILocationSelector;

    // private UnityEngine.AI.NavMeshAgent MyAgent; // Uncomment this if you want to put this code in your game.

    // Start is called before the first frame update
    private void Start()
    {
        if (MultiplayerManager.isActive) { gameObject.SetActive(false); return; }
        if (BirthdayBash.Instance != null) { gameObject.SetActive(false); return; }
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerScript>().transform;
        AILocationSelector = FindObjectOfType<AILocationSelectorScript>();
        // MyAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        wandering = true;
        Wander();
    }

    // Update is called once per frame
    protected override void Update() // Change to private void if you are putting this code in your game.
    {
        base.Update();

        if (wandering)
        {
            MyAgent.isStopped = false;
            base.Speed = normalSpeed;
            if (MyAgent.velocity.magnitude <= 0.1f)
            {
                Wander();
            }
        }
        else if (!forcedToStop) MyAgent.isStopped = !activated;
    }

    // private bool m_forcedToStop;

    // public bool forcedToStop
    // {
    //     get
    //     {
    //         return m_forcedToStop;
    //     }
    //     set
    //     {
    //         UnityEngine.AI.NavMeshAgent[] NPCs = FindObjectsOfType<UnityEngine.AI.NavMeshAgent>();
    //         foreach (UnityEngine.AI.NavMeshAgent NPC in NPCs)
    //         {
    //             NPC.isStopped = value; // NOTE: This may not work 100% to characters.
    //         }
    //         m_forcedToStop = value;
    //     }
    // }

    // private void SetTarget(Vector3 position)
    // {
    //     MyAgent.SetDestination(position);
    // }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (player == null)
        {
            player = FindObjectOfType<PlayerScript>().transform;
        }
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        if (!(IsInSameFloor() && !this.GameManager.player.isInvisible && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player"))
        {
            sightedPlayer = false;
            if (!wandering)
            {
                base.Speed = 0f;
            }
            if (forcedToStop)
            {
                StopNPCs(false);
            }
            return;
        }
        if (!activated)
        {
            wandering = false;
            if (!activating && (spriteRenderer.isVisible & !blinding))
            {
                StartCoroutine(Activate());
            }
            base.Speed = 0f;
            return;
        }
        SetTarget(player.position);
        if (!spriteRenderer.isVisible)
        {
            if (forcedToStop)
            {
                StopNPCs(false);
            }
            base.Speed = speed;
        }
        else if (!sightedPlayer)
        {
            if (!forcedToStop)
            {
                StopNPCs(true);
            }
            base.Speed = 0f;
        }
        sightedPlayer = true;
        // Some code bug
        // Vector3 direction = player.position - base.transform.position;
        // RaycastHit raycastHit;
        // if (IsInSameFloor() && !GameManager.player.isInvisible && Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player") // if the test sees the player
        // {
        //     if (!activated)
        //     {
        //         wandering = false;
        //         if (!activating && spriteRenderer.isVisible & !blinding & !IsFlatten)
        //         {
        //             StartCoroutine(Activate());
        //         }
        //         base.Speed = 0f;
        //         return;
        //     }
        //     ClearTargetsAndSetTarget(player.position);
        //     if (!spriteRenderer.isVisible) // if the player sees the test
        //     {
        //         if (forcedToStop) StopNPCs(false);
        //         base.Speed = speed;
        //     }
        //     else if (!sightedPlayer)
        //     {
        //         if (!forcedToStop) StopNPCs(true);
        //         base.Speed = 0f;
        //     }
        //     sightedPlayer = true;
        // }
        // else
        // {
        //     sightedPlayer = false;
        //     if (!wandering) base.Speed = 0f;
        //     if (forcedToStop) StopNPCs(false);
        // }
    }

    private void StopNPCs(bool value)
    {
        NPC.StopAllNPCs(value);
        if (value)
        {
            StartCoroutine("Audio_Spotted");
        }
        else
        {
            StopAllCoroutines();
            audioSource.Stop();
            speed += 1000f;
            if (speed > 10000f)
            {
                speed = 10000f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Player") return;
        if (other.isTrigger) return;
        if (!activated) return;
        if (blinding) return;
        if (IsFlatten) return;
        activated = false;
        blinding = true;
        spriteRenderer.enabled = false;
        spriteRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        fog.gc.TestDuration = 120f;
        fog.TheTest_BlindPlayer();
        animator.SetTrigger("normal");
        StartCoroutine(BlindTimer());
    }

    private IEnumerator BlindTimer()
    {
        StartCoroutine(Audio_FogStart());
        yield return new WaitForSeconds(120f);
        animator.SetTrigger("normal");
        spriteRenderer.enabled = true;
        OnDisable();
        blinding = false;

    }

    private IEnumerator Activate()
    {
        if (activating) yield break;
        activating = true;
        StartCoroutine("Audio_Activate");
        animator.SetTrigger("activate");
        yield return new WaitForSeconds(5f);
        activating = false;
        activated = true;
        yield break;
    }

    private IEnumerator Audio_Activate()
    {
        audioSource.PlayOneShot(audActivate);
        audioSource.clip = audLoop;
        audioSource.loop = true;
        yield return new WaitForSecondsRealtime(audActivate.length / Time.timeScale);
        audioSource.Play();
        yield break;
    }

    private IEnumerator Audio_FogStart()
    {
        audioSource.PlayOneShot(audBlindStart);
        audioSource.clip = audBlindLoop;
        audioSource.loop = true;
        yield return new WaitForSecondsRealtime(audBlindStart.length / Time.timeScale);
        audioSource.Play();
        yield break;
    }

    protected override void OnFlatten()
    {
        if (blinding) OnDisable();
    }

    private IEnumerator Audio_Spotted()
    {
        audioSource.PlayOneShot(audSighted);
        audioSource.clip = audLoop;
        audioSource.loop = true;
        yield return new WaitForSecondsRealtime(audSighted.length / Time.timeScale);
        audioSource.Play();
        yield break;
    }

    private void Wander()
    {
        AILocationSelectorScript AILocationSelector = FindObjectOfType<AILocationSelectorScript>();
        AILocationSelector.GetNewTargetTheTest();
        SetTarget(AILocationSelector.transform.position);
    }

    private void OnDisable()
    {
        if (AILocationSelector == null) return;
        StopAllCoroutines();
        spriteRenderer.enabled = true;
        spriteRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

        AILocationSelector.GetNewTargetTheTest();

        blinding = false;
        activated = false;
        activating = false;
        wandering = true;
        audioSource.Stop();
        fog.TheTest_StopBlinding();
    }
}
