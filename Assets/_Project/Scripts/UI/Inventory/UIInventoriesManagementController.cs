using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoriesManagementController : MonoBehaviour
{
    [Header("Inventories")]
    public UIInventoryController Inventory_L;
    public UIInventoryController Inventory_R;

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
    public Button TransferAllItemsButton_L;
    public Button TransferAllItemsButton_R;

    public RectTransform ArrowIcon;

    [Header("Other")]
    public UISetTransferAmountController TransferPartController;
    public UITrashController TrashController;
    public VoidEventChannelSO OnClosePopup;
    private void OnEnable()
    {
        Inventory_L.ItemSlotSelected.AddListener(SelectItemSlot);
        Inventory_R.ItemSlotSelected.AddListener(SelectItemSlot);

        TransferAllButton.onClick.AddListener(TransferAll);
        TransferStackButton.onClick.AddListener(TransferStack);
        TransferPartButton.onClick.AddListener(OpenTransferAmountPopup);
        TrashButton.onClick.AddListener(OpenTrashPopup);
        if (CloseButton != null)
        {
            CloseButton.onClick.AddListener(CloseMenu);
        }
        TransferAllItemsButton_L.onClick.AddListener(TransferAllItemsL);
        TransferAllItemsButton_R.onClick.AddListener(TransferAllItemsR);
        //reset selected item
        ResetSelected();
        CheckEmptyInventories();
        SelectItemToTransfer();
    }
    private void OnDisable()
    {
        Inventory_L.ItemSlotSelected.RemoveListener(SelectItemSlot);
        Inventory_R.ItemSlotSelected.RemoveListener(SelectItemSlot);

        TransferAllButton.onClick.RemoveListener(TransferAll);
        TransferStackButton.onClick.RemoveListener(TransferStack);
        TransferPartButton.onClick.RemoveListener(OpenTransferAmountPopup);
        TrashButton.onClick.RemoveListener(OpenTrashPopup);
        if (CloseButton != null)
        {
            CloseButton.onClick.RemoveListener(CloseMenu);
        }
        TransferAllItemsButton_L.onClick.RemoveListener(TransferAllItemsL);
        TransferAllItemsButton_R.onClick.RemoveListener(TransferAllItemsR);
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
            if (Inventory_R == uiInventory)
            {
                Inventory_R.SetSelectedUI(itemType, true);
                Inventory_L.SetSelectedUI(itemType, false);
                ArrowIcon.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                Inventory_R.SetSelectedUI(itemType, false);
                Inventory_L.SetSelectedUI(itemType, true);
                ArrowIcon.rotation = Quaternion.Euler(0f, 0f, 180f);
            }
            Debug.Log($"Inventario: {uiInventory.name}, ItemSlot: {itemType.i_Name} {SelectedInventory.UI_Inventory.GetAmountOfType(itemType)}");
        }


        SelectItemToTransfer();
    }
    //Transfer item from one inventory to another
    public void TransferItem(UIInventoryController from, UIInventoryController to, ItemSlot itemSlot)
    {
        int remainingAmount = to.AddItemToInventory(itemSlot);

        if (itemSlot.Amount - remainingAmount > 0)
        {
            from.RemoveItemFromInventory(itemSlot.ItemInfo, itemSlot.Amount - remainingAmount);
        }

        if (SelectedItemType != null) CheckRemainingItemSlot();
    }
    //Remove items from inventory
    public void RemoveItem(int amountToRemove)
    {
        SelectedInventory.RemoveItemFromInventory(SelectedItemType, amountToRemove);
        if (SelectedItemType != null) CheckRemainingItemSlot();
        CheckEmptyInventories();
    }
    //Update selected item UI
    public void SelectItemToTransfer()
    {
        if (SelectedItemType != null)
        {
            SelectedItemImage.sprite = ItemsRelatedUtilities.CheckItemIcon(SelectedItemType);
            SelectedItemImage.gameObject.SetActive(true);
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
            SelectedItemImage.gameObject.SetActive(false);
            SelectedItemName.text = "";
            SelectedItemDescription.text = "";
            SelectedItemAmount.text = $"";
            SelectedItemAmountStack.text = $"";
        }
        if (((SelectedInventory == Inventory_L) ? Inventory_R : Inventory_L).UI_Inventory.OnlyRemoveItems)
        {
            TransferAllButton.interactable = false;
            TransferStackButton.interactable = false;
            TransferPartButton.interactable = false;
            ArrowIcon.gameObject.SetActive(false);
        }
        else
        {
            TransferAllButton.interactable = SelectedItemType != null;
            TransferStackButton.interactable = SelectedItemType != null;
            TransferPartButton.interactable = SelectedItemType != null;
            ArrowIcon.gameObject.SetActive(SelectedItemType != null);
        }
        TrashButton.interactable = SelectedItemType != null;
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
    //Transfer all the items from one inventory to another
    public void TransferAllItems(bool inventory1)
    {
        List<ItemsSO> AllItemTypes = new List<ItemsSO>();
        if (inventory1)
        {
            AllItemTypes = Inventory_L.UI_Inventory.GetAllItemTypes();
            foreach (var slot in AllItemTypes)
            {
                ItemSlot transferedSlot = new ItemSlot(slot, Inventory_L.UI_Inventory.GetAmountOfType(slot));
                TransferItem(Inventory_L, Inventory_R, transferedSlot);
            }
        }
        else
        {
            AllItemTypes = Inventory_R.UI_Inventory.GetAllItemTypes();
            foreach (var slot in AllItemTypes)
            {
                ItemSlot transferedSlot = new ItemSlot(slot, Inventory_R.UI_Inventory.GetAmountOfType(slot));
                TransferItem(Inventory_R, Inventory_L, transferedSlot);
            }
        }
        CheckEmptyInventories();
    }
    public void TransferAllItemsL()
    {
        TransferAllItems(true);
    }
    public void TransferAllItemsR()
    {
        TransferAllItems(false);
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
        TransferItem((SelectedInventory == Inventory_L) ? Inventory_L : Inventory_R, (SelectedInventory == Inventory_L) ? Inventory_R : Inventory_L, transferedSlot);
        CheckEmptyInventories();
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

        Inventory_L.SetSelectedUI(null, false);
        Inventory_R.SetSelectedUI(null, false);

        TransferAllItemsButton_R.interactable = !Inventory_L.UI_Inventory.OnlyRemoveItems;
        TransferAllItemsButton_L.interactable = !Inventory_R.UI_Inventory.OnlyRemoveItems;

    }
    private void CheckEmptyInventories()
    {
        if (Inventory_R.UI_Inventory.Slots.Count <= 0)
        {
            TransferAllItemsButton_R.interactable = false;
        }
        else if (!Inventory_L.UI_Inventory.OnlyRemoveItems)
        {
            TransferAllItemsButton_R.interactable = true;
        }
        if (Inventory_L.UI_Inventory.Slots.Count <= 0)
        {
            TransferAllItemsButton_L.interactable = false;
        }
        else if (!Inventory_R.UI_Inventory.OnlyRemoveItems)
        {
            TransferAllItemsButton_L.interactable = true;
        }

    }
    public void CloseMenu()
    {
        if (OnClosePopup != null)
        {
            OnClosePopup.RaiseEvent();
        }
        gameObject.SetActive(false);
        GeneralUIController.Instance.OpenMenu(false);
    }
}
