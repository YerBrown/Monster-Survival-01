using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CombatActionsFlowManager : MonoBehaviour
{
    public List<CombatAction> ActionsOrder = new();
    public bool IsWaitingForInput = false;
    public class CombatAction
    {
        public Action C_Action;
        public float Time;
        public bool IsInputRequired;
        public bool IsActionFinished;
        public CombatAction(Action c_Action, float time, bool inputRequired)
        {
            C_Action = c_Action;
            Time = time;
            IsInputRequired = inputRequired;
        }
    }

    public void StartFlow(List<CombatAction> actionsFlow)
    {
        StartCoroutine(PlayActionsInOrder(actionsFlow));
    }

    public void WriteInConsole(string text)
    {
        Debug.Log(text);
    }


    public IEnumerator PlayActionsInOrder(List<CombatAction> combatActions)
    {

        foreach (CombatAction combatAction in combatActions)
        {
            // Ejecuta la acción actual
            combatAction.C_Action?.Invoke();

            // Si la acción requiere input del jugador, esperar hasta que se reciba el input
            if (combatAction.IsInputRequired)
            {
                IsWaitingForInput = true;
                while (IsWaitingForInput)
                {
                    yield return null; // Esperar un frame antes de verificar el input
                }
            }
            else
            {
                // Esperar el tiempo de espera de la acción actual
                yield return new WaitForSeconds(combatAction.Time);
            }
        }
        Debug.Log("All Combat Actions Finished");
    }

    public void InputDone()
    {
        if (IsWaitingForInput)
        {
            IsWaitingForInput = false;
        }
    }

    public void FisicalAttack()
    {
        Fighter currentTurnFighter = CombatManager.Instance.CurrentTurnFighter;
        Fighter targetFighter = CombatManager.Instance.TargetActionFighter;
        if (currentTurnFighter != null && targetFighter != null)
        {
            List<CombatAction> allActions = new List<CombatAction>
            {
                new CombatAction((() =>
                {
                    CombatManager.Instance.SetFighterMoveTarget(false);
                }),0f, true),
                new CombatAction((() =>
                {
                    int calculatedDamage = targetFighter.ReceiveDamage(currentTurnFighter.Stats.HitPower);
                    currentTurnFighter.AddEnergyPoints(calculatedDamage / 2, true);
                    Debug.Log($"{currentTurnFighter.Nickname} attacked fisicaly to {targetFighter.Nickname} dealing {calculatedDamage} hp");
                }),1f, false),
                new CombatAction((() =>
                {
                    currentTurnFighter.ReturnToFighterPos();
                }),0f, true),
                 new CombatAction((() =>
                {
                    CombatManager.Instance.FinishFighterMove();
                }),0f, false),
            };

            StartFlow(allActions);
        }
    }
    public void RangeAttack()
    {
        Fighter currentTurnFighter = CombatManager.Instance.CurrentTurnFighter;
        Fighter targetFighter = CombatManager.Instance.TargetActionFighter;
        if (currentTurnFighter != null && targetFighter != null)
        {
            List<CombatAction> allActions = new List<CombatAction>
            {
                new CombatAction((() =>
                {
                    CombatManager.Instance.SetFighterMoveTarget(true);
                }),0f, true),
                new CombatAction((() =>
                {
                    int calculatedDamage = targetFighter.ReceiveDamage(currentTurnFighter.Stats.RangePower);
                    currentTurnFighter.AddEnergyPoints(calculatedDamage / 2, true);
                    Debug.Log($"{currentTurnFighter.Nickname} attacked in raange to {targetFighter.Nickname} dealing {calculatedDamage} hp");
                }),1f, false),
                new CombatAction((() =>
                {
                    currentTurnFighter.ReturnToFighterPos();
                }),0f, true),
                 new CombatAction((() =>
                {
                    CombatManager.Instance.FinishFighterMove();
                }),0f, false),
            };

            StartFlow(allActions);

        }
    }
    public void MultipleTargetAttack()
    {
        Fighter currentTurnFighter = CombatManager.Instance.CurrentTurnFighter;
        Fighter targetFighter = CombatManager.Instance.TargetActionFighter;
        if (currentTurnFighter != null && targetFighter != null)
        {
            List<CombatAction> allActions = new List<CombatAction>
            {
                new CombatAction((() =>
                {
                    CombatManager.Instance.SetFighterMoveTarget(true);
                }),0f, true),
                new CombatAction((() =>
                {
                    CombatTeam targetTeam = CombatManager.Instance.TeamsController.GetCombatTeamOfFighter(targetFighter);
                    int calculatedMinDamage = 0;
                    List<int> posibleTargets = CombatManager.Instance.GetFightersNumInRange(targetFighter);
                    for (int i = 0; i < targetTeam.FightersInField.Length; i++)
                    {
                        if (targetTeam.FightersInField[i]!=null && posibleTargets.Contains(i))
                        {
                            int calculatedDamage = targetTeam.FightersInField[i].ReceiveDamage(currentTurnFighter.Stats.RangePower);
                            if(calculatedDamage<calculatedMinDamage)
                            {
                                calculatedMinDamage=calculatedDamage;
                            }
                        }
                    }
                    currentTurnFighter.AddEnergyPoints(-(currentTurnFighter.Stats.MaxEnergyPoints/2), true);
                    Debug.Log($"{currentTurnFighter.Nickname} attacked to all opposing fighters");
                }),1f, false),
                new CombatAction((() =>
                {
                    currentTurnFighter.ReturnToFighterPos();
                }),0f, true),
                 new CombatAction((() =>
                {
                    CombatManager.Instance.FinishFighterMove();
                }),0f, false),
            };

            StartFlow(allActions);

        }
    }
    public void SetDefenseMode()
    {
        Fighter currentTurnFighter = CombatManager.Instance.CurrentTurnFighter;
        if (currentTurnFighter != null)
        {
            List<CombatAction> allActions = new List<CombatAction>
            {
                new CombatAction((() =>
                {
                    currentTurnFighter.SetDefenseMode();
                    currentTurnFighter.AddEnergyPoints(5, true);
                    Debug.Log($"{currentTurnFighter.Nickname} set to defense mode");
                }),1f, false),
                 new CombatAction((() =>
                {
                    CombatManager.Instance.FinishFighterMove();
                }),0f, false),
            };

            StartFlow(allActions);
        }
    }
    public void ChangeFighter(Fighter fighterChanged, FighterData newFighterData, CombatTeam team)
    {
        int fighterChangedNum = CombatManager.Instance.TeamsController.GetFighterInFieldNum(fighterChanged);
        if (fighterChanged != null && newFighterData != null && team != null)
        {
            List<CombatAction> allActions = new List<CombatAction>
            {
                new CombatAction((() =>
                {
                    // TODO : Change animation
                    Debug.Log("Change animation started");
                }),1f, false),
                new CombatAction((() =>
                {
                    CombatManager.Instance.TeamsController.RemoveFighterFromField(fighterChanged, team);
                    Debug.Log("Removed previous fighter");
                }),1f, false),
                new CombatAction((() =>
                {
                    CombatManager.Instance.TeamsController.SpawnFighterInTeam(fighterChangedNum, newFighterData, team);
                    Debug.Log("Spawned new fighter");
                }),1f, false),
                new CombatAction((() =>
                {
                    CombatManager.Instance.CalculateTurnOrderOnFighterChanged();
                    Debug.Log("Spawned new fighter");
                }),1f, false),
                 new CombatAction((() =>
                {
                    CombatManager.Instance.FinishFighterMove();
                }),0f, false),
            };

            StartFlow(allActions);
        }
    }
    public void ChangeDiedFighters(List<Fighter> diedPlayerFighters, List<Fighter> diedEnemyFighters, List<FighterData> fightersPlayerToChange, List<FighterData> fightersEnemyToChange)
    {
        CombatTeam playerTeam = CombatManager.Instance.TeamsController.PlayerTeam;
        CombatTeam enemyTeam = CombatManager.Instance.TeamsController.EnemyTeam;

        if ((diedPlayerFighters.Count > 0 && fightersPlayerToChange.Count > 0) || (diedEnemyFighters.Count > 0 && fightersEnemyToChange.Count > 0))
        {
            List<CombatAction> allActions = new List<CombatAction>
            {
                new CombatAction((() =>
                {
                    // TODO : Change animation
                    Debug.Log("Change animation started");
                }),1f, false),
                new CombatAction((() =>
                {
                    for (int i = 0; i < diedPlayerFighters.Count; i++)
                    {
                        CombatManager.Instance.TeamsController.RemoveFighterFromField(diedPlayerFighters[i], playerTeam);
                    }
                    for (int i = 0; i < diedEnemyFighters.Count; i++)
                    {
                        CombatManager.Instance.TeamsController.RemoveFighterFromField(diedEnemyFighters[i], enemyTeam);
                    }
                    Debug.Log("Removed previous fighters");
                }),1f, false),
                new CombatAction((() =>
                {
                    if (fightersPlayerToChange.Count>0)
                    {
                        for (int i = 0; i < fightersPlayerToChange.Count; i++)
                        {
                            if (fightersPlayerToChange[i]!=null && !string.IsNullOrEmpty(fightersPlayerToChange[i].ID))
                            {
                                    CombatManager.Instance.TeamsController.SpawnFighterInTeam(i, fightersPlayerToChange[i], playerTeam);
                            }
                        }
                    }
                    if (fightersEnemyToChange.Count>0)
                    {
                        for (int i = 0; i < fightersEnemyToChange.Count; i++)
                        {
                            if (fightersEnemyToChange[i]!=null && !string.IsNullOrEmpty(fightersEnemyToChange[i].ID))
                            {
                             CombatManager.Instance.TeamsController.SpawnFighterInTeam(i, fightersEnemyToChange[i], enemyTeam);
                            }
                        }

                    }

                    Debug.Log("Spawned new fighters");
                }),1f, false),
                new CombatAction((() =>
                {
                    CombatManager.Instance.CalculateTurnOrderOnFighterChanged();
                    Debug.Log("Calculate new turns order");
                }),1f, false),
                 new CombatAction((() =>
                {
                    CombatManager.Instance.FinishFighterMove();
                }),0f, false),

            };

            StartFlow(allActions);
        }
        else
        {
            List<CombatAction> allActions = new List<CombatAction>
            {
                new CombatAction((() =>
                {
                    // TODO : Change animation
                    Debug.Log("Change animation started");
                }),1f, false),
                new CombatAction((() =>
                {
                    for (int i = 0; i < diedPlayerFighters.Count; i++)
                    {
                        CombatManager.Instance.TeamsController.RemoveFighterFromField(diedPlayerFighters[i], playerTeam);
                    }
                    for (int i = 0; i < diedEnemyFighters.Count; i++)
                    {
                        CombatManager.Instance.TeamsController.RemoveFighterFromField(diedEnemyFighters[i], enemyTeam);
                    }
                    Debug.Log("Removed previous fighters");
                }),1f, false),
                new CombatAction((() =>
                {
                    CombatManager.Instance.CalculateTurnOrderOnFighterChanged();
                    Debug.Log("Calculate new turns order");
                }),1f, false),
                 new CombatAction((() =>
                {
                    CombatManager.Instance.FinishFighterMove();
                }),0f, false),

            };

            StartFlow(allActions);
        }
    }

}
