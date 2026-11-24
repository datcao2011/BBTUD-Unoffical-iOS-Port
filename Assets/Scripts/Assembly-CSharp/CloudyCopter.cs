using UnityEngine;
using System.Collections;

public class CloudyCopter : NPC
{
    private GameControllerScript gc;

    public AudioSource audioSource;
    public AudioClip blowing;
    public AudioClip pah;

    public float timer;
    public float cooldown;
    public bool isBlowing;
    public bool findingAreaToBlow;
    public CC_Trigger cc_Trigger;
    public Transform[] CloudyCopterTriggers;
    public Vector3 area;
    [HideInInspector]
    public Vector3 spawnArea;
    private float idleTime;

    private void Start()
    {
        if (MultiplayerManager.isActive) { gameObject.SetActive(false); return; }
        if (BirthdayBash.Instance != null) { gameObject.SetActive(false); return; }
        spawnArea = transform.position;
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ACharacters : null;
            gameObject.AddComponent<AudioSourceSpeed>();
        }
        gc = FindObjectOfType<GameControllerScript>();
        cooldown = 30f;
    }

    protected override void Update()
    {
        base.Update();
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer) return;
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime * 2f;
        }
        else
        {
            if (!isBlowing)
            {
                if (!findingAreaToBlow)
                {
                    findingAreaToBlow = true;
                    FindAreaToBlow();
                }
                else
                {
                    if (MyAgent.velocity.magnitude < 0.1f)
                    {
                        idleTime += Time.deltaTime;
                        if (idleTime >= 1f)
                        {
                            FleeTo(spawnArea);
                            findingAreaToBlow = false;
                            idleTime = 0f;
                        }
                    }
                    else
                    {
                        idleTime = 0f;
                    }
                }
            }
            else
            {
                if (timer > 0f)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    cc_Trigger.WindManager.SetEnabled(false);
                    FleeTo(spawnArea);
                    isBlowing = false;
                    audioSource.Stop();
                }
            }
        }
    }

    protected override void OnPartyHaveToAttend()
    {
        if (isBlowing)
        {
            Halt();
        }
        AttendParty();
    }

    protected override void OnFlatten()
    {
        if (isBlowing) Halt();
    }

    protected override void OnUnflatten()
    {
        if (isBlowing && cc_Trigger != null)
        {
            cc_Trigger.WindManager.SetEnabled(true);
        }
    }

    public void Halt()
    {
        cc_Trigger.WindManager.SetEnabled(false);
        cooldown = 45f;
        isBlowing = false;
        timer = 0f;
        audioSource.Stop();
        audioSource.PlayOneShotWC(pah);
    }

    private void FindAreaToBlow()
    {
        if (AttendingParty)
        {
            cooldown = 2f;
            findingAreaToBlow = false;
            if (!OnPartyHavingFun)
            {
                AttendParty();
            }
            return;
        }
        Vector3 destination = CloudyCopterTriggers[Mathf.RoundToInt(Random.Range(0f, (float)CloudyCopterTriggers.Length - 1f))].position;
        area = destination;
        SetTarget(destination);
    }

    // Unneccessary function
    // public void Flee(Vector3 position)
    // {
    //     cooldown = 15f;
    //     FleeFromTarget(position);
    // }

    public void FleeTo(Vector3 position)
    {
        if (AttendingParty)
        {
            cooldown = 2f;
            findingAreaToBlow = false;
            if (!OnPartyHavingFun)
            {
                AttendParty();
            }
            return;
        }
        cooldown = 15f;
        SetTarget(position);
    }


    public void Blow()
    {
        timer = Random.Range(15f, 30f);
        isBlowing = true;
        findingAreaToBlow = false;
        idleTime = 0f;
        audioSource.clip = blowing;
        audioSource.loop = true;
        audioSource.Play();
    }
}