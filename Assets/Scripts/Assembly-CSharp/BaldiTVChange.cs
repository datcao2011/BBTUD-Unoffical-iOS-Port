using UnityEngine;

public class BaldiTVChange : MonoBehaviour
{
    [SerializeField]
    private GameControllerScript gc;

    private void OnEnable()
    {
        if (Settings.GetInt("TVHud") < 2)
        {
            gc.TV = GetComponent<BaldiTV>();
        }
    }
}