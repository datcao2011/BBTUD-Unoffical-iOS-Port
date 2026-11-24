using UnityEngine;
using System.Collections;
using TMPro;

public class ElevatorScript : MonoBehaviour
{
    [Header("For Floor 1")]
    public EntranceScript entrance;

    [Header("For Floor 2")]
    public Animator F2E1;
    public Animator F2E2;
    public AudioClip Open;
    public AudioClip Close;

    [HideInInspector]
    public bool elevating;

    [HideInInspector]
    public bool up;

    [Header("Component Variables")]
    public int floor;

    private PlayerScript PlayerS;

    [HideInInspector]
    public int CurrentFloor;

    public MeshRenderer buttonRenderer;

    public Transform height;

    public Transform player;

    public GameObject clickCursor;

    public Animator animator;

    private GameObject click;

    public GameObject[] npcSpaces;

    public TMP_Text[] outTexts;

    public GameObject[] blackS;

    public GameObject EL1;
    public GameObject ER1;
    public GameObject EL2;
    public GameObject ER2;

    public GameObject ObstacleF1;
    public GameObject ObstacleF2;

    public GameObject BaseE1;
    public GameObject BaseE2;

    public Material green;

    public Material white;

    public TMP_Text elevatorText;

    public bool unavailable;
    private bool OnTopFloor;

    public string DownFloor;
    public string UpFloor;

    private void Start()
    {
        PlayerS = entrance.gc.player;
        SetOutText(DownFloor, Color.yellow);
        CurrentFloor = floor - 1;
        if (floor == 3) F2E1.SetBool("closed", false);

        if (Settings.GetString("ModeType", "Normal") == "Speedy") StartCoroutine(ElevatorStartCoolDown());

    }

    public static void U(bool u) { foreach (ElevatorScript e in FindObjectsOfType<ElevatorScript>()) { e.unavailable = u; } }

    private IEnumerator ElevatorStartCoolDown()
    {
        float seconds = Random.Range(35f, 90f);
        if (MultiplayerManager.isActive) { seconds = 20f; }
        elevating = true;
        while (seconds > 0f)
        {
            elevatorText.color = Color.red;
            elevatorText.text = Mathf.CeilToInt(seconds).ToString();
            SetOutText(Mathf.CeilToInt(seconds).ToString(), Color.red);
            seconds -= Time.deltaTime;
            yield return null;
        }
        elevating = false;
        if (up)
        {
            elevatorText.color = Color.yellow;
            elevatorText.text = UpFloor;
            SetOutText(UpFloor, Color.yellow);
        }
        else
        {
            elevatorText.color = Color.yellow;
            elevatorText.text = DownFloor;
            SetOutText(DownFloor, Color.yellow);
        }
    }

    private void Update()
    {
        if ((entrance.exitTaken || (!entrance.exitTaken && entrance.gc.exitsReached == 3)) && (!elevating || elevatorText.color == Color.red) && BirthdayBash.Instance == null)
        {
            if (!unavailable) // Not yet
            {
                // Make sure current NPCs don't want the elevator anymore
                foreach (NPC other in FindObjectsOfType<NPC>())
                {
                    if (other.wantsElevate)
                    {
                        other.GetComponent<NPC>().wantsElevate = false;
                        other.GetComponent<NPC>().SetTarget(new Vector3(-41, 0, 304));
                    }
                }
                // Check for Baldi
                if (NullManager.Instance != null)
                {
                    if (NullManager.Instance.NULL.transform.position.y >= 20f)
                    {
                        entrance.gc.crafters.SetActive(true);
                        entrance.gc.crafters.transform.SetParent(null);
                        entrance.gc.crafters.GetComponent<CraftersScript>().BecomeInstantAngry();
                    }
                }
                else
                {
                    if (entrance.gc.baldiTransform.position.y >= 20f)
                    {
                        if (entrance.gc.crafters.GetComponent<DisableOnLoad>())
                        {
                            Destroy(entrance.gc.crafters.GetComponent<DisableOnLoad>(), 0f);
                        }
                        entrance.gc.crafters.SetActive(true);
                        if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
                        {
                            entrance.gc.crafters.GetComponent<CraftersScript>().BecomeInstantAngry();
                        }
                    }
                }
            }
            unavailable = true;
        }
        Vector3 OriginalPosition = BaseE1.transform.position;
        Vector3 NewPosition;
        if (floor == 3)
        {
            if (!OnTopFloor)
            {
                NewPosition = EL1.transform.position;
                NewPosition.z += 0.07f;
                NewPosition.y = OriginalPosition.y;
                NewPosition.x = -30f;
                BaseE1.transform.position = NewPosition;
                NewPosition = ER1.transform.position;
                NewPosition.z += 0.07f;
                NewPosition.y = OriginalPosition.y;
                NewPosition.x = -30f;
                BaseE2.transform.position = NewPosition;
            }
            else
            {
                NewPosition = EL2.transform.position;
                NewPosition.z += 0.07f;
                NewPosition.y = OriginalPosition.y;
                NewPosition.x = -30f;
                BaseE1.transform.position = NewPosition;
                NewPosition = ER2.transform.position;
                NewPosition.z += 0.07f;
                NewPosition.y = OriginalPosition.y;
                NewPosition.x = -30f;
                BaseE2.transform.position = NewPosition;
            }
        }
        else
        {
            if (!OnTopFloor)
            {
                NewPosition = EL1.transform.position;
                NewPosition.x -= 0.07f;
                NewPosition.y = OriginalPosition.y;
                NewPosition.z = 350.01f;
                BaseE1.transform.position = NewPosition;
                NewPosition = ER1.transform.position;
                NewPosition.x -= 0.07f;
                NewPosition.y = OriginalPosition.y;
                NewPosition.z = 350.01f;
                BaseE2.transform.position = NewPosition;
            }
            else
            {
                NewPosition = EL2.transform.position;
                NewPosition.x -= 0.07f;
                NewPosition.y = OriginalPosition.y;
                NewPosition.z = 350.01f;
                BaseE1.transform.position = NewPosition;
                NewPosition = ER2.transform.position;
                NewPosition.x -= 0.07f;
                NewPosition.y = OriginalPosition.y;
                NewPosition.z = 350.01f;
                BaseE2.transform.position = NewPosition;
            }
        }

        if (!unavailable && elevatorText.text == "Unavailable")
        {
            if (up)
            {
                elevatorText.color = Color.yellow;
                elevatorText.text = UpFloor;
                SetOutText(UpFloor, Color.yellow);
            }
            else
            {
                elevatorText.color = Color.yellow;
                elevatorText.text = DownFloor;
                SetOutText(DownFloor, Color.yellow);
            }
        }

        if (!unavailable)
        {
            if (elevating)
            {
                buttonRenderer.sharedMaterial = white;
            }
            else
            {
                buttonRenderer.sharedMaterial = green;
            }
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit raycastHit;
            bool IsHovering = Vector3.Distance(player.position, buttonRenderer.transform.position) < 7f && !elevating && Physics.Raycast(ray, out raycastHit) && raycastHit.transform.name == "Button1";
            if (InteractScript.Intracting)
            {

                if (IsHovering)
                {
                    if (MultiplayerManager.isActive) Multiplayer.ClientSend.SendElevatorEventFromPlayer(up, false);
                    else
                        StartCoroutine(elevate(null));
                }
            }
            if (IsHovering)
            {
                if (click == null)
                    click = Object.Instantiate(clickCursor);
            }
            else
            {
                if (click != null)
                    Object.Destroy(click);
            }
        }
        else
        {
            elevatorText.color = Color.red;
            elevatorText.text = "Unavailable";
            SetOutText("Unavailable", Color.red);
            buttonRenderer.sharedMaterial = white;
        }
    }

    public bool playerIsOn;
    public static bool PlayerIsOnElv;

    private bool CurrentlyMoving;

    public void NPCOn(GameObject NPC)
    {
        StartCoroutine(elevate(NPC));
    }

    public void SetOutText(string text, Color color)
    {
        outTexts[0].color = color;
        outTexts[1].color = color;
        if (color == Color.yellow)
        {
            outTexts[0].text = "Elevator is in\n" + text;
            outTexts[1].text = "Elevator is in\n" + text;
            CurrentlyMoving = false;
        }
        else if (color == Color.red & text != "Unavailable")
        {
            outTexts[0].text = "Elevator Cooldown:\n" + text;
            outTexts[1].text = "Elevator Cooldown:\n" + text;
            CurrentlyMoving = false;
        }
        else if (color == Color.green)
        {
            outTexts[0].text = "Elevator Elevating:\n" + text;
            outTexts[1].text = "Elevator Elevating:\n" + text;
            CurrentlyMoving = true;
        }
        else
        {
            outTexts[0].text = "Elevator is " + text;
            outTexts[1].text = "Elevator is " + text;
            if (text == "Closing")
            {
                CurrentlyMoving = true;
            }
            else
            {
                CurrentlyMoving = false;
            }
        }
    }

    private IEnumerator elevate(GameObject NPC)
    {
        while (elevating)
        {
            yield return null;
        }
        if (unavailable) yield break;
        elevating = true;
        up = !up;
        CurrentFloor = CurrentFloor == floor ? floor - 1 : floor;
        ObstacleF1.SetActive(true);
        ObstacleF2.SetActive(true);
        if (up)
        {
            if (floor == 2)
                entrance.CloseDownElevate();
            else
            {
                F2E1.SetBool("closed", true);
#if UNITY_EDITOR
                if (F2E1.GetComponent<AudioSource>())
                {
#endif
                    F2E1.GetComponent<AudioSource>().PlayOneShot(Close);
#if UNITY_EDITOR
                }
#endif
            }
        }
        else
        {
            if (floor == 2)
                entrance.CloseUpElevate();
            else
            {
                F2E2.SetBool("closed", true);
#if UNITY_EDITOR
                if (F2E2.GetComponent<AudioSource>())
                {
#endif
                    F2E2.GetComponent<AudioSource>().PlayOneShot(Close);
#if UNITY_EDITOR
                }
#endif
            }
        }
        elevatorText.color = Color.gray;
        elevatorText.text = "Closing";
        SetOutText("Closing", Color.gray);
        float s = 2f;
        while (s > 0f)
        {
            Vector3 localHeight = this.height.localPosition;
            localHeight.y = 2f;
            height.localPosition = localHeight;
            if (NPC != null)
            {
                NPC.transform.position = height.position;
            }
            s -= Time.deltaTime;
            yield return null;
        }
        blackS[0].SetActive(false);
        blackS[1].SetActive(false);
        animator.SetBool("Up", up);
        float seconds = 10f;
        while (seconds > 0f)
        {
            Vector3 localHeight = this.height.localPosition;
            localHeight.y = 2f;
            height.localPosition = localHeight;
            if (NPC != null)
            {
                NPC.transform.position = height.position;
            }
            localHeight.y = 4f;
            height.localPosition = localHeight;
            if (playerIsOn)
            {
                Vector3 pos = player.position;
                pos.y = height.position.y;
                PlayerS.SetPosition(pos);
            }
            elevatorText.color = Color.green;
            elevatorText.text = Mathf.CeilToInt(seconds).ToString();
            SetOutText(Mathf.CeilToInt(seconds).ToString(), Color.green);
            seconds -= Time.deltaTime;
            yield return null;
        }
        if (up)
        {
            ObstacleF2.SetActive(false);
            if (floor == 2)
                entrance.OpenUpElevate();
            else
            {
                F2E2.SetBool("closed", false);
#if UNITY_EDITOR
                if (F2E2.GetComponent<AudioSource>())
                {
#endif
                    F2E2.GetComponent<AudioSource>().PlayOneShot(Open);
#if UNITY_EDITOR
                }
#endif
            }

        }
        else
        {
            ObstacleF1.SetActive(false);
            if (floor == 2)
                entrance.OpenDownElevate();
            else
            {
                F2E1.SetBool("closed", false);
#if UNITY_EDITOR
                if (F2E1.GetComponent<AudioSource>())
                {
#endif
                    F2E1.GetComponent<AudioSource>().PlayOneShot(Open);
#if UNITY_EDITOR
                }
#endif
            }
        }
        if (up)
        {
            blackS[0].SetActive(true);
            buttonRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
            if (NPC != null)
            {
                NPC.GetComponent<NPC>().floor = floor - 1;
                NPC.GetComponent<NPC>().position = npcSpaces[1].transform.position;
                NPC.GetComponent<NPC>().SE(NPC.GetComponent<NPC>().savedPosition);
                yield return new WaitForSecondsRealtime(0.025f);
                NPC.GetComponent<NPC>().usingElevate = false;
            }
        }
        else
        {
            blackS[1].SetActive(true);
            buttonRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
            if (NPC != null)
            {
                NPC.GetComponent<NPC>().floor = floor;
                NPC.GetComponent<NPC>().position = npcSpaces[0].transform.position;
                NPC.GetComponent<NPC>().SE(NPC.GetComponent<NPC>().savedPosition);
                yield return new WaitForSecondsRealtime(0.025f);
                NPC.GetComponent<NPC>().usingElevate = false;
            }
        }
        elevatorText.color = Color.gray;
        OnTopFloor = !OnTopFloor;
        elevatorText.text = "Opening";
        SetOutText("Opening", Color.gray);
        if (playerIsOn)
        {
            Vector3 pos2 = player.position;
            pos2.y = height.position.y;
            PlayerS.SetPosition(pos2);
        }
        seconds = Random.Range(2f, 30f);
        if (MultiplayerManager.isActive) { seconds = 20f; }
        while (seconds > 0f)
        {
            elevatorText.color = Color.red;
            elevatorText.text = Mathf.CeilToInt(seconds).ToString();
            SetOutText(Mathf.CeilToInt(seconds).ToString(), Color.red);
            seconds -= Time.deltaTime;
            yield return null;
        }
        elevating = false;
        if (up)
        {
            elevatorText.color = Color.yellow;
            elevatorText.text = UpFloor;
            SetOutText(UpFloor, Color.yellow);
        }
        else
        {
            elevatorText.color = Color.yellow;
            elevatorText.text = DownFloor;
            SetOutText(DownFloor, Color.yellow);
        }
        yield break;
    }

    public void MP_StartElevatorFromPlayer(bool IsUp)
    {
        if (outTexts[0].color == Color.red && !outTexts[0].text.Contains("unavailable"))
        {
            elevating = false;
            StopAllCoroutines();
        }
        up = IsUp;
        OnTopFloor = IsUp;
        StartCoroutine(elevate(null));
    }

    public void MP_StartElevatorFromBaldi(bool IsUp)
    {
        if (outTexts[0].color == Color.red && !outTexts[0].text.Contains("unavailable"))
        {
            elevating = false;
            StopAllCoroutines();
        }
        up = IsUp;
        OnTopFloor = IsUp;
        entrance.gc.baldiScrpt.wantsElevate = false;
        entrance.gc.baldiScrpt.usingElevate = true;
        StartCoroutine(elevate(GameObject.Find("Baldi")));
    }

    public void ElevatorGo(bool IsUp)
    {
        up = IsUp;
        OnTopFloor = IsUp;
        ElevatorGo();
    }

    public void ElevatorGo()
    {
        StartCoroutine(elevateNoCooldown());
    }

    private IEnumerator elevateNoCooldown()
    {
        while (elevating)
        {
            yield return null;
        }
        if (unavailable) yield break;
        up = !up;
        elevating = true;
        ObstacleF1.SetActive(true);
        ObstacleF2.SetActive(true);
        if (up)
        {
            if (floor == 2)
                entrance.CloseDownElevate();
            else
            {
                F2E1.SetBool("closed", true);
#if UNITY_EDITOR
                if (F2E1.GetComponent<AudioSource>())
                {
#endif
                    F2E1.GetComponent<AudioSource>().PlayOneShot(Close);
#if UNITY_EDITOR
                }
#endif
            }
        }
        else
        {
            if (floor == 2)
                entrance.CloseUpElevate();
            else
            {
                F2E2.SetBool("closed", true);
#if UNITY_EDITOR
                if (F2E2.GetComponent<AudioSource>())
                {
#endif
                    F2E2.GetComponent<AudioSource>().PlayOneShot(Close);
#if UNITY_EDITOR
                }
#endif
            }
        }
        elevatorText.color = Color.gray;
        elevatorText.text = "Closing";
        SetOutText("Closing", Color.gray);
        yield return new WaitForSeconds(2f);
        blackS[0].SetActive(false);
        blackS[1].SetActive(false);
        animator.SetBool("Up", up);
        float seconds = 10f;
        while (seconds > 0f)
        {
            Vector3 localHeight = this.height.localPosition;
            Vector3 pos = player.position;
            localHeight.y = 4f;
            height.localPosition = localHeight;
            if (playerIsOn)
            {
                pos.y = height.transform.position.y;
                PlayerS.SetPosition(pos);
            }
            elevatorText.color = Color.green;
            elevatorText.text = Mathf.CeilToInt(seconds).ToString();
            SetOutText(Mathf.CeilToInt(seconds).ToString(), Color.green);
            seconds -= Time.deltaTime;
            yield return null;
        }
        if (up)
        {
            ObstacleF2.SetActive(false);
            if (floor == 2)
                entrance.OpenUpElevate();
            else
            {
                F2E2.SetBool("closed", false);
#if UNITY_EDITOR
                if (F2E2.GetComponent<AudioSource>())
                {
#endif
                    F2E2.GetComponent<AudioSource>().PlayOneShot(Open);
#if UNITY_EDITOR
                }
#endif
            }

        }
        else
        {
            ObstacleF1.SetActive(false);
            if (floor == 2)
                entrance.OpenDownElevate();
            else
            {
                F2E1.SetBool("closed", false);
#if UNITY_EDITOR
                if (F2E1.GetComponent<AudioSource>())
                {
#endif
                    F2E1.GetComponent<AudioSource>().PlayOneShot(Open);
#if UNITY_EDITOR
                }
#endif
            }
        }
        if (up)
        {
            buttonRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            buttonRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
        }
        elevatorText.color = Color.gray;
        elevatorText.text = "Opening";
        OnTopFloor = !OnTopFloor;
        SetOutText("Opening", Color.gray);
        if (playerIsOn)
        {
            Vector3 pos2 = player.position;
            pos2.y = height.position.y;
            PlayerS.SetPosition(pos2);
        }
        if (up)
        {
            blackS[0].SetActive(true);
        }
        else
        {
            blackS[1].SetActive(true);
        }
        yield return new WaitForSeconds(2f);
        elevating = false;
        if (up)
        {
            elevatorText.color = Color.yellow;
            elevatorText.text = UpFloor;
            SetOutText(UpFloor, Color.yellow);
        }
        else
        {
            elevatorText.color = Color.yellow;
            elevatorText.text = DownFloor;
            SetOutText(DownFloor, Color.yellow);
        }
        yield break;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Player")
        {
            PlayerIsOnElv = true;
            playerIsOn = true;
        }
        else if (CurrentlyMoving && other.tag == "NPC")
        {
            NPC npc = other.GetComponent<NPC>();
            if ((!npc.wantsElevate && !npc.usingElevate) || (other.name == "Baldi" && MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi && !npc.usingElevate))
            {
                npc.position = npcSpaces[other.GetComponent<NPC>().floor - (floor - 1)].transform.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            PlayerIsOnElv = false;
            playerIsOn = false;
        }
    }
}
