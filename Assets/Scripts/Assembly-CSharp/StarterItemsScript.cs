using UnityEngine;

public class StarterItemsScript : MonoBehaviour
{
    // public Sprite[] itemSprites = new Sprite[17];

    // public GameObject prizePrefab;

    // public int itemCount;

    // public Transform parent;

    // public GameObject button;

    // private string[] itemNames = new string[17]
    // {
    //     "Nothing",
    //     "Energy flavored Zesty Bar",
    //     "Yellow Door Lock",
    //     "Principal's Keys",
    //     "BSODA",
    //     "Quarter",
    //     "Baldi Anti Hearing and Disorienting Tape",
    //     "Alarm Clock",
    //     "WD-NoSquee (Door Type)",
    //     "Safety Scissors",
    //     "Big Ol' Boots",
    //     "Apple",
    //     "Legendary Apple",
    //     "Teleportation Teleporter",
    //     "Swinging Door Sale",
    //     "Pencil",
    //     "Emergency Health Kit"
    // };

    // private void Start()
    // {
    //     int removeCounts = 0;
    //     if (Settings.GetInt("ItemsCollected", 0) > 0)
    //     {
    //         GetComponent<UnityEngine.UI.RawImage>().enabled = true;
    //         button.SetActive(true);
    //         for (int i = 0; i < Settings.GetInt("ItemsCollected", 0); i++)
    //         {
    //             int slot = Settings.GetInt("ItemCollected" + i);
    //             if (slot != 0)
    //             {
    //                 CreateItem(itemSprites[slot], itemNames[slot]);
    //             }
    //             else
    //             {
    //                 removeCounts++;
    //             }
    //         }
    //     }
    //     else
    //     {
    //         parent.parent.parent.gameObject.SetActive(false);
    //     }
    //     if (removeCounts > 0)
    //     {
    //         bool isNotEmpty = false;
    //         removeCounts = 0;
    //         for (int i = 1; i < Settings.GetInt("ItemsCollected"); i++)
    //         {
    //             if (Settings.GetInt("ItemCollected" + i) == 0)
    //             {
    //                 Settings.DeleteKey("ItemCollected" + i);
    //                 removeCounts++;
    //             }
    //             else
    //             {
    //                 isNotEmpty = true;
    //                 Settings.SetInt("ItemCollected" + (i - 1), Settings.GetInt("ItemCollected" + i));
    //             }
    //         }

    //         Settings.SetInt("ItemsCollected", Settings.GetInt("ItemsCollected") - removeCounts);
    //         if (!isNotEmpty)
    //         {
    //             Settings.SetInt("ItemsCollected", 0);
    //             parent.parent.parent.gameObject.SetActive(false);
    //             button.SetActive(false);
    //             GetComponent<UnityEngine.UI.RawImage>().enabled = false;
    //         }
    //     }
    // }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         parent.parent.parent.gameObject.SetActive(false);
    //         button.SetActive(false);
    //         GetComponent<UnityEngine.UI.RawImage>().enabled = false;
    //     }
    // }

    // private void CreateItem(Sprite ImageSprite, string ItemName)
    // {
    //     Vector3 pos = prizePrefab.GetComponent<RectTransform>().anchoredPosition;


    //     if (itemCount > 0)
    //     {
    //         pos.x = buttons[itemCount - 1].transform.position.x + 300f;
    //     }
    //     GameObject item = Instantiate(prizePrefab);
    //     item.GetComponent<RectTransform>().anchoredPosition = pos;
    //     item.transform.SetParent(parent);
    //     item.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ChooseToggle(item.GetComponent<UnityEngine.UI.Button>(), item.GetComponent<ItemSelector>()));
    //     item.GetComponent<ItemSelector>().itemData = itemCount;
    //     item.GetComponentInChildren<TMPro.TMP_Text>().text = ItemName;
    //     item.GetComponentInChildren<UnityEngine.UI.Image>().sprite = ImageSprite;
    //     item.transform.localScale = new Vector3(1f, 1f, 1f);
    //     buttons[itemCount] = item.GetComponentInChildren<UnityEngine.UI.Image>();
    //     itemCount++;
    // }

    // public void ChooseToggle(UnityEngine.UI.Button currentButton, ItemSelector itemSelector)
    // {
    //     itemSelector.isSelected = !itemSelector.isSelected;
    //     if (itemSelector.isSelected & itemSelected < 6)
    //     {
    //         currentButton.image.color = Color.green;
    //         itemSelector.item = itemSelected;
    //         Settings.SetInt("ItemSaved" + itemSelected, Settings.GetInt("ItemCollected" + itemSelector.itemData));
    //         itemSelected++;
    //     }
    //     else
    //     {
    //         currentButton.image.color = Color.white;
    //         Settings.DeleteKey("ItemSaved" + itemSelector.item);
    //         for (int i = itemSelector.item + 1; i < itemSelected; i++)
    //         {
    //             Settings.SetInt("ItemSaved" + (itemSelected - 1), Settings.GetInt("ItemSaved" + i));
    //         }

    //         itemSelected--;
    //     }
    // }

    // public void SaveSlots()
    // {
    //     parent.parent.parent.gameObject.SetActive(false);
    //     button.SetActive(false);
    //     GetComponent<UnityEngine.UI.RawImage>().enabled = false;
    // }

    // int itemSelected;

    // [HideInInspector]
    // private UnityEngine.UI.Image[] buttons = new UnityEngine.UI.Image[500];
}