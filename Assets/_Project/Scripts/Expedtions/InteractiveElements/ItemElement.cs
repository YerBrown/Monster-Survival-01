using System;
using System.IO;
using UnityEngine;
public class ItemElement : InteractiveElement
{
    public ItemSlot Item;
    public SpriteRenderer ItemRenderer;
    private void Start()
    {
        ChangeCursorColor("#1EFF00");
    }
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        if (character == null) { return; };
        int remaining = character.PlayerInventory.AddNewItem(Item);
        if (remaining != Item.Amount)
        {
            Debug.Log($"Player has taken {Item.Amount - remaining} {Item.ItemInfo.i_Name}");
            Notify(transform.position, $"+ {Item.Amount - remaining} {Item.ItemInfo.i_Name}");
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
        }
        CheckItemAmount();
    }
    private void CheckItemAmount()
    {
        if (Item.Amount <= 0)
        {
            ItemRenderer.enabled = false;
            EnableElement(false);
        }
        else
        {
            EnableElement(true);
        }
    }
    public override void UpdateElement(ExpeditionData.ParentData data)
    {
        //Check if the instance is of the same type.
        if (data is ExpeditionData.ItemData)
        {
            base.UpdateElement(data);
            if (ItemInfoWiki.Instance != null)
            {
                Item.ItemInfo = ItemInfoWiki.Instance.GetItemByID(((ExpeditionData.ItemData)data).ItemID);
            }
            else
            {
                Debug.LogWarning("Item Info Wiki not found in scene.");
            }
            Item.Amount = ((ExpeditionData.ItemData)data).Amount;
            CheckItemAmount();
        }
        else
        {
            Debug.LogWarning("Can not be updated from another type of element.");
        }
    }
}
