using System.Collections.Generic;
using Multiplayer;
using SchoolSaves;
using UnityEngine;
using UnityEngine.AI;

// Name Reference: https://youtu.be/BvpNkorRd8E?t=13
public class BANANAPEELAAAA : MonoBehaviour
{
    public Rigidbody rb;

    public Collider Mine;
    private Collider Player;
    private GameControllerScript GameManager;

    public bool IsMoving;

    public Vector3 Velocity { get; private set; }

    public AudioClip SlippingLoop;
    public AudioClip SLIP;
    public AudioClip Splat;

    public AudioSource audioDevice;

    private List<Transform> LinkedAgents = new List<Transform>();
    private Transform MainLinked;

    public bool ReturnNone;
    private bool WaitForNPC;
    private float ReturningNoneTimer;
    private float SlippingTimer;
    private float SlipStopTimer;
    private float StartVelocityTimer;
    private float DestroyTimer = 0f;

    // public int MP_ID;
    // public Vector3 MP_Velocity;
    // public Vector3 LastPosition;
    // private Vector3 LastVelocity;

    // public bool CustomStartAnimation;
    // public Vector3 Custom_Velocity;

    public float Distance = 3f;

    public bool StartNoVelocity;
    
    private void Start()
    {
        GameManager = FindObjectOfType<GameControllerScript>();
        Player = GameManager.playerColider;
        Physics.IgnoreCollision(Mine, Player, true);
        if (!StartNoVelocity)
        {
            StartAnim();
        }
        else
        {
            WaitForNPC = true;
            ReturnNone = true;
            ReturningNoneTimer = 0.3f;
            StartVelocityTimer = 0f;
            DestroyTimer = float.PositiveInfinity;
            Velocity = Vector3.zero;
        }
    }

    public void StartAnim()
    {
        WaitForNPC = true;
        ReturnNone = true;
        ReturningNoneTimer = 0.3f;
        StartVelocityTimer = 0.8f;
        DestroyTimer = float.PositiveInfinity;
        Vector3 Pos = Player.transform.position;
        Pos.y -= 4f;
        rb.position = Pos;
        Velocity = Player.transform.forward * GameManager.cameraScript.GetLookBehindValue() * 25f;
        audioDevice.PlayOneShot(SLIP);
        // if (MultiplayerManager.isActive)
        // {
        //     float[] Position = Pos.ToFloat();
        //     float[] Velcoity = Velocity.ToFloat();
        //     string STR_1 = Position[0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Position[1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Position[2].ToString(System.Globalization.CultureInfo.InvariantCulture);
        //     string STR_2 = Velcoity[0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Velcoity[1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Velcoity[2].ToString(System.Globalization.CultureInfo.InvariantCulture);
        //     ClientSend.SendActionToOtherPlayer("spawnpeel:" + MP_ID + ":" + STR_1 + ":" + STR_2);
        //     MultiplayerManager.instance.AddBananaItem(MP_ID, this);
        // }
    }

    private void Update()
    {
        rb.velocity = Velocity;
        // if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer)
        // {
        //     if (LastPosition != transform.position)
        //     {
        //         transform.position = Vector3.MoveTowards(transform.position, LastPosition, Mathf.Clamp(Vector3.Distance(transform.position, LastPosition), 1.5f, 25f) * 0.8f * Time.unscaledDeltaTime);
        //     }
        // }
        // if (MainLinked != null && Vector3.Distance(Y(MainLinked.position), Y(transform.position)) > 0.3f)
        // {
        //     MainLinked.position = Y(rb.position, MainLinked.position.y);
        // }
        // If the MainLinked is yet very far.

        // Animation Time
        if (WaitForNPC)
        {
            if (StartVelocityTimer > 0f)
            {
                StartVelocityTimer -= Time.deltaTime;
            }
            else
            {
                Velocity = Vector3.zero;
            }
        }

        if (MainLinked != null)
        {
            if (SlippingTimer < 30f && Vector3.Distance(Y(MainLinked.position), Y(transform.position)) <= 10f)
            {
                SlippingTimer += Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (SlippingTimer > 0f)
        {
            SlippingTimer = 0f;
        }
        // If MainLinked is null
        if (MainLinked == null && SlipStopTimer <= 0f && audioDevice.clip != null)
        {
            SlipStopTimer = Time.deltaTime; // So, it'll stop instantly below.
            //Debug.Log("[Banana] Still slipping with 0 seconds, stopping audio.");
        }
        // The player won't get slipped.
        if (ReturningNoneTimer > 0f)
        {
            ReturningNoneTimer -= Time.deltaTime;
        }
        else if (ReturnNone)
        {
            ReturnNone = false;
        }
        // Slip stop timer for the audio not to glitch.
        if (SlipStopTimer > 0f)
        {
            SlipStopTimer -= Time.deltaTime;
            if (SlipStopTimer <= 0f)
            {
                audioDevice.clip = null;
                audioDevice.Stop();
                audioDevice.PlayOneShot(SLIP);
                DestroyTimer = 1f;
            }
        }
        // Destroy Timer
        if (DestroyTimer > 0f)
        {
            DestroyTimer -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject, 0f);
        }
    }

    private void FixedUpdate()
    {
        // if (MultiplayerManager.isActive)
        // {
        //     if (MultiplayerManager.instance.isMainPlayer)
        //     {
        //         if (LastPosition != transform.position || LastVelocity != rb.velocity)
        //         {
        //             ClientSend.BananaMovement(MP_ID, transform.position, rb.velocity);
        //             LastPosition = transform.position;
        //             LastVelocity = rb.velocity;
        //         }
        //     }
        // }

        // if (!MultiplayerManager.isActive || !MultiplayerManager.instance.isBaldi)
        // {
        if (!Player.isTrigger && Vector3.Distance(Y(transform.position), Y(Player.transform.position)) <= Distance)
        {
            GameManager.player.IsNearbyABanana(this);
        }
        //}
    }

    public Vector3 GetVelocity()
    {
        // if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        // {
        //     return MP_Velocity;
        //}
        if (Velocity.magnitude <= 0 || ReturnNone) return Vector3.zero;
        return rb.velocity;
    }

    private Vector3 Y(Vector3 pos, float y = 0f)
    {
        pos.y = y;
        return pos;
    }

    // Returns velocity
    public void Trip(Vector3 CurrentVelocity)
    {
        if ((!WaitForNPC && Velocity.magnitude > 0) || ReturnNone) return;
        if (CurrentVelocity.magnitude <= 0) return;
        WaitForNPC = false;
        DestroyTimer = float.PositiveInfinity;
        CurrentVelocity.y = 0f;
        Velocity = CurrentVelocity;
        rb.velocity = Velocity;
    }

    public void Link(Transform agent)
    {
        if (!LinkedAgents.Contains(agent))
        {
            LinkedAgents.Add(agent);
            DestroyTimer = float.PositiveInfinity;
            if (MainLinked == null)
            {
                MainLinked = agent.transform;
                SlipStopTimer = 30f;
                if (audioDevice.clip == null)
                {
                    audioDevice.clip = SlippingLoop;
                    audioDevice.Play();
                }
            }
        }
    }

    public void Unlink(Transform agent)
    {
        if (LinkedAgents.Contains(agent))
        {
            LinkedAgents.Remove(agent);
            if (MainLinked == agent)
            {
                MainLinked = (LinkedAgents.Count > 0) ? LinkedAgents[0] : null;
                if (MainLinked == null)
                {
                    SlipStopTimer = 0.05f;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (ReturnNone) return;
        if (other.gameObject.name.Contains("Handle")) return;
        if (other.gameObject.name == "WarningStripes") return;
        if (Velocity.magnitude <= 0) return;
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isMainPlayer) return;
        Velocity = Vector3.zero;
        audioDevice.PlayOneShot(Splat);
    }
}