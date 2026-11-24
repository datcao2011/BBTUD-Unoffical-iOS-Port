using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class LockDownDoor : MonoBehaviour
{
    public bool busy;

    public AudioSource audioDevice;

    public AudioClip door_loop;

    public AudioClip door_shut;

    public bool opened;


    public float strength = 10f;

    private float damage;

    public bool closed;

    [HideInInspector]
    public bool broken;

    public GameObject guy;

    private NavMeshObstacle obstacle;

    private MeshCollider[] wallColliders;
    private CapsuleCollider[] handleColliders;

    private void Start()
    {
        obstacle = gameObject.GetComponentInChildren<NavMeshObstacle>();
        MeshCollider[] meshColliders = transform.GetChild(0).GetComponentsInChildren<MeshCollider>();
        CapsuleCollider[] capsuleColliders = transform.GetChild(0).GetChild(5).GetComponentsInChildren<CapsuleCollider>();

        wallColliders = new MeshCollider[meshColliders.Length];
        wallColliders = meshColliders;

        handleColliders = new CapsuleCollider[capsuleColliders.Length];
        handleColliders = capsuleColliders;
    }

    private void Update()
    {
        if (guy.activeSelf & (base.transform.position - guy.transform.position).magnitude <= 50f & !broken)
        {
            damage += Time.deltaTime * 4f;
            if (damage < strength)
            {
                return;
            }
            if (!broken)
            {
                broken = true;
                audioDevice.PlayOneShot(door_shut);
            }
            obstacle.enabled = false;
            AllowCollide(true);
        }
        else if (!guy.activeSelf && !obstacle.enabled & broken)
        {
            damage = 0f;
            broken = false;
            obstacle.enabled = true;
            AllowCollide(false);
        }
    }

    public void AllowCollide(bool collide)
    {
        foreach (MeshCollider meshCollider in wallColliders)
        {
            meshCollider.convex = true;
            meshCollider.isTrigger = collide;
        }
        foreach (CapsuleCollider meshCollider in handleColliders)
        {
            meshCollider.isTrigger = collide;
        }
    }

    public void Open()
    {
        if (opened)
        {
            return;
        }
        StartCoroutine(OpenWaiter(true, Random.Range(0.9f, 4f)));

    }

    public IEnumerator openDoor(bool open, float speed)
    {
        audioDevice.clip = door_loop;
        audioDevice.loop = true;
        audioDevice.Play();
        if (open)
        {
            while (base.transform.position.y < 14.5f)
            {
                Vector3 position = base.transform.position;
                position.y += Time.deltaTime * speed;
                base.transform.position = position;
                yield return null;
            }
            audioDevice.Stop();
            audioDevice.clip = null;
            audioDevice.loop = false;
            Vector3 pos = base.transform.position;
            pos.y = 14.5f;
            opened = true;
            base.transform.position = pos;
        }
        else
        {
            while (base.transform.position.y > 5f)
            {
                Vector3 position = base.transform.position;
                position.y -= Time.deltaTime * speed;
                base.transform.position = position;
                yield return null;
            }
            audioDevice.Stop();
            audioDevice.clip = null;
            audioDevice.loop = false;
            audioDevice.PlayOneShot(door_shut);
            closed = true;
            Vector3 pos = base.transform.position;
            pos.y = 5f;
            base.transform.position = pos;
        }
        busy = false;
        yield break;
    }

    public void Close()
    {
        if (closed)
        {
            return;
        }
        StartCoroutine(OpenWaiter(false, Random.Range(0.9f, 4f)));
    }

    public void InstantShut()
    {
        if (closed)
        {
            return;
        }
        StartCoroutine(OpenWaiter(false, 5030f));
    }

    public void InstantOpen()
    {
        if (opened)
        {
            return;
        }
        StartCoroutine(OpenWaiter(true, 5030f));
    }

    private IEnumerator OpenWaiter(bool open, float speed)
    {
        while (busy)
        {
            yield return null;
        }
        if (open)
        {
            closed = false;
        }
        else
        {
            opened = false;
        }
        busy = true;
        StartCoroutine(openDoor(open, speed));
    }
}