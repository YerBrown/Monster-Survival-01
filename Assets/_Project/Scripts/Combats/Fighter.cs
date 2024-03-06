using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public string ID;
    public string Nickname;
    public Sprite AvatarSprite;
    public BasicStats Stats = new();
    public int HealthPoints;
    public int EnergyPoints;
    public bool IsInDefenseMode = false;
    public Animator Anim;
    public UIFighterController UIController;
    private void OnEnable()
    {
        if (UIController == null) { return; }
        UIController.CurrentFighter = this;
    }
    public void UpdateFighter(FighterData data)
    {
        if (data == null) { return; }
        ID = data.ID;
        Nickname = data.Nickname;

        Stats.MaxHealtPoints = data.MaxHealtPoints;
        Stats.MaxEnergyPoints = data.MaxEnergyPoints;
        Stats.HitPower = data.HitPower;
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
            Anim.SetTrigger("Disable Shield");
            UIController.EnableDefenseIcon(false);
        }
        if (damagePoints <= 0)
        {
            damagePoints = 1;
        }
        HealthPoints -= damagePoints;
        UIController.UpdateGeneralUI();
        CombatManager.Instance.UpdateFighterData(this);
        CombatManager.Instance.UIManager.UpdatePlayerFighterPanel(this);
        // TODO: Hit animation
        Anim.SetTrigger("GetHit");
        if (HealthPoints == 0)
        {
            // TODO: Die animation
            CombatManager.Instance.OnFighterDied(this);
        }
        return damagePoints;
    }

    public void SetDefenseMode()
    {
        IsInDefenseMode = true;
        // TODO: Defense mode animation
        Anim.SetTrigger("Enable Shield");
        UIController.EnableDefenseIcon(true);
    }
    public void Heal(int healedPoints)
    {
        if (healedPoints + HealthPoints > Stats.MaxHealtPoints)
        {
            healedPoints = Stats.MaxHealtPoints - HealthPoints;
        }
        HealthPoints += healedPoints;
        CombatManager.Instance.UpdateFighterData(this);
        // TODO: Heal animation
    }
}
[Serializable]
public class FighterData
{
    public string ID;
    public string TypeID;
    public string Nickname;

    public int MaxHealtPoints;
    public int MaxEnergyPoints;
    public int HitPower;
    public int RangePower;
    public int Defense;
    public int Speed;

    public int HealthPoints;
    public int EnergyPoints;
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
