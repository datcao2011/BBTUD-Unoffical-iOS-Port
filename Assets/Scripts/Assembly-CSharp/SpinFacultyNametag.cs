using TMPro;
using UnityEngine;

public class SpinFacultyNametag : MonoBehaviour
{
    public SpriteRenderer Front;
    public Sprite Front_Blank;
    public TMP_Text Front_Text;
    public float Speed;
    private Transform player;

    private Vector3 Offset = new Vector3(0f, 0.25f, 0f);

    // Start is called before the first frame update
    private void Start()
    {
        player = FindObjectOfType<PlayerScript>().transform;
        if (!string.IsNullOrEmpty(AccountManager.getUsername()))
        {
            Front.sprite = Front_Blank;
            Front_Text.gameObject.SetActive(true);
            Front_Text.text = HasNameTag(AccountManager.getUsername(), AccountManager.UserTag);
        }
    }

    private void LateUpdate()
    {
        Vector3 position = player.position + (player.forward * 2f);
        transform.position = position + Offset;
    }

    private string HasNameTag(string username, string UserTag)
    {
        string color = "";
        switch (UserTag.ToLower())
        {
            case "creator":
                color = "#2fad42";
                break;
            case "helper":
                color = "#2f98ad";
                break;
            case "yt":
                color = "#FF4040";
                break;
            case "supporter":
                color = "yellow";
                break;
            case "dev":
            case "verified":
                color = "blue";
                break;
        }
        return string.IsNullOrEmpty(UserTag) ? username : ("<color=" + color + ">" + username + "</color>");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up, Speed * Time.deltaTime);
    }
}
