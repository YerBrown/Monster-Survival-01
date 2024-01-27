using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[Serializable]
public class Inventory
{
    public List<ItemSlot> Slots = new List<ItemSlot>();
    public int MaxSlots;
    private ItemSlot CheckItemInInventory(ItemSlot slot)
    {
        for (int i = Slots.Count - 1; i >= 0; i--)
        {
            if (slot.ItemInfo == Slots[i].ItemInfo)
            {
                return Slots[i];
            }
        }
        return null;
    }
    private int TryAddNewSlot(ItemSlot newItem)
    {
        if (Slots.Count == MaxSlots)
        {
            //Trigger evento fin
            return newItem.Amount;
        }
        else
        {
            int newAmount = newItem.Amount;
            ItemSlot addedSlot = new ItemSlot(newItem.ItemInfo, 0);
            Slots.Add(addedSlot);
            Slots.Sort(new ItemsRelatedUtilities.CompareItemsByName());
            if (newAmount > addedSlot.ItemInfo.i_StackMax)
            {
                addedSlot.Amount = addedSlot.ItemInfo.i_StackMax;

                int excess = newAmount - addedSlot.ItemInfo.i_StackMax;
                ItemSlot newItemExcess = new ItemSlot(addedSlot.ItemInfo, excess);
                return TryAddNewSlot(newItemExcess);
            }
            else
            {
                addedSlot.Amount = newAmount;
                //Trigger evento fin
                return 0;
            }
        }
    }
    public int AddNewItem(ItemSlot newItem)
    {
        ItemSlot inv_slot = CheckItemInInventory(newItem);
        if (inv_slot != null)
        {
            int newAmount = inv_slot.Amount + newItem.Amount;
            if (newAmount > inv_slot.ItemInfo.i_StackMax)
            {
                inv_slot.Amount = inv_slot.ItemInfo.i_StackMax;

                int excess = newAmount - inv_slot.ItemInfo.i_StackMax;
                ItemSlot newItemExcess = new ItemSlot(inv_slot.ItemInfo, excess);
                return TryAddNewSlot(newItemExcess);
            }
            else
            {
                inv_slot.Amount = newAmount;
                return 0;
            }
        }
        else
        {
            return TryAddNewSlot(newItem);
        }
    }
    public void RemoveItemOfType(ItemsSO itemType, int amountRemoved)
    {
        for (int i = Slots.Count - 1; i >= 0; i--)
        {
            if (Slots[i].ItemInfo == itemType)
            {
                int newAmount = Slots[i].Amount - amountRemoved;
                if (newAmount <= 0)
                {
                    Slots.RemoveAt(i);
                    if (newAmount < 0)
                    {
                        RemoveItemOfType(itemType, Mathf.Abs(newAmount));

                    }
                }
                else
                {
                    Slots[i].Amount = newAmount;
                }
                return;
            }
        }
    }
    public int GetAmountOfType(ItemsSO itemType)
    {
        int amount = 0;
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].ItemInfo == itemType)
            {
                amount += Slots[i].Amount;
            }
        }
        return amount;
    }
}
