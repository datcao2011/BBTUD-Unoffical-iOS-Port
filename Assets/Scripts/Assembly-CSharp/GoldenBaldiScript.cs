using UnityEngine;
using UnityEngine.AI;

public class GoldenBaldiScript : NPC
{
    // Start is called before the first frame update
    private void Start()
    {
        this.audioDevice = base.GetComponent<AudioSource>();
        this.Wander();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (this.coolDown > 0f)
        {
            this.coolDown -= 1f * Time.deltaTime;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (base.MyAgent.velocity.magnitude <= 1f & this.coolDown <= 0f)
        {
            if (this.PartyEvent.party)
            {
                return;
            }
            this.Wander();
        }
    }

    private void Wander()
    {
        WanderAround(WanderType.All);
        this.coolDown = 1f;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" & !collider.isTrigger & !PartyEvent.party)
        {
            float rng = UnityEngine.Random.Range(0f, 1f);
            if (rng >= 0.5f)
            {
                if (baldi.isActiveAndEnabled)
                    this.baldi.Hear(this.player.transform.position, 1f);
            }
            else if (rng <= 0.25f & baldi.isActiveAndEnabled)
            {
                NavMeshHit hit;
                Vector3 Distance = new Vector3(baldi.transform.position.x, 0f, baldi.transform.position.z) + baldi.transform.forward * 20f;
                if (NavMesh.FindClosestEdge(Distance, out hit, NavMesh.AllAreas))
                {
                    gc.player.SetPosition(hit.position + new Vector3(0f, 4f, 0f));
                }
                else
                {
                    gc.player.SetPosition(Distance + new Vector3(0f, 4f, 0f));
                }
                this.audioDevice.PlayOneShot(this.aud_Teleport);
            }
        }
    }

    protected override void OnPartyHaveToAttend()
    {
        AttendParty();
        this.coolDown = 1f;
    }

    public FloodScript FloodEvent;

    public BaldiScript baldi;

    public GameControllerScript gc;

    public Transform player;

    public RandomEvent PartyEvent;

    // Token: 0x04000345 RID: 837
    public AudioSource audioDevice;

    // Token: 0x04000346 RID: 838
    public AudioClip aud_Teleport;

    public float coolDown;
}