using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryManagementController : MonoBehaviour
{
    public UIInventoryController UIPlayerInventory;
    [SerializeField] private ItemsSO SelectedItemType;

    public GameObject UIParent;
    [Header("Selected Item UI Window elements")]
    public TMP_Text SelectedItemName;
    public TMP_Text SelectedItemDescription;
    public TMP_Text SelectedItemAmount;
    public TMP_Text SelectedItemWeight;
    public GameObject WeightIcon;
    public TMP_Text TotalWeightText;

    public Button TrashButton;
    public Button CloseButton;
    [Header("Other")]
    public VoidEventChannelSO OnOpenPLayerInventory;
    public UITrashController TrashController;
    private void OnEnable()
    {
        OnOpenPLayerInventory.OnEventRaised += OpenPopup;
        UIPlayerInventory.ItemSlotSelected.AddListener(SelectItemSlot);

        TrashButton.onClick.AddListener(OpenTrashPopup);
        CloseButton.onClick.AddListener(CloseMenu);

    }
    private void OnDisable()
    {
        OnOpenPLayerInventory.OnEventRaised -= OpenPopup;
        UIPlayerInventory.ItemSlotSelected.RemoveListener(SelectItemSlot);

        TrashButton.onClick.RemoveListener(OpenTrashPopup);
        CloseButton.onClick.RemoveListener(CloseMenu);
    }
    public void OpenPopup()
    {
        if (PlayerManager.Instance != null) UIPlayerInventory.UI_Inventory = PlayerManager.Instance.P_Inventory;
        //reset selected item
        ResetSelected();
        SelectItemToTransfer();
        CalculateTotalWeight();
        UIParent.gameObject.SetActive(true);
        GeneralUIController.Instance.OpenMenu(true);
    }
    //Select item and inventory 
    private void SelectItemSlot(UIInventoryController uiInventory, ItemsSO itemType)
    {
        if (itemType == SelectedItemType)
        {
            ResetSelected();
        }
        else
        {
            SelectedItemType = itemType;

            UIPlayerInventory.SetSelectedUI(itemType, true);

        }
        Debug.Log($"Inventario: {UIPlayerInventory.name}, ItemSlot: {itemType.i_Name} {UIPlayerInventory.UI_Inventory.GetAmountOfType(itemType)}");
        SelectItemToTransfer();
    }
    //Remove items from inventory
    public void RemoveItem(int amountToRemove)
    {
        UIPlayerInventory.RemoveItemFromInventory(SelectedItemType, amountToRemove);

        CheckRemainingItemSlot();
    }
    //Update selected item UI
    public void SelectItemToTransfer()
    {
        int selectedItemAmount = 0;
        if (SelectedItemType != null)
        {
            selectedItemAmount = UIPlayerInventory.UI_Inventory.GetAmountOfType(SelectedItemType);
            SelectedItemName.text = SelectedItemType.i_Name;
            SelectedItemDescription.text = SelectedItemType.i_Description;
            SelectedItemAmount.text = $"x{selectedItemAmount}";
            SelectedItemWeight.text = $"x {SelectedItemType.i_Weight}kg = {SelectedItemType.i_Weight * selectedItemAmount}kg";
            TrashButton.gameObject.SetActive(true);
            WeightIcon.SetActive(true);
        }
        else
        {
            SelectedItemName.text = "";
            SelectedItemDescription.text = "";
            SelectedItemAmount.text = "";
            SelectedItemWeight.text = "";
            TrashButton.gameObject.SetActive(false);
            WeightIcon.SetActive(false);
        }
    }
    //Open remove item popup
    public void OpenTrashPopup()
    {
        TrashController.CurrentAmount = UIPlayerInventory.UI_Inventory.GetAmountOfType(SelectedItemType);
        TrashController.ItemToTransfer = SelectedItemType;
        TrashController.gameObject.SetActive(true);
    }
    private void CheckRemainingItemSlot()
    {
        if (UIPlayerInventory.UI_Inventory.GetAmountOfType(SelectedItemType) <= 0)
        {
            ResetSelected();
        }
        SelectItemToTransfer();
        CalculateTotalWeight();
    }
    private void CalculateTotalWeight()
    {
        TotalWeightText.text = $"{UIPlayerInventory.UI_Inventory.GetTotalWeight().ToString("00.00")} kg";
    }
    //Reset current inventory and item values
    private void ResetSelected()
    {
        SelectedItemType = null;

        UIPlayerInventory.SetSelectedUI(null, false);
    }
    public void CloseMenu()
    {
        UIParent.gameObject.SetActive(false);
        GeneralUIController.Instance.OpenMenu(false);
    }
}

