using UnityEngine;

public class MathBalloon : MonoBehaviour
{
    public float number;

    public MathMachine machine;

    public static bool balloonBeingUsed;

    public bool beingUsed;

    public AudioClip popSound;

    private float offTime = 15f;

    public bool IsInClassroom = false;
    public float CheckTime;
    public bool Check;

    private Transform player;

    // In case you wanted to implement this. Remove the #if UNITY_EDITOR from since I already implemented it
#if UNITY_EDITOR
#pragma warning disable CS0414
    private GameObject click = null;
    [SerializeField]
    private GameObject ClickCursor;
    private Animator anim;
#pragma warning restore CS0414
#endif

    private Balloon balloon;
    private Collider m_collider;

    private readonly Vector3 Offset = new Vector3(0f, 0.91f, 0f);

    private void Start()
    {
        if (machine == null)
        {
            machine = FindObjectOfType<MathMachine>();
        }
        player = FindObjectOfType<PlayerScript>().transform;
        m_collider = GetComponent<Collider>();
        balloon = GetComponent<Balloon>();
        //anim = GetComponentInChildren<Animator>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "RoomCollide")
        {
            beingUsed = false;
            Vector3 Pos = other.transform.position;
            Pos.y = transform.position.y;
            transform.position = Pos;
            balloon.SetPosition(Pos);
            Check = false;
            CheckTime = 0f;
            IsInClassroom = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "RoomCollide")
        {
            IsInClassroom = true;
        }
    }

    private void Update()
    {
        if (offTime > 0f & beingUsed)
        {
            offTime -= Time.deltaTime;
        }
        else if (beingUsed)
        {
            beingUsed = false;
            offTime = 15f;
            transform.position = transform.position + Offset;
            balloon.SetPosition(transform.position); // Set Y Position back
            // Check if out of boundary
            CheckTime = 1f + Time.unscaledDeltaTime;
            Check = true;
            IsInClassroom = false;
        }

        m_collider.enabled = !beingUsed;
        if (machine.solved)
        {
            // if (!ReferenceEquals(click, null))
            // {
            //     Destroy(click);
            //     anim.SetBool("onHover", false);
            //     click = null;
            // }
            if (number > 0f)
            {
                number -= Time.deltaTime * 4.5f;
            }
            else
            {
                if (BirthdayBash.Instance == null || !BirthdayBash.Instance.glitchActive)
                {
                    GameObject gameObject = new GameObject("Pop! for " + number);
                    Object.Destroy(gameObject, popSound.length);
                    AudioSource audioDevice = gameObject.AddComponent<AudioSource>();
                    audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
                    gameObject.AddComponent<AudioSourceSpeed>();
                    audioDevice.clip = popSound;
                    audioDevice.spatialBlend = 1f;
                    audioDevice.minDistance = 10f;
                    audioDevice.maxDistance = 50f;
                    audioDevice.dopplerLevel = 0f;
                    audioDevice.transform.position = transform.position;
                    audioDevice.Play();
                }
                Object.Destroy(base.gameObject, 0f);
            }
            return;
        }
        if (beingUsed)
        {
            Vector3 position = player.position + (player.forward * 2f);
            transform.position = position;
        }
        #region Boundary Check
        if (Check)
        {
            if (CheckTime > 0f)
            {
                CheckTime -= Time.deltaTime;
            }
            else
            {
                CheckTime = 0f;
                Check = false;
                if (!IsInClassroom)
                {
                    beingUsed = false;
                    Vector3 Pos = machine.trigger.transform.position;
                    Pos.y = transform.position.y;
                    transform.position = Pos;
                    balloon.SetPosition(Pos);
                    IsInClassroom = true;
                }
            }
        }
        #endregion
        if (CheckTime <= 1f && (transform.position - player.position).magnitude < 20f) // Making sure the balloon gets off perfectly in next frame.
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            // bool IsHovering = Physics.Raycast(ray, out raycastHit);
            if (InteractScript.Intracting)
            {
                bool IsHovering = Physics.Raycast(ray, out raycastHit);
                if (IsHovering)
                {
                    // if (!ReferenceEquals(click, null))
                    // {
                    //     Destroy(click);
                    //     click = null;
                    // }
                    if (!beingUsed && raycastHit.transform == transform && (raycastHit.transform.position - player.position).magnitude < 20f)
                    {
                        if (!balloonBeingUsed)
                        {
                            beingUsed = true;
                            balloonBeingUsed = true;
                        }
                        else
                        {
                            MathBalloon[] balloons = FindObjectsOfType<MathBalloon>();
                            foreach (MathBalloon balloon in balloons)
                            {
                                balloon.beingUsed = false;
                            }

                            beingUsed = true;
                        }
                    }
                    else if (beingUsed)
                    {
                        beingUsed = false;
                        balloonBeingUsed = false;
                        if (raycastHit.transform == machine.transform && (raycastHit.transform.position - player.position).magnitude < 20f)
                        {
                            machine.AnswerProblem(number);
                            Object.Destroy(gameObject, 0f);
                        }
                        else
                        {
                            // Check if out of boundary
                            CheckTime = 1f + Time.unscaledDeltaTime;
                            Check = true;
                            IsInClassroom = false;
                            transform.position = transform.position + Offset;
                            balloon.SetPosition(transform.position); // Set Y Position back
                            //anim.SetBool("onHover", false);
                        }
                    }
                }
            }
            // if (!balloonBeingUsed && IsHovering && raycastHit.transform == transform)
            // {
            //     if (ReferenceEquals(click, null))
            //     {
            //         click = Instantiate(ClickCursor);
            //         Destroy(click, 13f);
            //         anim.SetBool("onHover", true);
            //     }
            // }
            // else
            // {
            //     if (!ReferenceEquals(click, null))
            //     {
            //         Destroy(click);
            //         click = null;
            //         if (!beingUsed) anim.SetBool("onHover", false);
            //     }
            // }
        }
        // else if (!ReferenceEquals(click, null))
        // {
        //     Destroy(click);
        //     click = null;
        //     anim.SetBool("onHover", false);
        // }
    }
}