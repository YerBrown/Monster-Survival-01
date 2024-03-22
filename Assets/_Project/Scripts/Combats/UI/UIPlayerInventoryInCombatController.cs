using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInventoryInCombatController : PlayerInventoryManagementController
{
    public Sprite SelectedFilterSprite;
    public Sprite UnselectedFilterSprite;

    public List<Button> FilterButtons = new();
    public Button CancelButton;
    public CombatItemType CombatFilter;
    public UITargetController TargetController;
    public UIActionsController ActionsController;
    public override void OpenPopup()
    {
        base.OpenPopup();
        CancelButton.gameObject.SetActive(false);

        SelectItemSlot(UIPlayerInventory, GetFirstCombatItem());
    }
    private ItemsSO GetFirstCombatItem()
    {
        foreach (var item in UIPlayerInventory.Items)
        {
            if (item.Slot.ItemInfo.i_ItemType == ItemType.COMBAT)
            {
                return item.Slot.ItemInfo;
            }
        }
        return null;
    }
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
    public void UseSelectedItem()
    {
        switch (SelectedItemType.i_TargetType)
        {
            case TargetSelectType.ALL:
                TargetController.EnableAllTarget(true);
                break;
            case TargetSelectType.PLAYER_TEAM:
                TargetController.EnablePlayerTargets(true);
                break;
            case TargetSelectType.ENEMY_TEAM:
                TargetController.EnableEnemyTargets(true);
                break;
            default:
                break;
        }
        UIParent.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(true);
        CombatManager.Instance.SetSelectedAction(() =>
        {
            CancelButton.gameObject.SetActive(false);
            CombatManager.Instance.ThrowItem((CombatItemSO)SelectedItemType);
        });
    }
    public void CancelTarget()
    {
        OpenPopup();
        TargetController.EnableAllTarget(false);
    }
    public override void CloseMenu()
    {
        UIParent.gameObject.SetActive(false);
        ActionsController.EnableAction(true);
    }
    // Enable combat type filter
    public void SelectCombatFilter(int filterIndex)
    {
        for (int i = 0; i < FilterButtons.Count; i++)
        {
            if (i == filterIndex)
            {
                FilterButtons[i].GetComponent<Image>().sprite = SelectedFilterSprite;
            }
            else
            {
                FilterButtons[i].GetComponent<Image>().sprite = UnselectedFilterSprite;
            }
        }
        MainFilter = ItemType.COMBAT;
        CombatFilter = (CombatItemType)Enum.Parse(typeof(CombatItemType), filterIndex.ToString());
        IsFilterEnabled = true;
        EnableFilter();
    }
    // DIsable combat type filter
    public override void DisableFilter()
    {
        for (int i = 0; i < FilterButtons.Count; i++)
        {
            if (i == 0)
            {
                FilterButtons[i].GetComponent<Image>().sprite = SelectedFilterSprite;
            }
            else
            {
                FilterButtons[i].GetComponent<Image>().sprite = UnselectedFilterSprite;
            }
        }
        MainFilter = ItemType.COMBAT;
        CombatFilter = CombatItemType.GENERAL;
        IsFilterEnabled = true;
        EnableFilter();
    }
    // Manage filters
    public override void EnableFilter()
    {
        UIPlayerInventory.SetFilter(IsFilterEnabled, MainFilter, CombatFilter);
    }
}
