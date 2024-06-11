using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UICombatInventoryController : UIInventoryController
{
    public CombatItemType CombatFilter;
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
                    CombatItemSO combatItemInfo = item.Slot.ItemInfo as CombatItemSO;
                    if (combatItemInfo == null)
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
                    CombatItemSO combatItemInfo = item.Slot.ItemInfo as CombatItemSO;
                    if (combatItemInfo != null)
                    {
                        if (CombatFilter == CombatItemType.GENERAL || combatItemInfo.i_CombatType == CombatFilter)
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
    public void SetFilter(CombatItemType combatType = CombatItemType.GENERAL)
    {
        CombatFilter = combatType;
        UpdateFullInventory();
    }
}
