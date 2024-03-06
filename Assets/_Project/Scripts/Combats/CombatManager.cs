using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    private static CombatManager _instance;
    public static CombatManager Instance { get { return _instance; } }

    public CombatTeam PlayerTeam;
    public CombatTeam EnemyTeam;

    [SerializeField] private List<Fighter> _CurrentTurnOrder = new();
    [SerializeField] private List<Fighter> _NextTurnOrder = new();
    public int CurrentTurn = 1;
    public Fighter CurrentTurnFighter;
    public Fighter TargetActionFighter;

    public Transform CurrentFighterPointer;
    public Transform NextFighterPointer;
    public float PointerYOffset = 1f;
    public Action OnFinishedSelectAction;
    public CombatState CurrentCombatState = CombatState.WAITING_ACTION;

    public UICombatManager UIManager;
    public VoidEventChannelSO FinishCurrentFighterAction;
    [Serializable]
    public class CombatTeam
    {
        public FighterData[] Fighters = new FighterData[6];
        public Fighter[] FightersInField = new Fighter[3];
        public Transform[] FightersPos = new Transform[3];
    }

    public enum CombatState
    {
        WAITING_ACTION,
        SELECTING_TARGET,
        DURING_ACTION,
        ACTION_FINISHED
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    private void Start()
    {
        SpawnFighters();
        _CurrentTurnOrder = CalculateTurnOrder();
        _NextTurnOrder = CalculateTurnOrder();
        if (UIManager != null)
        {
            for (int i = 0; i < PlayerTeam.FightersInField.Length; i++)
            {
                UIManager.AsignFighter(PlayerTeam.FightersInField[i], i);
            }
        }
        SelectNextFighter();
    }
    public void SpawnFighters()
    {
        SpawnFightersByTeam(PlayerTeam);
        SpawnFightersByTeam(EnemyTeam);
    }
    public void FisicalAttack()
    {
        if (CurrentTurnFighter != null && TargetActionFighter != null)
        {
            int calculatedDamage = TargetActionFighter.ReceiveDamage(CurrentTurnFighter.Stats.HitPower);
            Invoke(nameof(FinishFighterMove), 2f);
            Debug.Log($"{CurrentTurnFighter.Nickname} attacked fisicaly to {TargetActionFighter.Nickname} dealing {calculatedDamage} hp");
        }
    }
    public void RangeAttack()
    {
        if (CurrentTurnFighter != null && TargetActionFighter != null)
        {
            int calculatedDamage = TargetActionFighter.ReceiveDamage(CurrentTurnFighter.Stats.RangePower);
            Invoke(nameof(FinishFighterMove), 2f);
            Debug.Log($"{CurrentTurnFighter.Nickname} attacked in raange to {TargetActionFighter.Nickname} dealing {calculatedDamage} hp");
        }
    }
    public void SetDefenseMode()
    {
        if (CurrentTurnFighter != null && TargetActionFighter != null)
        {
            TargetActionFighter.SetDefenseMode();
            Invoke(nameof(FinishFighterMove), 2f);
            Debug.Log($"{TargetActionFighter.Nickname} set to defense mode");
        }
    }
    public void SetSelectedAction(Action newAction)
    {
        OnFinishedSelectAction = newAction;
    }
    public void SelectTargetFighter(Fighter fighter)
    {
        TargetActionFighter = fighter;

        OnFinishedSelectAction.Invoke();
        UIManager.EnableAction(false);
    }
    public void ChangeFighter(Fighter fighterChanged, FighterData newFighterData, CombatTeam team)
    {
        for (int i = 0; i < team.FightersInField.Length; i++)
        {
            if (team.FightersInField[i] == fighterChanged)
            {
                Destroy(team.FightersInField[i].gameObject);
                team.FightersInField[i] = null;
                SpawnFighterInTeam(i, newFighterData, team);
                // Calculate the order for the nex turn
                _NextTurnOrder = CalculateTurnOrder();
                Invoke(nameof(FinishFighterMove), 2f);
            }
        }
    }
    private void SpawnFightersByTeam(CombatTeam team)
    {
        if (FightersInfoWiki.Instance == null) { return; }
        for (int i = 0; i < team.FightersInField.Length; i++)
        {
            FighterData nextFighterData = GetNextFighter(team);
            if (team.FightersInField[i] == null && nextFighterData != null)
            {
                SpawnFighterInTeam(i, nextFighterData, team);
            }
        }
    }
    private void SpawnFighterInTeam(int fighterNum, FighterData fighterData, CombatTeam team)
    {
        GameObject fighterPrefab = FightersInfoWiki.Instance.GetFighterPrefab(fighterData.TypeID);
        if (fighterPrefab != null)
        {
            Fighter newFighter = Instantiate(fighterPrefab, team.FightersPos[fighterNum]).GetComponent<Fighter>();

            newFighter.transform.position = team.FightersPos[fighterNum].position + Vector3.forward;
            newFighter.UpdateFighter(fighterData);
            team.FightersInField[fighterNum] = newFighter;
        }
        else
        {
            return;
        }
    }
    private FighterData GetNextFighter(CombatTeam team)
    {
        foreach (FighterData fighterData in team.Fighters)
        {
            if (GetFighterByID(fighterData.ID, team) == null)
            {
                return fighterData;
            }
            else
            {
                continue;
            }
        }
        return null;
    }
    private Fighter GetFighterByID(string id, CombatTeam team)
    {
        foreach (Fighter fighter in team.FightersInField)
        {
            if (fighter != null && fighter.ID == id)
            {
                return fighter;
            }
        }
        return null;
    }
    private List<Fighter> CalculateTurnOrder()
    {
        List<Fighter> allFighterInField = new();
        foreach (var fighterInField in PlayerTeam.FightersInField)
        {
            if (fighterInField != null)
            {
                allFighterInField.Add(fighterInField);
            }
        }
        foreach (var fighterInField in EnemyTeam.FightersInField)
        {
            if (fighterInField != null)
            {
                allFighterInField.Add(fighterInField);
            }
        }
        List<Fighter> fightersOrderBySpeed = new();
        var groupedBySpeed = allFighterInField.GroupBy(fighter => fighter.Stats.Speed);
        groupedBySpeed = groupedBySpeed.OrderByDescending(fighter => fighter.Key);
        foreach (var groupSpeed in groupedBySpeed)
        {
            if (groupSpeed.Count() > 1)
            {
                // If there are more than one fighter with the same speed, calculate the initiative of each fighter and reorder the group
                var fightersWithRandomNumbers = groupSpeed.Select(fighter => new
                {
                    Fighter = fighter,
                    Initiative = fighter.Stats.Speed + new System.Random().NextDouble() // Generate the intuitive number randomly 
                });
                fightersOrderBySpeed.AddRange(fightersWithRandomNumbers.OrderBy(f => f.Initiative).Select(f => f.Fighter));

            }
            else
            {
                fightersOrderBySpeed.Add(groupSpeed.First());
            }
        }
        return fightersOrderBySpeed;
    }
    public void FinishFighterMove()
    {
        _CurrentTurnOrder.RemoveAt(0);
        if (_CurrentTurnOrder.Count == 0)
        {
            FinishTurn();
        }
        else
        {
            SelectNextFighter();
        }
    }
    private void FinishTurn()
    {
        CurrentTurn++;
        _CurrentTurnOrder = new List<Fighter>(_NextTurnOrder);
        _NextTurnOrder = CalculateTurnOrder();
        SelectNextFighter();
    }
    public void UpdateFighterData(Fighter fighter)
    {
        FighterData fighterToUpdate = GetFighterDataByFighter(fighter);
        fighterToUpdate.HealthPoints = fighter.HealthPoints;
        fighterToUpdate.EnergyPoints = fighter.EnergyPoints;
    }
    private FighterData GetFighterDataByFighter(Fighter fighter)
    {
        if (IsPlayerTeamFighter(fighter))
        {
            foreach (var fighterData in PlayerTeam.Fighters)
            {
                if (fighterData.ID == fighter.ID)
                {
                    return fighterData;
                }
            }
            return null;
        }
        else
        {
            foreach (var fighterData in EnemyTeam.Fighters)
            {
                if (fighterData.ID == fighter.ID)
                {
                    return fighterData;
                }
            }
            return null;
        }
    }
    private void SelectNextFighter()
    {
        CurrentTurnFighter = _CurrentTurnOrder.First();
        if (CurrentFighterPointer != null)
        {
            CurrentFighterPointer.transform.position = Camera.main.WorldToScreenPoint(CurrentTurnFighter.transform.position + Vector3.up * PointerYOffset);
        }
        if (NextFighterPointer != null)
        {
            NextFighterPointer.transform.position = Camera.main.WorldToScreenPoint(GetCurrentAndNextTurn()[1].transform.position + Vector3.up * PointerYOffset);
        }
        FinishCurrentFighterAction.RaiseEvent();
        UIManager.HiglightFighter(CurrentTurnFighter);
        if (IsPlayerTeamFighter(CurrentTurnFighter))
        {
            UIManager.EnableAction(true);
        }
        else
        {
            SetNextEnemyAction();
        }
    }
    public void OnFighterDied(Fighter diedFighter)
    {
        if (diedFighter == null) { return; }
        CombatTeam fighterTeam = GetCombatTeamOfFighter(diedFighter);
        if (fighterTeam == PlayerTeam)
        {
            if (IsTeamWithMoreFightersAliveThanInTheField(PlayerTeam))
            {
                // TODO: Open change menu in case of more fighters alive

            }
            else
            {
                if (PlayerTeam.FightersInField.Count(fighter => fighter != null) > 1)
                {

                }
                else
                {
                    // TODO: Player Losed all fighters
                }
            }
        }
        else
        {
            if (IsTeamWithMoreFightersAliveThanInTheField(EnemyTeam))
            {
                // TODO: Change fighter with another one alive
            }
            else
            {
                if (EnemyTeam.FightersInField.Count(fighter => fighter != null) > 1)
                {

                }
                else
                {
                    // TODO: Enemy Losed all fighters
                }
            }
        }
    }
    private bool IsTeamWithMoreFightersAliveThanInTheField(CombatTeam team)
    {
        foreach (var fighterData in team.Fighters)
        {
            if (fighterData.HealthPoints > 0)
            {
                bool currentlyInField = team.FightersInField.Any(fighter => fighter.ID == fighterData.ID);
                if (!currentlyInField)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public List<Fighter> GetCurrentAndNextTurn()
    {
        List<Fighter> currentNextTurns = new List<Fighter>();
        currentNextTurns.AddRange(_CurrentTurnOrder);
        currentNextTurns.AddRange(_NextTurnOrder);
        return currentNextTurns;
    }
    public bool IsPlayerTeamFighter(Fighter fighter)
    {
        foreach (var fighterInField in PlayerTeam.FightersInField)
        {
            if (fighterInField == fighter)
            {
                return true;
            }
        }
        return false;
    }
    public int GetAmountOfFighterInField()
    {
        int playerTeamFighter = PlayerTeam.FightersInField.Count(fighter => fighter != null); ;
        int enemyTeamFighter = EnemyTeam.FightersInField.Count(fighter => fighter != null); ;
        return playerTeamFighter + enemyTeamFighter;
    }
    private CombatTeam GetCombatTeamOfFighter(Fighter fighter)
    {
        if (IsPlayerTeamFighter(fighter))
        {
            return PlayerTeam;
        }
        else
        {
            return EnemyTeam;
        }
    }
    public int GetFighterInFieldNum(Fighter fighter)
    {
        CombatTeam team = GetCombatTeamOfFighter(fighter);
        for (int i = 0; i < team.FightersInField.Length; i++)
        {
            if (fighter == team.FightersInField[i])
            {
                return i;
            }
        }
        return -1;
    }
    public Fighter GetRandonFighterOfTeam(CombatTeam team)
    {
        List<Fighter> activeFighters = team.FightersInField.Where(fighter => fighter != null).ToList();
        return activeFighters[UnityEngine.Random.Range(0, activeFighters.Count)];
    }
    private void SetNextEnemyAction()
    {
        int randomAction = UnityEngine.Random.Range(0, 3);
        switch (randomAction)
        {
            case 0:
                SetSelectedAction(FisicalAttack);
                SelectTargetFighter(GetRandonFighterOfTeam(PlayerTeam));
                break;
            case 1:
                SetSelectedAction(RangeAttack);
                SelectTargetFighter(GetRandonFighterOfTeam(PlayerTeam));
                break;
            case 2:
                SetSelectedAction(SetDefenseMode);
                SelectTargetFighter(CurrentTurnFighter);
                break;
            default:
                break;
        }
    }
}
