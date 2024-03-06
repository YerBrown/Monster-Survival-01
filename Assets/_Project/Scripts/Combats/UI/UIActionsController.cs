using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionsController : MonoBehaviour
{
    public Transform ActionsParent;
    public UITargetController TargetController;
    public Button CancelButton;
    public void EnableAction(bool enable)
    {    
        ActionsParent.gameObject.SetActive(enable);
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
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.FisicalAttack);
            TargetController.EnableEnemyTargets(true);
        }
    }
    public void OnRangeAttack()
    {
        if (CombatManager.Instance != null)
        {
            EnableAction(false);
            EnableCancelButton(true);
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.RangeAttack);
            TargetController.EnableEnemyTargets(true);
        }
    }

    public void OnDefenseMode()
    {
        if (CombatManager.Instance != null)
        {
            EnableAction(false);
            EnableCancelButton(true);
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.SetDefenseMode);
            TargetController.EnablePlayerTarget(CombatManager.Instance.GetFighterInFieldNum(CombatManager.Instance.CurrentTurnFighter), true);
        }
    }
    public void OnTryToCapture()
    {

    }
    public void OnTryQuit()
    {

    }
    public void OnUseItem()
    {

    }
}
