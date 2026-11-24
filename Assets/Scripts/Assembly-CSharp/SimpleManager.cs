using UnityEngine;

public class SimpleManager : MonoBehaviour
{
    public GameObject MainControls;
    public GameObject BaldiControls;
    public GameObject FirstControls;
    public GameObject MPControls;

    public void OnDropdown(int b)
    {
        MainControls.SetActive(b <= 1);
        BaldiControls.SetActive(b == 2);
        FirstControls.SetActive(b == 0);
        MPControls.SetActive(b == 1);
    }

    public void Reset()
    {
        MBPlacement[] placements = FindObjectsOfType<MBPlacement>();
        foreach (MBPlacement placement in placements)
        {
            placement.DefaultPlacement();
        }
    }
}