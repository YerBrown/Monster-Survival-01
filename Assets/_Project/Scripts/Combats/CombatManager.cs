using System;
using System.Collections.Generic;
using System.Linq;
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
    public Fighter CurrentFighterTurn;
    public Fighter TargetActionFighter;

    public Transform CurrentFighterPointer;
    public Transform NextFighterPointer;
    public float PointerYOffset = 1f;

    public UICombatManager UIManager;
    public VoidEventChannelSO FinishCurrentFighterAction;
    [Serializable]
    public class CombatTeam
    {
        public FighterData[] Fighters = new FighterData[6];
        public Fighter[] FightersInField = new Fighter[3];
        public Transform[] FightersPos = new Transform[3];
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
        SelectNextFighter();
        if (UIManager != null)
        {
            for (int i = 0; i < PlayerTeam.FightersInField.Length; i++)
            {
                UIManager.AsignFighter(PlayerTeam.FightersInField[i], i);
            }
        }
    }
    public void SpawnFighters()
    {
        SpawnFightersByTeam(PlayerTeam);
        SpawnFightersByTeam(EnemyTeam);
    }
    public void FisicalAttack()
    {
        int calculatedDamage = CurrentFighterTurn.Stats.HitPower - TargetActionFighter.Stats.Defense / 2;
        TargetActionFighter.ReceiveDamage(calculatedDamage);
        FinishFighterMove();
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
                FinishFighterMove();
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
        List<Fighter> fightersOrderBySpeed = allFighterInField.OrderByDescending(fighter => fighter.Stats.Speed).ToList();
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
        SelectNextFighter();
    }

    private void SelectNextFighter()
    {
        CurrentFighterTurn = _CurrentTurnOrder.First();
        if (CurrentFighterPointer != null)
        {
            CurrentFighterPointer.transform.position = Camera.main.WorldToScreenPoint(CurrentFighterTurn.transform.position + Vector3.up * PointerYOffset);
        }
        if (NextFighterPointer != null)
        {
            NextFighterPointer.transform.position = Camera.main.WorldToScreenPoint(GetCurrentAndNextTurn()[1].transform.position + Vector3.up * PointerYOffset);
        }
        FinishCurrentFighterAction.RaiseEvent();
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
}
