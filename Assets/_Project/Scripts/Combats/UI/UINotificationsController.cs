using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINotificationsController : MonoBehaviour
{
    [SerializeField] UIWinAnimationController WinController;
    [SerializeField] UIActionInfoController ActionInfoController;
    [SerializeField] UIActionInFighterDataController ActionInFighterDataController;
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
}
