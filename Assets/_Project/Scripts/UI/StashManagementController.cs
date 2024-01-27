using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StashManagementController : MonoBehaviour
{
    public UIInventoryController PlayerInventory;
    public UIInventoryController StashInventory;

    public UIInventoryController SelectedInventory;
    public ItemsSO SelectedItemType;


    public Image SelectedItemImage;
    public TMP_Text SelectedItemName;
    public TMP_Text SelectedItemAmount;
    public TMP_Text SelectedItemAmountStack;

    public Button TransferAllButton;
    public Button TransferStackButton;
    public Button TransferPartButton;

    public UISetTransferAmountController TransferPartController;
    private void OnEnable()
    {
        PlayerInventory.ItemSlotSelected.AddListener(SelectItemSlot);
        StashInventory.ItemSlotSelected.AddListener(SelectItemSlot);

        TransferAllButton.onClick.AddListener(TransferAll);
        TransferStackButton.onClick.AddListener(TransferStack);
        TransferPartButton.onClick.AddListener(OpenTransferAmountPopup);
    }
    private void OnDisable()
    {
        PlayerInventory.ItemSlotSelected.RemoveListener(SelectItemSlot);
        StashInventory.ItemSlotSelected.RemoveListener(SelectItemSlot);

        TransferAllButton.onClick.RemoveListener(TransferAll);
        TransferStackButton.onClick.RemoveListener(TransferStack);
        TransferPartButton.onClick.RemoveListener(OpenTransferAmountPopup);
    }
    private void SelectItemSlot(UIInventoryController uiInventory, ItemsSO itemType)
    {
        if (uiInventory == SelectedInventory && itemType == SelectedItemType)
        {
            ResetSelected();
        }
        else
        {
            SelectedInventory = uiInventory;
            SelectedItemType = itemType;
            if (StashInventory == uiInventory)
            {

                StashInventory.SetSelectedUI(itemType, true);
                PlayerInventory.SetSelectedUI(itemType, false);
            }
            else
            {
                StashInventory.SetSelectedUI(itemType, false);
                PlayerInventory.SetSelectedUI(itemType, true);
            }
            Debug.Log($"Inventario: {uiInventory.name}, ItemSlot: {itemType.i_Name} {SelectedInventory.UI_Inventory.GetAmountOfType(itemType)}");
        }


        SelectItemToTransfer();
    }
    public void TransferItem(UIInventoryController from, UIInventoryController to, ItemSlot itemSlot)
    {
        int remainingAmount = to.AddItemToInventory(itemSlot);

        from.RemoveItemFromInventory(itemSlot.ItemInfo, itemSlot.Amount - remainingAmount);

        CheckRemainingItemSlot();
    }
    public void SelectItemToTransfer()
    {
        if (SelectedItemType != null)
        {
            SelectedItemImage.sprite = ItemsRelatedUtilities.CheckItemIcon(SelectedItemType);
            SelectedItemName.text = SelectedItemType.i_Name;
            SelectedItemAmount.text = $"x{SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType)}";
            if (SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType) >= SelectedItemType.i_StackMax)
            {
                SelectedItemAmountStack.text = $"x{SelectedItemType.i_StackMax}";
            }
            else
            {
                SelectedItemAmountStack.text = $"x{SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType)}";
            }
        }
        else
        {
            SelectedItemImage.sprite = ItemsRelatedUtilities.DefaultIcon();
            SelectedItemName.text = "???";
            SelectedItemAmount.text = $"x???";
            SelectedItemAmountStack.text = $"x???";
        }
        TransferAllButton.interactable = SelectedItemType != null;
        TransferStackButton.interactable = SelectedItemType != null;
        TransferPartButton.interactable = SelectedItemType != null;
    }
    public void OpenTransferAmountPopup()
    {
        TransferPartController.CurrentAmount = SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType);
        TransferPartController.ItemToTransfer = SelectedItemType;
        TransferPartController.gameObject.SetActive(true);
    }

    public void TransferAll()
    {
        ItemSlot transferedSlot = new ItemSlot(SelectedItemType, SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType));
        SelectTransferTargets(transferedSlot);
    }
    public void TransferStack()
    {
        ItemSlot transferedSlot = null;
        if (SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType) >= SelectedItemType.i_StackMax)
        {
            transferedSlot = new ItemSlot(SelectedItemType, SelectedItemType.i_StackMax);
        }
        else
        {
            transferedSlot = new ItemSlot(SelectedItemType, SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType));
        }

        SelectTransferTargets(transferedSlot);
    }
    public void TransferPart(int amount)
    {
        ItemSlot transferedSlot = new ItemSlot(SelectedItemType, amount);
        SelectTransferTargets(transferedSlot);
    }
    private void SelectTransferTargets(ItemSlot transferedSlot)
    {
        if (SelectedInventory == StashInventory)
        {
            TransferItem(StashInventory, PlayerInventory, transferedSlot);
            StashInventory.SetSelectedUI(transferedSlot.ItemInfo, true);
            PlayerInventory.SetSelectedUI(transferedSlot.ItemInfo, false);
        }
        else
        {
            TransferItem(PlayerInventory, StashInventory, transferedSlot);
            StashInventory.SetSelectedUI(transferedSlot.ItemInfo, false);
            PlayerInventory.SetSelectedUI(transferedSlot.ItemInfo, true);
        }
    }

    private void CheckRemainingItemSlot()
    {
        if (SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType) <= 0)
        {
            ResetSelected();
        }
        SelectItemToTransfer();
    }
    private void ResetSelected()
    {
        SelectedInventory = null;
        SelectedItemType = null;

        PlayerInventory.SetSelectedUI(null, false);
        StashInventory.SetSelectedUI(null, false);
    }
}
