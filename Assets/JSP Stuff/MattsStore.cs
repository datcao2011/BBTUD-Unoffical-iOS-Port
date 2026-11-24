using UnityEngine;
using System.IO;
using SimpleJSON;
using TMPro;
using System.Collections.Generic;

public class MattsStore : MonoBehaviour
{
    private int Bucks;

    public TMP_Text Text;

    public AudioSource Matt;

    private string SaveFilePath;

    public bool Loaded;

    private JSONNode Json;

    private float displayNumber;
    private float deltaSpeed;
    private bool updateShop;

    [Header("Matt's Voicelines")]
    public AudioClip MattWelcome;
    public AudioClip MattPurchase;
    public AudioClip MattNotEnough;

    public Animator MattAnim;

    private bool FirstTime;

    public bool IsOpen;

    private Transform player;

    [Header("Collab Items")]
    [SerializeField]
    private CollabItem[] collabItems;

    [SerializeField]
    private int[] RandomItems;

    [SerializeField]
    private int[] Costs;

    [SerializeField]
    private Sprite[] ItemTextures;

    private void Start()
    {
        if (!IsOpen)
        {
            player = FindObjectOfType<PlayerScript>().transform;
        }
        else
        {
            List<int> ReceivedItems = new List<int>();
            for (int i = 0; i < collabItems.Length; i++)
            {
                int j = Random.Range(0, RandomItems.Length);
                int ID = RandomItems[j];
                while (ReceivedItems.Contains(ID))
                {
                    j = Random.Range(0, RandomItems.Length);
                    ID = RandomItems[j];
                }
                ReceivedItems.Add(ID);
                collabItems[i].Cost = Costs[j];
                collabItems[i].CostText.text = Costs[j].ToString();
                collabItems[i].sprite.sprite = ItemTextures[j];
                collabItems[i].Item = ID;
            }
        }
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Bucks = Mathf.Clamp(CoinSaveSystem.Load(), 0, 9999);
            return;
        }
        try
        {
            AttemptToLoadBaldiBucks();
        }
        catch (System.Exception)
        {
            Bucks = Mathf.Clamp(CoinSaveSystem.Load(), 0, 9999);
            throw;
        }
    }

    private void Update()
    {
        if (updateShop) CoinUpdate();
        if (!IsOpen)
        {
            if (player.position.y >= 20f && player.position.z < -127.5)
            {
                transform.parent.parent.gameObject.SetActive(false);
                ApplicationConfigure.closeApplication();
            }
        }
    }

    private void FixedUpdate()
    {
        MattAnim.SetBool("Talking", Matt.isPlaying);
    }

    private void CoinUpdate()
    {
        Text.text = displayNumber.ToString("0");

        if (displayNumber > Bucks)
        {
            if (deltaSpeed == 0f)
            {
                deltaSpeed = (displayNumber - (float)Bucks) * 2f;
            }
            if ((displayNumber - (float)Bucks) < (Time.deltaTime * deltaSpeed))
            {
                displayNumber = (float)Bucks;
                return;
            }
            displayNumber -= Time.deltaTime * deltaSpeed;
        }
        else if (displayNumber < Bucks)
        {
            if (deltaSpeed == 0f)
            {
                deltaSpeed = ((float)Bucks - displayNumber) * 2f;
            }
            if (((float)Bucks - displayNumber) < (Time.deltaTime * deltaSpeed))
            {
                displayNumber = (float)Bucks;
                return;
            }
            displayNumber += Time.deltaTime * deltaSpeed;
        }
        else
        {
            deltaSpeed = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            if (!IsOpen) // Someone trying to hack inside
            {
                transform.parent.parent.gameObject.SetActive(false);
                return;
            }
            FirstTime = !FirstTime;
            updateShop = FirstTime;
            Matt.Stop();
            if (FirstTime) { Matt.PlayOneShotWC(MattWelcome); }
            if (!FirstTime)
            {
                displayNumber = Bucks;
            }
        }
    }
    private void AttemptToLoadBaldiBucks()
    {
        string path = Application.persistentDataPath;
        if (path.EndsWith("/") || path.EndsWith("\\")) path = path.Substring(0, path.Length - 1);
        path = path.Substring(0, path.Length - (Application.productName.Length + Application.companyName.Length + 1)) + $"JohnsterSpaceGames{Path.DirectorySeparatorChar}Baldi's Fun New School Plus Ultimate Edition{Path.DirectorySeparatorChar}";
        if (!Directory.Exists(path))
        {
            Debug.Log("This user hasn't played BFNSUE+");
            Bucks = 0;
            return;
        }
        SaveFilePath = $"{path}Saves{Path.DirectorySeparatorChar}PlayerSaveFile.bfnsplusue";
        JSONNode json = JSON.Parse(File.ReadAllText(SaveFilePath));
        Bucks = Mathf.Clamp(json["currentBaldiBucks"].AsInt, 0, 9999);
        Loaded = true;
        Json = json;
    }

    public int Get()
    {
        return Bucks;
    }

    public void Buy(int Cost)
    {
        Bucks -= Cost;
        if (Loaded)
        {
            Json["currentBaldiBucks"] = Bucks;
            try
            {
                Json["spentBaldiBucks"] = Json["spentBaldiBucks"].AsInt + Cost;
            }
            catch
            {

            }
            File.WriteAllText(SaveFilePath, Json.ToString());
        }
    }
}