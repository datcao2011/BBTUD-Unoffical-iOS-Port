using UnityEngine;
using UnityEngine.AI;
using Multiplayer;
using Unity.VisualScripting;

public class NPC : MonoBehaviour
{
    private NavMeshAgent currentAgent;

    public bool Partygoer;

    [HideInInspector]
    public bool AttendingParty;

    [HideInInspector]
    public bool OnPartyHavingFun;

    private Transform WanderTarget;

    protected GameControllerScript GameManager;
    [HideInInspector]
    public Vector3 savedPosition;

    public bool usingElevate;

    public bool preventWanderToElevate;

    private AILocationSelectorScript wandererScript;
    [HideInInspector]
    public Vector3 MP_Position;
    [HideInInspector]
    public Quaternion MP_Rotation;

    protected Vector3 MP_CurrentPos;

    protected Quaternion MP_CurrentRot;

    [HideInInspector]
    public bool forcedToStop;

    private ElevatorScript[] es;

    private Vector3 OriginalScale;
    public float FlattenDuration;
    public bool IsFlatten
    {
        get
        {
            return m_IsFlatten && !GameManager.player.IsFlat; // Actions will occur if the player and NPC is flat. Got the idea?
        }
    }

    private bool m_IsFlatten;

    private float m_Speed;

    public float Speed
    {
        get
        {
            return m_Speed;
        }
        set
        {
            m_Speed = value;
            if (m_FrameDependentSpeed)
            {
                UpdateNPCSpeed();
            }
            else
            {
                MyAgent.speed = m_Speed;
            }
        }
    }

    protected Vector3 Destination
    {
        get
        {
            if (wantsElevate || usingElevate)
            {
                return savedPosition;
            }
            return currentAgent.destination;
        }
    }

    private bool m_FrameDependentSpeed;

    // This will slow the speed in lower framerates, but in return, fixing the issue of NPCs being stuck in doors.
    protected bool FrameDependentSpeed
    {
        get
        {
            return m_FrameDependentSpeed;
        }
        set
        {
            m_FrameDependentSpeed = value;
            if (m_FrameDependentSpeed)
            {
                UpdateNPCSpeed();
            }
            else
            {
                MyAgent.speed = m_Speed;
            }
        }
    }

    protected bool IsInSameFloor()
    {
        if (GameManager.scene != "School" && GameManager.scene != "School_2") return true;
        int NPCFloor = 1;
        if (transform.position.y >= 20f)
        {
            NPCFloor = 2;
        }
        if (transform.position.y >= 40f)
        {
            NPCFloor = 3;
        }
        int PlayerFloor = 1;
        if (GameManager.playerTransform.position.y >= 20f)
        {
            PlayerFloor = 2;
        }
        if (GameManager.playerTransform.position.y >= 40f)
        {
            PlayerFloor = 3;
        }
        if (NPCFloor != PlayerFloor) return false;
        return true;
    }

    private void Awake()
    {
        es = FindObjectsOfType<ElevatorScript>();
        if (es.Length >= 2)
        {
            if (es[0].transform.position.y >= +20)
            {
                ElevatorScript es2 = es[0];
                es[0] = es[1];
                es[1] = es2;
            }
        }
        currentAgent = GetComponent<NavMeshAgent>();
        m_Speed = currentAgent.speed;
        GameManager = FindObjectOfType<GameControllerScript>();
        wandererScript = FindObjectOfType<AILocationSelectorScript>();
        WanderTarget = wandererScript.transform;
        floor = GetFloor();
        MP_Position = transform.position;
        MP_Rotation = transform.rotation;
        MP_CurrentPos = transform.position;
        OriginalScale = transform.localScale;
    }

    private int GetFloor()
    {
        if (transform.position.y >= 40f) return 3;
        if (transform.position.y >= 20f) return 2;
        return 1;
    }

    public static void StopAllNPCs(bool value)
    {
        NPC[] NPCs = Object.FindObjectsOfType<NPC>();
        foreach (NPC npc in NPCs)
        {
            npc.forcedToStop = value;
        }
    }

    protected NavMeshAgent MyAgent
    {
        get
        {
            if (currentAgent == null) currentAgent = GetComponent<NavMeshAgent>();
            return currentAgent;
        }
    }

    [HideInInspector]
    public Vector3 lPos;
    [HideInInspector]
    public Quaternion lRot = Quaternion.identity;

    protected virtual void Update()
    {
        floor = GetFloor();
        if (currentAgent.isOnNavMesh)
        {
            if (m_FrameDependentSpeed)
            {
                UpdateNPCSpeed();
            }
            if (forcedToStop && !currentAgent.isStopped)
            {
                currentAgent.isStopped = true;
            }
            if (!forcedToStop && currentAgent.isStopped)
            {
                currentAgent.isStopped = false;
            }
        }
        if (MultiplayerManager.isActive)
        {
            bool IsNamedBaldi = gameObject.name == "Baldi";
            if ((MultiplayerManager.instance.isMainPlayer && !IsNamedBaldi) || ((MultiplayerManager.instance.isBaldi || (MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer)) && IsNamedBaldi))
            {
                if (lPos != transform.position || lRot != transform.rotation)
                {
                    ClientSend.NPCMovement(gameObject.name, transform.position, transform.rotation.eulerAngles);
                    lPos = transform.position;
                    lRot = transform.rotation;
                }
            }
            else
            {
                if (transform.position != MP_Position)
                {
                    MP_CurrentPos = Vector3.Lerp(MP_CurrentPos, MP_Position, 20f * Time.unscaledDeltaTime);
                    transform.position = MP_CurrentPos;
                }
                if (IsNamedBaldi)
                {
                    if (transform.rotation != MP_Rotation)
                    {
                        MP_CurrentRot = Quaternion.Lerp(MP_CurrentRot, MP_Rotation, 20f * Time.unscaledDeltaTime);
                        transform.rotation = MP_CurrentRot;
                    }
                }
                else
                {
                    if (transform.rotation != MP_Rotation)
                    {
                        transform.rotation = MP_Rotation;
                    }
                }
            }
        }
        if (wantsElevate)
        {
            if (es[0].unavailable)
            {
                wantsElevate = false;
                WanderAround(WanderType.All);
            }
            if (GameManager.notebooks < 2)
            {
                wantsElevate = false;
                WanderAround(WanderType.All);
            }
        }
        if (!m_IsFlatten)
        {
            if (FlattenDuration > 0f)
            {
                m_IsFlatten = true;
                Vector3 Scale = OriginalScale;
                Scale.y = OriginalScale.y - 1.2f;
                transform.localScale = Scale;
                if (GetComponent<AudioSource>()) GetComponent<AudioSource>().pitch = Settings.GetFloat("TimeSpeed", 1f) + 0.75f;
                OnFlatten();
            }
        }
        else
        {
            if (FlattenDuration > 0f)
            {
                FlattenDuration -= Time.deltaTime;
            }
            else
            {
                m_IsFlatten = false;
                transform.localScale = OriginalScale;
                if (GetComponent<AudioSource>()) GetComponent<AudioSource>().pitch = Settings.GetFloat("TimeSpeed", 1f);
                OnUnflatten();
            }
        }
        if (Partygoer)
        {
            if (GameManager.cheatingConsole.schoolEventSystem.party)
            {
                if (!AttendingParty)
                {
                    AttendingParty = true;
                    OnPartyHaveToAttend();
                }
            }
            else if (AttendingParty)
            {
                AttendingParty = false;
                OnPartyHavingFun = false;
            }
        }
    }

    private void UpdateNPCSpeed()
    {
        if (Time.unscaledDeltaTime >= 0.02f)
        {
            MyAgent.speed = m_Speed * (0.02f / Time.unscaledDeltaTime);
        }
        else if (m_Speed != MyAgent.speed)
        {
            MyAgent.speed = m_Speed;
        }
    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void OnFlatten() { }

    protected virtual void OnUnflatten() { }

    protected virtual void OnPartyHaveToAttend() { }

    protected void AttendParty()
    {
        ClearTargetsAndSetTarget(GameManager.cheatingConsole.schoolEventSystem.PartyManager.transform.position);
    }

    [HideInInspector]
    public int floor;

    public bool wantsElevate;

    public void SE(Vector3 position) => SetDestination(position, true);

    public void SetTarget(Vector3 target)
    {
        if (wantsElevate || usingElevate)
        {
            if (GameManager.notebooks < 2 & wantsElevate)
            {
                if (!(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi))
                    wantsElevate = false;
                WanderAround(WanderType.All);
            }
            return;
        }
        if (floor == 1)
        {
            if (target.y < 20f)
            {
                SetDestination(target);
            }
            else
            {
                if (!(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi))
                    wantsElevate = true;
                savedPosition = target;
                SetDestination(es[0].npcSpaces[0].transform.position);
            }
        }
        else if (floor == 2)
        {
            if (target.y >= 20f && target.y < 40)
            {
                SetDestination(target);
            }
            else
            {
                if (!(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi))
                    wantsElevate = true;
                savedPosition = target;
                if (target.y < 20)
                {
                    SetDestination(es[0].npcSpaces[1].transform.position);
                }
                else SetDestination(es[1].npcSpaces[0].transform.position);
            }
        }
        else if (floor == 3)
        {
            if (target.y >= 40f)
            {
                SetDestination(target);
            }
            else
            {
                if (!(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi))
                    wantsElevate = true;
                savedPosition = target;
                SetDestination(es[1].npcSpaces[1].transform.position);
            }
        }
    }

    private void SetDestination(Vector3 target, bool HeightCheck = false)
    {
        if (MultiplayerManager.isActive)
        {
            if (gameObject.name == "Baldi")
            {
                if (MultiplayerManager.instance.isBaldi)
                {
                    return;
                }
                else if (!(MultiplayerManager.instance.isMainPlayer && MultiplayerManager.instance.isAIBaldi))
                {
                    return;
                }
            }
            if (MultiplayerManager.instance.isBaldi && gameObject.name != "Baldi")
            {
                return;
            }
        }
        if (!currentAgent.isActiveAndEnabled) return;
        if (!currentAgent.isOnNavMesh) return; // This prevents the nav mesh console error message.
        if (HeightCheck)
        {
            if (target.y >= 40 && position.y >= 20 && position.y < 40)
            {
                if (!(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi))
                    wantsElevate = true;
                savedPosition = target;
                currentAgent.SetDestination(es[1].npcSpaces[0].transform.position);
                //Debug.Log("Detected");
                return;
            }
        }
        currentAgent.SetDestination(target);
    }

    private Vector3 downstairHiding = new Vector3(-45f, 0f, 45f);
    private Vector3 upstairHiding = new Vector3(45f, 20f, 165f);

    private ElevatorScript GetNearestElevator()
    {
        if (position.y < 20) return es[0];
        if (es.Length == 1) return es[0];
        if (position.y >= 40) return es[1];
        if ((position - es[0].transform.position).magnitude >= (position - es[1].transform.position).magnitude)
        {
            return es[0];
        }
        else
        {
            return es[1];
        }
    }

    public void FleeFromTarget(Vector3 target)
    {
        // Credit to waterlight for this code
        // URL: https://discussions.unity.com/t/navmesh-flee-ai-flee-from-player/126809/4
        float multiplier = 2.5f;
        Vector3 runTo = transform.position + ((transform.position - target) * multiplier);

        ElevatorScript NearestElevator = GetNearestElevator();
        Vector3 elevator = NearestElevator.transform.position;
        if (Vector3.Distance(transform.position, elevator) <= 100f && NearestElevator.CurrentFloor == floor && !NearestElevator.elevating && !wantsElevate) // Is near to elevator?
        {
            wantsElevate = true;
            if (floor == 1)
            {
                SetTarget(upstairHiding);
            }
            else if (floor == 2)
            {
                SetTarget(downstairHiding);
            }
            else if (floor == 3)
            {
                SetTarget(new Vector3(110f, 0f, 230f));
            }
            return;
        }
        SetTarget(runTo);
    }

    public void ClearTargetsAndSetTarget(Vector3 target)
    {
        if (!(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi))
            wantsElevate = false;
        SetTarget(target);
    }

    public void WanderAround(WanderType type)
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isAIBaldi && gameObject.name == "Baldi") return;
        if (wantsElevate || usingElevate)
        {
            if (GameManager.notebooks < 2 & wantsElevate)
            {
                wantsElevate = false;
                WanderAround(WanderType.All);
            }
            return;
        }
        if (Partygoer && AttendingParty)
        {
            // WE CAN'T WANDER. LET'S PARTY!
            if (!OnPartyHavingFun)
            {
                AttendParty();
            }
            return;
        }
        if (type == WanderType.All)
        {
            wandererScript.GetNewTarget();
        }
        else if (type == WanderType.Halls)
        {
            wandererScript.GetNewTargetHallway();
        }
        else if (type == WanderType.Rooms)
        {
            wandererScript.GetNewTargetRoom();
        }
        if (!GameManager.finaleMode)
        {
            if (floor == 1)
            {
                if (WanderTarget.position.y < 20f)
                {
                    SetDestination(WanderTarget.position);
                }
                else
                {
                    if (!(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi))
                        wantsElevate = true;
                    savedPosition = WanderTarget.position;
                    SetDestination(es[0].npcSpaces[0].transform.position);
                }
            }
            else if (floor == 2)
            {
                if (WanderTarget.position.y >= 20f && WanderTarget.position.y < 40f)
                {
                    SetDestination(WanderTarget.position);
                }
                else
                {
                    if (!(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi))
                        wantsElevate = true;
                    savedPosition = WanderTarget.position;
                    if (WanderTarget.position.y < 20)
                    {
                        SetDestination(es[0].npcSpaces[1].transform.position);
                    }
                    else SetDestination(es[1].npcSpaces[0].transform.position);
                }
            }
            else if (floor == 3)
            {
                if (WanderTarget.position.y >= 40f)
                {
                    SetDestination(WanderTarget.position);
                }
                else
                {
                    wantsElevate = true;
                    savedPosition = WanderTarget.position;
                    SetDestination(es[1].npcSpaces[1].transform.position);
                }
            }
        }
        else
        {
            if (floor == 1)
            {
                if (WanderTarget.position.y <= 0f)
                {
                    SetDestination(WanderTarget.position);
                }
                else
                {
                    WanderAround(type);
                }
            }
            else if (floor == 2)
            {
                if (WanderTarget.position.y >= 20f && WanderTarget.position.y < 40f)
                {
                    SetDestination(WanderTarget.position);
                }
                else
                {
                    wantsElevate = true;
                    savedPosition = WanderTarget.position;
                    if (WanderTarget.position.y < 20)
                    {
                        SetDestination(es[0].npcSpaces[1].transform.position);
                    }
                    else SetDestination(es[1].npcSpaces[0].transform.position);
                }
            }
            else if (floor == 3)
            {
                if (WanderTarget.position.y >= 40f)
                {
                    SetDestination(WanderTarget.position);
                }
                else
                {
                    wantsElevate = true;
                    savedPosition = WanderTarget.position;
                    SetDestination(es[1].npcSpaces[1].transform.position);
                }
            }
        }
    }

    public enum WanderType
    {
        All,
        Rooms,
        Halls
    }

    public NavMeshAgent getAgent()
    {
        return currentAgent;
    }

    public Vector3 position
    {
        get
        {
            return currentAgent.transform.position;
        }
        set
        {
            if (MultiplayerManager.isActive &&
            ((MultiplayerManager.instance.isMainPlayer && gameObject.name != "Baldi") ||
            (MultiplayerManager.instance.isBaldi && gameObject.name != "Baldi"))) return;
            currentAgent = GetComponent<NavMeshAgent>();
            currentAgent.Warp(value);
        }
    }

    public Vector3 target
    {
        get
        {
            return currentAgent.destination;
        }
        set
        {
            SetTarget(value);
        }
    }
}