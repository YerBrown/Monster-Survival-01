using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerInventoryInCombatController : PlayerInventoryManagementController
{
    public override void SelectItemToTransfer()
    {
        int selectedItemAmount = 0;

        if (SelectedItemType != null)
        {      

            selectedItemAmount = UIPlayerInventory.UI_Inventory.GetAmountOfType(SelectedItemType);
            SelectedItemName.text = SelectedItemType.i_Name;
            if (SelectedItemImage != null)
            {
                SelectedItemImage.sprite = SelectedItemType.i_Sprite;
            }
            SelectedItemDescription.text = SelectedItemType.i_Description;
            SelectedItemAmount.text = $"x{selectedItemAmount}";

        }
        else
        {
            SelectedItemName.text = "";
            if (SelectedItemImage != null)
            {
                SelectedItemImage.sprite = DefaultItemSprite;
            }
            SelectedItemDescription.text = "";
            SelectedItemAmount.text = $"";
        }
    }
    public override void CloseMenu()
    {
        UIParent.gameObject.SetActive(false);
    }
}
