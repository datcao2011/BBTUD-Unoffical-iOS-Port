// // AgentTest
// using UnityEngine;
// using UnityEngine.AI;
// using TMPro;

// public class BossTest : NPC
// {
//     public bool db;

//     public Transform player;

//     public Transform wanderTarget;

//     public AILocationSelectorScript wanderer;

//     public float coolDown;

//     public bool IsMinion;

//     private NavMeshAgent agent;

//     public float Health;
//     public float MaxHealth = 500f;
//     public float DamageMultiplier = 0.01f;
//     public MainBossFight Game;

//     private void Start()
//     {
//         agent = base.GetComponent<NavMeshAgent>();
//         Wander();
//     }

//     protected override void Update()
//     {
//         if (coolDown > 0f)
//         {
//             coolDown -= 1f * Time.deltaTime;
//         }

//         if (Input.GetKeyDown(KeyCode.H))
//         {
//             ShootBSODA();
//         }
//     }

//     public float DamageSubtraction = 1.21f;

//     private bool HasRegenerateNoticed;

//     protected override void FixedUpdate()
//     {
//         base.FixedUpdate();
//         if (!IsMinion)
//         {
//             if (Health < MaxHealth)
//             {
//                 Health += Time.unscaledDeltaTime * ((MaxHealth - Health) * (DamageSubtraction - DamageMultiplier));
//                 if (!HasRegenerateNoticed)
//                 {
//                     HasRegenerateNoticed = true;
//                     Game.ShowHudText("The <color=black>BOSS</color> can regenerate health up to 250%.");
//                 }
//             }
//             if (Health >= 500)
//             {
//                 Health = 500f;
//             }
//         }
//         else
//         {
//             if (Health <= 0f)
//             {
//                 Destroy(gameObject, 0f);
//             }
//         }
//         HealthText.text = (IsMinion ? "<color=#5C5C5C>Minion</color>" : "<color=black>Boss</color>") + "\n<color=#B3B3B3>Health: " + Health.ToString("0") + "%";

//         Vector3 direction = player.position - base.transform.position;
//         RaycastHit raycastHit;
//         if (Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) & raycastHit.transform.tag == "Player" & !GameManager.player.isInvisible)
//         {
//             db = true;
//             TargetPlayer();
//         }
//         else
//         {
//             db = false;
//             if (agent.velocity.magnitude <= 1f & coolDown <= 0f & !wantsElevate & !usingElevate)
//             {
//                 Wander();
//             }
//         }
//     }

//     private void Wander()
//     {
//         WanderAround(WanderType.All);
//         coolDown = 1f;
//     }

//     private void TargetPlayer()
//     {
//         SetTarget(player.position);
//         coolDown = 1f;
//     }

//     public GameObject TheBadBSODA;

//     public TMP_Text HealthText;

//     public void ShootBSODA()
//     {
//         transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
//         Object.Instantiate(TheBadBSODA, transform.position, transform.rotation);
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.tag == "BSODA")
//         {
//             Health -= Random.Range(0f, 60f) * DamageMultiplier;
//             if (Health <= 5f && !IsMinion)
//             {
//                 Health = 5f;
//             }
//             Destroy(other.gameObject, 0f);
//         }
//         if (other.tag == "Player")
//         {
//             Game.TakeDamage(Random.Range(10f, 60f) * (1.11f - DamageMultiplier));
//         }
//     }
// }