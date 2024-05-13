using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInventoryInCombatController : UIPlayerInventoryMenuController
{
    [Header("In Combat Variables")]
    public Sprite SelectedFilterSprite;
    public Sprite UnselectedFilterSprite;

    public List<Button> FilterButtons = new();
    public Button CancelButton;
    public Button TeamButton;
    public CombatItemType CombatFilter;
    public UITargetController TargetController;
    public UIActionsController ActionsController;
    public UIFighterChangeController FighterChangeController;
    public override void OpenPopup()
    {
        base.OpenPopup();
        CancelButton.gameObject.SetActive(false);
        TeamButton.gameObject.SetActive(false);
        if (SelectedItemType != null && SelectedItemType.i_ItemType == ItemType.COMBAT && UIPlayerInventory.UI_Inventory.GetAmountOfType(SelectedItemType) > 0)
        {
            SelectItemSlot(UIPlayerInventory, SelectedItemType);
        }
        else
        {
            SelectItemSlot(UIPlayerInventory, GetFirstCombatItem());
        }
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
    public void SelectFighterDataTarget(FighterData fighterData)
    {
        Fighter fighterInField = CombatManager.Instance.TeamsController.PlayerTeam.GetFighterInField(fighterData.ID);
        if (fighterInField != null)
        {
            int fighterIndex = CombatManager.Instance.TeamsController.PlayerTeam.GetFighterDataIndex(fighterData.ID);
            CombatManager.Instance.SelectTargetFighter(fighterInField, fighterIndex);
        }
        else
        {
            UseSelectedItemInTeam(fighterData);
        }
    }
    public void UseSelectedItem()
    {
        CombatManager.Instance.UIManager.NotificationController.EnableActionInfoPopup(SelectedItemType.i_Sprite, SelectedItemType.i_Name, "Item", ElementType.NO_TYPE, SelectedItemType.i_Description);
        OpenSelectTargets();
        CombatManager.Instance.SetSelectedAction(() =>
        {
            CancelButton.gameObject.SetActive(false);
            TeamButton.gameObject.SetActive(false);
            CombatManager.Instance.ThrowItem((CombatItemSO)SelectedItemType);
        });
    }
    public void OpenSelectTargets()
    {
        switch (SelectedItemType.i_TargetType)
        {
            case TargetSelectType.ALL:
                TargetController.EnableAllTarget(false);
                break;
            case TargetSelectType.PLAYER_TEAM:
                TargetController.EnablePlayerTargets();
                break;
            case TargetSelectType.ENEMY_TEAM:
                TargetController.EnableEnemyTargets();
                break;
            default:
                break;
        }
        UIParent.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(true);
        TeamButton.gameObject.SetActive(((CombatItemSO)SelectedItemType).IsUsableInTeamFighters);
    }
    public void UseSelectedItemInTeam(FighterData fighterTarget)
    {
        UIParent.gameObject.SetActive(false);
        CombatManager.Instance.UseItemInFighterData(fighterTarget, (CombatItemSO)SelectedItemType);
        CombatManager.Instance.UIManager.NotificationController.DisableActionInfoPopup();
    }
    public void UseSelectedItemInField(Fighter fighter)
    {
        int fighterIndex = CombatManager.Instance.TeamsController.PlayerTeam.GetFighterDataIndex(fighter.ID);
        CombatManager.Instance.SelectTargetFighter(fighter, fighterIndex);
        CombatManager.Instance.UIManager.NotificationController.DisableActionInfoPopup();
    }
    public void OpenTeam()
    {
        FighterChangeController.OpenPopupToSetItemTarget();
        TargetController.DisableAllTargets();
        CancelButton.gameObject.SetActive(false);
        TeamButton.gameObject.SetActive(false);
    }
    public void CancelTarget()
    {
        OpenPopup();
        TargetController.DisableAllTargets();
        CombatManager.Instance.UIManager.NotificationController.DisableActionInfoPopup();
    }
    public override void ClosePopup()
    {
        UIParent.gameObject.SetActive(false);
        ActionsController.OnCancelAction();
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
