using UnityEngine;
using UnityEngine.SceneManagement;

public class HALP : MonoBehaviour
{
    private MeshRenderer[] renderers;

    private float time = 1f;
    private float Duration = 0f;
    private float TimeTo = 1f;

    public Material DSCI_0000;

    public AudioSource Music;

    private int Index;

    [SerializeField]
    private TheWeirdestEndingEver Ending;

    private void Awake()
    {
        renderers = FindObjectsOfType<MeshRenderer>();
        TimeTo = 1f;
        Index = 0;
    }

    private void Update()
    {
        Duration += Time.unscaledDeltaTime;
        if (Duration >= 5f) TimeTo = 0.1f;
        if (Duration >= 13f) TimeTo = 0.02f;
        if (Duration >= 18f) TimeTo = 0f;
        if (!Music.isPlaying || Music.time >= 162.32f)
        {
            Ending.Next();
        }
        if (time > 0f)
        {
            time -= Time.unscaledDeltaTime;
        }
        else
        {
            time = TimeTo;
            if (Index >= renderers.Length)
            {
                Duration = -999f;
                return;
            }
            renderers[Index].material = DSCI_0000;
            Index++;
        }
    }
}