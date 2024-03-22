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
    [SerializeField] public List<Fighter> CurrentTurnOrder = new(); // Current turn fighters order.
    [SerializeField] private List<Fighter> _NextTurnOrder = new(); // Next turn fighters order.
    public int CurrentTurn = 1;
    [Header("Fighters")]
    public Fighter CurrentTurnFighter;
    public Fighter TargetActionFighter;
    public int TargetIndex;
    [Header("Map Tiles")]
    [SerializeField] public Tilemap BattleFieldTileMap;
    [SerializeField] private OverlayTile OverlayTilePrefab; // Overlay tile prefab ref.
    [SerializeField] private GameObject OverlayContainer; // Overlay tiles parent for all overlay tiles spawned infield.
    private List<OverlayTile> AllTiles = new();
    public Dictionary<Vector2Int, OverlayTile> Map;
    [Header("Change Fighter Values")]
    private List<Fighter> _PlayerDeadFighters; // This turn fighters dead from players team.
    private List<Fighter> _EnemyDeadFighters; // This turn fighters dead from enemy team.
    private List<FighterData> _PlayerNewFighters; // This turn added fighters to replace dead fighters from players team.
    private List<FighterData> _EnemyNewFighters; // This turn added fighters to replace dead fighters from enemy team.
    [Header("Controllers / Managers")]
    public CombatTeamsController TeamsController; // Teams management controller.
    public CombatActionsFlowManager ActionsFlowManager;  // Actions in combat manager.
    public UICombatManager UIManager; // General combat UI controller. 
    public EnemyActionsManager EnemyManager; // Decides the actions of the enemy team.
    [Header("Actions")]
    public VoidEventChannelSO FinishCurrentFighterAction; // Event when the currect fighter action finished and the next fighter turn is selected.
    private Action _OnFinishedSelectAction; // Action played when the action is selected by any team.
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
    // Set up battlefield.
    public IEnumerator StartCombat()
    {
        AddAllOverlayTiles();
        // Add player team info
        if (PlayerManager.Instance != null)
        {
            TeamsController.PlayerTeam.Fighters = PlayerManager.Instance.Team;
        }
        yield return new WaitForSeconds(.2f);
        TeamsController.SpawnStartingFighters();
        yield return new WaitForSeconds(.2f);
        CurrentTurnOrder = CalculateTurnOrder();
        _NextTurnOrder = CalculateTurnOrder();
        SelectNextFighter();
    }
    // Adds all the tiles to the current battle field.
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
    // Calculate the turn order of the fighters in field by their speed.
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
        var groupedBySpeed = allFighterInField.GroupBy(fighter => fighter.CurrentStats.Speed);
        groupedBySpeed = groupedBySpeed.OrderByDescending(fighter => fighter.Key);
        foreach (var groupSpeed in groupedBySpeed)
        {
            if (groupSpeed.Count() > 1)
            {
                // If there are more than one fighter with the same speed, calculate the initiative of each fighter and reorder the group
                var fightersWithRandomNumbers = groupSpeed.Select(fighter => new
                {
                    Fighter = fighter,
                    Initiative = fighter.CurrentStats.Speed + new System.Random().NextDouble() // Generate the intuitive number randomly 
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
    // Returns the current turn fighters order and the next turn order .
    public List<Fighter> GetCurrentAndNextTurn()
    {
        List<Fighter> currentNextTurns = new List<Fighter>();
        currentNextTurns.AddRange(CurrentTurnOrder);
        currentNextTurns.AddRange(_NextTurnOrder);
        return currentNextTurns;
    }
    // Remove the fighter from the current turn order.
    public void RemoveFighterFromCurrentOrder(Fighter removedFighter)
    {
        CurrentTurnOrder.Remove(removedFighter);
        CalculateTurnOrderOnFighterChanged();
    }
    // Calculate the order for the next turn when some fighters have been changed.
    public void CalculateTurnOrderOnFighterChanged()
    {
        bool firstNull = CurrentTurnOrder[0] == null;
        CurrentTurnOrder.RemoveAll(fighter => fighter == null);
        if (firstNull)
        {
            CurrentTurnOrder.Insert(0, null);
        }
        _NextTurnOrder = CalculateTurnOrder();
    }
    // Set up the current turn fighter to the next one in order list.
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
                fighterInField.UIController.NextText.gameObject.SetActive(false);
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
                fighterInField.UIController.NextText.gameObject.SetActive(false);
            }
        }
        GetCurrentAndNextTurn()[1].UIController.NextText.gameObject.SetActive(true);
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
    // Triggers the finish of the action done by the current turn fighter.
    public void FinishFighterMove()
    {
        if (TeamsController.IsSomeOneDeadInField())
        {
            // TODO: Handle death
            (List<Fighter>, List<Fighter>) deadFighters = TeamsController.GetDiedFightersInField();
            _PlayerDeadFighters = deadFighters.Item1;
            _EnemyDeadFighters = deadFighters.Item2;
            _EnemyNewFighters = EnemyManager.GetNewFighters(_EnemyDeadFighters);

            // Check if someone died from player team, to open the change menu 
            if (TeamsController.IsTeamWithMoreFightersAliveThanInTheField(TeamsController.PlayerTeam) && _PlayerDeadFighters.Count > 0)
            {
                UIManager.ChangeFighterController.OpenPopup(_PlayerDeadFighters, true);
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
    // When all the actions from current turn are done, this triggers the pass to the next turn.
    private void FinishTurn()
    {
        // TODO: Flow when the turn finished for fighters, apply damage from states, remove boost, check if some one died,  etc.
        //foreach (var fighter in _NextTurnOrder)
        //{
        //    fighter.NextTurn();
        //}
        CurrentTurn++;
        CurrentTurnOrder = new List<Fighter>(_NextTurnOrder);
        _NextTurnOrder = CalculateTurnOrder();
        SelectNextFighter();
    }
    // Set up the action selected.
    public void SetSelectedAction(Action newAction)
    {
        _OnFinishedSelectAction = newAction;
    }
    // Set up the action target fighter.
    public void SelectTargetFighter(Fighter fighter, int index)
    {
        TargetActionFighter = fighter;
        TargetIndex = index;
        _OnFinishedSelectAction.Invoke();
        UIManager.EnableAction(false);
    }
    // Set the move target of the current turn fighter.
    public void SetFighterMoveTarget(bool range)
    {
        if (CurrentTurnFighter != null)
        {
            Vector2 targetOffsetPosition = TeamsController.GetTargetAttackPosition(TargetActionFighter, range);
            CurrentTurnFighter.SetTargetPosition(GetTile(targetOffsetPosition));
        }
    }
    // Calls the change a player fighter flow.
    public void ChangePlayerFighter(FighterData newFighter)
    {
        SwipeFightersInPlayerData(CurrentTurnFighter, newFighter);
        ActionsFlowManager.ChangeFighter(CurrentTurnFighter, newFighter, TeamsController.PlayerTeam);
    }
    // Call the change fighters when someone died in this turn.
    public void ChangePlayerDeadFighters(List<FighterData> newAddedFighters)
    {
        for (int i = 0; i < newAddedFighters.Count; i++)
        {
            if (newAddedFighters[i] != null && !string.IsNullOrEmpty(newAddedFighters[i].ID))
            {
                SwipeFightersInPlayerData(TeamsController.PlayerTeam.FightersInField[i], newAddedFighters[i]);
            }
        }
        _PlayerNewFighters = newAddedFighters;
        ActionsFlowManager.ChangeDiedFighters(_PlayerDeadFighters, _EnemyDeadFighters, _PlayerNewFighters, _EnemyNewFighters);
    }
    // Swipe the position in the list of player fighters data.
    public void SwipeFightersInPlayerData(Fighter activeFighter, FighterData addedFighter)
    {
        int newFighterIndex = TeamsController.PlayerTeam.GetFighterDataIndex(activeFighter.ID);
        int currentFighterIndex = TeamsController.PlayerTeam.GetFighterDataIndex(addedFighter.ID);
        TeamsController.PlayerTeam.SwipeFightersOrder(currentFighterIndex, newFighterIndex);
    }
    public void SwipeFightersInPlayerData(Fighter activeFighter, Fighter addedFighter)
    {
        int newFighterIndex = TeamsController.PlayerTeam.GetFighterDataIndex(activeFighter.ID);
        int currentFighterIndex = TeamsController.PlayerTeam.GetFighterDataIndex(addedFighter.ID);
        TeamsController.PlayerTeam.SwipeFightersOrder(currentFighterIndex, newFighterIndex);
    }
    public void ThrowItem(CombatItemSO item)
    {
        ActionsFlowManager.UseItemInFieldFighter(item);
    }
    // Return the tile of the position given.
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
    // If the input is needed in the fighter action this triggers it as done.
    public void TriggerTurnFlowInput()
    {
        ActionsFlowManager.InputDone();
    }
}
