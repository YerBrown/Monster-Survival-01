using DG.Tweening;
using GeneralValues;
using System;
using System.Collections.Generic;
using UnityEngine;
public class Fighter : MonoBehaviour
{
    [Header("Fighter Info")]
    public string ID; // Fighter identification.
    public string TypeID; // Fighter identification.
    public string Nickname;
    public ElementType Element;
    public Sprite AvatarSprite;
    public BasicStats BaseStats = new();
    public BasicStats CurrentStats = new();
    public int HealthPoints;
    public int EnergyPoints;
    [SerializeField] private bool _IsInDefenseMode = false;
    [Header("Fighter Combat Status")]
    public List<(BasicStats, int)> StatsModifiers = new();
    public BasicStats StatusProblemStatsModifier;
    public StatusProblemType CurrentStatusProblem = StatusProblemType.NONE;
    public int CurrentStatusProblemActiveTurns = 0;
    public int CurrentFriendshipPoints = 0;
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
        TypeID = data.TypeID;
        Nickname = data.Nickname;
        CreatureSO fighterInfo = data.GetCreatureInfo();
        if (fighterInfo != null)
        {
            Element = fighterInfo.c_Element;
            AvatarSprite = fighterInfo.c_AvatarSprite;
            AnimationController.Anim.runtimeAnimatorController = fighterInfo.c_Animator;
        }
        else
        {
            AvatarSprite = null;
            AnimationController.Anim.runtimeAnimatorController = null;
        }

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
    public int ReceiveDamage(int damagePoints, ElementType attackElement)
    {
        Effectiveness damageEffectiveness = StaticCombatGeneralValues.GetDamageMultiplier(attackElement, Element);
        float multiplier = 1;
        switch (damageEffectiveness)
        {
            case Effectiveness.NORMAL:
                break;
            case Effectiveness.VERY_EFFECTIVE:
                multiplier += 0.5f;
                break;
            case Effectiveness.LESS_EFFECTIVE:
                multiplier -= 0.5f;
                break;
        }
        float realDamage = damagePoints * multiplier;
        damagePoints = (int)Math.Round(realDamage);
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
        UIController.HealthPointsChanged(-damagePoints, damageEffectiveness);
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
    public void Heal(int healedPoints, ElementType healType)
    {
        Effectiveness healEffectiveness = StaticCombatGeneralValues.GetHealMultiplier(healType, Element);
        float multiplier = 1;
        switch (healEffectiveness)
        {
            case Effectiveness.NORMAL:
                break;
            case Effectiveness.VERY_EFFECTIVE:
                multiplier += 0.5f;
                break;
            case Effectiveness.LESS_EFFECTIVE:
                multiplier -= 0.5f;
                break;
        }
        float realHeal = healedPoints * multiplier;
        healedPoints = (int)Math.Round(realHeal);
        // Limit heal to missing health points
        if (healedPoints + HealthPoints > CurrentStats.MaxHealthPoints)
        {
            healedPoints = CurrentStats.MaxHealthPoints - HealthPoints;
        }
        HealthPoints += healedPoints;
        // Update the data of this fighter
        CombatManager.Instance.TeamsController.UpdateFighterData(this);

        UIController.UpdateGeneralUI();
        UIController.HealthPointsChanged(healedPoints, healEffectiveness);
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
        // Update the data of this fighter
        CombatManager.Instance.TeamsController.UpdateFighterData(this);
    }
    // Add friendship points to this fighter.
    public void AddFriendshipPoints(CombatItemSO foodItem, int addedFriendshipPoints)
    {
        if (CombatManager.Instance.TeamsController.IsPlayerTeamFighter(this))
        {
            return;
        }
        CreatureSO creatureInfo = GetCreatureInfo();
        if (creatureInfo != null)
        {
            if (creatureInfo.c_FavouriteFood.Contains(foodItem))
            {
                addedFriendshipPoints *= 2;
                CurrentFriendshipPoints += addedFriendshipPoints;
                if (CurrentFriendshipPoints > creatureInfo.c_MaxFrindshipPoints)
                {
                    CurrentFriendshipPoints = creatureInfo.c_MaxFrindshipPoints;
                }
                UIController.FrienshipPointsChanged(true);
            }
            else
            {
                CurrentFriendshipPoints += addedFriendshipPoints;
                if (CurrentFriendshipPoints > creatureInfo.c_MaxFrindshipPoints)
                {
                    CurrentFriendshipPoints = creatureInfo.c_MaxFrindshipPoints;
                }
                UIController.FrienshipPointsChanged(false);
            }
        }
    }
    // Enable the defense mode in fighter
    public void SetDefenseMode()
    {
        _IsInDefenseMode = true;

        UIController.EnableDefenseIcon(true);
        AnimationController.PlayDefenseMode();
        // Update the data of this fighter
        CombatManager.Instance.TeamsController.UpdateFighterData(this);
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
        ManageStatusOnFinishTurn();
        CalculateCurrentStats();
    }
    // Manage the problem status when the fighter turn finished
    private void ManageStatusOnFinishTurn()
    {
        if (CurrentStatusProblem == StatusProblemType.BURNED)
        {
            ReceiveDamage(CurrentStats.MaxHealthPoints / 10, ElementType.NO_TYPE);
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
            ReceiveDamage(CurrentStats.MaxHealthPoints / 10, ElementType.NO_TYPE);
            CurrentStatusProblemActiveTurns++;
        }
    }
    // Adds a new problem status if there isn't currently affected by another one.
    public void AddStatusProblem(StatusProblemType newStatusProblem)
    {
        if (CurrentStatusProblem == StatusProblemType.NONE)
        {
            CurrentStatusProblemActiveTurns = 0;
            CurrentStatusProblem = newStatusProblem;
            UIController.UpdateGeneralUI();
            if (newStatusProblem == StatusProblemType.FROZEN)
            {
                StatusProblemStatsModifier = new BasicStats()
                {
                    Speed = -Mathf.RoundToInt(BaseStats.Speed / 2f)
                };
                CalculateCurrentStats();
            }
        }
    }
    // Remove the current problem status
    public void ClearStateProblem()
    {
        CurrentStatusProblem = StatusProblemType.NONE;
        CurrentStatusProblemActiveTurns = 0;
        StatusProblemStatsModifier = new();
        UIController.UpdateGeneralUI();
    }
    // Check if the player can do any action if is paralized
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
        int prevSpeed = CurrentStats.Speed;
        newStats.AddStats(BaseStats);
        newStats.AddStats(StatusProblemStatsModifier);
        foreach (var modifier in StatsModifiers)
        {
            newStats.AddStats(modifier.Item1);
        }

        CurrentStats = newStats;
        if (HealthPoints > CurrentStats.MaxHealthPoints)
        {
            HealthPoints = CurrentStats.MaxHealthPoints;
        }
        UIController.UpdateGeneralUI();
        if (prevSpeed != CurrentStats.Speed && prevSpeed != 0)
        {
            CombatManager.Instance.CalculateTurnOrderOfCurrentTurn();
        }
    }
    // Return the resolution of tried to catch this fighter
    public bool TryCatchFighter(int captureIntensity)
    {
        int randomNumber = UnityEngine.Random.Range(0, 100);
        Debug.Log($"Random number: {randomNumber}");
        int capturePercetage = GetCaptureRate(captureIntensity);
        Debug.Log($"Capture percentage: {capturePercetage}");
        if (randomNumber < capturePercetage)
        {
            HealthPoints = 0;
            CombatManager.Instance.TeamsController.UpdateFighterData(this);
            return true;
        }
        else
        {
            return false;
        }
    }
    //Returns capture rate
    public int GetCaptureRate(int captureIntensity)
    {
        CreatureSO creatureInfo = GetCreatureInfo();
        int intensityRate = StaticCombatGeneralValues.Capture_CaptureIntensity_CaptureRate[captureIntensity];
        float creatureCaptureRate = creatureInfo.c_CaptureRate;

        float healthPointsModifier = ((float)(CurrentStats.MaxHealthPoints - HealthPoints) / CurrentStats.MaxHealthPoints) * (creatureCaptureRate / 100 * StaticCombatGeneralValues.Capture_Modifier_HealthPoints);
        float frienshipPointsModifier = ((float)CurrentFriendshipPoints / creatureInfo.c_MaxFrindshipPoints) * (creatureCaptureRate / 100 * StaticCombatGeneralValues.Capture_Modifier_FriendshipPoints);
        float statusPoblemModifier = 0;
        // Check status problems
        if (CurrentStatusProblem == StatusProblemType.PARALIZED)
        {
            statusPoblemModifier = (creatureCaptureRate / 100 * StaticCombatGeneralValues.Capture_Modifier_Paralized);
        }
        else if (CurrentStatusProblem == StatusProblemType.FROZEN)
        {
            statusPoblemModifier = (creatureCaptureRate / 100 * StaticCombatGeneralValues.Capture_Modifier_Frozen);
        }
        float captureWithModifierPosibility = (creatureCaptureRate + healthPointsModifier + frienshipPointsModifier + statusPoblemModifier) / intensityRate * 100;
        if (captureWithModifierPosibility > 100)
        {
            captureWithModifierPosibility = 100;
        }
        return UnityEngine.Mathf.RoundToInt(captureWithModifierPosibility);
    }
    // Get creature info of this fighter.
    public CreatureSO GetCreatureInfo()
    {
        if (MainWikiManager.Instance.GetCreatureInfo(TypeID, out CreatureSO fighterInfo))
        {
            return fighterInfo;
        }
        else
        {
            return null;
        }
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

    public int Lvl;
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
    public void Heal(int healPoints, ElementType healElement)
    {
        CreatureSO creatureInfo = GetCreatureInfo();
        if (creatureInfo != null)
        {
            Effectiveness healEffectiveness = StaticCombatGeneralValues.GetHealMultiplier(healElement, creatureInfo.c_Element);
            float multiplier = 1;
            switch (healEffectiveness)
            {
                case Effectiveness.NORMAL:
                    break;
                case Effectiveness.VERY_EFFECTIVE:
                    multiplier += 0.5f;
                    break;
                case Effectiveness.LESS_EFFECTIVE:
                    multiplier -= 0.5f;
                    break;
            }
            float realHeal = healPoints * multiplier;
            healPoints = (int)Math.Round(realHeal);
            HealthPoints += healPoints;
            if (HealthPoints > MaxHealthPoints)
            {
                HealthPoints = MaxHealthPoints;
            }

        }

    }
    public CreatureSO GetCreatureInfo()
    {
        if (MainWikiManager.Instance.GetCreatureInfo(TypeID, out CreatureSO fighterInfo))
        {
            return fighterInfo;
        }
        else
        {
            return null;
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
        MaxHealthPoints = SumWithMinValue(MaxHealthPoints, addedStats.MaxHealthPoints);
        MaxEnergyPoints = SumWithMinValue(MaxEnergyPoints, addedStats.MaxEnergyPoints);
        HitPower = SumWithMinValue(HitPower, addedStats.HitPower);
        RangePower = SumWithMinValue(RangePower, addedStats.RangePower);
        Defense = SumWithMinValue(Defense, addedStats.Defense);
        Speed = SumWithMinValue(Speed, addedStats.Speed);
    }
    public void RemoveStats(BasicStats removedStats)
    {
        MaxHealthPoints = SubstractWithMinValue(MaxHealthPoints, removedStats.MaxHealthPoints);
        MaxEnergyPoints = SubstractWithMinValue(MaxEnergyPoints, removedStats.MaxEnergyPoints);
        HitPower = SubstractWithMinValue(HitPower, removedStats.HitPower);
        RangePower = SubstractWithMinValue(RangePower, removedStats.RangePower);
        Defense = SubstractWithMinValue(Defense, removedStats.Defense);
        Speed = SubstractWithMinValue(Speed, removedStats.Speed);
    }

    private int SumWithMinValue(int a, int b)
    {
        int result = a + b;
        return (result <= 0) ? 1 : result;
    }
    private int SubstractWithMinValue(int a, int b)
    {
        int result = a - b;
        return (result <= 0) ? 1 : result;
    }
}
