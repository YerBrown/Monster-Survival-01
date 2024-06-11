using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEditorInternal.Profiling.Memory.Experimental;
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
    public List<ItemType> ItemTypeOrder = new();
    public List<EquipType> EquipItemTypeOrder = new();
    public List<CombatItemType> CombatTypeItemTypeOrder = new();
    private Dictionary<ItemType, int> _ItemOrder = new Dictionary<ItemType, int>();
    private Dictionary<EquipType, int> _EquipItemOrder = new Dictionary<EquipType, int>();
    private Dictionary<CombatItemType, int> _CombatItemOrder = new Dictionary<CombatItemType, int>();
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

        int remainingAmount = newItem.Amount;
        foreach (var slot in Slots)
        {
            if (slot.ItemInfo == newItem.ItemInfo)
            {
                int posibleAmount = Math.Min(remainingAmount, newItem.ItemInfo.i_StackMax - slot.Amount);

                slot.Amount += posibleAmount;
                remainingAmount -= posibleAmount;

                if (remainingAmount == 0)
                {
                    SortInventory();
                    if (OnItemAdded != null)
                    {
                        OnItemAdded.RaiseEvent(new ItemSlot(newItem.ItemInfo, newItem.Amount));
                    }
                    return 0; // Se añadió toda la cantidad a objetos existentes
                }
            }
        }
        while (remainingAmount > 0)
        {
            if (!FlexibleSlots && Slots.Count >= MaxSlots)
            {
                if (remainingAmount < newItem.Amount)
                {
                    SortInventory();
                    if (OnItemAdded != null)
                    {
                        OnItemAdded.RaiseEvent(new ItemSlot(newItem.ItemInfo, newItem.Amount - remainingAmount));
                    }
                }
                return remainingAmount; // No se puede añadir más objetos, devolver la cantidad que no se pudo añadir
            }

            int posibleAmount = Math.Min(remainingAmount, newItem.ItemInfo.i_StackMax);

            ItemSlot addedItem = new ItemSlot(newItem.ItemInfo, posibleAmount);

            Slots.Add(addedItem);
            remainingAmount -= posibleAmount;
        }
        SortInventory();
        if (OnItemAdded != null)
        {
            OnItemAdded.RaiseEvent(new ItemSlot(newItem.ItemInfo, newItem.Amount));
        }
        return 0; // Se pudo añadir toda la cantidad
    }
    public virtual void SortInventory()
    {
        _ItemOrder = new();
        for (int i = 0; i < ItemTypeOrder.Count; i++)
        {
            _ItemOrder.Add(ItemTypeOrder[i], i);
        }
        _EquipItemOrder = new();
        for (int i = 0; i < EquipItemTypeOrder.Count; i++)
        {
            _EquipItemOrder.Add(EquipItemTypeOrder[i], i);
        }
        _CombatItemOrder = new();
        for (int i = 0; i < CombatTypeItemTypeOrder.Count; i++)
        {
            _CombatItemOrder.Add(CombatTypeItemTypeOrder[i], i);
        }


        Comparison<ItemSlot> comparador = (x, y) =>
        {
            // Detect item types
            EquipableItemSO xEquipItem = x.ItemInfo as EquipableItemSO;
            EquipableItemSO yEquipItem = y.ItemInfo as EquipableItemSO;
            CombatItemSO xCombatItem = x.ItemInfo as CombatItemSO;
            CombatItemSO yCombatItem = y.ItemInfo as CombatItemSO;

            int resultado = _ItemOrder[x.ItemInfo.i_ItemType].CompareTo(_ItemOrder[y.ItemInfo.i_ItemType]);
            if (resultado == 0 && xEquipItem != null && yEquipItem != null)
            {
                resultado = _EquipItemOrder[xEquipItem.EquipmentType].CompareTo(_EquipItemOrder[yEquipItem.EquipmentType]);
            }else if(resultado == 0 && xCombatItem != null && yCombatItem != null)
            {
                resultado = _CombatItemOrder[xCombatItem.i_CombatType].CompareTo(_CombatItemOrder[yCombatItem.i_CombatType]);
            }
            if (resultado == 0)
            {
                resultado = x.ItemInfo.i_Name.CompareTo(y.ItemInfo.i_Name);
            }
            if (resultado == 0)
            {
                resultado = y.Amount.CompareTo(x.Amount);
            }
            return resultado;
        };
        Slots.Sort(comparador);
    }
    public int RemoveItemOfType(ItemsSO itemType, int removedAmount)
    {
        Slots = Slots.OrderBy(slot => slot.ItemInfo == itemType).ThenBy(slot => slot.Amount).ToList();

        List<ItemSlot> slotsToRemove = new List<ItemSlot>();
        int remainingAmount = removedAmount;
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].ItemInfo == itemType && Slots[i].Amount > 0)
            {
                int cantidadARestar = Math.Min(remainingAmount, Slots[i].Amount);
                Slots[i].Amount -= cantidadARestar;
                remainingAmount -= cantidadARestar;

                if (Slots[i].Amount <= 0)
                {
                    slotsToRemove.Add(Slots[i]);
                }

                if (remainingAmount == 0)
                {
                    break;
                }
            }
        }

        foreach (ItemSlot slot in slotsToRemove)
        {
            Slots.Remove(slot);
        }
        if (removedAmount > remainingAmount)
        {
            SortInventory();
            if (OnItemRemoved != null)
            {
                OnItemRemoved.RaiseEvent(new ItemSlot(itemType, removedAmount - remainingAmount));
            }
        }
        return remainingAmount;
    }
    private void OptimizeInventorySpace()
    {
        Inventory newInventory = new();
        newInventory.MaxSlots = MaxSlots;
        newInventory.FlexibleSlots = FlexibleSlots;
        newInventory.OnlyRemoveItems = OnlyRemoveItems;
        foreach (var item in Slots)
        {
            newInventory.AddNewItem(item);
        }
        Slots = newInventory.Slots;
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
