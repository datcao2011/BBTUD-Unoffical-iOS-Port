using UnityEngine;

public class ItemDisapperence : MonoBehaviour
{
    public bool disappearIfNotMultiplayer;
    public bool disappearIfMultiplayer;
    public int maxChance;
    public int minChance;

    public float chance = -1f;

    private void Start()
    {
        if (!MultiplayerManager.isActive && disappearIfNotMultiplayer)
        {
            gameObject.SetActive(false);
            return;
        }
        if (MultiplayerManager.isActive && disappearIfMultiplayer)
        {
            gameObject.SetActive(false);
            return;
        }
        chance = Mathf.RoundToInt(Random.Range(1f, (float)maxChance));
        if (chance > (float)minChance)
        {
            gameObject.SetActive(false);
        }
    }
}