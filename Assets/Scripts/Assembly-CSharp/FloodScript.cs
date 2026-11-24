using UnityEngine;

public class FloodScript : MonoBehaviour
{
    [SerializeField]
    private RandomEvent events;

    public bool started;

    private AudioSource audioDevice;

    [SerializeField]
    private AudioClip Normal;
    [SerializeField]
    private AudioClip Underwater;

    private MeshRenderer meshRenderer;

    [SerializeField]
    private MeshRenderer current;

    [SerializeField]
    private MeshRenderer under;

    private bool _LastUnderwaterDetected;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        meshRenderer.material.mainTextureOffset += new Vector2(0.2f * Time.deltaTime, 0.2f * Time.deltaTime);
        if (events.eventEndTimer > 0f & transform.position.y < -20f)
        {
            Vector3 movePosition = transform.position;
            movePosition.y = movePosition.y + 0.9f * Time.deltaTime;
            transform.position = movePosition;
        }
        else if (events.eventEndTimer > 0f)
        {
            Vector3 movePosition = transform.position;
            movePosition.y = -20f;
            transform.position = movePosition;
        }
        if (events.eventEndTimer <= 0f & transform.position.y > -23f)
        {
            Vector3 movePosition = transform.position;
            movePosition.y = movePosition.y - 0.9f * Time.deltaTime;
            transform.position = movePosition;
        }
        else if (events.eventEndTimer <= 0f)
        {
            gameObject.SetActive(false);
            Vector3 movePosition = transform.position;
            movePosition.y = -23f;
            transform.position = movePosition;
        }
        // Audio
        if (events.fogScript.FloodFog != _LastUnderwaterDetected && audioDevice != null)
        {
            if (events.fogScript.FloodFog)
            {
                audioDevice.clip = Underwater;
                audioDevice.Play();
                meshRenderer = under;
            }
            else
            {
                audioDevice.clip = Normal;
                audioDevice.Play();
                meshRenderer = current;
            }
        }
        _LastUnderwaterDetected = events.fogScript.FloodFog;
    }

    public void StartEvent()
    {
        base.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        started = true;
        audioDevice = GetComponent<AudioSource>();
        audioDevice.Play();
    }

    private void OnDisable()
    {
        started = false;
        audioDevice.Stop();
    }
}