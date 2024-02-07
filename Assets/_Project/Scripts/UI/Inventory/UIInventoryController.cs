using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInventoryController : MonoBehaviour
{
    public Inventory UI_Inventory;
    public TMP_Text InventoryNameText;
    public List<GameObject> ItemsBackground = new List<GameObject>();
    public List<UIItemSlotController> Items = new List<UIItemSlotController>();
    public UnityEvent<UIInventoryController, ItemsSO> ItemSlotSelected;

    private void Awake()
    {
        //Add all the items in the parent to the list "Items"
        UIItemSlotController[] allItems = GetComponentsInChildren<UIItemSlotController>();
        Items.AddRange(allItems);

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
}
