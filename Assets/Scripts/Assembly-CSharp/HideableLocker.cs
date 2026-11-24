using UnityEngine;
using Platforms;

public class HideableLocker : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer lockerFront;

    [SerializeField]
    private Material closed;

    [SerializeField]
    private Material open;

    public GameObject clickCursor;

    public GameObject click;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private BaldiScript baldi;

    [SerializeField]
    private PlayerScript playerScript;

    [SerializeField]
    private BoxCollider invisibleBarrier;

    [SerializeField]
    private BoxCollider trigger;

    public bool bDoorOpen;

    public float openTime;

    public bool LimitedTime;
    public float HidingDuration = 20f;
    private float HidingTime;

    private float time;

    [SerializeField]
    private Animator lockerAnim;

    private bool hided;

    private bool showed = true;

    private bool hiding;
    private PlatPlayer input;

    private EventMessage msg;

    [SerializeField]
    private Collider[] meshColliders;

    public bool AllowCharactersSeeInside;

    [SerializeField]
    private float Negative = -1f;

    private float CameraMin;

    private void Start()
    {
        input = PlatTools.GetPlatformPlayer();
    }

    private void Update()
    {
        if (openTime > 0f)
        {
            openTime -= Settings.GetFloat("TimeSpeed") * Time.deltaTime;
        }
        if (openTime <= 0f & bDoorOpen)
        {
            bDoorOpen = false;
            invisibleBarrier.enabled = true;
            lockerFront.sharedMaterial = closed;
            lockerHider.SetActive(true);
            myAudio.PlayOneShot(doorClose, 1f);
        }
        if (hiding && time < 1.1f & !hided)
        {
            showed = false;
            SetEnabled(false);
            time += Time.deltaTime;
            playerScript.gc.playerColider.enabled = false;
            Vector3 rot = lockerAnimTransform.localEulerAngles;
            rot.y = rot.y * Negative;
            playerScript.SetRotation(Quaternion.Euler(rot));
        }
        else if (!hided & hiding)
        {
            hided = true;
            playerScript.gc.playerColider.enabled = AllowCharactersSeeInside;
            SetEnabled(false);
        }
        if (!hiding && time < 1.1f && !showed)
        {
            hided = false;
            time += Time.deltaTime;
            playerScript.gc.playerColider.enabled = false;
            Vector3 pos = lockerAnimTransform.position;
            pos.y = player.position.y;
            playerScript.SetPosition(pos);
        }
        else if (!showed & !hiding)
        {
            playerScript.gc.playerColider.enabled = true;
            SetEnabled(true);
            showed = true;
        }
        if (hiding)
        {
            Vector3 pos = lockerAnimTransform.position;
            pos.y = player.position.y;
            playerScript.SetPosition(pos);
            if (LimitedTime)
            {
                if (HidingTime > 0f)
                {
                    HidingTime -= Time.unscaledDeltaTime;
                }
                else
                {
                    Unhide();
                }
            }
        }
        if (hiding && (input.GetButtonDown("Jump") || input.GetButtonDown("ShowButton")))
        {
            Unhide();
        }
        Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit raycastHit;
        bool CanInteract = !playerScript.hiding && Vector3.Distance(player.position, base.transform.position) < 10f && Physics.Raycast(ray, out raycastHit) && (Object)raycastHit.collider == (Object)trigger;
        if (InteractScript.Intracting && Time.timeScale != 0f)
        {
            if (CanInteract)
            {
                time = 0f;
                OpenDoor();
                HidingTime = HidingDuration;
                hiding = true;
                playerScript.hiding = true;
                CameraMin = Camera.main.nearClipPlane;
                Camera.main.nearClipPlane = 0.1f;
                if (!bDoorOpen)
                {
                    myAudio.PlayOneShot(doorOpen, 1f);
                }
                bDoorOpen = true;
                lockerAnim.SetTrigger("hide");
                openTime = 1f;
                if (LimitedTime)
                {
                    msg = InputManager.DisplayEvent("You can only hide for " + HidingDuration.ToString("0") + " seconds.", HidingDuration);
                }
            }
        }
        if (CanInteract)
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

    public void SetEnabled(bool enabled)
    {
        if (!AllowCharactersSeeInside)
        {
            return;
        }
        foreach (Collider col in meshColliders)
        {
            col.enabled = enabled;
        }
    }

    public void OpenDoor()
    {
        lockerHider.SetActive(false);
        invisibleBarrier.enabled = false;
        lockerFront.sharedMaterial = open;
    }

    public void Unhide()
    {
        time = 0f;
        OpenDoor();
        playerScript.hiding = false;
        hiding = false;
        if (!bDoorOpen)
        {
            myAudio.PlayOneShot(doorOpen, 1f);
        }
        bDoorOpen = true;
        lockerAnim.SetTrigger("show");
        openTime = 1f;
        HidingTime = 0f;
        if (baldi.isActiveAndEnabled)
            baldi.Hear(base.transform.position, 6.5f);
        if (baldi.gameController.DoctorReflex != null && baldi.gameController.DoctorReflex.Angry && !baldi.gameController.DoctorReflex.WantsTheHammer)
        {
            baldi.gameController.DoctorReflex.Hear(transform.position); // Hear like Baldi
        }
        if (msg != null) msg.Disappear();
        Camera.main.nearClipPlane = CameraMin;
    }

    public AudioClip doorOpen;

    public AudioClip doorClose;

    [SerializeField]
    private GameObject lockerHider;

    public Transform lockerAnimTransform;

    public AudioSource myAudio;
}