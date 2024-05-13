using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UINotificationsController : MonoBehaviour
{
    [SerializeField] UIWinAnimationController WinController;
    [SerializeField] UIActionInfoController ActionInfoController;
    [SerializeField] UIActionInFighterDataController ActionInFighterDataController;
    [SerializeField] UIFighterHealthChangeNotificactionController FighterHealthChangeNotificationController;
    public TMP_Text CombatBeginsText;
    Sequence CombatBeginsSequence;
    public void PlayerWin()
    {
        WinController.PlayPlayerWin();
    }
    public void EnemyWin()
    {
        WinController.PlayEnemyWin();
    }
    public void EnableActionInfoPopup(Sprite actionImage, string actionName, string actionType, ElementType element, string actionDescription)
    {
        ActionInfoController.OpenInfoPanel(actionImage, actionName, actionType, element, actionDescription);
    }
    public void DisableActionInfoPopup()
    {
        ActionInfoController.CloseInfoPanel();
    }
    public void EnableActionInFighterData(FighterData fighterData, int currentHP, int targetHP, int currentEnergy, int targetEnergy)
    {
        ActionInFighterDataController.UpdateFighterDataPanel(fighterData, currentHP, targetHP, currentEnergy, targetEnergy);
    }
    public void EnableStartCombat()
    {
        CombatBeginsSequence = DOTween.Sequence();
        CombatBeginsSequence.Append(CombatBeginsText.rectTransform.DOScale(2f, 2f));
        CombatBeginsSequence.Join(CombatBeginsText.DOFade(1, 0.5f));
        CombatBeginsSequence.Append(CombatBeginsText.DOFade(0, 0.5f)).OnComplete(() =>
        {
            CombatManager.Instance.SelectNextFighter();
        });

    }
    public void OnHealthChanged(Vector3 fighterPosition, int amount, Effectiveness effectiveness)
    {
        FighterHealthChangeNotificationController.NotificateHealthChange(fighterPosition, amount, effectiveness);
    }
}
