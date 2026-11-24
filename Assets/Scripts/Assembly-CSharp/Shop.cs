using TMPro;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
    private void Start()
    {
        Status("Shop Manager started");
        StartCoroutine(LoadShop());
    }

    private class SessionResponse
    {
        public bool valid;
        public string error;
        public string username;
    }

    public ShopItem[] JohnnyStoreItems;
    public ShopItem[] shopItems;

    public Animator JohnnyAnim;
    public JohnnyStore Store;
    public TMP_Text JohnnyStoreLabel;
    public string JohnnyStoreText = "<size=18><color=red>Johnny's </color><color=green>Store</color> <color=black>Items</size></color>";
    public bool IsAtJohnny = true;

    [System.Serializable]
    public class ShopItem
    {
        public string name;
        [TextArea]
        public string description;
        public ShopItemType type;
        public Sprite image;
        public int itemId;
        public int cost;
        public int Sale;
    }

    public enum ShopItemType
    {
        Item,
        MysteryDoor
    }

    private string accSessionId;
    private bool receiveOnlineCoins;
    private int onlineCoins;

    public GameObject JohnnyTemplate;
    public GameObject Template;
    public ScrollRect itemList;

    public GameObject CoinDetail;
    public GameObject ExitDetail;
    public GameObject ItemDetail;

    private bool AbortRequest;

    public Button ExitButton;
    public TMP_Text ExitPercentage;
    public ButtonUISelection BackButton;

    public Image ItemImage;
    public TMP_Text ItemTitle;
    public TMP_Text ItemDescription;
    public TMP_Text CoinsDescription;
    public Button PurchaseButton;

    public void Back()
    {
        Transitions.alpha = 1f;
        itemList.gameObject.SetActive(true);
        CoinDetail.SetActive(false);
        ItemDetail.SetActive(false);
        BackButton.gameObject.SetActive(false);
        PurchaseButton.onClick.RemoveAllListeners();
        ExitButton.gameObject.SetActive(true);
    }

    public void ItemDetails(ShopItem item, bool IsFromJohnny = false)
    {
        Transitions.alpha = IsFromJohnny ? 1f : 0f;
        if (IsFromJohnny && !IsAtJohnny)
        {
            Vector3 pos = itemList.content.anchoredPosition;
            pos.y = TestValue - 15f;
            itemList.content.anchoredPosition = pos;
        }
        PurchaseButton.interactable = false;
        itemList.gameObject.SetActive(false);
        ItemDetail.SetActive(true);
        BackButton.gameObject.SetActive(true);
        ExitButton.gameObject.SetActive(false);
        ItemImage.sprite = item.image;
        ItemTitle.text = item.name;
        ItemDescription.text = item.description;
        int Cost = Mathf.RoundToInt(item.cost * ((100f - item.Sale) / 100f));

        string template1 = "You are able to purchase this for {0} coins. Once purchased, you will have {1} coins left.";
        string template2 = "Not enough to buy {0} coins. You need {1} more coins to purchase his item.";
        if (item.Sale > 0) // On sale
        {
            template1 = "<color=red>" + item.Sale + "% OFF SALE, YOU CAN BUY THIS ITEM!</color> " + template1;
            template2 = "<color=red>THIS ITEM IS ON A " + item.Sale + "% SALE!</color> " + template2;
        }

        if (Cost <= Coins)
        {
            CoinsDescription.text = string.Format(template1, NumberWithCommas(Cost), NumberWithCommas(Coins - Cost));
            PurchaseButton.GetComponentInChildren<TMP_Text>().text = $"Purchase ({AbbreviateNumber(Cost)} coins)";
            PurchaseButton.interactable = true;
            PurchaseButton.onClick.AddListener(() =>
            {
                if (Cost > Coins)
                {
                    PurchaseButton.interactable = false;
                    PurchaseButton.GetComponentInChildren<TMP_Text>().text = "Not Enough";
                    InputManager.DisplayException(new System.Exception("Not enough coins to purchase this item."), 5f);
                    return;
                }

                AbortRequest = true;
                if (updateDelay) updateTime = 1f;
                PurchaseButton.interactable = false;
                PurchaseButton.GetComponentInChildren<TMP_Text>().text = "Purchasing";
                if (Cost <= Coins)
                {
                    if (IsFromJohnny)
                    {
                        if (Store.isActiveAndEnabled)
                        {
                            Store.ItemPurchase();
                            if (AchievementManager.Instance != null)
                            {
                                AchievementManager.Instance.Earn(Achievements.ShopItem);
                                if (item.Sale > 0)
                                {
                                    AchievementManager.Instance.Earn(Achievements.JohnnySale);
                                }
                            }
                        }
                    }
                    BackButton.interactable = false;
                    int coins = (Coins - Cost);
                    updateTime = float.PositiveInfinity;
                    CoinSaveSystem.SaveViaShop(coins, (() =>
                    {
                        BackButton.interactable = true;
                        updateTime = 1f;
                        CoinsDescription.text = $"You have purchased this item for {NumberWithCommas(Cost)} coins. ";
                        if (item.type == ShopItemType.Item)
                        {
                            CoinsDescription.text += "The item has been collected and can be seen on your collected items. ";
                            Settings.SetInt("ItemsCollected", Settings.GetInt("ItemsCollected") + 1);
                            Settings.SetInt("ItemCollected" + (Settings.GetInt("ItemsCollected", 0) - 1).ToString(), item.itemId);
                        }
                        else if (item.type == ShopItemType.MysteryDoor)
                        {
                            CoinsDescription.text += "A mystery door should appear on your screen. ";
                            SceneManager.LoadSceneAsync("MysteryRoom", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                        }
                        CoinsDescription.text += "If you want to buy this item again, go back to the list and press on the item again.";
                        Coins -= Cost;
                        PurchaseButton.GetComponentInChildren<TMP_Text>().text = "Purchased";
                        updateTime = 1f;
                    }));
                }
            });
        }
        else
        {
            CoinsDescription.text = String.Format(template2, NumberWithCommas(Cost), NumberWithCommas(Cost - Coins));
            PurchaseButton.GetComponentInChildren<TMP_Text>().text = $"Purchase ({AbbreviateNumber(Cost)} coins)";
            PurchaseButton.interactable = false;
        }
    }

    public void CoinDetails()
    {
        itemList.gameObject.SetActive(false);
        CoinDetail.SetActive(true);
        ItemDetail.SetActive(false);
        PurchaseButton.onClick.RemoveAllListeners();
        BackButton.gameObject.SetActive(true);
        ExitButton.gameObject.SetActive(false);
    }

    public CanvasGroup Transitions;

    public void Exit()
    {
        if (Store.isActiveAndEnabled)
        {
            Store.IsLeaving();
        }
        Transitions.alpha = 0f;
        receiveOnlineCoins = false;
        itemList.gameObject.SetActive(false);
        ExitDetail.SetActive(true);
        ExitButton.interactable = false;
        StartCoroutine(LoadMainMenu());
    }

    private IEnumerator LoadMainMenu()
    {
        ExitButton.gameObject.SetActive(false);
        string TEMPLATE = ExitPercentage.text;
        ExitPercentage.text = TEMPLATE.Replace("{PERCENTAGE}", "0");
        yield return new WaitForSecondsRealtime(0.1f);
        AsyncOperation sceneOperation = SceneManager.LoadSceneAsync("MainMenu");
        while (!sceneOperation.isDone)
        {
            float progress = Mathf.Clamp01(sceneOperation.progress / .9f);
            ExitPercentage.text = TEMPLATE.Replace("{PERCENTAGE}", Mathf.CeilToInt(progress * 100f).ToString());
            yield return null;
        }
        yield break;
    }

    private IEnumerator LoadShop()
    {
        Status("Checking internet reachability");
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Status("Network is not reachable. Skipping internet process");
            StartCoroutine(FinishLoading(true));
            yield break;
        }

        yield return StartCoroutine(ReceiveCoins());

        if (receiveOnlineCoins)
        {
            if (onlineCoins == 500)
            {
                UpdateNumber();
                yield return StartCoroutine(FirstSync());
            }
        }

        StartCoroutine(FinishLoading(false));
        yield break;
    }

    private bool FoundJohnnyItems;

    private IEnumerator FinishLoading(bool SkipNetworkProcess)
    {
        Debug.Log("[Shop] Almost finishing loading");
        Status("Please wait");
        float MaxWaitingTime = Settings.GetBool("SkipUpdatesAndGaps") ? 0f : 3f;
        if (!SkipNetworkProcess)
        {
            StartCoroutine(FetchJohnnyStore());
            while (!FoundJohnnyItems || MaxWaitingTime > 0f)
            {
                MaxWaitingTime -= Time.unscaledDeltaTime;
                yield return null;
            }
        }
        else if (!Settings.GetBool("SkipUpdatesAndGaps"))
        {
            yield return new WaitForSecondsRealtime(3f);
        }
        SetupItems();
        Debug.Log("[Shop] Done");
        Status("You're going in", false);
        if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSeconds(0.4f);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(false);
        UpdateNumber();
        ShopStart();
        yield break;
    }

    private IEnumerator FetchJohnnyStore()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(InputManager.siteRequestUrl + "JohnnyStoreItems?v=" + Application.version);
        webRequest.timeout = 15;
        Debug.Log("[Shop] Fetching Johnny's store");
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[Shop] Unable to fetch Johnny's store: " + webRequest.result + " : " + webRequest.error);
            FoundJohnnyItems = true;
            yield break;
        }
        try
        {
            List<ShopItem> FinishedItems = new List<ShopItem>();
            JSONArray JohnnyStoreItems = JSON.Parse(webRequest.downloadHandler.text)["items"].AsArray;
            foreach (JSONNode Item in JohnnyStoreItems)
            {
                int ItemId = Item["itemId"].AsInt;
                FinishedItems.Add(new ShopItem
                {
                    name = !string.IsNullOrEmpty(Item["title"].Value) ? Item["title"] : ItemNames[ItemId],
                    description = !string.IsNullOrEmpty(Item["description"].Value) ? Item["description"] :  ItemDescriptions[ItemId],
                    image = ItemSprites[ItemId],
                    itemId = ItemId,
                    type = (ShopItemType)Item["type"].AsInt,
                    cost = Item["coins"].AsInt,
                    Sale = Item["sale"].AsInt
                });
            }
            this.JohnnyStoreItems = FinishedItems.ToArray();
            Debug.Log("[Shop] Succesfully fetched Johnny's store");
            FoundJohnnyItems = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("[Shop] Unable to complete Johnny's store items: " + ex);
            FoundJohnnyItems = true;
        }
        yield break;
    }

    [Header("Item Details")]
    public Sprite[] ItemSprites;
    private string[] ItemNames = new string[26]
    {
        "Nothing",
        "Energy flavored Zesty Bar",
        "Swinging Door Lock",
        "Principal's Keys",
        "BSODA",
        "Quarter",
        "Baldi Anti Hearing and Disorienting Tape",
        "Alarm Clock",
        "WD-NoSquee (Door Type)",
        "Safety Scissors",
        "Big Ol' Boots",
        "An Apple for Baldi",
        "Legendary Apple",
        "Teleportation Teleporter",
        "Swinging Door Sale",
        "Pencil",
        "Emergency Health Kit",
        "Principal's Whistle",
        "Class Pass",
        "Secret Formula",
        "Grappling Hook",
        "Portal Poster",
        "Faculty Nametag",
        "Dirty Chalk Eraser",
        "Reflex Hammer",
        "'Nana Peel"
    };
    public string[] ItemDescriptions;

    public void Status(string text, bool log = true)
    {
        if (log) Debug.Log("[Shop] " + text);
        statusText.text = text;
    }

    bool Setup_IsFirstItem = true;

    public void SetupItems()
    {
        foreach (ShopItem ShopItem in JohnnyStoreItems)
        {
            AddJohnnyItem(ShopItem);
        }
        Store.HasOffers = JohnnyStoreItems.Length > 0;
        if (Store.HasOffers)
        {
            JohnnyStoreLabel.text = JohnnyStoreText;
        }
        else
        {
            JohnnyStoreLabel.text = JohnnyStoreText + "\n<color=orange>OUT OF STOCK</color>";
        }
        foreach (ShopItem shopItem in shopItems)
        {
            AddItem(shopItem);
        }
        Destroy(JohnnyTemplate);
        Destroy(Template);
    }

    public void AddJohnnyItem(ShopItem item)
    {
        GameObject createdObject = Instantiate(JohnnyTemplate, itemList.content);
        if (Setup_IsFirstItem)
        {
            Setup_IsFirstItem = false;
        }
        Button button = createdObject.GetComponent<Button>();
        createdObject.transform.GetChild(1).GetComponent<Image>().sprite = item.image;
        createdObject.transform.GetChild(2).GetComponent<TMP_Text>().text = item.name;
        createdObject.transform.GetChild(3).GetComponent<TMP_Text>().text = NumberWithCommas(item.cost) + " coins";
        if (item.Sale > 0)
        {
            int NewCost = Mathf.RoundToInt(item.cost * ((100f - item.Sale) / 100f));
            createdObject.transform.GetChild(2).GetComponent<TMP_Text>().text = item.name + " <color=red>" + item.Sale + "% OFF</color>";
            createdObject.transform.GetChild(3).GetComponent<TMP_Text>().text = $"<s>{NumberWithCommas(item.cost)}</s><color=red>{AbbreviateNumber(NewCost)}</color> coins";
        }
        button.onClick.AddListener(() =>
        {
            ItemDetails(item, true);
            if (Store.isActiveAndEnabled)
            {
                Store.ItemPhrase();
            }
        });
    }

    public void AddItem(ShopItem item)
    {
        GameObject createdObject = Instantiate(Template, itemList.content);
        if (Setup_IsFirstItem)
        {
            Setup_IsFirstItem = false;
        }
        Button button = createdObject.GetComponent<Button>();
        createdObject.transform.GetChild(1).GetComponent<Image>().sprite = item.image;
        createdObject.transform.GetChild(2).GetComponent<TMP_Text>().text = item.name;
        createdObject.transform.GetChild(3).GetComponent<TMP_Text>().text = NumberWithCommas(item.cost) + " coins";
        button.onClick.AddListener(() =>
        {
            ItemDetails(item);
        });
    }

    private IEnumerator ReceiveCoins()
    {
        if (!Settings.HasKey("AccountSession"))
        {
            yield break;
        }
        if (!AccountManager.hasInstance())
        {
            SceneManager.LoadSceneAsync("Logon");
            AccountScript.LOAD_TO_SCENE = "Shop";
            yield break;
        }
        string sessionId = Settings.GetString("AccountSession");
        Status("Checking account coins");
        UnityWebRequest webRequest = UnityWebRequest.Get(InputManager.siteRequestUrl + "coins/get?id=" + sessionId);
        webRequest.timeout = 15;
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    InputManager.DisplayException(new System.Exception("Connection failed | " + webRequest.error));
                    yield break;
                case UnityWebRequest.Result.DataProcessingError:
                    InputManager.DisplayException(new System.Exception("Data Processing failed | " + webRequest.error));
                    yield break;
            }
        }
        if (webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            InputManager.DisplayException(new System.Exception(webRequest.error + ": " + webRequest.downloadHandler.text));
            yield break;
        }
        int toInt;
        bool isNum = int.TryParse(webRequest.downloadHandler.text, out toInt);
        if (!isNum)
        {
            InputManager.DisplayException(new System.Exception(webRequest.downloadHandler.text));
            yield break;
        }
        accSessionId = sessionId;
        onlineCoins = toInt;
        receiveOnlineCoins = true;
        yield break;
    }

    private void DisplayException(Exception ex)
    {
        Debug.LogError("[Shop] An network error occurred: " + ex.Message);
    }

    [System.Serializable]
    private class SyncCoin
    {
        public int coins;
        public string sessionId;
    }

    private IEnumerator FirstSync()
    {
        if (!Settings.HasKey("AccountSession"))
        {
            yield break;
        }
        Status("Syncing to online");
        SyncCoin coinJson = new SyncCoin
        {
            coins = Coins,
            sessionId = accSessionId
        };
        string json = JsonUtility.ToJson(coinJson);
        using (UnityWebRequest webRequest = UnityWebRequest.Put(InputManager.siteRequestUrl + "coins/set", json))
        {
            webRequest.timeout = 15;
            webRequest.SetRequestHeader("Content-Type", "text/plain");
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        InputManager.DisplayException(new System.Exception("Unable to connect to " + webRequest.url + ": " + webRequest.error), 15f, "We could not sync your coins to the server, so your coin balance will remain the same. Congrats on your free item :D");
                        yield break;
                    case UnityWebRequest.Result.DataProcessingError:
                        InputManager.DisplayException(new System.Exception("Receiving data from " + webRequest.url + " encountered an error | " + webRequest.error), 15f, "We could not sync your coins to the server, so your coin balance will remain the same. Congrats on your free item :D");
                        yield break;
                }
            }
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                InputManager.DisplayException(new System.Exception(webRequest.error + ": " + webRequest.downloadHandler.text), 15f, "We could not sync your coins to the server, so your coin balance will remain the same. Congrats on your free item :D");
                yield break;
            }
            int toInt;
            bool isNum = int.TryParse(webRequest.downloadHandler.text, out toInt);
            if (!isNum)
            {
                InputManager.DisplayException(new System.Exception(webRequest.downloadHandler.text), 15f, "We could not sync your coins to the server, so your coin balance will remain the same. Congrats on your free item :D");
                yield break;
            }
            onlineCoins = toInt;
            AccountManager.coins = toInt;
        }
        yield break;
    }

    public int Coins { get; private set; }
    private float displayNumber;

    public TMP_Text coinsText;
    public TMP_Text statusText;

    private bool hl;

    private float deltaSpeed;
    private bool updateShop;

    public float TestValue;

    private void Update()
    {
        if (updateShop)
        {
            ShopUpdate();
            if (receiveOnlineCoins)
            {
                CoinOnlineUpdate();
            }
        }
        TestValue = (Mathf.CeilToInt(JohnnyStoreItems.Length / 3) * 155f) + 5f;
        if (IsAtJohnny && itemList.content.anchoredPosition.y >= TestValue)
        {
            if (Store.isActiveAndEnabled)
            {
                Store.JohnnyVoice.Stop();
            }
            IsAtJohnny = false;
            JohnnyAnim.SetBool("b", !IsAtJohnny);
            JohnnyStoreLabel.text = "SHOP ITEMS";
        }
        else if (!IsAtJohnny && itemList.content.anchoredPosition.y < TestValue)
        {
            IsAtJohnny = true;
            JohnnyAnim.SetBool("b", !IsAtJohnny);
            if (Store.HasOffers) // Text change
            {
                JohnnyStoreLabel.text = JohnnyStoreText;
            }
            else
            {
                JohnnyStoreLabel.text = JohnnyStoreText + "\n<color=orange>OUT OF STOCK</color>";
            }
        }
    }

    [SerializeField]
    private bool updateDelay;
    [SerializeField]
    private float updateTime;

    private void CoinOnlineUpdate()
    {
        if (updateDelay)
        {
            if (updateTime > 0f)
            {
                updateTime -= Time.unscaledDeltaTime;
                if (AbortRequest)
                {
                    AbortRequest = false;
                    updateTime = 1f;
                }
            }
            else
            {
                updateDelay = false;
                StartCoroutine(SyncCoins());
            }
        }
    }

    private IEnumerator SyncCoins()
    {
        string sessionId = accSessionId;
        UnityWebRequest webRequest = UnityWebRequest.Get(InputManager.siteRequestUrl + "coins/get?id=" + sessionId);
        webRequest.timeout = 15;
        webRequest.SendWebRequest();
        while (!webRequest.isDone)
        {
            if (AbortRequest)
            {
                webRequest.Abort();
                updateTime = 1f;
                updateDelay = true;
                yield break;
            }
            yield return null;
        }
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    DisplayException(new System.Exception("Unable to connect to " + webRequest.url + " | " + webRequest.error));
                    updateTime = 15f;
                    updateDelay = true;
                    yield break;
                case UnityWebRequest.Result.DataProcessingError:
                    DisplayException(new System.Exception("Receiving data from " + webRequest.url + " encountered an error | " + webRequest.error));
                    updateTime = 15f;
                    updateDelay = true;
                    yield break;
            }
        }
        if (webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            DisplayException(new System.Exception(webRequest.error + ": " + webRequest.downloadHandler.text));
            updateTime = 15f;
            updateDelay = true;
            yield break;
        }
        int toInt;
        bool isNum = int.TryParse(webRequest.downloadHandler.text, out toInt);
        if (!isNum)
        {
            DisplayException(new System.Exception(webRequest.downloadHandler.text));
            updateTime = 15f;
            updateDelay = true;
            yield break;
        }
        Coins = toInt;
        AccountManager.coins = toInt;
        CoinSaveSystem.Save(Coins, false);
        if (updateTime <= 0f) updateTime = 10f;
        updateDelay = true;
        yield break;
    }

    private void ShopStart()
    {
        UpdateNumber();
        if (receiveOnlineCoins)
        {
            updateTime = 5f;
            updateDelay = true;
        }
        updateShop = true;
    }

    private void ShopUpdate()
    {
        coinsText.text = (hl ? NumberWithCommas(displayNumber) : AbbreviateNumber(displayNumber) + " coins");

        if (displayNumber > Coins)
        {
            if (deltaSpeed == 0f)
            {
                deltaSpeed = (displayNumber - (float)Coins) * 2f;
            }
            if ((displayNumber - (float)Coins) < (Time.deltaTime * deltaSpeed))
            {
                displayNumber = (float)Coins;
                return;
            }
            displayNumber -= Time.deltaTime * deltaSpeed;
        }
        else if (displayNumber < Coins)
        {
            if (deltaSpeed == 0f)
            {
                deltaSpeed = ((float)Coins - displayNumber) * 2f;
            }
            if (((float)Coins - displayNumber) < (Time.deltaTime * deltaSpeed))
            {
                displayNumber = (float)Coins;
                return;
            }
            displayNumber += Time.deltaTime * deltaSpeed;
        }
        else
        {
            deltaSpeed = 0f;
        }
    }

    public void H1()
    {
        hl = true;
    }

    public void H2()
    {
        hl = false;
    }

    public void UpdateNumber()
    {
        Coins = CoinSaveSystem.Load();
    }

    public string NumberWithCommas(float num)
    {
        return string.Format("{0:n0}", num);
    }

    public string AbbreviateNumber(float num)
    {
        if (num >= 1000000)
            return (Mathf.Floor(num / 100000) / 10).ToString("0.0") + "M";
        else if (num >= 1000)
            return (Mathf.Floor(num / 100) / 10).ToString("0.0") + "K";
        else
            return num.ToString("0");
    }
}