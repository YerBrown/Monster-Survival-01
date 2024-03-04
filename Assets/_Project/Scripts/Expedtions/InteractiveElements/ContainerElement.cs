using System;
using UnityEngine;
public class ContainerElement : InteractiveElement
{
    public Inventory ContainerInventory;
    public bool IsOpened = false;
    [Header("")]
    public SpriteRenderer Renderer;
    public Sprite FullSprite;
    public Sprite EmptySprite;
    private void Start()
    {
        ChangeCursorColor("#00A0FF");
    }
    public override void Interact(CharacterInfo characterInfo = null)
    {
        base.Interact();
        if (characterInfo == null) { return; };
        characterInfo.OpenContainer(ContainerInventory);
        ChangeOpenedState(true);
    }
    public override void UpdateElement(ExpeditionData.ParentData data)
    {
        //Check if the instance is of the same type.
        if (data is ExpeditionData.ContainerData)
        {
            base.UpdateElement(data);
            ContainerInventory = GetInventoryByData((ExpeditionData.ContainerData)data);
            bool isOpened = ((ExpeditionData.ContainerData)data).Opened;
            ChangeOpenedState(isOpened);
        }
        else
        {
            Console.WriteLine("Can not be updated from another type of element.");
        }
    }
    // Returns the inventory from a container data.
    private Inventory GetInventoryByData(ExpeditionData.ContainerData data)
    {
        Inventory newInventory = new();
        newInventory.Inv_Name = data.Inv_Name;
        newInventory.MaxSlots = data.MaxSlots;
        newInventory.FlexibleSlots = data.FlexibleSlots;
        newInventory.OnlyRemoveItems = data.OnlyRemoveItems;

        if (ItemInfoWiki.Instance != null)
        {
            // Adds all items in data to real inventory.
            foreach (var itemSlot in data.AllItems)
            {
                ItemSlot newSlot = new ItemSlot(ItemInfoWiki.Instance.GetItemByID(itemSlot.ItemID), itemSlot.Amount);
                newInventory.AddNewItem(newSlot, false);
            }
        }
        else
        {
            Debug.LogWarning("Item Info Wiki not found in scene.");
        }
        return newInventory;
    }
    private void ChangeOpenedState(bool isOpened)
    {
        IsOpened = isOpened;
        if (IsOpened)
        {
            if (EmptySprite != null)
            {
                Renderer.sprite = EmptySprite;
            }
        }
        else
        {
            if (EmptySprite != null)
            {
                Renderer.sprite = FullSprite;
            }
        }
    }
}
