using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInventoryController : MonoBehaviour
{
    public Inventory UI_Inventory;
    public List<UIItemSlotController> Items = new List<UIItemSlotController>();
    public UnityEvent<UIInventoryController, ItemsSO> ItemSlotSelected;

    public Color SelectedColor;
    public Color UnselectedColor;
    private void Awake()
    {
        UIItemSlotController[] allItems = GetComponentsInChildren<UIItemSlotController>();
        Items.AddRange(allItems);


    }
    private void OnEnable()
    {
        if (UI_Inventory != null)
        {
            UpdateFullInventory();
            for (int i = 0; i < Items.Count; i++)
            {
                
                Items[i].SelectSlotEvent?.AddListener(SelectItem);
            }
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
    private void DisableItemSlot(UIItemSlotController itemSLot)
    {
        itemSLot.Slot = null;
        itemSLot.gameObject.SetActive(false);
    }

    private void SelectItem(UIItemSlotController itemSlot)
    {
        ItemSlotSelected?.Invoke(this, itemSlot.Slot.ItemInfo);
    }
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
                    Items[i].gameObject.SetActive(true);
                }
                else
                {
                    DisableItemSlot(Items[i]);
                }
                
            }
        }
    }

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

    public int AddItemToInventory(ItemSlot itemSlot)
    {
        int remainingAmount = UI_Inventory.AddNewItem(itemSlot);
        UpdateFullInventory();
        return remainingAmount;
    }
    public void RemoveItemFromInventory(ItemsSO itemInfo, int amount)
    {
        UI_Inventory.RemoveItemOfType(itemInfo, amount);
        UpdateFullInventory();
    }
}
