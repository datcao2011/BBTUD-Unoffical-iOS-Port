using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    private NavMeshAgent Agent;
    private AILocationSelectorScript wanderer;
    public AnimalType type = AnimalType.Chicken;
    private AnimalFarm controller;

    // Start is called before the first frame update
    private void Start()
    {
        wanderer = FindObjectOfType<AILocationSelectorScript>();
        controller = FindObjectOfType<AnimalFarm>();
        Agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerScript>().transform;
        Wander();
        playerScript = FindObjectOfType<PlayerScript>();
    }
    private float wanderDelay;

    public bool captured;
    [HideInInspector]
    public int id;
    public bool beingHeld;

    public Transform player;

    private GameObject click;
    public GameObject clickCursor;
    public SpriteRenderer spriteRenderer;
    private PlayerScript playerScript;
    private float y;

    // Update is called once per frame
    private void Update()
    {
        if (!beingHeld)
        {
            if (Agent.velocity.magnitude < 0.1f)
            {
                if (wanderDelay > 0f)
                {
                    wanderDelay -= Time.deltaTime;
                }
                else
                {
                    Wander();
                }
            }
        }
        else
        {
            transform.position = player.position + (player.forward * 4f);
            controller.ShowTimer = 30f;
        }
        if (InteractScript.Intracting && Time.timeScale != 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == base.transform & Vector3.Distance(player.position, base.transform.position) < 10f)
                {
                    if (click != null) { Destroy(click); }
                    if (captured)
                    {
                        return;
                    }
                    Vector3 newPos = Vector3.zero;
                    if (!beingHeld)
                    {
                        y = transform.position.y;
                    }
                    else
                    {
                        newPos = player.transform.position;
                        newPos.y = y;
                    }
                    beingHeld = !beingHeld;
                    if (!beingHeld && playerScript.isInBarn && controller.IsAcceptedAnimal(this))
                    {
                        Agent.Warp(newPos);
                        captured = true;
                    }
                    else
                    {
                        transform.position = player.position + (player.forward * 2f);
                        controller.ShowTimer = 30f;

                        FindObjectOfType<DoorIncidator>().Hide();
                        FindObjectOfType<DoorIncidator>().transform.parent = null;
                    }
                }
            }
        }
        if (captured) return;
        Ray ray2 = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit hit2;
        if (Physics.Raycast(ray2, out hit2))
        {
            if (hit2.transform == transform & Vector3.Distance(player.position, base.transform.position) < 10f)
            {
                if (click == null)
                {
                    click = Instantiate(clickCursor);
                    Destroy(click, 13f);
                }
            }
            else
            {
                if (click != null)
                    Destroy(click);
            }
        }

    }

    private void Wander()
    {
        wanderDelay = Random.Range(7f, 25f);
        if (!captured)
        {
            wanderer.GetNewTargetHallway();
        }
        else
        {
            wanderer.GetNewTargetRoom();
        }
        Agent.SetDestination(wanderer.transform.position);
    }
}
