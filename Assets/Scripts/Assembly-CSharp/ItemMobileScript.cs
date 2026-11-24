using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ItemMobileScript : MonoBehaviour
{
    public GameControllerScript gc;

    public Image itemImage;

    private void Start()
    {
        gc = FindObjectOfType<GameControllerScript>();
    }

    private void Update()
    {
        if (gc.scene == InputManager.IsSchoolScene())
            itemImage.sprite = gc.itemSprites[gc.item[gc.itemSelected]];
    }
}