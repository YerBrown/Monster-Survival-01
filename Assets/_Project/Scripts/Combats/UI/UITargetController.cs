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
        List<int> posibleTargets = CombatManager.Instance.TeamsController.GetFightersNumInRange(CombatManager.Instance.CurrentTurnFighter);
        for (int i = 0; i < EnemyTargets.Count; i++)
        {
            EnemyTargets[i].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.EnemyTeam.FightersPos[i].transform.position + Vector3.up * 0.15f);
            if (posibleTargets.Contains(i))
            {
                EnableEnemyTarget(i, enable);
            }
        }
    }
    public void EnableEnemyFighterParnersTargets(bool enable, int fighterNum)
    {
        for (int i = 0; i < EnemyTargets.Count; i++)
        {
            EnemyTargets[i].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.EnemyTeam.FightersPos[i].transform.position + Vector3.up * 0.15f);
            if (i != fighterNum)
            {
                EnemyTargets[i].SetActive(enable);
            }
        }
    }
    public void EnablePlayerTargets(bool enable)
    {
        List<int> posibleTargets = CombatManager.Instance.TeamsController.GetFightersNumInRange(CombatManager.Instance.CurrentTurnFighter);
        for (int i = 0; i < PlayerTargets.Count; i++)
        {
            PlayerTargets[i].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.PlayerTeam.FightersPos[i].transform.position + Vector3.up * 0.15f);
            if (posibleTargets.Contains(i))
            {
                EnablePlayerTarget(i, enable);
            }
        }
    }
    public void EnablePlayerFighterParnersTargets(bool enable, int fighterNum)
    {
        for (int i = 0; i < PlayerTargets.Count; i++)
        {
            PlayerTargets[i].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.PlayerTeam.FightersPos[i].transform.position + Vector3.up * 0.15f);
            if (i != fighterNum)
            {
                PlayerTargets[i].SetActive(enable);
            }
        }
    }
    public void EnableEnemyTarget(int id, bool enable)
    {
        EnemyTargets[id].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.EnemyTeam.FightersPos[id].transform.position + Vector3.up * 0.15f);
        if (CombatManager.Instance != null && CombatManager.Instance.TeamsController.EnemyTeam.FightersInField[id] != null)
        {
            EnemyTargets[id].SetActive(enable);
        }
    }
    public void EnablePlayerTarget(int id, bool enable)
    {
        PlayerTargets[id].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.PlayerTeam.FightersPos[id].transform.position + Vector3.up * 0.15f);
        if (CombatManager.Instance != null && CombatManager.Instance.TeamsController.PlayerTeam.FightersInField[id] != null)
        {
            PlayerTargets[id].SetActive(enable);
        }
    }
    public void SelectEnemyTarget(int id)
    {
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.SelectTargetFighter(CombatManager.Instance.TeamsController.EnemyTeam.FightersInField[id], id);
            EnableAllTarget(false);
        }
    }
    public void SelectPlayerTarget(int id)
    {
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.SelectTargetFighter(CombatManager.Instance.TeamsController.PlayerTeam.FightersInField[id], id);
            EnableAllTarget(false);
        }
    }
}
