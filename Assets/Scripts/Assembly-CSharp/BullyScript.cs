// BullyScript
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class BullyScript : MonoBehaviour
{
    public Transform player;

    public GameControllerScript gc;

    public Renderer bullyRenderer;

    public Transform wanderTarget;

    public AILocationSelectorScript wanderer;

    public float waitTime;

    public float activeTime;

    public float guilt;

    public bool active;

    public bool spoken;

    private AudioSource audioDevice;

    public AudioClip[] aud_Taunts = new AudioClip[2];

    public AudioClip[] aud_Thanks = new AudioClip[2];

    public AudioClip aud_Denied;

    public AudioClip aud_Bored;

    public PrincipalScript prin;

    public bool inDetention;

    public GameObject Wall;

    private float TemporaryWallDisable;

    public float FlattenDuration;

    public bool IsFlatten
    {
        get
        {
            return FlattenDuration > 0f && !gc.player.IsFlat; // Actions will occur if the player and NPC is flat. Got the idea?
        }
    }

    private void Start()
    {
        audioDevice = base.GetComponent<AudioSource>();
        waitTime = Random.Range(60f, 120f);
    }

    private void Update()
    {
        if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
        {
            if (waitTime > 0f)
            {
                waitTime -= Time.deltaTime;
            }
            else if (!active)
            {
                Activate();
            }
        }
        if (active && (!MultiplayerManager.isActive || MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer))
        {
            activeTime += Time.deltaTime;
            if (activeTime >= 180f)
            {
                PlayOneShot(aud_Bored);
                Reset();
            }
        }
        if (guilt > 0f)
        {
            guilt -= Time.deltaTime;
        }
        if (FlattenDuration > 0f)
        {
            FlattenDuration -= Time.deltaTime;
            if (FlattenDuration <= 0f)
            {
                Transform Sprite = GetComponentInChildren<SpriteRenderer>().transform;
                Vector3 Scale = Sprite.localScale;
                if (Scale.y == 2f)
                {
                    Scale.y = 4f;
                    Sprite.localScale = Scale;
                    Sprite.transform.localPosition = Vector3.zero; // Black to normal
                }
                if (GetComponent<AudioSource>()) GetComponent<AudioSource>().pitch = Settings.GetFloat("TimeSpeed", 1f);
            }
        }
        if (Wall != null)
        {
            if (TemporaryWallDisable > 0f)
            {
                TemporaryWallDisable -= Time.deltaTime;
                Wall.SetActive(false);
            }
            else if (!Wall.activeSelf)
            {
                Wall.SetActive(true);
            }
        }
        if (inDetention)
        {
            if (prin.bullyDetention <= 0f)
            {
                active = false;
                spoken = false;
                activeTime = 0f;
                waitTime = Random.Range(60f, 120f);
                inDetention = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!active)
        {
            return;
        }
        Vector3 direction = player.position - base.transform.position;
        RaycastHit raycastHit;
        if ((base.transform.position - player.position).magnitude <= 20f && active && !gc.player.isInvisible && gc.player.NoFacultyAlert <= 0f && Physics.Raycast(base.transform.position + new Vector3(0f, 4f, 0f), direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) && raycastHit.transform.tag == "Player")
        {
            if (!spoken && bullyRenderer.isVisible)
            {
                int num = Mathf.RoundToInt(Random.Range(0f, 1f));
                PlayOneShot(aud_Taunts[num]);
                spoken = true;
            }
            if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer) guilt = 1f;
        }
    }

    private void PlayOneShot(AudioClip aud)
    {
        if (!MultiplayerManager.isActive)
        {
            audioDevice.PlayOneShotWC(aud);
            if (gc.CM.isActiveAndEnabled)
            {
                StartCoroutine(DeleteCaptionDelay(aud));
            }
            return;
        }

        if (!MultiplayerManager.instance.isBaldi)
        {
            audioDevice.PlayOneShotWC(aud);
            if (gc.CM.isActiveAndEnabled)
            {
                StartCoroutine(DeleteCaptionDelay(aud));
            }
            Multiplayer.ClientSend.PlaySoundClip(gameObject.name, aud.name);
        }
    }

    private System.Collections.IEnumerator DeleteCaptionDelay(AudioClip clip)
    {
        yield return new WaitForSeconds(4f);
        if (gc.CM.isActiveAndEnabled)
        {
            gc.CM.DeleteOneShotCaption(gameObject, clip.name);
        }
        yield break;
    }

    private void Activate()
    {
        wanderer.GetNewTargetHallway();
        base.transform.position = wanderTarget.position + new Vector3(0f, 5f, 0f);
        while ((base.transform.position - player.position).magnitude < 20f)
        {
            wanderer.GetNewTargetHallway();
            base.transform.position = wanderTarget.position + new Vector3(0f, 5f, 0f);
        }
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
        {
            Vector3 t = transform.position;
            Multiplayer.ClientSend.SendActionToOtherPlayer($"bully:{t.x.ToString(CultureInfo.InvariantCulture)},{t.y.ToString(CultureInfo.InvariantCulture)},{t.z.ToString(CultureInfo.InvariantCulture)}");
        }
        active = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && !other.isTrigger)
        {
            CheckWithPlayer();
            if (TemporaryWallDisable > 0f) return; // Can't do anything
            if (gc.item[0] == 0 & gc.item[1] == 0 & gc.item[2] == 0 & gc.item[3] == 0 & gc.item[4] == 0 & gc.item[5] == 0)
            {
                PlayOneShot(aud_Denied);
            }
            else
            {
                int num = Mathf.RoundToInt(Random.Range(0f, 5f));
                while (gc.item[num] == 0)
                {
                    num = Mathf.RoundToInt(Random.Range(0f, 5f));
                }
                gc.LoseItem(num);
                int num2 = Mathf.RoundToInt(Random.Range(0f, 1f));
                PlayOneShot(aud_Thanks[num2]);
                Reset();
            }
        }
    }

    private void CheckWithPlayer()
    {
        if (gc.player.NoFacultyAlert > 0f || IsFlatten)
        {
            TemporaryWallDisable = 1f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player" && !other.isTrigger)
        {
            CheckWithPlayer();
        }
    }

    public void SetFlatDuration(float Duration)
    {
        if (FlattenDuration <= 0f)
        {
            Transform Sprite = GetComponentInChildren<SpriteRenderer>().transform;
            Vector3 Scale = Sprite.localScale;
            if (Scale.y == 4f)
            {
                Scale.y = 2f;
                Sprite.localScale = Scale;
                Sprite.transform.localPosition = new Vector3(0f, -2.475f, 0f);
            }
            if (GetComponent<AudioSource>()) GetComponent<AudioSource>().pitch = Settings.GetFloat("TimeSpeed", 1f) + 0.75f;
        }
        FlattenDuration = Duration;
    }

    public void Reset()
    {
        waitTime = 9999999f;
        base.transform.position = base.transform.position - new Vector3(0f, 20f, 0f);
        active = false;
        activeTime = 0f;
        spoken = false;
        if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi)
        {
            Vector3 t = transform.position;
            Multiplayer.ClientSend.SendActionToOtherPlayer($"bully:{t.x.ToString(CultureInfo.InvariantCulture)},{t.y.ToString(CultureInfo.InvariantCulture)},{t.z.ToString(CultureInfo.InvariantCulture)}");
        }
        if (!inDetention)
        {
            //console.log("Bully Cooldown");
            waitTime = Random.Range(60f, 120f);
        }
    }
}