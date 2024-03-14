using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    private static CombatManager _instance;
    public static CombatManager Instance { get { return _instance; } }

    [Header("Turns")]
    [SerializeField] public List<Fighter> CurrentTurnOrder = new();
    [SerializeField] private List<Fighter> _NextTurnOrder = new();
    public int CurrentTurn = 1;
    [Header("Fighters")]
    public Fighter CurrentTurnFighter;
    public Fighter TargetActionFighter;
    [Header("UI")]
    public Transform CurrentFighterPointer;
    public Transform NextFighterPointer;
    public float PointerYOffset = 1f;
    [Header("Map Tiles")]
    public Tilemap BattleFieldTileMap;
    public OverlayTile OverlayTilePrefab;
    public GameObject OverlayContainer;
    public Dictionary<Vector2Int, OverlayTile> Map;
    public List<OverlayTile> AllTiles = new();
    [Header("Actions")]
    public Action OnFinishedSelectAction;
    [Header("Change Values")]
    private List<Fighter> _PlayerDeadFighters;
    private List<Fighter> _EnemyDeadFighters;
    private List<FighterData> _PlayerNewFighters;
    private List<FighterData> _EnemyNewFighters;
    [Header("Other")]
    public CombatTeamsController TeamsController;
    public CombatActionsFlowManager ActionsFlowManager;
    public UICombatManager UIManager;
    public EnemyActionsManager EnemyManager;
    public VoidEventChannelSO FinishCurrentFighterAction;
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
        StartCoroutine(StartCombat());
    }
    public IEnumerator StartCombat()
    {
        AddAllOverlayTiles();
        yield return new WaitForSeconds(.2f);
        TeamsController.SpawnStartingFighters();
        yield return new WaitForSeconds(.2f);
        CurrentTurnOrder = CalculateTurnOrder();
        _NextTurnOrder = CalculateTurnOrder();
        SelectNextFighter();
    }
    public void OpenChangeMenu()
    {
        if (CurrentTurnFighter != null)
        {
            List<Fighter> changedFighters = new List<Fighter>() { CurrentTurnFighter };
            UIManager.ChangeFighterController.OpenPopup(changedFighters, false);
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
    private void SelectNextFighter()
    {
        CurrentTurnFighter = CurrentTurnOrder.First();
        foreach (var fighterInField in TeamsController.PlayerTeam.FightersInField)
        {
            if (fighterInField != null)
            {
                if (fighterInField == CurrentTurnFighter)
                {
                    CurrentTurnFighter.CurrentTurnPointer.SetActive(true);
                }
                else
                {
                    fighterInField.CurrentTurnPointer.SetActive(false);
                }
            }
        }
        foreach (var fighterInField in TeamsController.EnemyTeam.FightersInField)
        {
            if (fighterInField != null)
            {
                if (fighterInField == CurrentTurnFighter)
                {
                    CurrentTurnFighter.CurrentTurnPointer.SetActive(true);
                }
                else
                {
                    fighterInField.CurrentTurnPointer.SetActive(false);
                }
            }
        }
        if (NextFighterPointer != null)
        {
            NextFighterPointer.transform.position = Camera.main.WorldToScreenPoint(GetCurrentAndNextTurn()[1].transform.position + Vector3.up * PointerYOffset);
        }
        FinishCurrentFighterAction.RaiseEvent();
        UIManager.HiglightFighter(CurrentTurnFighter);
        if (TeamsController.IsPlayerTeamFighter(CurrentTurnFighter))
        {
            UIManager.EnableAction(true);
        }
        else
        {
            EnemyManager.SelectNextAction();
        }
    }
    private List<Fighter> CalculateTurnOrder()
    {
        List<Fighter> allFighterInField = new();
        foreach (var fighterInField in TeamsController.PlayerTeam.FightersInField)
        {
            if (fighterInField != null)
            {
                allFighterInField.Add(fighterInField);
            }
        }
        foreach (var fighterInField in TeamsController.EnemyTeam.FightersInField)
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
        if (TeamsController.IsSomeOneDeadInField())
        {
            // TODO: Handle death
            (List<Fighter>, List<Fighter>) deadFighters = TeamsController.GetDiedFightersInField();
            _PlayerDeadFighters = deadFighters.Item1;
            _EnemyDeadFighters = deadFighters.Item2;
            _EnemyNewFighters = EnemyManager.GetNewFighters(_EnemyDeadFighters);


            if (TeamsController.IsTeamWithMoreFightersAliveThanInTheField(TeamsController.PlayerTeam) && _PlayerDeadFighters.Count > 0)
            {
                OpenChangeMenuOnPlayerFightersDied(_PlayerDeadFighters);
            }
            else
            {
                ChangePlayerDeadFighters(new List<FighterData>());
            }

            return;
        }
        if (TeamsController.PlayerTeam.IsAllTeamDefeated())
        {
            // TODO: Enemy won
            UIManager.PlayEnemyWinAnim();
            return;
        }
        else if (TeamsController.EnemyTeam.IsAllTeamDefeated())
        {
            // TODO: Player won
            UIManager.PlayPlayerWinAnim();
            return;
        }
        CurrentTurnOrder.RemoveAt(0);
        if (CurrentTurnOrder.Count == 0)
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
        CurrentTurnOrder = new List<Fighter>(_NextTurnOrder);
        _NextTurnOrder = CalculateTurnOrder();
        SelectNextFighter();
    }
    public List<Fighter> GetCurrentAndNextTurn()
    {
        List<Fighter> currentNextTurns = new List<Fighter>();
        currentNextTurns.AddRange(CurrentTurnOrder);
        currentNextTurns.AddRange(_NextTurnOrder);
        return currentNextTurns;
    }



    public void SetFighterMoveTarget(bool range)
    {
        if (CurrentTurnFighter != null)
        {
            Vector2 targetOffsetPosition = TeamsController.GetTargetAttackPosition(TargetActionFighter, range);
            CurrentTurnFighter.SetTargetPosition(GetTile(targetOffsetPosition));
        }
    }
    public OverlayTile GetTile(Vector2 tilePos)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(tilePos, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First().collider.gameObject.GetComponent<OverlayTile>();
        }
        else
        {
            return null;
        }
    }
    private void AddAllOverlayTiles()
    {
        Map = new Dictionary<Vector2Int, OverlayTile>();
        BoundsInt bounds = BattleFieldTileMap.cellBounds;
        //looping throug all of our tiles
        for (int z = bounds.max.z; z > bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);
                    if (BattleFieldTileMap.HasTile(tileLocation) && !Map.ContainsKey(tileKey))
                    {
                        OverlayTile overlayTile = Instantiate(OverlayTilePrefab, OverlayContainer.transform);
                        AllTiles.Add(overlayTile);
                        var cellWorldPosition = BattleFieldTileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = BattleFieldTileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gridLocation = tileLocation;
                        overlayTile.I_Element = null;
                        overlayTile.previous = null;
                        overlayTile.name = $"{tileLocation}";
                        overlayTile.transform.SetAsFirstSibling();
                        Map.Add(tileKey, overlayTile);
                    }
                }
            }
        }
    }
    public void TriggerTurnFlowInput()
    {
        ActionsFlowManager.InputDone();
    }
    public void RemoveFighterFromCurrentOrder(Fighter removedFighter)
    {
        CurrentTurnOrder.Remove(removedFighter);
        CalculateTurnOrderOnFighterChanged();
    }
    public void CalculateTurnOrderOnFighterChanged()
    {
        CurrentTurnOrder.RemoveAll(fighter => fighter == null);
        _NextTurnOrder = CalculateTurnOrder();
    }
    public void ChangePlayerFighter(FighterData newFighter)
    {
        ActionsFlowManager.ChangeFighter(CurrentTurnFighter, newFighter, TeamsController.PlayerTeam);
    }
    public void ChangePlayerDeadFighters(List<FighterData> newAddedFighters)
    {
        _PlayerNewFighters = newAddedFighters;
        ActionsFlowManager.ChangeDiedFighters(_PlayerDeadFighters, _EnemyDeadFighters, _PlayerNewFighters, _EnemyNewFighters);
    }
    public void OpenChangeMenuOnPlayerFightersDied(List<Fighter> selectedFightersToChange)
    {
        UIManager.ChangeFighterController.OpenPopup(selectedFightersToChange, true);
    }
    public List<int> GetFightersNumInRange(Fighter selectedFighter)
    {
        int currentFighterNum = TeamsController.GetFighterInFieldNum(selectedFighter);

        List<int> fightersInRangeNum = new();
        fightersInRangeNum.Add(currentFighterNum);
        if (currentFighterNum - 1 >= 0)
        {
            fightersInRangeNum.Add(currentFighterNum - 1);
        }
        if (currentFighterNum + 1 <= 2)
        {
            fightersInRangeNum.Add(currentFighterNum + 1);
        }

        return fightersInRangeNum;
    }
}
