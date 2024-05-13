using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Inventory
{
    public string Inv_Name;
    public List<ItemSlot> Slots = new List<ItemSlot>();
    public int MaxSlots;
    public bool FlexibleSlots = false;
    public bool OnlyRemoveItems = false;
    public ItemSlotEventChannelSO OnItemAdded;
    public ItemSlotEventChannelSO OnItemRemoved;
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
        if (!FlexibleSlots && Slots.Count == MaxSlots)
        {
            //Trigger evento fin
            return newItem.Amount;
        }
        else
        {
            int newAmount = newItem.Amount;
            ItemSlot addedSlot = new ItemSlot(newItem.ItemInfo, 0);
            Slots.Add(addedSlot);
            if (Slots.Count != MaxSlots && FlexibleSlots)
            {
                MaxSlots = Slots.Count;
            }
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
    public int AddNewItem(ItemSlot newItem, bool transfer = true)
    {
        if (OnlyRemoveItems && transfer) return newItem.Amount;
        ItemSlot inv_slot = CheckItemInInventory(newItem);
        int remainingAmount = 0;
        if (inv_slot != null)
        {
            int newAmount = inv_slot.Amount + newItem.Amount;
            if (newAmount > inv_slot.ItemInfo.i_StackMax)
            {
                inv_slot.Amount = inv_slot.ItemInfo.i_StackMax;

                int excess = newAmount - inv_slot.ItemInfo.i_StackMax;
                ItemSlot newItemExcess = new ItemSlot(inv_slot.ItemInfo, excess);
                remainingAmount = TryAddNewSlot(newItemExcess);
                if (OnItemAdded != null)
                {
                    if (newItem.Amount - remainingAmount > 0)
                        OnItemAdded.RaiseEvent(new ItemSlot(newItem.ItemInfo, newItem.Amount - remainingAmount));
                }
                return remainingAmount;
            }
            else
            {
                inv_slot.Amount = newAmount;
                if (OnItemAdded != null)
                {
                    if (newItem.Amount - remainingAmount > 0)
                        OnItemAdded.RaiseEvent(new ItemSlot(newItem.ItemInfo, newItem.Amount));
                }
                return 0;
            }
        }
        else
        {
            remainingAmount = TryAddNewSlot(newItem);
            if (OnItemAdded != null)
            {
                if (newItem.Amount - remainingAmount > 0)
                    OnItemAdded.RaiseEvent(new ItemSlot(newItem.ItemInfo, newItem.Amount - remainingAmount));
            }
            return remainingAmount;
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
                    if (OnItemRemoved != null && amountRemoved > 0)
                    {
                        OnItemRemoved.RaiseEvent(new ItemSlot(itemType, Slots[i].Amount));
                    }
                }
                else
                {
                    Slots[i].Amount = newAmount;
                    if (OnItemRemoved != null && amountRemoved > 0)
                    {
                        OnItemRemoved.RaiseEvent(new ItemSlot(itemType, amountRemoved));
                    }
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
    public List<ItemsSO> GetAllItemTypes()
    {
        List<ItemsSO> allItemTypes = new List<ItemsSO>();
        foreach (var item in Slots)
        {
            if (!allItemTypes.Contains(item.ItemInfo))
            {
                allItemTypes.Add(item.ItemInfo);
            }
        }
        return allItemTypes;
    }
}
