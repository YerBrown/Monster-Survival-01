using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemElement : InteractiveElement
{
    public ItemSlot Item;
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        if (character == null) return;
        int remaining = character.PlayerInventory.AddNewItem(Item);
        if (remaining != Item.Amount)
        {
            Debug.Log($"Player has taken {Item.Amount-remaining} {Item.ItemInfo.i_Name}");
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

            gameObject.SetActive(false);
        }

    }
}
