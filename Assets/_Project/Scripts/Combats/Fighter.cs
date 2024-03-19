using System;
using System.Collections.Generic;
using UnityEngine;
public class Fighter : MonoBehaviour
{
    [Header("Fighter Info")]
    public string ID; // Fighter identification.
    public string Nickname;
    public Sprite AvatarSprite;
    public BasicStats Stats = new();
    public int HealthPoints;
    public int EnergyPoints;
    [SerializeField] private bool _IsInDefenseMode = false;
    [Header("Movement")]
    public Vector2Int ForwardDirection; // The forward direction, depending of the team side in the field.
    public OverlayTile FighterStartTile; // Start position tile, for knowing where to come back if fighter moved from there.
    private OverlayTile _ActiveTile; // Current tile behind the fighter.
    [SerializeField] private float _MoveSpeed = GeneralValues.StaticCombatGeneralValues.Fighter_MoveSpeed;
    private List<OverlayTile> _Path = new List<OverlayTile>(); // Current path for the movement of the fighter.
    private PathFinder _PathFinder;
    private Action _OnPathFinishedAction; // Action to be invoked when the fighter reached the finish of the path.
    [Header("Controllers")]
    public UIFighterController UIController;
    public FighterAnimationController AnimationController;
    [Header("Other")]
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
        Stats.MaxHealtPoints = data.MaxHealtPoints;
        Stats.MaxEnergyPoints = data.MaxEnergyPoints;
        Stats.HitPower = data.FisicalPower;
        Stats.RangePower = data.RangePower;
        Stats.Defense = data.Defense;
        Stats.Speed = data.Speed;

        HealthPoints = data.HealthPoints;
        EnergyPoints = data.EnergyPoints;

        // Enable animator with delay.
        AnimationController.StartDelayAnim(animDelay);

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
            damagePoints -= Stats.Defense / GeneralValues.StaticCombatGeneralValues.Fighter_DefenseSplitter_InDefenseMode;
            AnimationController.PlayRemoveDefense();
            UIController.EnableDefenseIcon(false);
        }
        else
        {
            damagePoints -= Stats.Defense / GeneralValues.StaticCombatGeneralValues.Fighter_DefenseSplitter_WithoutDefenseMode;
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
        AddEnergyPoints(damagePoints / GeneralValues.StaticCombatGeneralValues.Fighter_EnergySplitter_OnReceiveDamage, false);

        UIController.UpdateGeneralUI();
        UIController.HealthPointsChanged(-damagePoints);
        // Update the data of this fighter
        CombatManager.Instance.TeamsController.UpdateFighterData(this);
        //CombatManager.Instance.UIManager.UpdatePlayerFighterPanel(this);

        AnimationController.PlayReceiveHit();

        // Check if fighter died
        if (HealthPoints == 0)
        {
            AnimationController.PlayDieAnimation();
        }
        return damagePoints;
    }
    // Heal the fighter
    public void Heal(int healedPoints)
    {
        // Limit heal to missing health points
        if (healedPoints + HealthPoints > Stats.MaxHealtPoints)
        {
            healedPoints = Stats.MaxHealtPoints - HealthPoints;
        }
        HealthPoints += healedPoints;
        // Update the data of this fighter
        CombatManager.Instance.TeamsController.UpdateFighterData(this);

        AnimationController.PlayReceiveHeal();
        UIController.HealthPointsChanged(healedPoints);
    }
    // Add energy points to this fighter.
    public void AddEnergyPoints(int energyAdded, bool updateUI)
    {
        EnergyPoints += energyAdded;
        // Limit the energy points to max energy .
        if (EnergyPoints > Stats.MaxEnergyPoints)
        {
            EnergyPoints = Stats.MaxEnergyPoints;
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
        var step = _MoveSpeed * Time.deltaTime;
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

    public int MaxHealtPoints;
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
}
[Serializable]
public class BasicStats
{
    public int MaxHealtPoints;
    public int MaxEnergyPoints;
    public int HitPower;
    public int RangePower;
    public int Defense;
    public int Speed;
}
