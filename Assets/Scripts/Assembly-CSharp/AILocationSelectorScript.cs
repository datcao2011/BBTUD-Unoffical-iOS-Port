// AILocationSelectorScript
using UnityEngine;

public class AILocationSelectorScript : MonoBehaviour
{
    public Transform[] hallways;
    public Transform[] library;
    public Transform[] rooms;

    public AmbienceScript ambience;

    private int id;

    public Vector3 GetNewTarget()
    {
        id = Mathf.RoundToInt(Random.Range(0f, (hallways.Length - 1) + (rooms.Length - 1)));
        if (id < hallways.Length)
        {
            transform.position = hallways[id].position;
        }
        else
        {
            id -= hallways.Length;
            transform.position = rooms[id].position;
        }
        ambience.PlayAudio();
        return transform.position;
    }

    public Vector3 GetNewTargetHallway()
    {
        id = Mathf.RoundToInt(Random.Range(0f, hallways.Length - 1));
        transform.position = hallways[id].position;
        ambience.PlayAudio();
        return hallways[id].position;
    }

    public Vector3 GetNewTargetTheTest()
    {
        id = Mathf.RoundToInt(Random.Range(0f, library.Length - 1));
        transform.position = library[id].position;
        ambience.PlayAudio();
        return library[id].position;
    }

    public Vector3 GetNewTargetRoom()
    {
        id = Mathf.RoundToInt(Random.Range(0f, rooms.Length - 1));
        transform.position = rooms[id].position;
        ambience.PlayAudio();
        return rooms[id].position;
    }

    // public void GetNewTarget2ndFloor()
    // {
    //     id = Mathf.RoundToInt(Random.Range(32f, 33f));
    //     transform.position = newLocation[id].position;
    //     ambience.PlayAudio();
    // }

    public Vector3 QuarterExclusive()
    {
        id = Mathf.RoundToInt(Random.Range(1f, hallways.Length - 1));
        base.transform.position = hallways[id].position;
        return hallways[id].position;
    }
}