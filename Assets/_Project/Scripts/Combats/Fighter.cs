using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Fighter : MonoBehaviour
{
    public string ID;
    public string Nickname;
    public Sprite AvatarSprite;
    public BasicStats Stats = new();
    public int HealthPoints;
    public int EnergyPoints;
    public bool IsInDefenseMode = false;
    public float MoveSpeed;
    public Vector2Int ForwardDirection;
    private PathFinder _PathFinder;
    private List<OverlayTile> _Path = new List<OverlayTile>();
    private OverlayTile _ActiveTile;
    private OverlayTile _FighterStartTile;
    private Action _OnPathFinishedAction;
    public GameObject CurrentTurnPointer;
    public UIFighterController UIController;
    public FighterAnimationController AnimationController;
    private void OnEnable()
    {
        if (UIController == null) { return; }
        UIController.CurrentFighter = this;
    }
    private void Start()
    {
        _PathFinder = new PathFinder();
    }
    public void UpdateFighter(FighterData data)
    {
        if (data == null) { return; }
        ID = data.ID;
        Nickname = data.Nickname;

        Stats.MaxHealtPoints = data.MaxHealtPoints;
        Stats.MaxEnergyPoints = data.MaxEnergyPoints;
        Stats.HitPower = data.FisicalPower;
        Stats.RangePower = data.RangePower;
        Stats.Defense = data.Defense;
        Stats.Speed = data.Speed;

        HealthPoints = data.HealthPoints;
        EnergyPoints = data.EnergyPoints;
        if (UIController == null) { return; }
        UIController.UpdateGeneralUI();
    }

    public int ReceiveDamage(int damagePoints)
    {
        if (IsInDefenseMode)
        {
            damagePoints -= Stats.Defense / 2;
            IsInDefenseMode = false;
            // TODO: disable defense mode animation
            AnimationController.PlayRemoveDefense();
            UIController.EnableDefenseIcon(false);
        }
        if (damagePoints <= 0)
        {
            damagePoints = 1;
        }
        if (HealthPoints - damagePoints < 0)
        {
            damagePoints = HealthPoints;
        }
        HealthPoints -= damagePoints;
        AddEnergyPoints(damagePoints / 2, false);
        UIController.UpdateGeneralUI();
        CombatManager.Instance.TeamsController.UpdateFighterData(this);
        CombatManager.Instance.UIManager.UpdatePlayerFighterPanel(this);
        // TODO: Hit animation
        AnimationController.PlayReceiveHit();
        if (HealthPoints == 0)
        {
            // TODO: Die animation
            //CombatManager.Instance.TeamsController.OnFighterDied(this);
        }
        return damagePoints;
    }

    public void SetDefenseMode()
    {
        IsInDefenseMode = true;
        // TODO: Defense mode animation
        AnimationController.PlayDefenseMode();
        UIController.EnableDefenseIcon(true);
    }
    public void Heal(int healedPoints)
    {
        if (healedPoints + HealthPoints > Stats.MaxHealtPoints)
        {
            healedPoints = Stats.MaxHealtPoints - HealthPoints;
        }
        HealthPoints += healedPoints;
        CombatManager.Instance.TeamsController.UpdateFighterData(this);
        // TODO: Heal animation
        AnimationController.PlayReceiveHeal();
    }
    public void AddEnergyPoints(int energyAdded, bool updateUI)
    {
        EnergyPoints += energyAdded;
        if (EnergyPoints > Stats.MaxEnergyPoints)
        {
            EnergyPoints = Stats.MaxEnergyPoints;
        }
        if (updateUI)
        {
            UIController.UpdateGeneralUI();
        }
    }
    private void Update()
    {
        if (_Path.Count > 0)
        {
            MoveAlongPath();
        }
    }
    public void SetTargetPosition(OverlayTile targetTile, Action onPathFinishedAction = null)
    {
        List<OverlayTile> newPath = _PathFinder.FindPath(_ActiveTile, targetTile);
        if (newPath != null && newPath.Count > 0)
        {
            _Path = newPath;
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
    public void ReturnToFighterPos()
    {
        _FighterStartTile.isBlocked = false;
        SetTargetPosition(_FighterStartTile);
    }
    private void MoveAlongPath()
    {
        var step = MoveSpeed * Time.deltaTime;

        var zIndex = _Path[0].transform.position.z;
        transform.position = Vector2.MoveTowards(transform.position, _Path[0].transform.position, step);

        transform.position = new Vector3(transform.position.x, transform.position.y, zIndex);

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
            AnimationController.SetMovement((_Path[0].transform.position - transform.position).normalized);
        }
    }
    public void PositionCharacterOnTile(OverlayTile tile)
    {
        transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + .0001f, tile.transform.position.z);
        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        _ActiveTile = tile;
        if (_FighterStartTile == null)
        {
            _FighterStartTile = tile;
            _FighterStartTile.isBlocked = true;
        }
    }
    public void LookForward()
    {
        AnimationController.SetMovement(Vector2.zero);
        AnimationController.SetMovementIdle(ForwardDirection);
    }
    public void TriggerPathFinished()
    {
        if (_OnPathFinishedAction != null)
        {
            _OnPathFinishedAction();
        }
        LookForward();
        _FighterStartTile.isBlocked = true;
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.TriggerTurnFlowInput();
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
