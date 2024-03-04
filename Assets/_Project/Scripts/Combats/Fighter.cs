using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public string ID;
    public string Nickname;
    public BasicStats Stats = new();
    public int HealthPoints;
    public int EnergyPoints;

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

        HealthPoints = data.HealthPoints;
        EnergyPoints = data.EnergyPoints;
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
}
