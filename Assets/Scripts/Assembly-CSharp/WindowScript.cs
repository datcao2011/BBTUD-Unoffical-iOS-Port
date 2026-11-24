using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WindowScript : MonoBehaviour
{
    public Material Broken;
    public NavMeshObstacle Obstacle;
    public MeshCollider[] MeshColliders;
    public AudioClip WindowBreak;
    [Header("Has Dark")]
    public bool HasDark;
    public Material DarkBroken;

    // Start is called before the first frame update
    private void Start()
    {
        if (NullManager.Instance != null)
        {
            Obstacle.enabled = false;
        }
    }

    public void Break()
    {
        AudioSource source = new GameObject("Crash! " + Random.Range(0, 10000)).AddComponent<AudioSource>();
        source.gameObject.AddComponent<AudioSourceSpeed>();
        source.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
        source.transform.position = transform.position;
        source.spatialBlend = 1f;
        source.dopplerLevel = 0f;
        source.minDistance = 5f;
        source.maxDistance = 100f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.PlayOneShotWC(WindowBreak);
        Destroy(source.gameObject, WindowBreak.length);
        if (HasDark)
        {
            MeshColliders[0].enabled = false;
            MeshColliders[0].GetComponent<MeshRenderer>().material = Broken;
            MeshColliders[1].enabled = false;
            MeshColliders[1].GetComponent<MeshRenderer>().material = DarkBroken;
        }
        else
        {
            foreach (MeshCollider collider in MeshColliders)
            {
                collider.enabled = false;
                if (collider.GetComponent<MeshRenderer>())
                {
                    collider.GetComponent<MeshRenderer>().material = Broken;
                }
            }
        }
        Obstacle.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "NULL")
        {
            Break();
        }
    }
}
