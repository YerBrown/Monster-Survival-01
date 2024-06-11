using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInventoryController : MonoBehaviour
{
    public Inventory UI_Inventory;
    protected ItemsSO _SelectedItem;
    public TMP_Text InventoryNameText;
    public List<GameObject> ItemsBackground = new List<GameObject>();
    public List<UIItemSlotController> Items = new List<UIItemSlotController>();
    public List<ItemType> InventoryOrder = new List<ItemType>();
    public List<EquipType> EquipOrder = new List<EquipType>();
    public List<CombatItemType> CombatOrder = new List<CombatItemType>();
    public UnityEvent<UIInventoryController, ItemsSO> ItemSlotSelected;
    public ScrollRect I_ScrollRect;
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
            foreach (var item in Items)
            {
                //Add select item functionality to each button
                item.SelectSlotEvent?.AddListener(SelectItem);
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
    protected void DisableItemSlot(UIItemSlotController itemSLot)
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
    public virtual void UpdateFullInventory()
    {
        if (UI_Inventory != null)
        {
            UI_Inventory.ItemTypeOrder = InventoryOrder;
            UI_Inventory.EquipItemTypeOrder = EquipOrder;
            UI_Inventory.CombatTypeItemTypeOrder = CombatOrder;
            UI_Inventory.SortInventory();
            for (int i = 0; i < Items.Count; i++)
            {
                if (i < UI_Inventory.Slots.Count)
                {
                    Items[i].Slot = UI_Inventory.Slots[i];
                }
                else
                {
                    Items[i].Slot = null;
                }
                Items[i].UpdateUI();
            }
        }
        if (_SelectedItem != null)
        {
            EnableSelectedUI();
        }
    }
    //Update all buttons to show item selected status
    public void SetSelectedUI(ItemsSO selectedItemType, bool thisInventory)
    {
        if (thisInventory)
        {
            _SelectedItem = selectedItemType;
        }
        else
        {
            _SelectedItem = null;
        }
        EnableSelectedUI();
    }
    protected void EnableSelectedUI()
    {
        foreach (var item in Items)
        {
            if (_SelectedItem != null && item.Slot != null && item.Slot.ItemInfo == _SelectedItem)
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
