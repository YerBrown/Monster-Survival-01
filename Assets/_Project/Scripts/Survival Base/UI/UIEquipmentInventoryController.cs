using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipmentInventoryController : UIInventoryController
{
    public EquipType EquipFilter;
    public override void UpdateFullInventory()
    {
        base.UpdateFullInventory();
        if (UI_Inventory != null)
        {            
            List<UIItemSlotController> disabledItemSlots = new();
            foreach (var item in Items)
            {
                if (item.Slot != null && item.Slot.ItemInfo != null)
                {
                    EquipableItemSO equipItemInfo = item.Slot.ItemInfo as EquipableItemSO;
                    if (equipItemInfo == null)
                    {
                        disabledItemSlots.Add(item);
                    }
                }
            }
            List<UIItemSlotController> combatItemSlots = new();
            foreach (var item in Items)
            {
                if (item.Slot != null && item.Slot.ItemInfo != null)
                {
                    EquipableItemSO equipItemInfo = item.Slot.ItemInfo as EquipableItemSO;
                    if (equipItemInfo != null)
                    {
                        if (EquipFilter == EquipType.NONE || equipItemInfo.EquipmentType == EquipFilter)
                        {
                            combatItemSlots.Add(item);
                        }
                        else
                        {
                            disabledItemSlots.Add(item);
                        }
                    }
                }
            }
            foreach (var disabledItem in disabledItemSlots)
            {
                disabledItem.EnableFilter(true);
            }
            foreach (var combatItem in combatItemSlots)
            {
                combatItem.EnableFilter(false);
            }
        }
        if (_SelectedItem != null)
        {
            EnableSelectedUI();
        }
    }
    // Add filter to inventory.
    public void SetFilter(EquipType equipType = EquipType.NONE)
    {
        EquipFilter = equipType;
        UpdateFullInventory();
    }
}
