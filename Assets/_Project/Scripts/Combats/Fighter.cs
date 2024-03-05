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

    public void ReceiveDamage(int damagePoints)
    {
        HealthPoints -= damagePoints;
        UIController.UpdateGeneralUI();
        if (HealthPoints == 0)
        {

        }

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
