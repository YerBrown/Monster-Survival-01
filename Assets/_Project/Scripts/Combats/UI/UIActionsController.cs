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
    public Sprite FisicalAttackIcon;
    public Sprite RangeAttackIcon;
    public Sprite DefenseIcon;
    public Sprite MoveIcon;
    public Sprite CaptureIcon;
    [Header("Events")]
    public VoidEventChannelSO OpenPlayerInventory;
    [Header("Other")]
    public UITargetController TargetController;
    public ElementType CurrentFighterElement;
    public void EnableAction(bool enable)
    {
        ActionsParent.gameObject.SetActive(enable);
        Fighter currentFighter = CombatManager.Instance.CurrentTurnFighter;
        FighterData currentData = CombatManager.Instance.TeamsController.GetFighterDataByFighter(currentFighter);
        CurrentFighterElement = FightersInfoWiki.Instance.FightersDictionary[currentData.TypeID].c_Element;
        SpecialMoveButton.interactable = currentFighter.EnergyPoints >= GeneralValues.StaticCombatGeneralValues.Fighter_EnergyNeededFor_SpecialMovement;
        if (currentData != null)
        {
            CreatureSO creatureInfo = currentData.GetCreatureInfo();
            if (creatureInfo != null)
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
        TargetController.DisableAllTargets();
        EnableAction(true);
        CombatManager.Instance.UIManager.NotificationController.DisableActionInfoPopup();
    }
    public void OnFisicalAttack()
    {
        if (CombatManager.Instance != null)
        {
            EnableAction(false);
            EnableCancelButton(true);
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.ActionsFlowManager.FisicalAttack);
            TargetController.EnableEnemyTargets();
            CombatManager.Instance.UIManager.NotificationController.EnableActionInfoPopup(FisicalAttackIcon, "Fisical Attack", "Attack", CurrentFighterElement, "This attack makes direct contact with the target.");
        }
    }
    public void OnRangeAttack()
    {
        if (CombatManager.Instance != null)
        {
            EnableAction(false);
            EnableCancelButton(true);
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.ActionsFlowManager.RangeAttack);
            TargetController.EnableEnemyTargets();
            CombatManager.Instance.UIManager.NotificationController.EnableActionInfoPopup(RangeAttackIcon, "Range Attack", "Attack", CurrentFighterElement, "This attack hits the target from distance.");
        }
    }

    public void OnDefenseMode()
    {
        if (CombatManager.Instance != null)
        {
            EnableAction(false);
            EnableCancelButton(true);
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.ActionsFlowManager.SetDefenseMode);
            TargetController.EnablePlayerTarget(CombatManager.Instance.TeamsController.GetFighterInFieldNum(CombatManager.Instance.CurrentTurnFighter));
            CombatManager.Instance.UIManager.NotificationController.EnableActionInfoPopup(DefenseIcon, "Enable Shield", "Defense", ElementType.NO_TYPE, "Activates a shield that causes the fighter to take half damage the next time he is hattacked.");
        }
    }
    public void OnSpecialMovement()
    {
        if (CombatManager.Instance != null)
        {
            EnableAction(false);
            EnableCancelButton(true);
            CombatManager.Instance.SetSelectedAction(CombatManager.Instance.ActionsFlowManager.MultipleTargetAttack);
            TargetController.EnableEnemyTargets();
            CombatManager.Instance.UIManager.NotificationController.EnableActionInfoPopup(RangeAttackIcon, "Multiple Attack", "Attack", ElementType.NO_TYPE, "This attack hits the target and adjacent fighters.");
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
        TargetController.EnablePlayerFighterParnersTargets(CombatManager.Instance.TeamsController.GetFighterInFieldNum(currentTurnFighter));
        CombatManager.Instance.UIManager.NotificationController.EnableActionInfoPopup(MoveIcon, "Swipe Position", "Action", ElementType.NO_TYPE, "Swipe the position with another team member.");
    }
    public void OnTryToCapture()
    {
        EnableAction(false);
        CombatManager.Instance.UIManager.CaptureController.OpenPopup();
        CombatManager.Instance.SetSelectedAction(() =>
        {
            CombatManager.Instance.UIManager.CaptureController.ClosePopup();
            CombatManager.Instance.ActionsFlowManager.TryCaptureFighter();
        });
        CombatManager.Instance.UIManager.NotificationController.EnableActionInfoPopup(CaptureIcon, "Capture Creature", "Action", ElementType.NO_TYPE, "Try to capture the target creature.");
    }
    public void OnTryQuit()
    {
        SceneLoadManager.Instance.LoadExpeditionFromCombat();
    }
    public void OnUseItem()
    {
        OpenPlayerInventory.RaiseEvent();
        EnableAction(false);
    }
}
