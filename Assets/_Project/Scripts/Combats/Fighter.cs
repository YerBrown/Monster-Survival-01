using DG.Tweening;
using GeneralValues;
using System;
using System.Collections.Generic;
using UnityEngine;
public class Fighter : MonoBehaviour
{
    [Header("Fighter Info")]
    public string ID; // Fighter identification.
    public string Nickname;
    public Sprite AvatarSprite;
    public BasicStats BaseStats = new();
    public BasicStats CurrentStats = new();
    public int HealthPoints;
    public int EnergyPoints;
    [SerializeField] private bool _IsInDefenseMode = false;
    [Header("Fighter Combat Status")]
    public List<(BasicStats, int)> StatsModifiers = new();
    public StatusProblemType CurrentStatusProblem = StatusProblemType.NONE;
    public int CurrentStatusProblemActiveTurns = 0;
    [Header("Movement")]
    public Vector2Int ForwardDirection; // The forward direction, depending of the team side in the field.
    public OverlayTile FighterStartTile; // Start position tile, for knowing where to come back if fighter moved from there.
    private OverlayTile _ActiveTile; // Current tile behind the fighter.
    private List<OverlayTile> _Path = new List<OverlayTile>(); // Current path for the movement of the fighter.
    private PathFinder _PathFinder;
    private Action _OnPathFinishedAction; // Action to be invoked when the fighter reached the finish of the path.
    [Header("Controllers")]
    public UIFighterController UIController;
    public FighterAnimationController AnimationController;
    [Header("Other")]
    public GameObject ThrowItemObject;
    public GameObject CurrentTurnPointer; // Circle image that represents that the current action turn belongs to this fighter.
    private void Start()
    {
        // Initialize path finder.
        _PathFinder = new PathFinder();
    }
    private void Update()
    {
        // Check if path active
        if (_Path.Count > 0)
        {
            MoveAlongPath();
        }
    }
    // Update this fighter info with a fighter data.
    public void UpdateFighter(FighterData data, bool animDelay = false)
    {
        if (data == null) { return; }
        ID = data.ID;
        Nickname = data.Nickname;
        AvatarSprite = FightersInfoWiki.Instance.FightersDictionary[data.TypeID].c_AvatarSprite;
        AnimationController.Anim.runtimeAnimatorController = FightersInfoWiki.Instance.FightersDictionary[data.TypeID].c_Animator;
        BaseStats.MaxHealthPoints = data.MaxHealthPoints;
        BaseStats.MaxEnergyPoints = data.MaxEnergyPoints;
        BaseStats.HitPower = data.FisicalPower;
        BaseStats.RangePower = data.RangePower;
        BaseStats.Defense = data.Defense;
        BaseStats.Speed = data.Speed;
        CalculateCurrentStats();

        HealthPoints = data.HealthPoints;
        EnergyPoints = data.EnergyPoints;

        // Enable animator with delay.
        AnimationController.StartDelayAnim(false);

        if (UIController == null) { return; }
        UIController.UpdateGeneralUI(false);
    }
    // Trigger receive damage by damage points parameter .
    public int ReceiveDamage(int damagePoints)
    {
        // Check if the fighter is in defense mode to receive less damage.
        if (_IsInDefenseMode)
        {
            _IsInDefenseMode = false;
            damagePoints -= CurrentStats.Defense / GeneralValues.StaticCombatGeneralValues.Fighter_DefenseSplitter_InDefenseMode;
            AnimationController.PlayRemoveDefense();
            UIController.EnableDefenseIcon(false);
        }
        else
        {
            damagePoints -= CurrentStats.Defense / GeneralValues.StaticCombatGeneralValues.Fighter_DefenseSplitter_WithoutDefenseMode;
        }
        // Never do 0 damage.
        if (damagePoints <= 0)
        {
            damagePoints = 1;
        }
        // Limiting the damage to the fighter's remaining health points.
        if (HealthPoints - damagePoints < 0)
        {
            damagePoints = HealthPoints;
        }
        HealthPoints -= damagePoints;
        // Add energy points based on the damage 
        AddEnergyPoints(damagePoints / GeneralValues.StaticCombatGeneralValues.Fighter_EnergySplitter_OnReceiveDamage, true);

        UIController.UpdateGeneralUI();
        UIController.HealthPointsChanged(-damagePoints);
        // Update the data of this fighter
        CombatManager.Instance.TeamsController.UpdateFighterData(this);
        //CombatManager.Instance.UIManager.UpdatePlayerFighterPanel(this);

        AnimationController.PlayReceiveHit();

        // Check if fighter died        
        return damagePoints;
    }
    // Check if the fighter died
    public void CheckIfDied()
    {
        if (HealthPoints == 0)
        {
            AnimationController.PlayDieAnimation();
        }
    }
    // Heal the fighter
    public void Heal(int healedPoints)
    {
        // Limit heal to missing health points
        if (healedPoints + HealthPoints > CurrentStats.MaxHealthPoints)
        {
            healedPoints = CurrentStats.MaxHealthPoints - HealthPoints;
        }
        HealthPoints += healedPoints;
        // Update the data of this fighter
        CombatManager.Instance.TeamsController.UpdateFighterData(this);

        UIController.UpdateGeneralUI();
        UIController.HealthPointsChanged(healedPoints);
        AnimationController.PlayReceiveHeal();
    }
    // Add energy points to this fighter.
    public void AddEnergyPoints(int energyAdded, bool updateUI)
    {
        EnergyPoints += energyAdded;
        // Limit the energy points to max energy .
        if (EnergyPoints > CurrentStats.MaxEnergyPoints)
        {
            EnergyPoints = CurrentStats.MaxEnergyPoints;
        }
        if (updateUI)
        {
            UIController.UpdateGeneralUI();
        }
    }
    // Enable the defense mode in fighter
    public void SetDefenseMode()
    {
        _IsInDefenseMode = true;

        UIController.EnableDefenseIcon(true);
        AnimationController.PlayDefenseMode();
        AddEnergyPoints(StaticCombatGeneralValues.Fighter_DefenseSplitter_InDefenseMode, true);
    }
    // Play fisical attack animation.
    public void FisicalAttack()
    {
        AnimationController.PlayFisicalAttack();
    }
    // Play range attack animation.
    public void RangeAttack()
    {
        AnimationController.PlayRangeAttack();
    }
    // Set the target location for this fighter.
    public void SetTargetPosition(OverlayTile targetTile, Action onPathFinishedAction = null)
    {
        // Creates the path to that tile.
        List<OverlayTile> newPath = _PathFinder.FindPath(_ActiveTile, targetTile);
        // Check if is posible to reach that target.
        if (newPath != null && newPath.Count > 0)
        {
            _Path = newPath;
            // Add action if is needed on path finished.
            if (onPathFinishedAction != null)
            {
                _OnPathFinishedAction = onPathFinishedAction;
            }
            else
            {
                _OnPathFinishedAction = null;
            }
        }
        else
        {
            Debug.Log("It is not possible to reach this location");
        }
    }
    // Calculate the movement with the current path.
    private void MoveAlongPath()
    {
        var step = GeneralValues.StaticCombatGeneralValues.Fighter_MoveSpeed * Time.deltaTime;
        var zIndex = _Path[0].transform.position.z;

        transform.position = Vector2.MoveTowards(transform.position, _Path[0].transform.position, step);
        transform.position = new Vector3(transform.position.x, transform.position.y, zIndex);
        // Check if fighter is near the next path point.
        if (Vector2.Distance(transform.position, _Path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(_Path[0]);
            _Path.RemoveAt(0);
            if (_Path.Count == 0)
            {
                TriggerPathFinished();
            }
        }
        else
        {
            // Set the animation values for movement direction.
            AnimationController.SetMovement((_Path[0].transform.position - transform.position).normalized);
        }
    }
    // Repositioning the fighter to the tile parameter
    public void PositionCharacterOnTile(OverlayTile tile)
    {
        transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + .0001f, tile.transform.position.z);
        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        _ActiveTile = tile;
        if (FighterStartTile == null)
        {
            ChangeStartTile(tile);
            FighterStartTile.isBlocked = true;
        }
    }
    // Trigger the finishing of the path
    public void TriggerPathFinished()
    {
        if (_OnPathFinishedAction != null)
        {
            _OnPathFinishedAction();
        }
        LookForward();
        FighterStartTile.isBlocked = true;
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.TriggerTurnFlowInput();
        }
    }
    // Returns the fighter to starting position.
    public void ReturnToFighterPos()
    {
        FighterStartTile.isBlocked = false;
        SetTargetPosition(FighterStartTile);
    }
    // Change the start tile of the fighter
    public void ChangeStartTile(OverlayTile tile)
    {
        FighterStartTile = tile;
    }
    // Set fighter look direction to match forward direction
    public void LookForward()
    {
        AnimationController.SetMovement(Vector2.zero);
        AnimationController.SetMovementIdle(ForwardDirection);
    }
    // Trigger the animation finish input
    public void FinishAnimationTrigger()
    {
        CombatManager.Instance.TriggerTurnFlowInput();
        Debug.Log("Input triggered");
    }
    // Throws an item to a target fighter
    public void ThrowItem(Fighter targetFighter, CombatItemSO combatItem)
    {
        if (targetFighter == this)
        {
            // TODO: If traget fighter is the same fighter, not throw just consume
            combatItem.Use(this);
            Invoke(nameof(FinishAnimationTrigger), .5f);
            FinishAnimationTrigger();
        }
        else
        {
            // TODO: Throw anim
            ThrowItemObject.transform.position = transform.position;
            ThrowItemObject.GetComponent<SpriteRenderer>().sprite = combatItem.i_Sprite;
            ThrowItemObject.SetActive(true);
            ThrowItemObject.transform.DOMove(targetFighter.transform.position, GeneralValues.StaticCombatGeneralValues.Fighter_ThrowAnimDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                combatItem.Use(targetFighter);
                FinishAnimationTrigger();
                ThrowItemObject.SetActive(false);
            });
        }
    }
    // Adds a new stat modifier
    public void AddStatModifier(BasicStats newStats, int turns)
    {
        StatsModifiers.Add((newStats, turns));
        CalculateCurrentStats();
    }
    // Manage the fighter things that happen when the turn finished
    public void NextTurn()
    {
        EnergyPoints += StaticCombatGeneralValues.Fighter_Energy_ObtainedOnTurn;
        for (int i = 0; i < StatsModifiers.Count; i++)
        {
            StatsModifiers[i] = (StatsModifiers[i].Item1, StatsModifiers[i].Item2 - 1);
        }
        StatsModifiers.RemoveAll(modifier => modifier.Item2 <= 0);
        CalculateCurrentStats();
        ManageStatusOnFinishTurn();
    }
    private void ManageStatusOnFinishTurn()
    {
        if (CurrentStatusProblem == StatusProblemType.BURNED)
        {
            ReceiveDamage(CurrentStats.MaxHealthPoints / 10);
            CurrentStatusProblemActiveTurns++;
            if (CurrentStatusProblemActiveTurns >= StaticCombatGeneralValues.Fighter_StatusProblem_BurnedMaxTurns || UnityEngine.Random.Range(0, 10) < 2)
            {
                ClearStateProblem();
            }
            else
            {
                UIController.UpdateGeneralUI();
            }
        }
        else if (CurrentStatusProblem == StatusProblemType.POISONED)
        {
            ReceiveDamage(CurrentStats.MaxHealthPoints / 10);
            CurrentStatusProblemActiveTurns++;
        }
    }
    public void AddStatusProblem(StatusProblemType newStatusProblem)
    {
        if (CurrentStatusProblem == StatusProblemType.NONE)
        {
            CurrentStatusProblemActiveTurns = 0;
            CurrentStatusProblem = newStatusProblem;
            UIController.UpdateGeneralUI();
        }
    }
    public void ClearStateProblem()
    {
        CurrentStatusProblem = StatusProblemType.NONE;
        CurrentStatusProblemActiveTurns = 0;
        UIController.UpdateGeneralUI();
    }
    public bool IsParalized()
    {
        if (CurrentStatusProblem == StatusProblemType.PARALIZED)
        {
            int chance = UnityEngine.Random.Range(0, 100);
            return chance < StaticCombatGeneralValues.Fighter_StatusProblem_ParalizedNotMoveRate;
        }
        return false;
    }
    // Calculate the current stats of the fighter adding the basic stats and the modifier stats
    private void CalculateCurrentStats()
    {
        BasicStats newStats = new();
        foreach (var modifier in StatsModifiers)
        {
            newStats.AddStats(modifier.Item1);
        }
        newStats.AddStats(BaseStats);
        CurrentStats = newStats;
        if (HealthPoints > CurrentStats.MaxHealthPoints)
        {
            HealthPoints = CurrentStats.MaxHealthPoints;
        }
        UIController.UpdateGeneralUI();
    }
}
[Serializable]
public class FighterData
{
    [Tooltip("The ID of this particular fighter")]
    public string ID;
    [Tooltip("The ID or name of type of fighter")]
    public string TypeID;
    public string Nickname;

    public int MaxHealthPoints;
    public int MaxEnergyPoints;
    public int FisicalPower;
    public int RangePower;
    public int Defense;
    public int Speed;

    public int HealthPoints;
    public int EnergyPoints;

    public bool IsDead()
    {
        return HealthPoints <= 0;
    }
    public void Heal(int healPoints)
    {
        HealthPoints += healPoints;
        if (HealthPoints > MaxHealthPoints)
        {
            HealthPoints = MaxHealthPoints;
        }
    }
}
[Serializable]
public class BasicStats
{
    public int MaxHealthPoints;
    public int MaxEnergyPoints;
    public int HitPower;
    public int RangePower;
    public int Defense;
    public int Speed;

    public void AddStats(BasicStats addedStats)
    {
        MaxHealthPoints += addedStats.MaxHealthPoints;
        MaxEnergyPoints += addedStats.MaxEnergyPoints;
        HitPower += addedStats.HitPower;
        RangePower += addedStats.RangePower;
        Defense += addedStats.Defense;
        Speed += addedStats.Speed;
    }
    public void RemoveStats(BasicStats removedStats)
    {
        MaxHealthPoints -= removedStats.MaxHealthPoints;
        MaxEnergyPoints -= removedStats.MaxEnergyPoints;
        HitPower -= removedStats.HitPower;
        RangePower -= removedStats.RangePower;
        Defense -= removedStats.Defense;
        Speed -= removedStats.Speed;
    }
}
