using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIActionsController : MonoBehaviour
{
    [Header("UI")]
    public Transform ActionsParent;
    public Button SpecialMoveButton;
    public Button CaptureButton;
    public Button UseItemsButton;
    public Button CancelButton;
    [Header("Events")]
    public VoidEventChannelSO OpenPlayerInventory;
    [Header("Other")]
    public UITargetController TargetController;
    public void EnableAction(bool enable)
    {

        ActionsParent.gameObject.SetActive(enable);
        Fighter currentFighter = CombatManager.Instance.CurrentTurnFighter;
        FighterData currentData = CombatManager.Instance.TeamsController.GetFighterDataByFighter(currentFighter);
        SpecialMoveButton.interactable = currentFighter.EnergyPoints >= GeneralValues.StaticCombatGeneralValues.Fighter_EnergyNeededFor_SpecialMovement;
        if (currentData != null)
        {
            if (FightersInfoWiki.Instance.GetCreatureInfo(currentData.TypeID, out CreatureSO creatureInfo))
            {
                CaptureButton.interactable = creatureInfo.c_Skills.Contains(Skills.CAPTURE);
                UseItemsButton.interactable = creatureInfo.c_Skills.Contains(Skills.USE_ITEMS);
            }
        }
    }
    public void EnableCancelButton(bool enable)
    {
        CancelButton.gameObject.SetActive(enable);
    }
    public void OnCancelAction()
    {
        TargetController.EnableAllTarget(false);
        EnableAction(true);
    }
    public void OnFisicalAttack()
    {
        if (CombatManager.Instance != null)
        {
            EnableAction(false);
            EnableCancelButton(true);
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.ActionsFlowManager.FisicalAttack);
            TargetController.EnableEnemyTargets(true);
        }
    }
    public void OnRangeAttack()
    {
        if (CombatManager.Instance != null)
        {
            EnableAction(false);
            EnableCancelButton(true);
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.ActionsFlowManager.RangeAttack);
            TargetController.EnableEnemyTargets(true);
        }
    }

    public void OnDefenseMode()
    {
        if (CombatManager.Instance != null)
        {
            EnableAction(false);
            EnableCancelButton(true);
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.ActionsFlowManager.SetDefenseMode);
            TargetController.EnablePlayerTarget(CombatManager.Instance.TeamsController.GetFighterInFieldNum(CombatManager.Instance.CurrentTurnFighter), true);
        }
    }
    public void OnSpecialMovement()
    {
        if (CombatManager.Instance != null)
        {
            EnableAction(false);
            EnableCancelButton(true);
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.ActionsFlowManager.MultipleTargetAttack);
            TargetController.EnableEnemyTargets(true);
        }
    }
    public void OnChange()
    {
        EnableAction(false);
        List<Fighter> changedFighters = new List<Fighter>() { CombatManager.Instance.CurrentTurnFighter };
        CombatManager.Instance.UIManager.ChangeFighterController.OpenPopup(changedFighters, false);
    }
    public void OnSwipePositions()
    {
        EnableAction(false);
        EnableCancelButton(true);
        CombatManager.Instance.SetSelectedAction(CombatManager.Instance.ActionsFlowManager.SwipePositions);
        Fighter currentTurnFighter = CombatManager.Instance.CurrentTurnFighter;
        TargetController.EnablePlayerFighterParnersTargets(true, CombatManager.Instance.TeamsController.GetFighterInFieldNum(currentTurnFighter));
    }
    public void OnTryToCapture()
    {

    }
    public void OnTryQuit()
    {

    }
    public void OnUseItem()
    {
        OpenPlayerInventory.RaiseEvent();
    }
}
