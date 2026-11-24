using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectedItems : MonoBehaviour
{
    public static CollectedItemToBeUsed[] itemIdsToUse = new CollectedItemToBeUsed[6];

    public static int itemToUse;

    private int itemCollected;

    private int page = 1;

    private CollectedItem[] collectedItems;

    public Sprite[] itemSprites = new Sprite[17];

    public Button[] itemButtons = new Button[9];

    public GameObject noItems;

    public Button pageNextButton;

    public Button pageBackButton;

    public TMP_Text pageNumber;

    public TMP_Text itemsUsed;

    private string[] itemNames = new string[26]
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

    private bool[] toggleSelected;

    private Color highlightedRegular;

    private Color highlightedHighlighted;

    private class CollectedItem
    {
        public CollectedItem(Sprite sprite, string name, int id)
        {
            this.sprite = sprite;
            this.name = name;
            this.id = id;
        }

        public Sprite sprite;

        public string name;

        public int id;
    }

    public class CollectedItemToBeUsed
    {
        public CollectedItemToBeUsed(int id, int cid)
        {
            this.id = id;
            collectedId = cid;
        }

        public int id;

        public int collectedId;
    }

    private void Start()
    {
        ColorUtility.TryParseHtmlString("#f5f5f5", out highlightedRegular);
        ColorUtility.TryParseHtmlString("#14a1a8", out highlightedHighlighted);
        ClearButtons();
        itemIdsToUse[0] = null;
        itemIdsToUse[1] = null;
        itemIdsToUse[2] = null;
        itemIdsToUse[3] = null;
        itemIdsToUse[4] = null;
        itemIdsToUse[5] = null;
        itemToUse = 0;
        int num = 0;
        collectedItems = new CollectedItem[Settings.GetInt("ItemsCollected", 0)];
        toggleSelected = new bool[Settings.GetInt("ItemsCollected", 0)];
        if (Settings.GetInt("ItemsCollected", 0) > 0)
        {
            for (int i = 0; i < Settings.GetInt("ItemsCollected", 0); i++)
            {
                int @int = Settings.GetInt("ItemCollected" + i.ToString());
                if (@int != 0)
                {
                    CreateItem(itemSprites[@int], itemNames[@int], @int);
                }
                else
                {
                    num++;
                }
            }
        }
        else
        {
            noItems.SetActive(true);
            GetComponent<Animator>().SetTrigger("dontanimate");
        }
        if (num > 0)
        {
            bool flag = false;
            List<int> CollectedItemsList = new List<int>();
            List<int> PreviousItemsList = new List<int>();
            for (int j = 0; j < Settings.GetInt("ItemsCollected"); j++)
            {
                int Item = Settings.GetInt("ItemCollected" + j.ToString());
                PreviousItemsList.Add(Item);
                if (Item != 0)
                {
                    flag = true;
                    CollectedItemsList.Add(Item);
                }
            }
            for (int j = 0; j < Settings.GetInt("ItemsCollected"); j++)
            {
                if (j >= CollectedItemsList.Count)
                {
                    Settings.DeleteKey("ItemCollected" + j.ToString());
                }
                else if (PreviousItemsList[j] != CollectedItemsList[j])
                {
                    Settings.SetInt("ItemCollected" + j.ToString(), CollectedItemsList[j]);
                }
            }
            Settings.SetInt("ItemsCollected", CollectedItemsList.Count);
            if (!flag)
            {
                Settings.SetInt("ItemsCollected", 0);
                noItems.SetActive(true);
            }
            else
            {
                noItems.SetActive(false);
            }
        }
        SetupButtons(1);
    }

    public void PreviousPage()
    {
        ClearButtons();
        page--;
        if (page <= 1)
        {
            pageBackButton.interactable = false;
        }
        pageNumber.text = page.ToString();
        SetupButtons(page);
    }

    public void NextPage()
    {
        ClearButtons();
        page++;
        if (page > 1)
        {
            pageBackButton.interactable = true;
        }
        pageNumber.text = page.ToString();
        SetupButtons(page);
    }

    private void ClearButtons()
    {
        foreach (Button button in itemButtons)
        {
            button.transform.GetChild(0).GetComponent<Image>().sprite = itemSprites[0];
            button.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            button.interactable = false;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = highlightedRegular;
            colors.pressedColor = Color.white;
            colors.selectedColor = highlightedRegular;
            button.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.black;
            button.colors = colors;
            button.onClick.RemoveAllListeners();
        }
    }

    private void CreateItem(Sprite ImageSprite, string ItemName, int ItemId)
    {
        collectedItems[itemCollected] = new CollectedItem(ImageSprite, ItemName, ItemId);
        itemCollected++;
    }

    private void SetupButtons(int page)
    {
        int num = page * 9 - 9;
        int num2 = page * 9;
        pageNextButton.interactable = (num2 < itemCollected);
        int num3 = num;
        while (num < num2 && num3 < itemCollected && num3 < num2)
        {
            int b = num3 - num;
            SetButton(num3, b);
            num3++;
        }
    }

    private void SetButton(int i, int b)
    {
        Button button = itemButtons[b];
        CollectedItem item = collectedItems[i];
        if (toggleSelected[i])
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.cyan;
            colors.highlightedColor = highlightedHighlighted;
            colors.pressedColor = Color.cyan;
            colors.selectedColor = highlightedHighlighted;
            button.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
            button.colors = colors;
        }
        button.transform.GetChild(0).GetComponent<Image>().sprite = item.sprite;
        button.transform.GetChild(1).GetComponent<TMP_Text>().text = item.name;
        button.interactable = true;
        button.onClick.AddListener(delegate
        {
            toggleSelected[i] = !toggleSelected[i];
            if (!toggleSelected[i])
            {
                ColorBlock colors2 = button.colors;
                colors2.normalColor = Color.white;
                colors2.highlightedColor = highlightedRegular;
                colors2.pressedColor = Color.white;
                colors2.selectedColor = highlightedRegular;
                button.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.black;
                button.colors = colors2;
                RemoveItem(i);
                return;
            }
            if (itemToUse >= 6)
            {
                toggleSelected[i] = false;
                //InputManager.DisplayEvent("Max picked collected items of 6 exceeded. To pick that collected item, remove one of more picked collected item(s).", 7f);
                return;
            }
            ColorBlock colors3 = button.colors;
            colors3.normalColor = Color.cyan;
            colors3.highlightedColor = highlightedHighlighted;
            colors3.pressedColor = Color.cyan;
            colors3.selectedColor = highlightedHighlighted;
            button.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
            button.colors = colors3;
            AddItem(i, item.id);
        });
    }

    private void AddItem(int cid, int id)
    {
        itemIdsToUse[itemToUse] = new CollectedItemToBeUsed(id, cid);
        itemToUse++;
        itemsUsed.text = "Used " + itemToUse.ToString() + " of 6";
    }

    private void RemoveItem(int cid)
    {
        for (int i = 0; i < itemToUse; i++)
        {
            if (itemIdsToUse[i].collectedId == cid)
            {
                CollectedItemToBeUsed cITBU = itemIdsToUse[i];
                CollectedItemToBeUsed[] array = (from val in itemIdsToUse where val != cITBU select val).ToArray();
                itemIdsToUse[0] = null;
                itemIdsToUse[1] = null;
                itemIdsToUse[2] = null;
                itemIdsToUse[3] = null;
                itemIdsToUse[4] = null;
                itemIdsToUse[5] = null;
                itemIdsToUse = new CollectedItemToBeUsed[6];
                for (int j = 0; j < array.Length; j++)
                {
                    itemIdsToUse[j] = array[j];
                }
                itemToUse--;
                itemsUsed.text = "Used " + itemToUse.ToString() + " of 6";
                return;
            }
        }
    }
}
