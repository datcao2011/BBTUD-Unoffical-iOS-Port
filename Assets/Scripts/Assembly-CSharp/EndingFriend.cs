using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndingFriend : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private PlayerScript playerScript;

    [SerializeField]
    private Animator ModelAnim;

    private float Speed;

    private bool Billboard = true;

    private Transform m_Camera;

    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Speed = agent.speed;
        agent.speed = 0f;
        m_Camera = Camera.main.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Billboard)
        {
            agent.SetDestination(player.position - (player.forward * 4f));
            agent.speed = Mathf.Clamp(playerScript.rb.velocity.magnitude, 10f, 16F);
            ModelAnim.SetBool("w", agent.velocity.magnitude >= 0.1f);
        }
    }

    private void LateUpdate()
    {
        if (Billboard)
        {
            transform.LookAt(m_Camera.position, new Vector3(0f, 1f, 0f));
            transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
        }
    }

    public void ActivateWalking()
    {
        Billboard = false;
        agent.speed = Speed;
    }
}
