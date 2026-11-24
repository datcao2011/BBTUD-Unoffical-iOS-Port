using UnityEngine;
using UnityEngine.UI;

public class TrapScript : MonoBehaviour
{
    public PlayerScript ps;

    public SpriteRenderer spriteImage;

    public Sprite open;

    public Sprite close;

    private float timer;

    private bool trappedIT;

    public bool trapp;

    private bool done2;

    public AudioSource trapSound;

    private void Start()
    {
        base.name = "Trap";
        trapSound = GetComponent<AudioSource>();
        spriteImage = GetComponentInChildren<SpriteRenderer>();
        ps = FindObjectOfType<PlayerScript>();
        base.transform.position = new Vector3((float)UnityEngine.Random.Range(-this.trapSpawner.rangeX, this.trapSpawner.rangeX), transform.position.y, (float)UnityEngine.Random.Range(-this.trapSpawner.rangeZ, this.trapSpawner.rangeZ));
        while (Physics.CheckSphere(base.transform.position, this.separateDistance, 1024, QueryTriggerInteraction.Collide))
        {
            base.transform.position = new Vector3((float)UnityEngine.Random.Range(-this.trapSpawner.rangeX, this.trapSpawner.rangeX), transform.position.y, (float)UnityEngine.Random.Range(-this.trapSpawner.rangeZ, this.trapSpawner.rangeZ));
        }
        timer = 5f;
        spriteImage.sprite = open;
    }

    private void Update()
    {
        if (ps.trapped & trapp)
        {
            if (!trappedIT)
            {
                trappedIT = true;
                spriteImage.sprite = close;
                trapSound.Play();
            }
            timer -= Time.deltaTime;
        }
        if (timer <= 0f & !done2)
        {
            done2 = true;
            trapp = false;
            ps.trapped = false;
            base.gameObject.name = "TrapClosed";
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.name == "Player")
        {
            trapp = true;
        }
    }

    public float separateDistance;

    public TrapSpawnerScript trapSpawner;
}