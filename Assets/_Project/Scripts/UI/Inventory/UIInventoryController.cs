using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIInventoryController : MonoBehaviour
{
    public Inventory UI_Inventory;
    public TMP_Text InventoryNameText;
    public List<GameObject> ItemsBackground = new List<GameObject>();
    public List<UIItemSlotController> Items = new List<UIItemSlotController>();
    public UnityEvent<UIInventoryController, ItemsSO> ItemSlotSelected;
    public ScrollRect I_ScrollRect;
    public bool IsFilterEnabled = false;
    public ItemType ItemMainFilter;
    public CombatItemType CombatFilter;
    private void Awake()
    {
        //Add all the items in the parent to the list "Items"
        UIItemSlotController[] allItems = GetComponentsInChildren<UIItemSlotController>();
        Items = allItems.ToList();

        for (int i = 0; i < transform.childCount; i++)
        {
            ItemsBackground.Add(transform.GetChild(i).gameObject);
        }
    }
    private void OnEnable()
    {
        if (UI_Inventory != null)
        {
            if (InventoryNameText != null)
                InventoryNameText.text = UI_Inventory.Inv_Name;

            for (int i = 0; i < ItemsBackground.Count; i++)
            {
                if (UI_Inventory.MaxSlots > i)
                {
                    ItemsBackground[i].SetActive(true);
                }
                else
                {
                    ItemsBackground[i].SetActive(false);
                }
            }
            UpdateFullInventory();
            for (int i = 0; i < Items.Count; i++)
            {
                //Add select item functionality to each button
                Items[i].SelectSlotEvent?.AddListener(SelectItem);
            }
            if (I_ScrollRect != null)
                I_ScrollRect.verticalNormalizedPosition = 1;
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            DisableItemSlot(Items[i]);
        }
        for (int i = 0; i < Items.Count; i++)
        {

            Items[i].SelectSlotEvent?.RemoveListener(SelectItem);
        }
    }
    //Disable not needed button
    private void DisableItemSlot(UIItemSlotController itemSLot)
    {
        itemSLot.Slot = null;
        itemSLot.gameObject.SetActive(false);
    }
    //Trigger item selection event 
    private void SelectItem(UIItemSlotController itemSlot)
    {
        ItemSlotSelected?.Invoke(this, itemSlot.Slot.ItemInfo);
    }
    //Update UI after any change in inventory
    public void UpdateFullInventory()
    {
        if (UI_Inventory != null)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (i < UI_Inventory.Slots.Count)
                {
                    Items[i].Slot = UI_Inventory.Slots[i];
                    Items[i].UpdateUI();
                    if (IsFilterEnabled)
                    {
                        ItemsSO currentItem = UI_Inventory.Slots[i].ItemInfo;
                        bool activateFilter = currentItem.i_ItemType != ItemMainFilter;
                        if (ItemMainFilter == ItemType.COMBAT)
                        {
                            if (CombatFilter != CombatItemType.GENERAL)
                            {
                                if (currentItem.i_CombatType != CombatFilter)
                                {
                                    activateFilter = true;
                                }
                            }
                        }
                        Items[i].EnableFilter(activateFilter);
                    }
                    else
                    {
                        Items[i].EnableFilter(false);
                    }
                    Items[i].gameObject.SetActive(true);
                }
                else
                {
                    DisableItemSlot(Items[i]);
                }
            }
        }
    }
    //Update all buttons to show item selected status
    public void SetSelectedUI(ItemsSO selectedItemType, bool thisInventory)
    {
        foreach (var item in Items)
        {
            if (selectedItemType != null && item.Slot != null && item.Slot.ItemInfo == selectedItemType && thisInventory)
            {
                item.EnableBack(true);
            }
            else
            {
                item.EnableBack(false);
            }
        }
    }
    //Add item to the inventory
    public int AddItemToInventory(ItemSlot itemSlot)
    {
        int remainingAmount = UI_Inventory.AddNewItem(itemSlot);
        UpdateFullInventory();
        return remainingAmount;
    }
    //Remove item from inventory
    public void RemoveItemFromInventory(ItemsSO itemInfo, int amount)
    {
        UI_Inventory.RemoveItemOfType(itemInfo, amount);
        UpdateFullInventory();
    }
    // Add filter to inventory.
    public void SetFilter(bool enableFilter, ItemType itemType, CombatItemType combatType = CombatItemType.GENERAL)
    {
        IsFilterEnabled = enableFilter;
        ItemMainFilter = itemType;
        CombatFilter = combatType;
        UpdateFullInventory();
    }
}
