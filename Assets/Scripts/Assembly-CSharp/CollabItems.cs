using System;
using UnityEngine;

public class CollabItems : MonoBehaviour
{
    public static bool Active;
    public static CollabItems Instance;

    private bool[] Collabs = new bool[6];

    [SerializeField]
    private Texture[] NewItemTextures = new Texture[26];

    public GameControllerScript GameController;

    [SerializeField]
    private AudioClip[] ItemUses;

    private void Awake()
    {
        Active = true;
        Instance = this;
    }

    private void OnDestroy()
    {
        Active = false;
        Instance = null;
    }

    public void ClearCollabItem(int slot)
    {
        Collabs[slot] = false;
    }

    public void Collect(int item)
    {
        int Slot = GetItemSlot();
        GameController.CollectItem(item, GameController.playerTransform.position);
        Collabs[Slot] = true;
        LateUpdate();
    }

    private int GetItemSlot()
    {
        if (GameController.item[0] == 0)
        {
            return 0;
        }
        else if (GameController.item[1] == 0)
        {
            return 1;
        }
        else if (GameController.item[2] == 0)
        {
            return 2;
        }
        else if (GameController.item[3] == 0)
        {
            return 3;
        }
        else if (GameController.item[4] == 0)
        {
            return 4;
        }
        else if (GameController.item[5] == 0)
        {
            return 5;
        }
        else
        { return GameController.itemSelected; }
    }

    public void CheckForUseItem(int slot, int item)
    {
        if (!Collabs[slot]) return;
        if ((item == 1 || item == 9) && GameController.exitsReached <= 0)
        {
            GameController.audioDevice.Stop();
        }
        if (item == 1)
        {
            GameController.audioDevice.PlayOneShotWC(ItemUses[0]);
        }
        if (item == 9)
        {
            GameController.audioDevice.PlayOneShotWC(ItemUses[1]);
        }
        ClearCollabItem(slot);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < 6; i++)
        {
            if (Collabs[i] && NewItemTextures[GameController.item[i]] != null)
            {
                GameController.itemSlot[i].texture = NewItemTextures[GameController.item[i]];
            }
        }
    }
}