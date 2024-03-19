using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyActionsManager : MonoBehaviour
{
    private CombatManager _CombatManager;
    private (Fighter, int) _Target;
    private void Start()
    {
        _CombatManager = CombatManager.Instance;
    }
    public void SelectNextAction()
    {
        Fighter currentFighter = _CombatManager.CurrentTurnFighter;

        int randomAction = 0;
        // Check if are posible targets in range.
        if (GetOppositeRandomTarget().Item1 != null)
        {
            if (currentFighter.EnergyPoints >= currentFighter.Stats.MaxEnergyPoints / 2)
            {
                randomAction = Random.Range(0, 100);
            }
            else
            {
                randomAction = Random.Range(0, 18);
            }
        }
        else
        {
            // If there aren't targets posible, use defense mode.
            randomAction = Random.Range(0, 6);
        }

        switch (randomAction)
        {
            case < 3:
                _CombatManager.SetSelectedAction(_CombatManager.ActionsFlowManager.SwipePositions);

                _Target = GetRandomPartnerTarget();
                _CombatManager.SelectTargetFighter(_Target.Item1, _Target.Item2);
                break;
            case < 6:
                _CombatManager.SetSelectedAction(_CombatManager.ActionsFlowManager.SetDefenseMode);

                _Target = GetCurrentFighter();
                _CombatManager.SelectTargetFighter(_Target.Item1, _Target.Item2);
                break;
            case < 12:
                _CombatManager.SetSelectedAction(_CombatManager.ActionsFlowManager.FisicalAttack);

                _Target = GetOppositeRandomTarget();
                _CombatManager.SelectTargetFighter(_Target.Item1, _Target.Item2);
                break;
            case < 18:
                _CombatManager.SetSelectedAction(_CombatManager.ActionsFlowManager.RangeAttack);

                _Target = GetOppositeRandomTarget();
                _CombatManager.SelectTargetFighter(_Target.Item1, _Target.Item2);
                break;
            case < 100:
                _CombatManager.SetSelectedAction(_CombatManager.ActionsFlowManager.MultipleTargetAttack);

                _Target = GetOppositeRandomTarget();
                _CombatManager.SelectTargetFighter(_Target.Item1, _Target.Item2);
                break;
            default:
                break;
        }
    }
    public (Fighter, int) GetCurrentFighter()
    {
        Fighter currentFighter = _CombatManager.CurrentTurnFighter;
        int index = _CombatManager.TeamsController.GetFighterInFieldNum(currentFighter);

        return new(currentFighter, index);
    }
    public (Fighter, int) GetOppositeRandomTarget()
    {
        List<int> posibleTargets = _CombatManager.TeamsController.GetFightersNumInRange(_CombatManager.CurrentTurnFighter);
        List<Fighter> posibleFighterTargets = new();
        CombatTeam playerTeam = _CombatManager.TeamsController.PlayerTeam;
        for (int i = 0; i < posibleTargets.Count; i++)
        {
            if (playerTeam.FightersInField[posibleTargets[i]] != null)
            {
                posibleFighterTargets.Add(playerTeam.FightersInField[posibleTargets[i]]);
            }
        }

        int randomNum = Random.Range(0, posibleFighterTargets.Count);
        return new(posibleFighterTargets[randomNum], randomNum);
    }

    public (Fighter, int) GetRandomPartnerTarget()
    {
        List<int> posibleTargets = new();
        CombatTeam enemyTeam = _CombatManager.TeamsController.EnemyTeam;
        for (int i = 0; i < enemyTeam.FightersInField.Length; i++)
        {
            if (enemyTeam.FightersInField[i] != _CombatManager.CurrentTurnFighter)
            {
                posibleTargets.Add(i);
            }
        }
        int randomNum = Random.Range(0, posibleTargets.Count);
        return new(enemyTeam.FightersInField[posibleTargets[randomNum]], posibleTargets[randomNum]);
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
