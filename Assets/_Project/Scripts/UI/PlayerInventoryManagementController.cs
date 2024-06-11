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

    public CanvasGroup ItemSelectedPanel;
    [Header("Selected Item UI Window elements")]
    public Image SelectedItemImage;
    public TMP_Text SelectedItemName;
    public TMP_Text SelectedItemDescription;
    public TMP_Text SelectedItemAmount;
    public Button TrashButton;
    public Button CloseButton;
    public Sprite DefaultItemSprite;
    [Header("Other")]
    public bool ResetOnSameType = false;
    public bool SelectFirstOnOpen = false;
    public UITrashController TrashController;

    private Sequence secuencia; // Referencia a la secuencia de Dotween
    public virtual void OpenPopup()
    {
        UIPlayerInventory.ItemSlotSelected.AddListener(SelectItemSlot);

        if (TrashButton != null)
        {
            TrashButton.onClick.AddListener(OpenTrashPopup);
        }
        //reset selected item
        if (UIPlayerInventory.UI_Inventory.Slots.Count > 0 && SelectFirstOnOpen)
        {
            SelectItemSlot(UIPlayerInventory, UIPlayerInventory.UI_Inventory.Slots[0].ItemInfo);
        }
        else
        {
            ResetSelected();            
        }
        //DisableFilter();
    }
    public virtual void ClosePopup()
    {
        UIPlayerInventory.ItemSlotSelected.RemoveListener(SelectItemSlot);
        if (TrashButton != null)
        {
            TrashButton.onClick.RemoveListener(OpenTrashPopup);
        }
        if (secuencia != null)
        {
            secuencia.Kill();
        }
        ItemSelectedPanel.alpha = 0;
        ItemSelectedPanel.interactable = false;
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
            ItemSelectedPanel.interactable = true;

            // Crear una secuencia de tweens encadenados
            secuencia = DOTween.Sequence();
            // Agregar tweens para mover el texto y cambiar su color
            secuencia.Append(ItemSelectedPanel.DOFade(1, 1f).SetEase(Ease.InOutQuad));

            selectedItemAmount = UIPlayerInventory.UI_Inventory.GetAmountOfType(SelectedItemType);
            if (SelectedItemImage != null)
            {
                SelectedItemImage.sprite = SelectedItemType.i_Sprite;
            }
            SelectedItemName.text = SelectedItemType.i_Name;
            SelectedItemDescription.text = SelectedItemType.i_Description;
            SelectedItemAmount.text = $"x{selectedItemAmount}";
            if (TrashButton != null)
            {
                TrashButton.interactable = true;
            }

        }
        else
        {
            ItemSelectedPanel.interactable = false;
            // Crear una secuencia de tweens encadenados
            secuencia = DOTween.Sequence();
            // Agregar tweens para mover el texto y cambiar su color
            secuencia.Append(ItemSelectedPanel.DOFade(0, 1f).SetEase(Ease.InOutQuad));

            //SelectedItemName.text = "";
            //SelectedItemDescription.text = "";
            //SelectedItemAmount.text = "";
            //SelectedItemWeight.text = "";
            if (TrashButton != null)
            {
                TrashButton.interactable = false;
            }
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
}

