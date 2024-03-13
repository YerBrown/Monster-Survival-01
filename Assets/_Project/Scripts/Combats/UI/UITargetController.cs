using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITargetController : MonoBehaviour
{
    public List<GameObject> PlayerTargets = new();
    public List<GameObject> EnemyTargets = new();
    public UIActionsController ActionsController;
    public void EnableAllTarget(bool enable)
    {
        EnableEnemyTargets(enable);
        EnablePlayerTargets(enable);
        ActionsController.EnableCancelButton(enable);
    }

    public void EnableEnemyTargets(bool enable)
    {
        for (int i = 0; i < EnemyTargets.Count; i++)
        {
            EnableEnemyTarget(i, enable);
        }
    }
    public void EnablePlayerTargets(bool enable)
    {
        for (int i = 0; i < PlayerTargets.Count; i++)
        {
            EnablePlayerTarget(i, enable);
        }
    }
    public void EnableEnemyTarget(int id, bool enable)
    {
        if (CombatManager.Instance != null && CombatManager.Instance.TeamsController.EnemyTeam.FightersInField[id] != null)
        {
            EnemyTargets[id].SetActive(enable);
        }
    }
    public void EnablePlayerTarget(int id, bool enable)
    {
        if (CombatManager.Instance != null && CombatManager.Instance.TeamsController.PlayerTeam.FightersInField[id] != null)
        {
            PlayerTargets[id].SetActive(enable);
        }
    }
    public void SelectEnemyTarget(int id)
    {
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.SelectTargetFighter(CombatManager.Instance.TeamsController.EnemyTeam.FightersInField[id]);
            EnableAllTarget(false);
        }
    }
    public void SelectPlayerTarget(int id)
    {
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.SelectTargetFighter(CombatManager.Instance.TeamsController.PlayerTeam.FightersInField[id]);
            EnableAllTarget(false);
        }
    }
}
