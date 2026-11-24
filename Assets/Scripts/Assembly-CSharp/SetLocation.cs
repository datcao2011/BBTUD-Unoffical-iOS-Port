using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class SetLocation : MonoBehaviour
{
    private NPC agent;

    public GameObject Destination;

    [SerializeField]
    private bool SetYourDestination;

    private Vector3 currentPosition;

    private bool DestinationSet;

    private void Awake()
    {
        if (!Application.isEditor && Application.isPlaying)
        {
            DestroyImmediate(this);
            return;
        }
        if (!Application.isEditor) return;
        agent = GetComponent<NPC>();
        if (Destination == null)
        {
            Destination = new GameObject();
            Destination.transform.position = transform.position;
            Destination.transform.rotation = transform.rotation;
            Destination.name = "Agent Destination";
            Destination.transform.parent = transform;
        }
    }

    private void Update()
    {
        if (agent == null || Destination == null)
        {
            DestroyImmediate(this);
            return;
        }
        if (SetYourDestination)
        {
            if (!DestinationSet)
            {
                DestinationSet = true;
                currentPosition = Destination.transform.position;
            }
            Destination.SetActive(true);
        }
        else
        {
            Destination.SetActive(false);
            if (DestinationSet)
            {
                DestinationSet = false;
                currentPosition = transform.position;
                Destination.transform.position = currentPosition;
            }
            else if (Application.isPlaying & Time.timeScale != 0f)
            {
                if (agent.target.x != Mathf.Infinity)
                {
                    Destination.transform.position = agent.target;
                }
            }
        }
        if (SetYourDestination && Application.isPlaying)
        {
            Destination.transform.position = currentPosition;
            agent.ClearTargetsAndSetTarget(Destination.transform.position);
        }
        if (Vector3.Distance(agent.transform.position, Destination.transform.position) < 3f && Application.isPlaying)
        {
            SetYourDestination = false;
        }
    }

    private void OnDestroy()
    {
        DestroyImmediate(Destination);
    }
}