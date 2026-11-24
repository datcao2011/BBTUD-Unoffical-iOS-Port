using UnityEngine;

public class CoinButtonScript : MonoBehaviour
{
    private CoinScript coin;

    [SerializeField]
    private string buyName;

    [SerializeField]
    private int buyCost;

    private void Start()
    {
        coin = FindObjectOfType<CoinScript>();
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Buy);
    }

    public void Buy()
    {
        coin.Buy(buyName, buyCost);
    }
}