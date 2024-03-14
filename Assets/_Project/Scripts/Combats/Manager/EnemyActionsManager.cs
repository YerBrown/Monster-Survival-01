using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyActionsManager : MonoBehaviour
{
    private CombatManager _CombatManager;
    private void Start()
    {
        _CombatManager = CombatManager.Instance;
    }
    public void SelectNextAction()
    {
        Fighter currentFighter = _CombatManager.CurrentTurnFighter;

        int randomAction = 0;
        // Check if are posible targets in range.
        if (GetTarget(_CombatManager.TeamsController.PlayerTeam) != null)
        {
            if (currentFighter.EnergyPoints >= currentFighter.Stats.MaxEnergyPoints / 2)
            {
                randomAction = Random.Range(0, 100);
            }
            else
            {
                randomAction = Random.Range(0, 8);
            }
        }
        else
        {
            // If there aren't targets posible, use defense mode.
            randomAction = 7;
        }

        switch (randomAction)
        {
            case < 5:
                _CombatManager.SetSelectedAction(_CombatManager.ActionsFlowManager.FisicalAttack);
                _CombatManager.SelectTargetFighter(GetTarget(_CombatManager.TeamsController.PlayerTeam));
                break;
            case < 7:
                _CombatManager.SetSelectedAction(_CombatManager.ActionsFlowManager.RangeAttack);
                _CombatManager.SelectTargetFighter(GetTarget(_CombatManager.TeamsController.PlayerTeam));
                break;
            case < 8:
                _CombatManager.SetSelectedAction(_CombatManager.ActionsFlowManager.SetDefenseMode);
                _CombatManager.SelectTargetFighter(_CombatManager.CurrentTurnFighter);
                break;
            case < 100:
                _CombatManager.SetSelectedAction(_CombatManager.ActionsFlowManager.MultipleTargetAttack);
                _CombatManager.SelectTargetFighter(GetTarget(_CombatManager.TeamsController.PlayerTeam));
                break;
            default:
                break;
        }
    }

    public Fighter GetTarget(CombatTeam team)
    {
        List<int> posibleTargets = _CombatManager.GetFightersNumInRange(_CombatManager.CurrentTurnFighter);
        List<Fighter> posibleFighterTargets = new();
        for (int i = 0; i < posibleTargets.Count; i++)
        {
            if (team.FightersInField[posibleTargets[i]] != null)
            {
                posibleFighterTargets.Add(team.FightersInField[posibleTargets[i]]);
            }
        }

        int randomNum = Random.Range(0, posibleFighterTargets.Count);
        return posibleFighterTargets[randomNum];
    }

    public List<FighterData> GetNewFighters(List<Fighter> deadFighters)
    {
        Dictionary<int, Fighter> deadFightersDictionary = new Dictionary<int, Fighter>();
        for (int i = 0; i < deadFighters.Count; i++)
        {
            deadFightersDictionary.Add(_CombatManager.TeamsController.GetFighterInFieldNum(deadFighters[i]), deadFighters[i]);
        }
        List<FighterData> posibleFighters = _CombatManager.TeamsController.EnemyTeam.GetFightersNotInField(deadFighters.Count);
        FighterData[] newFighters = new FighterData[3];
        int currentFighterNum = 0;
        if (posibleFighters.Count > 0)
        {
            foreach (var fighter in deadFightersDictionary)
            {
                if (currentFighterNum < posibleFighters.Count)
                {
                    newFighters[fighter.Key] = posibleFighters[currentFighterNum];
                    currentFighterNum++;
                }
            }
        }
        return newFighters.ToList();
    }
}
