// TapePlayerScript
using Multiplayer;
using UnityEngine;

public class TapePlayerScript : MonoBehaviour
{
    public Sprite closedSprite;
    private Sprite openSprite;

    public SpriteRenderer sprite;

    public BaldiScript baldi;

    private AudioSource audioDevice;

    private NullScript NULL;
    private bool isNULL;

    private void Start()
    {
        audioDevice = base.GetComponent<AudioSource>();
        openSprite = sprite.sprite;
    }

    private void Update()
    {
        if (audioDevice.isPlaying)
        {
            if (Time.timeScale == 0f)
            {
                audioDevice.Pause();
            }
        }
        else if (Time.timeScale > 0f)
        {
            if ((isNULL && NULL.antiHearingTime > 0f) || (!isNULL && baldi.antiHearingTime > 0f))
            {
                audioDevice.UnPause();
            }
        }
    }

    public void Play()
    {
        sprite.sprite = closedSprite;
        audioDevice.maxDistance = 20000f;
        audioDevice.minDistance = 10000f;
        audioDevice.Play();
        if (baldi.isActiveAndEnabled)
        {
            baldi.ActivateAntiHearing(30f);
            if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi)
            {
                Multiplayer.ClientSend.SendActionToOtherPlayer("antihearing:" + ClientHandle.GetIdFromPosition<TapePlayerScript>(transform.position));
            }
        }
        if (NullManager.Instance != null)
        {
            NullManager.Instance.NULL.ActivateAntiHearing(30f);
            isNULL = true;
            NULL = NullManager.Instance.NULL;
        }
        Invoke("SpriteDelay", 30f);
    }

    private void SpriteDelay()
    {
        sprite.sprite = openSprite;
    }

    public void Play_MP()
    {
        sprite.sprite = closedSprite;
        audioDevice.maxDistance = 20000f;
        audioDevice.minDistance = 10000f;
        audioDevice.Play();
        if (baldi.isActiveAndEnabled)
        {
            baldi.ActivateAntiHearing(30f);
        }
        Invoke("SpriteDelay", 30f);
    }
}