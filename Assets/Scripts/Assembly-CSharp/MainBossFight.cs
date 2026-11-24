// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MainBossFight : MonoBehaviour
// {
//     public AudioSource source;
//     public AudioSource source2;
//     public TMPro.TMP_Text HudText;
//     public GameObject BSODA;
//     public bool EnableBSODAWeapon;
//     public AudioClip BSODASound;

//     public float maxHealth = 100f;
//     public float health;
//     public UnityEngine.UI.Slider healthBar;

//     private float BSODACooldown;


//     // Start is called before the first frame update
//     void Start()
//     {

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.K))
//         {
// 			GameObject.Find("FadeToWhite_Event").BroadcastMessage("_SetTrigger", "Error");
//         }
//         if (BSODACooldown > 0f)
//         {
//             BSODACooldown -= Time.unscaledDeltaTime;
//         }
//         else if (EnableBSODAWeapon && InteractScript.Intracting && (Time.timeScale != 0f || (MultiplayerManager.isActive && MultiplayerManager.instance.PauseMovement)))
//         {
//             BSODACooldown = (maxHealth - health) * 0.04f;
//             Object.Instantiate(BSODA, FindObjectOfType<PlayerScript>().transform.position, Camera.main.transform.rotation);
//             FindObjectOfType<GameControllerScript>().audioDevice.PlayOneShot(BSODASound);
//         }
//         healthBar.gameObject.SetActive(health < maxHealth);
//         if (health < maxHealth)
//         {
//             health += Time.deltaTime * 1.2f;
//         }
//         healthBar.value = health;
//     }

//     private bool HasDamageNoticed;
//     public GameObject[] Minions;
//     public GameObject Boss;

//     public void SetDamageStage2()
//     {
//         Boss.GetComponent<BossTest>().DamageMultiplier = 0.1f;
//     }
//     public void SetDamageStage3()
//     {
//         Boss.GetComponent<BossTest>().DamageMultiplier = 0.35f;
//     }
//     public void SetDamageStage4()
//     {
//         Boss.GetComponent<BossTest>().DamageMultiplier = 0.8f;
//     }
//     public void SetDamageStage5()
//     {
//         Boss.GetComponent<BossTest>().DamageMultiplier = 1f;
//     }

//     public void SpawnMinions()
//     {
//         foreach (GameObject Minion in Minions)
//         {
//             Minion.SetActive(true);
//         }
//         ShowHudText("The <color=#5C5C5C>Minions</color> were spawned. Watch out!");
//     }

//     public GameObject[] Points;

//     public GameObject HealthPickup;
//     public GameObject EnergyPickup;

//     public void TakeDamage(float damage)
//     {
//         if (!HasDamageNoticed)
//         {
//             HasDamageNoticed = true;
//             ShowHudText("Your <color=green>Health</color> affects your <color=blue>BSODA</color><color=red> fire rate.</color> Keep your health at the highest to increase the fire rate.");
//         }
//         health -= damage;
//         if (health <= 0.5f)
//         {
//             health = 0.5f;
//         }
//         if (damage > 0) StartCoroutine(FlashRed());
//     }

//     private IEnumerator FlashRed()
//     {
//         healthBar.GetComponentInChildren<TMPro.TMP_Text>().text = "Health " + Mathf.CeilToInt(health) + "%";
//         healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.red;
//         yield return new WaitForSeconds(0.1f);
//         healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.green;
//         yield return new WaitForSeconds(0.1f);
//         healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.red;
//         yield return new WaitForSeconds(0.1f);
//         healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.green;
//         yield return new WaitForSeconds(0.1f);
//         healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.red;
//         yield return new WaitForSeconds(0.1f);
//         healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.green;
//         yield return new WaitForSeconds(0.1f);
//         healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.red;
//         yield return new WaitForSeconds(0.1f);
//         healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.green;
//         yield return new WaitForSeconds(1f);
//         healthBar.GetComponentInChildren<TMPro.TMP_Text>().text = "Health";
//     }


//     public void ShowHudText(string str)
//     {
//         HudText.transform.parent.gameObject.SetActive(true);
//         HudText.text = str;
//         StartCoroutine(HideHudText());
//     }

//     public void SpawnBSODA()
//     {
//         Boss.GetComponent<BossTest>().ShootBSODA();
//     }

//     public void SpawnHealth()
//     {
//         Vector3 pos = Points[Mathf.RoundToInt(Random.Range(0f, Points.Length - 1f))].transform.position;
//         Instantiate(HealthPickup, new Vector3(pos.x, HealthPickup.transform.position.y, pos.z), HealthPickup.transform.rotation);
//     }

//     public void SpawnEnergy()
//     {
//         Vector3 pos = Points[Mathf.RoundToInt(Random.Range(0f, Points.Length - 1f))].transform.position;
//         Instantiate(EnergyPickup, new Vector3(pos.x, EnergyPickup.transform.position.y, pos.z), EnergyPickup.transform.rotation);
//     }

//     private IEnumerator HideHudText()
//     {
//         yield return new WaitForSecondsRealtime(5f);
//         HudText.transform.parent.gameObject.SetActive(false);
//         yield break;
//     }

//     public void LoadScene(string str)
//     {
//         UnityEngine.SceneManagement.SceneManager.LoadScene(str);
//     }

//     public void Countdown()
//     {
//         source.Play();
//         StartCoroutine(PitchAudio());
//     }

//     public void StartBossPhase()
//     {
//         source2.Play();
//         ShowHudText(InputManager.usingTouch ? "Tap the interact button to shoot your infinite BSODA weapon." : "Use " + Platforms.PlatTools.GetPlatformPlayer().GetKeyName("Interact") + " to shoot your infinite BSODA weapon.");
//         EnableBSODAWeapon = true;
//         Boss.SetActive(true);
//     }

//     public void PauseBossPhase()
//     {
//         EnableBSODAWeapon = false;
//         Boss.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
//     }

//     public void UnpauseBossPhase()
//     {
//         EnableBSODAWeapon = true;
//         Boss.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = false;
//     }
    
//     public void EndBossPhase()
//     {
//         EnableBSODAWeapon = false;
//         Boss.SetActive(false);
//     }

//     private IEnumerator PitchAudio()
//     {
//         yield return new WaitForSecondsRealtime(17f);
//         float pitch = 0.8f;
//         while (pitch > 0.01f)
//         {
//             pitch -= Time.unscaledDeltaTime * 0.3f;
//             if (pitch < 0.01f) pitch = 0.01f;
//             source2.pitch = pitch;
//             yield return null;
//         }
//         source2.Stop();
//         yield break;
//     }
// }
