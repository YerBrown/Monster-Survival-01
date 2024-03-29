using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryManagementController : MonoBehaviour
{
    public UIInventoryController UIPlayerInventory;
    [SerializeField] protected ItemsSO SelectedItemType;

    public GameObject UIParent;
    public RectTransform ItemPanel;
    [Header("Selected Item UI Window elements")]
    public Image SelectedItemImage;
    public TMP_Text SelectedItemName;
    public TMP_Text SelectedItemDescription;
    public TMP_Text SelectedItemAmount;



    public Button TrashButton;
    public Button CloseButton;
    public Sprite DefaultItemSprite;
    [Header("Other")]
    public bool IsFilterEnabled;
    public ItemType MainFilter;

    public bool ResetOnSameType = false;
    public bool SelectFirstOnOpen = false;
    public VoidEventChannelSO OnOpenPlayerInventory;
    public UITrashController TrashController;

    private Sequence secuencia; // Referencia a la secuencia de Dotween
    private void OnEnable()
    {
        OnOpenPlayerInventory.OnEventRaised += OpenPopup;
        UIPlayerInventory.ItemSlotSelected.AddListener(SelectItemSlot);

        if (TrashButton != null)
        {
            TrashButton.onClick.AddListener(OpenTrashPopup);
        }
        CloseButton.onClick.AddListener(CloseMenu);

    }
    private void OnDisable()
    {
        OnOpenPlayerInventory.OnEventRaised -= OpenPopup;
        UIPlayerInventory.ItemSlotSelected.RemoveListener(SelectItemSlot);
        if (TrashButton != null)
        {
            TrashButton.onClick.RemoveListener(OpenTrashPopup);
        }
        CloseButton.onClick.RemoveListener(CloseMenu);
    }
    public virtual void OpenPopup()
    {
        if (PlayerManager.Instance != null) UIPlayerInventory.UI_Inventory = PlayerManager.Instance.P_Inventory;
        //reset selected item
        UIParent.gameObject.SetActive(true);

        if (UIPlayerInventory.UI_Inventory.Slots.Count > 0)
        {
            if (SelectFirstOnOpen)
            {
                SelectItemSlot(UIPlayerInventory, UIPlayerInventory.UI_Inventory.Slots[0].ItemInfo);
            }            
        }
        else
        {
            ResetSelected();
            SelectItemToTransfer();
        }
        DisableFilter();
        if (GeneralUIController.Instance != null)
        {
            GeneralUIController.Instance.OpenMenu(true);
        }
    }
    //Select item and inventory 
    protected void SelectItemSlot(UIInventoryController uiInventory, ItemsSO itemType)
    {
        if (itemType == SelectedItemType && ResetOnSameType || itemType == null)
        {
            ResetSelected();
        }
        else if (itemType != SelectedItemType)
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
    public virtual void SelectItemToTransfer()
    {
        int selectedItemAmount = 0;
        if (secuencia != null)
        {
            secuencia.Kill();
        }
        if (SelectedItemType != null)
        {
            // Crear una secuencia de tweens encadenados
            secuencia = DOTween.Sequence();
            // Agregar tweens para mover el texto y cambiar su color
            secuencia.Append(ItemPanel.DOAnchorPosX(0, 1f, true).SetEase(Ease.InOutQuad)); // Mover durante 2 segundos           

            selectedItemAmount = UIPlayerInventory.UI_Inventory.GetAmountOfType(SelectedItemType);
            if (SelectedItemImage != null)
            {
                SelectedItemImage.sprite = SelectedItemType.i_Sprite;
            }
            SelectedItemName.text = SelectedItemType.i_Name;
            SelectedItemDescription.text = SelectedItemType.i_Description;
            SelectedItemAmount.text = $"x{selectedItemAmount}";
            TrashButton.interactable = true;

        }
        else
        {
            // Crear una secuencia de tweens encadenados
            secuencia = DOTween.Sequence();
            // Agregar tweens para mover el texto y cambiar su color
            secuencia.Append(ItemPanel.DOAnchorPosX(-250, 1f, true).SetEase(Ease.InOutQuad)); // Mover durante 2 segundos

            //SelectedItemName.text = "";
            //SelectedItemDescription.text = "";
            //SelectedItemAmount.text = "";
            //SelectedItemWeight.text = "";
            TrashButton.interactable = false;
            //WeightIcon.SetActive(false);
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
    }
    //Reset current inventory and item values
    private void ResetSelected()
    {
        SelectedItemType = null;

        UIPlayerInventory.SetSelectedUI(null, false);
    }
    // Disable current filter
    public virtual void DisableFilter()
    {
        IsFilterEnabled = false;
        EnableFilter();
    }
    // Enable a type of filter
    public void SetMainFilter(int itemFilter)
    {
        MainFilter = (ItemType)Enum.Parse(typeof(ItemType), itemFilter.ToString()); ;
        IsFilterEnabled = true;
        EnableFilter();
    }
    // Manage filters
    public virtual void EnableFilter()
    {
        UIPlayerInventory.SetFilter(IsFilterEnabled, MainFilter);
    }
    public virtual void CloseMenu()
    {
        if (secuencia != null)
        {
            secuencia.Kill();
        }
        ItemPanel.anchoredPosition = new Vector2(-250, ItemPanel.anchoredPosition.y);
        UIParent.gameObject.SetActive(false);
        if (GeneralUIController.Instance != null)
        {
            GeneralUIController.Instance.OpenMenu(false);
        }
    }
}

