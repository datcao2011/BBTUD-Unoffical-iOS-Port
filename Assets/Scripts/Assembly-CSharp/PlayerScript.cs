using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Platforms;
using Multiplayer;
using TMPro;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    public GameControllerScript gc;

    public Transform groundCheck;

    public float groundDistance = 2f;

    public LayerMask groundMask;

    public BaldiScript baldi;

    public PlatPlayer input;

    public DoorScript door;

    public bool ChangeHuggingPosition;

    public PlaytimeScript playtime;

    public bool gameOver;

    public bool trapped;

    public bool inCafe;

    public bool jumpRope;

    public bool sweeping;

    public bool hugging;

    public bool hugging1;

    private bool SlidingOnBananaPeel;
    private BANANAPEELAAAA BananaPeel;

    public bool isInBarn;

    public bool appleEat;

    public float sweepingFailsave;

    public float bananaFailsave;

    private Quaternion playerRotation;

    public Vector3 frozenPosition;

    public float mouseSensitivity;

    public float walkSpeed;

    public float runSpeed;

    public bool useStamina;

    public float maxStamina;

    public float staminaRate;

    public float guilt;

    public float initGuilt;

    public bool hiding;

    private float playerSpeed;

    public bool playerWalking;

    public float stamina;

    public Rigidbody rb;

    public SweepScript gottaSweep;

    public FirstPrizeScript firstPrize;

    public Transform firstPrizeTransform;

    public Slider staminaBar;

    public float fliparoo;

    public float bigBootsTime;

    public float height;

    public float flipaturn;

    public string guiltType;

    public GameObject jumpRopeScreen;

    public GameObject playerModel;

    public GameObject player;

    public Slider healthBar;

    public Animator playerMovement;
    public Vector3 MP_Position;
    public Quaternion MP_Rotation = Quaternion.identity;

    [HideInInspector]
    public Vector3 windVelocity;

    public float maxHealth = 100f;

    public float health;

    public bool dead;

    public GameObject FacultyNametagObject;
    public float NoFacultyAlert;
    public TMP_Text FacultyNametagDuration;

    public bool heldObject;

    private void Awake()
    {
        stamina = maxStamina;
    }

    private void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        input = PlatTools.GetPlatformPlayer();
        gc.scene = SceneManager.GetActiveScene().name;
        if (gc.scene == InputManager.IsSchoolScene())
        {
            healthBar.maxValue = maxHealth;
            health = maxHealth;
            healthBar.value = health;
        }
        useStamina = true;
        MobileInput.SetTriggerButton("Run", false);
        this.height = base.transform.position.y;
        rb = base.GetComponent<Rigidbody>();
        playerRotation = base.transform.rotation;
        mouseSensitivity = Settings.GetFloat("TurnSensitivity", 1f);
        if (mouseSensitivity == 0f)
        {
            mouseSensitivity = 0.5f;
            Settings.SetFloat("TurnSensitivity", 0.5f);
        }
    }

    public bool isGrounded;

    private float fallingTime;

    private Vector3 lastPosition;
    private Quaternion lastRotation = Quaternion.identity;
    [HideInInspector]
    public GameObject model;
    public bool Gravity = true;

    private void Update()
    {
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi)
        {
            if (MultiplayerManager.instance.PlayerList.Length > 2) return; // Make sure there's no other player events.
            if (transform.position != MP_Position)
            {
                transform.position = Vector3.Lerp(transform.position, MP_Position, 20f * Time.unscaledDeltaTime);
            }
            if (transform.rotation != MP_Rotation)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, MP_Rotation, 20f * Time.unscaledDeltaTime);
            }
            return;
        }
        MouseMove();
        StaminaCheck();
        GuiltCheck();
        CheckHealth();
        mouseSensitivity = Settings.GetFloat("TurnSensitivity", 1f);
        if (rb.velocity.magnitude > 0f && !gc.mouseLocked)
        {
            if (!gc.gamePaused && !gc.learningActive) gc.LockMouse();
        }
        if (jumpRope && (base.transform.position - frozenPosition).magnitude > System.Math.Max(walkSpeed, 1f))
        {
            DeactivateJumpRope();
        }
        if (sweepingFailsave > 0f)
        {
            sweepingFailsave -= Time.deltaTime;
        }
        else
        {
            sweeping = false;
            hugging = false;
            hugging1 = false;
        }
        if (bananaFailsave > 0f)
        {
            bananaFailsave -= Time.deltaTime;
        }
        else if (SlidingOnBananaPeel)
        {
            if (BananaPeel != null)
            {
                BananaPeel.Unlink(transform);
                BananaPeel = null;
            }
            SlidingOnBananaPeel = false;
        }
        if (NoFacultyAlert > 0f)
        {
            NoFacultyAlert -= Time.deltaTime;
            FacultyNametagObject.SetActive(true);
            FacultyNametagDuration.text = Mathf.CeilToInt(NoFacultyAlert) + "s";
        }
        else if (FacultyNametagObject != null && FacultyNametagObject.activeSelf)
        {
            NoFacultyAlert = 0f;
            FacultyNametagObject.SetActive(false);
        }
        if (bigBootsTime < 0f)
        {
            bigBootsTime = 0f;
        }
        if (bigBootsTime > 0f)
        {
            bigBootsTime -= Time.deltaTime;
        }
        if (bigBootsTime == 0f)
        {
            DeactivateBoots();
        }
    }

    bool falling;

    private void CheckHealth()
    {
        if (gc.scene == InputManager.IsSchoolScene())
        {
            healthBar.gameObject.SetActive(health < maxHealth);
            if (health > 0f) healthBar.GetComponentInChildren<TMPro.TMP_Text>().text = "Health " + Mathf.RoundToInt(health) + "%";
            if (health > 0f && health < maxHealth)
            {
                health += Time.deltaTime * 0.3f;
            }
            healthBar.value = health;
        }
    }

    public void Heal(float health)
    {
        if (this.health >= maxHealth)
        {
            GameObject.Find("Hud").GetComponent<HudScreenTextScript>().ShowHudText(3f, "You are already at max health.");
            return;
        }
        this.health += health;
    }

    public void TakeDamage(float damage)
    {
        if (MultiplayerManager.isActive) return;
        health -= damage;
        if (health <= 0f)
        {
            health = 0f;
            healthBar.GetComponentInChildren<TMPro.TMP_Text>().text = "DEAD 0%";
            healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.red;
            StartCoroutine(DeathDelay());
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.green;
        yield return new WaitForSeconds(0.1f);
        healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.green;
        yield return new WaitForSeconds(0.1f);
        healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.green;
        yield return new WaitForSeconds(0.1f);
        healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        healthBar.GetComponentInChildren<TMPro.TMP_Text>().color = Color.green;
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        dead = true;
        gameOver = true;
        cameraScript.GetComponent<Camera>().nearClipPlane = 0.1f;
    }

    private void FixedUpdate()
    {
        PlayerMove();
        UpdateGravity();
    }

    public float GravityMultiplier = 1f;

    private void UpdateGravity()
    {
        if (gc.scene.StartsWith("School") && Gravity && (!MultiplayerManager.isActive || !MultiplayerManager.instance.isBaldi))
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (!isGrounded && !ElevatorScript.PlayerIsOnElv & transform.position.y > 4f)
            {
                fallingTime += Time.fixedDeltaTime * 2f;
                transform.position -= new Vector3(0f, fallingTime * 10f * Time.fixedDeltaTime, 0f);
            }
            else
            {
                if (fallingTime > 0f)
                {
                    if (transform.position.y < 4f || (transform.position.y > 4f && transform.position.y < 24f))
                    {
                        Vector3 pos = transform.position;
                        pos.y = 4f;
                        transform.position = pos;
                    }
                    if (transform.position.y > 44f)
                    {
                        Vector3 pos = transform.position;
                        pos.y = 44f;
                        transform.position = pos;
                    }
                    else if (transform.position.y > 24f)
                    {
                        Vector3 pos = transform.position;
                        pos.y = 24f;
                        transform.position = pos;
                    }
                    if (fallingTime > 1.2f)
                    {
                        if (currentCollider == null) currentCollider = GetComponent<Collider>();
                        TakeDamage(fallingTime * (currentCollider.isTrigger ? 0f : 22.5f) * GravityMultiplier);
                        if (health <= 0f)
                        {
                            cameraScript.gameOverCharacterTarget = null;
                        }
                    }
                }
                fallingTime = 0f;
            }
        }
    }

    public void ToggleRotation()
    {
        rotateMobile = true;
    }

    public void UnToggleRotation()
    {
        rotateMobile = false;
    }

    [HideInInspector]
    public bool rotateMobile;

    private float rotation1;

    private float rotation2;

    private void MouseMove()
    {
        //ref Quaternion val = ref playerRotation;
        if (Time.timeScale != 0f)
        {
            PlayerRotate();
        }
    }

    private bool running;

    public bool cutGum;

    public bool isInvisible;

    private bool Grapple
    {
        get
        {
            return GrappleLocation.Count > 0;
        }
    }
    public List<GrapplingHookItem> GrappleLocation = new List<GrapplingHookItem>();

    public Vector3 GetAllGrappleHooks()
    {
        Vector3 Result = Vector3.zero;
        foreach (GrapplingHookItem Hook in GrappleLocation)
        {
            Result += Hook.GetFrom(transform.position);
        }
        return Result;
    }

    private void PlayerRotate()
    {
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi && MultiplayerManager.instance.PauseMovement)
        {
            return;
        }
        if (!flipped)
        {
            playerRotation.eulerAngles += new Vector3(0f, InputManager.TurnX(false) * mouseSensitivity, 0f);
        }
        else
        {
            playerRotation.eulerAngles += new Vector3(0f, -InputManager.TurnX(false) * mouseSensitivity, 0f);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        if (Time.timeSinceLevelLoad < 1) // To reduce memory, we'll only do this when the level just loaded.
            transform.position = pos; // Set the position of the transform first (the first fixed frame will have this position)
        rb.position = pos; // Set the position of the rigidbody so the player stays into that position after the fixed frame.
    }

    public void SetRotation(Quaternion rot)
    {
        playerRotation = rot;
    }

    public void SetOffsetPos()
    {
        SetPosition(new Vector3(5, 4, -15));
    }

    private Collider currentCollider;

    private Vector3 a;
    private Vector3 b;

    private void PlayerMove()
    {
        if (MultiplayerManager.isActive)
        {
            if (MultiplayerManager.instance.isBaldi && MultiplayerManager.instance.PlayerList.Length <= 2)
            {
                if (model == null) model = transform.GetChild(1).gameObject;
                if (!model.activeSelf)
                {
                    // Get avatar colors
                    try
                    {
                        string[] AvatarColors = (string[])Photon.Pun.PhotonNetwork.PlayerListOthers[0].CustomProperties["avatar"] ?? (new string[] { "", "", "", "", "", "" });
                        model.GetComponent<PlayerAvatarApplier>().Set(AvatarColors[0], AvatarColors[1], AvatarColors[2], AvatarColors[3], AvatarColors[4]);
                        model.GetComponent<PlayerAvatarApplier>().SetAccessory(int.Parse(AvatarColors[5]));
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
                model.SetActive(true);
                model.GetComponent<Animator>().SetBool("w", (transform.position - lastPosition).magnitude > 0.01f);
                lastPosition = transform.position;
                return;
            }
            else if (!MultiplayerManager.instance.isBaldi && !gameOver && !gc.SpectatingDisplay.activeSelf)
            {
                Vector3 Position = transform.position;
                if (jumpRope)
                {
                    Position.y += cameraScript.jumpHeight;
                }
                if (lastPosition != Position || lastRotation != transform.rotation)
                {
                    ClientSend.PlayerMovement(Position, transform.rotation.eulerAngles);
                    lastPosition = Position;
                    lastRotation = transform.rotation;
                }
            }
        }
        base.transform.rotation = playerRotation;
        bool isMovementPaused = MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi && MultiplayerManager.instance.PauseMovement;
        if (!isMovementPaused)
        {
            a = new Vector3(0f, 0f, 0f);
            b = new Vector3(0f, 0f, 0f);
        }

        if (currentCollider == null) currentCollider = GetComponent<Collider>();

        if (!isMovementPaused)
        {
            a = base.transform.forward * InputManager.Forward(true);
            b = base.transform.right * InputManager.Strafe(true) * (flipped ? -1 : 1);
        }
        float ClampedSens = Mathf.Clamp((a + b).magnitude, 0f, 1f);
        if (stamina > 0f)
        {
            if (input.GetButton("Run"))
            {
                if (!inFlood || currentCollider.isTrigger)
                {
                    playerSpeed = runSpeed;
                }
                else
                    playerSpeed = runSpeed * 0.3f;
                if (runSpeed != 16f && !running)
                {
                    running = true;
                    StartCoroutine(PlayerMovement(walkSpeed + 56f, Camera.main.fieldOfView));
                }
                if (rb.velocity.magnitude > 0.1f & !hugging & !sweeping & !jumpRope & !Grapple)
                {
                    ResetGuilt("running", 0.1f);
                }
            }
            else
            {
                running = false;
                if (!inFlood || currentCollider.isTrigger)
                    playerSpeed = walkSpeed;
                else
                    playerSpeed = walkSpeed * 0.3f;
            }
        }
        else
        {
            running = false;
            if (!inFlood || currentCollider.isTrigger)
                playerSpeed = walkSpeed;
            else
                playerSpeed = walkSpeed * 0.3f;
        }
        if (gc.scene == "CampScene")
        {
            int sticks = FindObjectOfType<CampingScript>().sticks;
            if (!trapped)
            {
                moveDirection = (a + b).normalized * ClampedSens * (playerSpeed - (float)(sticks > 3 ? (8 * (sticks - 3)) / ((float)(sticks - 3) + 2f) : 0f));
            }
            else
            {
                moveDirection = Vector3.zero;
            }
        }
        else if (Grapple)
        {
            moveDirection = GetAllGrappleHooks() + ((a + b) * playerSpeed * 0.25f);
            moveDirection.y = 0f; // You won't be able to go above anymore and take fall damage.
        }
        else if ((!sweeping & !hugging & !hugging1 || bootsActive) & !jumpRope & !hiding & !SlidingOnBananaPeel || currentCollider.isTrigger)
        {
            moveDirection = (a + b).normalized * ClampedSens * playerSpeed;
        }
        else if (sweeping && !bootsActive)
        {
            moveDirection = gottaSweep.GetVelocity() + (a + b).normalized * ClampedSens * playerSpeed * 0.3f;
        }
        else if (hugging && !bootsActive)
        {
            Vector3 a2 = firstPrize.GetVelocity() * 1.2f;
            Vector3 position = firstPrizeTransform.position;
            Vector3 forward = firstPrizeTransform.forward;
            float x = (float)Mathf.RoundToInt(forward.x);
            Vector3 forward2 = firstPrizeTransform.forward;
            moveDirection = a2 + (position + new Vector3(x, 0f, (float)Mathf.RoundToInt(forward2.z)) * 3f - base.transform.position);
        }
        else if (hugging1 && !bootsActive)
        {
            Vector3 a2 = gc.fifthPrize.GetComponent<NavMeshAgent>().velocity * 1.2f;
            Vector3 position = gc.fifthPrize.transform.position;
            Vector3 forward = gc.fifthPrize.transform.forward;
            float x = (float)Mathf.RoundToInt(forward.x);
            Vector3 forward2 = gc.fifthPrize.transform.forward;
            moveDirection = a2 + (position + new Vector3(x, 0f, (float)Mathf.RoundToInt(forward2.z)) * 3f - base.transform.position);
        }
        else if (SlidingOnBananaPeel && BananaPeel != null && BananaPeel.GetVelocity().magnitude >= 0.1f)
        {
            moveDirection = BananaPeel.GetVelocity();
            if (rb.velocity.magnitude <= 5f)
            {
                moveDirection = (a + b).normalized * ClampedSens * playerSpeed * 0.1f;
            }
            else
            {
                moveDirection += (a + b).normalized * ClampedSens * playerSpeed * 0.1f;
            }
        }
        else if (jumpRope && cameraScript.jumpHeight > 0f)
        {
            moveDirection = (a + b).normalized * ClampedSens * playerSpeed * 0.25f;
        }
        else
        {
            moveDirection = Vector3.zero;
        }
        if (windVelocity != Vector3.zero && !bootsActive && !currentCollider.isTrigger)
        {
            moveDirection += windVelocity;
        }
        // Check to remove the banana peel
        if (SlidingOnBananaPeel && !(BananaPeel != null && BananaPeel.GetVelocity().magnitude >= 0.1f))
        {
            if (BananaPeel != null)
            {
                BananaPeel.Unlink(transform);
                BananaPeel = null;
            }
            SlidingOnBananaPeel = false;
        }
        rb.velocity = moveDirection;
    }

    private IEnumerator PlayerMovement(float runValue, float lastValue)
    {
        if (!gc.cheatingConsole.IsFOVEnabled) yield break;
        while (Camera.main.fieldOfView < runValue && running)
        {
            Camera.main.fieldOfView += Time.unscaledDeltaTime * 20f;
            yield return null;
        }
        if (running)
            Camera.main.fieldOfView = runValue;
        while (running)
        {
            yield return null;
        }
        while (Camera.main.fieldOfView > lastValue)
        {
            Camera.main.fieldOfView -= Time.unscaledDeltaTime * 20f;
            yield return null;
        }
        if (!running)
            Camera.main.fieldOfView = lastValue;
        yield break;
    }

    private void StaminaCheck()
    {
        if (rb.velocity.magnitude > 0.1f & useStamina)
        {
            if (input.GetButton("Run") & stamina > 0f)
            {
                stamina -= staminaRate * Time.deltaTime;
            }
            if (stamina < 0f & stamina > -5f)
            {
                stamina = -5f;
            }
        }
        else if (stamina < maxStamina)
        {
            stamina += staminaRate * Time.deltaTime;
        }
        if (NullManager.Instance != null && NullManager.Instance.BossFight)
        {
            if (NullManager.Instance.health <= 5)
            {
                Transform NULL = NullManager.Instance.NULL.transform;
                float Distance = Mathf.Clamp(Vector3.Distance(transform.position, NULL.position), 0f, 200f);
                staminaBar.value = 100f - (Distance / 2f);
                if (Distance <= 25f)
                {
                    Flash += Time.deltaTime;
                    if (Flash >= 0.08f)
                    {
                        Flash = 0f;
                        FlashToggle = !FlashToggle;
                    }
                    staminaBar.fillRect.GetComponent<Image>().color = FlashToggle ? Color.green : gc.colored.Evaluate(1f - staminaBar.normalizedValue);
                }
                else { staminaBar.fillRect.GetComponent<Image>().color = gc.colored.Evaluate(1f - staminaBar.normalizedValue); }
            }
            else if (NullManager.Instance.health <= 7)
            {
                Transform NULL = NullManager.Instance.NULL.transform;
                float Distance = Mathf.Clamp(Vector3.Distance(transform.position, NULL.position), 0f, 50f);
                staminaBar.value = 100f - (Distance * 2f);
                if (Distance <= 25f)
                {
                    Flash += Time.deltaTime;
                    if (Flash >= 0.08f)
                    {
                        Flash = 0f;
                        FlashToggle = !FlashToggle;
                    }
                    staminaBar.fillRect.GetComponent<Image>().color = FlashToggle ? Color.green : gc.colored.Evaluate(1f - staminaBar.normalizedValue);
                }
                else { staminaBar.fillRect.GetComponent<Image>().color = gc.colored.Evaluate(1f - staminaBar.normalizedValue); }
            }
            else
            {
                staminaBar.value = -5f;
            }
        }
        else
        {
            staminaBar.value = stamina / maxStamina * 100f;
        }
    }

    private float Flash;
    private bool FlashToggle;

    private bool PlayerHasApple()
    {
        return gc.item[0] == 11 || gc.item[1] == 11 || gc.item[2] == 11 || gc.item[3] == 11 || gc.item[4] == 11 || gc.item[5] == 11;
    }

    public bool PlayerHasPass()
    {
        return gc.item[0] == 18 || gc.item[1] == 18 || gc.item[2] == 18 || gc.item[3] == 18 || gc.item[4] == 18 || gc.item[5] == 18;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Baldi" && !gc.debugMode)
        {
            if (gc.HASManager != null && gc.HASManager.hiderOrSeeker == "Seeker")
            {
                if (baldi.FlattenDuration > 0f || !IsFlat)
                {
                    gc.HASManager.EndRound();
                }
                return;
            }
            else if (baldi.IsFlatten)
            {
                return;
            }
            sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "CampScene" || sceneName.StartsWith("FarmScene"))
            {
                gameOver = true;
                cameraScript.GetComponent<Camera>().nearClipPlane = 0.1f;
                return;
            }
            if (appleEat)
            {
                return;
            }
            if (!gc.HASManager)
            {
                if (MultiplayerManager.isActive)
                {
                    if (!MultiplayerManager.instance.isBaldi)
                    {
                        if (!PlayerHasApple())
                        {
                            // Multiplayer.Client.DisconnectAction = ((reason, cause) =>
                            // {
                            //     Debug.LogException(reason);
                            // });
                            if (!baldi.IsAnimRulerBroken) MultiplayerManager.instance.client.GameOver();
                        }
                        else if (!baldi.IsAnimRulerBroken)
                        {
                            baldi.baldiPriority = baldi.currentPriority;
                            appleEat = true;
                            gc.UseApple();
                        }
                    }
                    return;
                }
                if (!PlayerHasApple())
                {
                    if (!baldi.IsAnimRulerBroken)
                    {
                        gameOver = true;
                        cameraScript.GM_PositionHeight = baldi.FlattenDuration > 0f ? 1f : 5f;
                        cameraScript.GetComponent<Camera>().nearClipPlane = 0.1f;
                    }
                }
                else if (!baldi.IsAnimRulerBroken)
                {
                    baldi.baldiPriority = baldi.currentPriority;
                    appleEat = true;
                    gc.UseApple();
                }
            }
            else
            {
                if (gc.HASManager.seekTimer <= 0f)
                {
                    if (gc.HASManager.hiderOrSeeker == "Hider")
                    {
                        if (!PlayerHasApple())
                        {
                            if (!baldi.IsAnimRulerBroken)
                            {
                                gameOver = true;
                                cameraScript.GM_PositionHeight = baldi.FlattenDuration > 0f ? 1f : 5f;
                                cameraScript.GetComponent<Camera>().nearClipPlane = 0.1f;
                            }
                        }
                        else
                        {
                            baldi.baldiPriority = baldi.currentPriority;
                            appleEat = true;
                            gc.UseApple();
                        }
                    }
                    else
                    {
                        console.log("Captured baldi!");
                        gc.HASManager.EndRound();
                    }
                }
            }
        }
        if (gc.scene != InputManager.IsSchoolScene())
        {
            if (other.transform.name == "Trap")
            {
                this.trapped = true;
            }
        }
    }
    public void IsNearbyABanana(BANANAPEELAAAA BananaPeel)
    {
        if (!BananaPeel.ReturnNone)
        {
            SlidingOnBananaPeel = true;
            this.BananaPeel = BananaPeel;
            BananaPeel.Link(transform);
            Vector3 Velocity = moveDirection.normalized;
            if (Velocity.magnitude <= 0) Velocity = transform.forward;
            BananaPeel.Trip(Velocity * 28f);
            bananaFailsave = 1.2f;
        }
    }

    public void SetBackBaldiPriority()
    {
        baldi.currentPriority = baldi.baldiPriority;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.name == "Crafters Detect" & gc.scene == "CampScene")
        {
            detected = true;
        }
        if (other.transform.name == "Barn Collider")
        {
            isInBarn = true;
        }
        if (gc.scene != InputManager.IsSchoolScene())
        {
            return;
        }
        if (other.transform.name == "Gotta Sweep" && other.GetComponent<NPC>() && !gc.debugMode)
        {
            if (!other.GetComponent<NPC>().IsFlatten && !other.GetComponent<NPC>().OnPartyHavingFun) sweeping = true;
            if (gc.cheatingConsole.ClonesInteract && other.GetComponent<SweepScript>())
            {
                gottaSweep = other.GetComponent<SweepScript>();
            }
            sweepingFailsave = 1f;
        }
        else if (other.transform.name == "1st Prize" && !gc.debugMode)
        {
            if (gc.cheatingConsole.ClonesInteract && other.GetComponent<FirstPrizeScript>())
            {
                firstPrize = other.GetComponent<FirstPrizeScript>();
                firstPrizeTransform = other.transform;
            }
            if (!firstPrize.IsFlatten && firstPrize.GetVelocity().magnitude > 5f)
            {
                hugging = true;
                sweepingFailsave = 1f;
            }
        }
        else if (other.transform.name == "5th Prize" && !gc.fifthPrize.GetComponent<NPC>().IsFlatten && gc.fifthPrize.GetComponent<NavMeshAgent>().velocity.magnitude > 5f && !gc.debugMode)
        {
            hugging1 = true;
            sweepingFailsave = 1f;
        }
        else if (other.transform.name == "Playtime" && NoFacultyAlert <= 0f && !jumpRope && playtime.playCool <= 0f && !playtime.IsFlatten && !gc.debugMode)
        {
            if (!MultiplayerManager.isActive || (!MultiplayerManager.instance.isBaldi && !MultiplayerManager.instance.HasMoreThan2Players))
            {
                ActivateJumpRope();
            }
            else if (!MultiplayerManager.instance.isBaldi)
            {
                ClientSend.StartJumpropeEvent();
            }
        }
        else if (other.transform.name == "Cafe Trigger")
        {
            inCafe = true;
        }
        else if (other.transform.name == "Flood")
        {
            inFlood = true;
        }
        else if (other.transform.name == "Pomp Class" & !gc.debugMode)
        {
            onClassroom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "Office Trigger")
        {
            ResetGuilt("escape", door.lockTime);
        }
        else if (other.transform.name == "Gotta Sweep")
        {
            sweeping = false;
        }
        else if (other.transform.name == "1st Prize")
        {
            hugging = false;
        }
        else if (other.transform.name == "5th Prize")
        {
            hugging1 = false;
        }
        else if (other.transform.name == "Cafe Trigger")
        {
            inCafe = false;
        }
        else if (other.transform.name == "Pomp Class")
        {
            onClassroom = false;
        }
        if (other.transform.name == "Flood")
        {
            inFlood = false;
        }
        if (other.transform.name == "Crafters Detect")
        {
            detected = false;
        }
        if (other.transform.name == "Barn Collider")
        {
            isInBarn = false;
        }
    }

    public void ResetGuilt(string type, float amount)
    {
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi) return;
        if (guiltType == "sneaking")
        {
            return;
        }
        if (amount >= guilt || (amount == 0f && guiltType == type))
        {
            guilt = amount;
            guiltType = type;
        }
    }

    private void GuiltCheck()
    {
        if (guilt > 0f)
        {
            guilt -= Time.deltaTime;
        }
    }

    public void ActivateJumpRope()
    {
        jumpRopeScreen.SetActive(true);
        jumpRope = true;
        frozenPosition = base.transform.position;
    }

    public void DeactivateJumpRope(bool MP_CutRope = false)
    {
        if (MultiplayerManager.isActive && jumpRope && MultiplayerManager.instance.HasMoreThan2Players)
        {
            ClientSend.EndJumpropeEvent(MP_CutRope);
            playtime.playCool = 15f;
        }
        jumpRopeScreen.SetActive(false);
        jumpRope = false;
    }

    public void runMobile()
    {
        if (!runNow)
        {
            runNow = true;
        }
        else
        {
            runNow = false;
        }
    }

    public void ActivateBoots()
    {
        bigBootsTime = 60f;
        bootsActive = true;
    }

    private void DeactivateBoots()
    {
        if (bootsActive)
        {
            gc.BootsOff();
        }
        bootsActive = false;
    }

    public IEnumerator HudOffForTheRestOfScene()
    {
        for (; ; )
        {
            gc.SetNoclip(false);
            gc.cheatConsole.SetActive(false);
            gc.cheatConsoleActive = false;
            gc.hud.SetActive(false);
            jumpRopeScreen.SetActive(false);
            gc.map.SetActive(false);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator CheatingOff(string reason)
    {
        for (; ; )
        {
            if (gc.cheatConsole.activeSelf)
            {
                InputManager.DisplayEvent(reason, 5f);
            }
            gc.cheatConsole.SetActive(false);
            gc.cheatConsoleActive = false;
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetHudOff()
    {
        StartCoroutine(HudOffForTheRestOfScene());
    }

    public void Fliparoo()
    {
        flipped = true;
        cameraScript.ChangeCamera(1, 0);
    }

    public void Gummed(float Duration, AudioClip Splat)
    {
        gumScreen.SetActive(true);
        gc.audioDevice.PlayOneShotWC(Splat);
        walkSpeed -= 7f;
        runSpeed -= 7f;
        playerSpeed = walkSpeed;
        StartCoroutine(gumTimer(Duration));
    }

    public bool IsFlat;
    public float FlatDuration;

    public void SetFlatDuration(float t)
    {
        StartCoroutine(GoFlat(t));
    }

    private IEnumerator GoFlat(float Duration)
    {
        IsFlat = true;
        FlatDuration = Duration;
        Vector3 Original = cameraScript.offset;
        Vector3 Diff = cameraScript.offset;
        Diff.y -= 3f;
        cameraScript.offset = Diff;
        while (FlatDuration > 0f)
        {
            FlatDuration -= Time.deltaTime;
            yield return null;
        }
        cameraScript.offset = Original;
        IsFlat = false;
        yield break;
    }

    private IEnumerator gumTimer(float Duration)
    {
        float timer = Duration;
        while (timer > 0f & !cutGum)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        cutGum = false;
        walkSpeed += 7f;
        runSpeed += 7f;
        playerSpeed = walkSpeed;
        gumScreen.SetActive(false);
        yield break;
    }

    public bool runNow;

    public GameObject gumScreen;

    public CharacterController cc;

    private Vector3 moveDirection;

    [HideInInspector]
    public bool flipped;

    public bool onClassroom;

    public bool inFlood;

    public CameraScript cameraScript;

    private string sceneName;

    private bool bootsActive;

    public bool detected;
}