using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoriesManagementController : MonoBehaviour
{
    [Header("Inventories")]
    public UIInventoryController Inventory1;
    public UIInventoryController Inventory2;

    [Header("Current")]
    [SerializeField] private UIInventoryController SelectedInventory;
    [SerializeField] private ItemsSO SelectedItemType;

    [Header("Selected Item UI Window elements")]
    public Image SelectedItemImage;
    public TMP_Text SelectedItemName;
    public TMP_Text SelectedItemDescription;
    public TMP_Text SelectedItemAmount;
    public TMP_Text SelectedItemAmountStack;

    public Button TransferAllButton;
    public Button TransferStackButton;
    public Button TransferPartButton;
    public Button TrashButton;
    public Button CloseButton;

    public RectTransform ArrowIcon;

    [Header("Other")]
    public UISetTransferAmountController TransferPartController;
    public UITrashController TrashController;
    private void OnEnable()
    {
        Inventory1.ItemSlotSelected.AddListener(SelectItemSlot);
        Inventory2.ItemSlotSelected.AddListener(SelectItemSlot);

        TransferAllButton.onClick.AddListener(TransferAll);
        TransferStackButton.onClick.AddListener(TransferStack);
        TransferPartButton.onClick.AddListener(OpenTransferAmountPopup);
        TrashButton.onClick.AddListener(OpenTrashPopup);
        CloseButton.onClick.AddListener(CloseMenu);
        //reset selected item
        ResetSelected();
        SelectItemToTransfer();
    }
    private void OnDisable()
    {
        Inventory1.ItemSlotSelected.RemoveListener(SelectItemSlot);
        Inventory2.ItemSlotSelected.RemoveListener(SelectItemSlot);

        TransferAllButton.onClick.RemoveListener(TransferAll);
        TransferStackButton.onClick.RemoveListener(TransferStack);
        TransferPartButton.onClick.RemoveListener(OpenTransferAmountPopup);
        TrashButton.onClick.RemoveListener(OpenTrashPopup);
        CloseButton.onClick.RemoveListener(CloseMenu);
    }
    //Select item and inventory 
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
            if (Inventory2 == uiInventory)
            {
                Inventory2.SetSelectedUI(itemType, true);
                Inventory1.SetSelectedUI(itemType, false);
                ArrowIcon.rotation = Quaternion.Euler(0f, 0f, 90f);
            }
            else
            {
                Inventory2.SetSelectedUI(itemType, false);
                Inventory1.SetSelectedUI(itemType, true);
                ArrowIcon.rotation = Quaternion.Euler(0f, 0f, 270f);
            }
            Debug.Log($"Inventario: {uiInventory.name}, ItemSlot: {itemType.i_Name} {SelectedInventory.UI_Inventory.GetAmountOfType(itemType)}");
        }


        SelectItemToTransfer();
    }
    //Transfer item from one inventory to another
    public void TransferItem(UIInventoryController from, UIInventoryController to, ItemSlot itemSlot)
    {
        int remainingAmount = to.AddItemToInventory(itemSlot);

        from.RemoveItemFromInventory(itemSlot.ItemInfo, itemSlot.Amount - remainingAmount);

        CheckRemainingItemSlot();
    }
    //Remove items from inventory
    public void RemoveItem(int amountToRemove)
    {
        SelectedInventory.RemoveItemFromInventory(SelectedItemType, amountToRemove);

        CheckRemainingItemSlot();
    }
    //Update selected item UI
    public void SelectItemToTransfer()
    {
        if (SelectedItemType != null)
        {
            SelectedItemImage.sprite = ItemsRelatedUtilities.CheckItemIcon(SelectedItemType);
            SelectedItemName.text = SelectedItemType.i_Name;
            SelectedItemDescription.text = SelectedItemType.i_Description;
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
            SelectedItemDescription.text = "";
            SelectedItemAmount.text = $"x???";
            SelectedItemAmountStack.text = $"x???";
        }
        TransferAllButton.interactable = SelectedItemType != null;
        TransferStackButton.interactable = SelectedItemType != null;
        TransferPartButton.interactable = SelectedItemType != null;
        TrashButton.interactable = SelectedItemType != null;
        ArrowIcon.gameObject.SetActive(SelectedItemType != null);
    }
    //Open transfer part popup
    public void OpenTransferAmountPopup()
    {
        TransferPartController.CurrentAmount = SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType);
        TransferPartController.ItemToTransfer = SelectedItemType;
        TransferPartController.gameObject.SetActive(true);
    }
    //Open remove item popup
    public void OpenTrashPopup()
    {
        TrashController.CurrentAmount = SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType);
        TrashController.ItemToTransfer = SelectedItemType;
        TrashController.gameObject.SetActive(true);
    }
    //Transfer all the amount of one type of item
    public void TransferAll()
    {
        ItemSlot transferedSlot = new ItemSlot(SelectedItemType, SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType));
        SelectTransferTargets(transferedSlot);
    }
    public void TransferAllItems()
    {
        //foreach (var item in collection)
        //{

        //}
        //ItemSlot transferedSlot = new ItemSlot(SelectedItemType, SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType));
        //SelectTransferTargets(transferedSlot);
    }
    //Transfer one stack of one type of item
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
    //Transfer an specific amount of one type of item
    public void TransferPart(int amount)
    {
        ItemSlot transferedSlot = new ItemSlot(SelectedItemType, amount);
        SelectTransferTargets(transferedSlot);
    }

    //Set the transfer origin and destination
    private void SelectTransferTargets(ItemSlot transferedSlot)
    {
        if (SelectedInventory == Inventory2)
        {
            TransferItem(Inventory2, Inventory1, transferedSlot);
            Inventory2.SetSelectedUI(transferedSlot.ItemInfo, true);
            Inventory1.SetSelectedUI(transferedSlot.ItemInfo, false);
        }
        else
        {
            TransferItem(Inventory1, Inventory2, transferedSlot);
            Inventory2.SetSelectedUI(transferedSlot.ItemInfo, false);
            Inventory1.SetSelectedUI(transferedSlot.ItemInfo, true);
        }
    }
    //After transfer check the amount of the item
    private void CheckRemainingItemSlot()
    {
        if (SelectedInventory.UI_Inventory.GetAmountOfType(SelectedItemType) <= 0)
        {
            ResetSelected();
        }
        SelectItemToTransfer();
    }
    //Reset current inventory and item values
    private void ResetSelected()
    {
        SelectedInventory = null;
        SelectedItemType = null;

        Inventory1.SetSelectedUI(null, false);
        Inventory2.SetSelectedUI(null, false);
    }
    public void CloseMenu()
    {
        gameObject.SetActive(false);
        GeneralUIController.Instance.OpenMenu(false);
    }
}
