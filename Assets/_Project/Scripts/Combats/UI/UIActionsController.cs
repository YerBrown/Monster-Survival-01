using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionsController : MonoBehaviour
{
    public Transform ActionsParent;
    public Button SpecialMoveButton;
    public Button CancelButton;

    public UITargetController TargetController;
    public VoidEventChannelSO OpenPlayerInventory;
    public void EnableAction(bool enable)
    {    
        ActionsParent.gameObject.SetActive(enable);
        Fighter currentFighter = CombatManager.Instance.CurrentTurnFighter;
        SpecialMoveButton.interactable = currentFighter.EnergyPoints >= currentFighter.Stats.MaxEnergyPoints / 2;
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
        CombatManager.Instance.OpenChangeMenu();
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
