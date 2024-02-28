using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ItemElement : InteractiveElement
{
    public ItemSlot Item;
    private void Start()
    {
        ChangeCursorColor("#1EFF00");
    }
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        if (character == null) return;
        int remaining = character.PlayerInventory.AddNewItem(Item);
        if (remaining != Item.Amount)
        {
            Debug.Log($"Player has taken {Item.Amount - remaining} {Item.ItemInfo.i_Name}");
        }
        if (remaining > 0)
        {
            if (remaining == Item.Amount)
            {
                Debug.Log($"No slot free in player inventory");
            }
            else
            {
                Debug.Log($"Not enough space in player inventory, {remaining} still remain");
            }
            Item.Amount = remaining;
        }
        else
        {
            Item.Amount = 0;
            gameObject.SetActive(false);
        }

    }

    public override void UpdateElement(ExpeditionData.ParentData data)
    {
        //Check if the instance is of the same type
        if (data is ExpeditionData.ItemData)
        {
            base.UpdateElement(data);
            Item.ItemInfo = ItemInfoWiki.Instance.GetItemByID(((ExpeditionData.ItemData)data).ItemID);
            Item.Amount = ((ExpeditionData.ItemData)data).Amount;
            if (Item.Amount == 0)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Console.WriteLine("cannot update from a different type element.");

        }
    }
}
