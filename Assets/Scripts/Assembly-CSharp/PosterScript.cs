using UnityEngine;

public class PosterScript : MonoBehaviour
{
    [SerializeField]
    private Material PC;

    [SerializeField]
    private Material Touch;

    // Start is called before the first frame update
    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (!MultiplayerManager.isActive && NullManager.Instance == null && BirthdayBash.Instance == null)
        {
            if (InputManager.usingTouch)
            {
                renderer.material = Touch;
            }
            else if (!Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
            {
                renderer.material = PC;
            }
        }
        Destroy(this);
    }
}
