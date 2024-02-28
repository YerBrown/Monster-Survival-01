using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerElement : InteractiveElement
{
    public Inventory ContainerInventory;
    public bool Opened = false;

    public SpriteRenderer Renderer;

    public Sprite FullContainer;
    public Sprite EmptyContainer;
    private void Start()
    {
        ChangeCursorColor("#00A0FF");
    }
    public override void Interact(CharacterInfo characterInfo = null)
    {
        base.Interact();
        if (characterInfo == null) return;
        characterInfo.OpenContainer(ContainerInventory);
        if (EmptyContainer != null)
            Renderer.sprite = EmptyContainer;
        Opened = true;
    }
    public override void UpdateElement(ExpeditionData.ParentData data)
    {
        //Check if the instance is of the same type
        if (data is ExpeditionData.ContainerData)
        {
            base.UpdateElement(data);
            ContainerInventory = GetInventoryByData((ExpeditionData.ContainerData)data);
            Opened = ((ExpeditionData.ContainerData)data).Opened;
            if (Opened)
            {
                if (EmptyContainer != null)
                    Renderer.sprite = EmptyContainer;
            }
        }
        else
        {
            Console.WriteLine("cannot update from a different type element.");

        }
    }

    private Inventory GetInventoryByData(ExpeditionData.ContainerData data)
    {
        Inventory newInventory = new Inventory();
        newInventory.Inv_Name = data.Inv_Name;
        newInventory.MaxSlots = data.MaxSlots;
        newInventory.FlexibleSlots = data.FlexibleSlots;
        newInventory.OnlyRemoveItems = data.OnlyRemoveItems;
        if (ItemInfoWiki.Instance == null)
        {
            Debug.LogWarning("Item info wiki missing!");
            return newInventory;
        }
        foreach (var itemSlot in data.AllItems)
        {
            ItemSlot newSlot = new ItemSlot(ItemInfoWiki.Instance.GetItemByID(itemSlot.ItemID), itemSlot.Amount);
            newInventory.AddNewItem(newSlot, false);
        }
        return newInventory;
    }
}
