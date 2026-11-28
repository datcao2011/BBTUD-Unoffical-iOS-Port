using UnityEngine;

using Platforms;

public class CameraScript : MonoBehaviour
{
    public GameObject player;

    public PlayerScript ps;

    public Transform baldi;

    public Transform gameOverCharacterTarget;

    public Transform gameOverFadeTransform;

    public GameObject gameOverFade;

    public float initVelocity;

    public float velocity;

    public float gravity;

    public bool DenyRotationYMovement;

    private int lookBehind;

    public int numTimes;

    [HideInInspector]
    public Vector3 offset;

    public float jumpHeight;

    public GameControllerScript gc;

    private Vector3 jumpHeightV3;

    public Vector3 fliparoo;

    public Transform playerPos;

    public Animator blackScreen;

    private PlatPlayer input;

    [SerializeField]
    private Camera[] mainCamera = new Camera[2];

    public float GM_PositionHeight = 5f;

    private void Start()
    {
        GM_PositionHeight = 5f;
        input = PlatTools.GetPlatformPlayer();
        gameOverCharacterTarget = baldi;
        offset = player.transform.InverseTransformPoint(transform.position);

        if (Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
        {
            Camera.main.nearClipPlane = 1;
        }

        OnDistanceChanged("Distance", Settings.GetInt("Distance", GameOptimizer.CameraOptimizationDefault));
        OnDistanceChanged("OcclusionCulling", Settings.GetBool("OcclusionCulling", GameOptimizer.OptimizationDefault));
        Settings.OnSettingChanged += OnDistanceChanged;
    }

    private void OnDistanceChanged(string name, object val)
    {
        if (name == "Distance")
        {
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi)
            {
                Camera.main.nearClipPlane = 0.1f;
                Camera.main.farClipPlane = 500f;
                return; // The logs burst out if we do this
            }
            int[] Distances = new int[] { 100, 200, 500, 10000 };
            Camera.main.farClipPlane = Distances[(int)val];
            Camera.main.nearClipPlane = (int)val >= 2 ? 0.1f : 1f;
        }
        if (name == "OcclusionCulling")
        {
            Camera.main.useOcclusionCulling = (bool)val;
            // if (Settings.GetString("ModeType", "Normal") == "BB")
            // {
            //     Camera.main.useOcclusionCulling = false;
            // }
            if (NullManager.Instance != null && !Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
            {
                Camera.main.useOcclusionCulling = true;
                Debug.LogWarning("[Camera Settings] Forcing the use of Occlusion Culling due to performance without optimization.");
            }
        }

    }

    private void OnDestroy()
    {
        Settings.OnSettingChanged -= OnDistanceChanged;
    }

    private bool Jumped;

    public bool NoFade;

    private void Update()
    {
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi)
        {
            if (ps.gameOver)
            {
                gameOverFade.SetActive(true);
            }
            mainCamera[0].farClipPlane = 1;
            return;
        }
        if (player.GetComponent<PlayerScript>().hiding)
        {
            lookBehind = 0;
        }
        else if (ps.jumpRope)
        {
            velocity -= gravity * Time.deltaTime;
            jumpHeight += velocity * Time.deltaTime;
            if (jumpHeight <= 0f)
            {
                if (Jumped)
                {
                    Jumped = false;
                    ps.frozenPosition = playerPos.position;
                }
                jumpHeight = 0f;
                if (input.GetButtonDown("Jump"))
                {
                    velocity = initVelocity;
                    Jumped = true;
                }
            }
            else
            {
                Jumped = true;
            }
            jumpHeightV3 = new Vector3(0f, jumpHeight, 0f);
        }

        bool lookBackHeld = input.GetButton("LookBack") || lookBehindButton;
        if (InputManager.usingTouch && Input.touchCount == 0 && !Input.GetMouseButton(0))
        {
            lookBehindButton = false;
            lookBackHeld = false;
        }
        if (Time.timeScale != 0f & !ps.hiding & (!ps.jumpRope || InputManager.usingTouch || input.GetKeyName("LookBack") != input.GetKeyName("Jump")) && lookBackHeld)
        {
            if (!(MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi && MultiplayerManager.instance.PauseMovement))
            {
                lookBehind = 180;
            }
        }
        else
        {
            lookBehind = 0;
        }
        if (ps.gameOver && !NoFade)
        {
            gameOverFade.SetActive(true);
        }
    }

    private float yawOffset;
    private float pitchOffset;

    public bool IgnoreCameraInputs;

    private bool Is3DMovementOn()
    {
        return !DenyRotationYMovement && Settings.GetBool("3d movement");
    }

    private void LateUpdate()
    {
        if (IgnoreCameraInputs) return;
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi)
        {
            if (ps.gameOver)
            {
                base.transform.position = gameOverCharacterTarget.position + gameOverCharacterTarget.forward * 2f + new Vector3(0f, GM_PositionHeight, 0f);
                Transform transform = base.transform;
                Vector3 position = gameOverCharacterTarget.position;
                float x = position.x;
                Vector3 position2 = gameOverCharacterTarget.position;
                float y = position2.y + GM_PositionHeight;
                Vector3 position3 = gameOverCharacterTarget.position;
                transform.LookAt(new Vector3(x, y, position3.z));
            }
            return;
        }
        if (!ps.gameOver)
        {
            if (Is3DMovementOn() & Time.timeScale != 0f)
            {
                if (!(MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi && MultiplayerManager.instance.PauseMovement))
                {
                    if (!ps.flipped)
                    {
                        this.yawOffset -= InputManager.TurnX(true) * Settings.GetFloat("TurnSensitivity", 1f);
                        this.pitchOffset -= InputManager.TurnY(true) * Settings.GetFloat("TurnSensitivity", 1f);
                    }
                    else
                    {
                        this.yawOffset -= -InputManager.TurnX(true) * Settings.GetFloat("TurnSensitivity", 1f);
                        this.pitchOffset -= -InputManager.TurnY(true) * Settings.GetFloat("TurnSensitivity", 1f);
                    }
                    pitchOffset = Mathf.Clamp(this.pitchOffset, -89.9f, 89.9f);
                }
            }
            if (!Is3DMovementOn())
            {
                yawOffset = 0f;
                pitchOffset = 0f;
            }
        }
        transform.position = player.transform.TransformPoint(offset);
        if (!ps.gameOver & !ps.jumpRope)
        {
            transform.position = player.transform.TransformPoint(offset);
            Vector3 lookDir = (player.transform.position - transform.position).normalized;
            if (lookDir == Vector3.zero || lookDir.sqrMagnitude < 0.0001f)
            {
                lookDir = player.transform.forward;
            }
            if (lookBehind != 0)
            {
                lookDir = -lookDir;
            }
            lookDir = Quaternion.Euler(0f, yawOffset, 0f) * lookDir;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude < 0.0001f)
                lookDir = player.transform.forward;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude < 0.0001f)
                lookDir = Vector3.forward;
            lookDir = Quaternion.Euler(pitchOffset, 0f, 0f) * lookDir;
            if (lookDir.sqrMagnitude < 0.0001f)
                lookDir = player.transform.forward;
            lookDir.Normalize();
            transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        }
        else if (ps.gameOver && gameOverCharacterTarget != null)
        {
            base.transform.position = gameOverCharacterTarget.position + gameOverCharacterTarget.forward * 2f + new Vector3(0f, GM_PositionHeight, 0f);
            Transform transform = base.transform;
            Vector3 position = gameOverCharacterTarget.position;
            float x = position.x;
            Vector3 position2 = gameOverCharacterTarget.position;
            float y = position2.y + GM_PositionHeight;
            Vector3 position3 = gameOverCharacterTarget.position;
            transform.LookAt(new Vector3(x, y, position3.z));
        }
        else if (ps.jumpRope)
        {
            transform.position = player.transform.TransformPoint(offset) + jumpHeightV3;
            Vector3 lookDir = (player.transform.position - transform.position).normalized;
            if (lookDir == Vector3.zero || lookDir.sqrMagnitude < 0.0001f)
            {
                lookDir = player.transform.forward;
            }
            if (lookBehind != 0)
            {
                lookDir = -lookDir;
            }
            lookDir = Quaternion.Euler(0f, yawOffset, 0f) * lookDir;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude < 0.0001f)
                lookDir = player.transform.forward;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude < 0.0001f)
                lookDir = Vector3.forward;
            lookDir = Quaternion.Euler(pitchOffset, 0f, 0f) * lookDir;
            if (lookDir.sqrMagnitude < 0.0001f)
                lookDir = player.transform.forward;
            lookDir.Normalize();
            transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        }
    }

    public void ChangeCamera(int num, int num2)
    {
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi)
        {
            return;
        }
        if (num > 1 || num2 > 1)
        {
            console.error("CameraScript: Cannot find camera.");
            return;
        }
        if (num == 0 & num2 == 0)
        {
            console.error("CameraScript: Value cannot be the same.");
            return;
        }
        if (num == 1 & num2 == 1)
        {
            console.error("CameraScript: Value cannot be the same.");
            return;
        }
        mainCamera[num2].enabled = false;
        mainCamera[num].enabled = true;
    }

    public void MobilePressed()
    {
        if (ps.jumpRope)
        {
            if (jumpHeight <= 0f)
            {
                velocity = initVelocity;
            }
        }
    }

    public float GetLookBehindValue()
    {
        return (lookBehind > 0) ? -1f : 1f;
    }

    public bool lookBehindButton;
}