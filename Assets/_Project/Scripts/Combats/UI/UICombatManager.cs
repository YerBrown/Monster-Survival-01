using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICombatManager : MonoBehaviour
{
    public UIActionsController ActionsController;
    public UIFighterChangeController ChangeFighterController;
    public UINotificationsController NotificationController;
    public void EnableAction(bool enable)
    {
        if (ActionsController != null)
        {
            ActionsController.EnableAction(enable);
        }
    }
}
