using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITargetController : MonoBehaviour
{
    public List<GameObject> PlayerTargets = new();
    public List<GameObject> EnemyTargets = new();
    public List<TMP_Text> EnemyCaptureRateTexts = new();
    public UIActionsController ActionsController;
    public void EnableAllTarget()
    {
        EnableEnemyTargets();
        EnablePlayerTargets();
        ActionsController.EnableCancelButton(true);
    }
    public void DisableAllTargets()
    {
        foreach (var playerTarget in PlayerTargets)
        {
            playerTarget.SetActive(false);
        }
        foreach (var enemyTarget in EnemyTargets)
        {
            enemyTarget.SetActive(false);
        }
        foreach(var captureRateText in EnemyCaptureRateTexts)
        {
            captureRateText.gameObject.SetActive(false);
        }
        ActionsController.EnableCancelButton(false);
    }
    public void EnableEnemyTargets()
    {
        List<int> posibleTargets = CombatManager.Instance.TeamsController.GetFightersNumInRange(CombatManager.Instance.CurrentTurnFighter);
        for (int i = 0; i < EnemyTargets.Count; i++)
        {
            EnemyTargets[i].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.EnemyTeam.FightersPos[i].transform.position + Vector3.up * 0.15f);
            if (posibleTargets.Contains(i))
            {
                EnableEnemyTarget(i);
            }
        }
    }
    public void EnableEnemyFighterParnersTargets(int fighterNum)
    {
        for (int i = 0; i < EnemyTargets.Count; i++)
        {
            EnemyTargets[i].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.EnemyTeam.FightersPos[i].transform.position + Vector3.up * 0.15f);
            if (i != fighterNum)
            {
                EnemyTargets[i].SetActive(true);
            }
        }
    }
    public void EnablePlayerTargets()
    {
        List<int> posibleTargets = CombatManager.Instance.TeamsController.GetFightersNumInRange(CombatManager.Instance.CurrentTurnFighter);
        for (int i = 0; i < PlayerTargets.Count; i++)
        {
            PlayerTargets[i].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.PlayerTeam.FightersPos[i].transform.position + Vector3.up * 0.15f);
            if (posibleTargets.Contains(i))
            {
                EnablePlayerTarget(i);
            }
        }
    }
    public void EnablePlayerFighterParnersTargets(int fighterNum)
    {
        for (int i = 0; i < PlayerTargets.Count; i++)
        {
            PlayerTargets[i].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.PlayerTeam.FightersPos[i].transform.position + Vector3.up * 0.15f);
            if (i != fighterNum)
            {
                PlayerTargets[i].SetActive(true);
            }
        }
    }
    public void EnableEnemyTarget(int id)
    {
        EnemyTargets[id].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.EnemyTeam.FightersPos[id].transform.position + Vector3.up * 0.15f);
        if (CombatManager.Instance != null && CombatManager.Instance.TeamsController.EnemyTeam.FightersInField[id] != null)
        {
            EnemyTargets[id].SetActive(true);
        }
    }
    public void EnablePlayerTarget(int id)
    {
        PlayerTargets[id].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.PlayerTeam.FightersPos[id].transform.position + Vector3.up * 0.15f);
        if (CombatManager.Instance != null && CombatManager.Instance.TeamsController.PlayerTeam.FightersInField[id] != null)
        {
            PlayerTargets[id].SetActive(true);
        }
    }
    public void SelectEnemyTarget(int id)
    {
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.SelectTargetFighter(CombatManager.Instance.TeamsController.EnemyTeam.FightersInField[id], id);
            DisableAllTargets();
            CombatManager.Instance.UIManager.NotificationController.DisableActionInfoPopup();
        }
    }
    public void SelectPlayerTarget(int id)
    {
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.SelectTargetFighter(CombatManager.Instance.TeamsController.PlayerTeam.FightersInField[id], id);
            DisableAllTargets();
            CombatManager.Instance.UIManager.NotificationController.DisableActionInfoPopup();
        }
    }
    public void EnableCaptureRates(int captureIntensity)
    {
        List<int> posibleTargets = CombatManager.Instance.TeamsController.GetFightersNumInRange(CombatManager.Instance.CurrentTurnFighter);
        for (int i = 0; i < EnemyTargets.Count; i++)
        {
            if (posibleTargets.Contains(i))
            {
                EnemyCaptureRateTexts[i].text = $"{CombatManager.Instance.TeamsController.EnemyTeam.FightersInField[i].GetCaptureRate(captureIntensity)}%";
                EnemyCaptureRateTexts[i].gameObject.SetActive(true);
            }
        }
    }
}
