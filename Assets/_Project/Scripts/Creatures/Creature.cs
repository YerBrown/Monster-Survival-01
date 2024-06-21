using MonsterSurvival.Data;
using System;
using UnityEngine;
[Serializable]
public class Creature
{
    public CreatureSO CreatureInfo;

    [Tooltip("The ID of this particular fighter")]
    public string ID;
    [Tooltip("The name of this creature")]
    public string Nickname;
    [Tooltip("The level of this creature")]
    public int Lvl;
    [Tooltip("The maximum health points of this creature")]
    public int MaxHealthPoints;
    [Tooltip("The maximum energy points of this creature")]
    public int MaxEnergyPoints;
    [Tooltip("The fisical attack power of this creature")]
    public int FisicalPower;
    [Tooltip("The range attack power of this creature")]
    public int RangePower;
    [Tooltip("The defense of this creature")]
    public int Defense;
    [Tooltip("The speed of this creature")]
    public int Speed;

    [Tooltip("The current health points of this creature")]
    public int HealthPoints;
    [Tooltip("The current energy points of this creature")]
    public int EnergyPoints;

    public Creature(FighterData fighterData)
    {
        if (MainWikiManager.Instance.GetCreatureInfo(fighterData.TypeID, out CreatureSO creatureInfo))
        {
            CreatureInfo = creatureInfo;
            ID = fighterData.ID;
            Nickname = fighterData.Nickname;
            Lvl = fighterData.Lvl;
            MaxHealthPoints = fighterData.MaxHealthPoints;
            MaxEnergyPoints = fighterData.MaxEnergyPoints;
            FisicalPower = fighterData.FisicalPower;
            RangePower = fighterData.RangePower;
            Defense = fighterData.Defense;
            Speed = fighterData.Speed;
            HealthPoints = fighterData.HealthPoints;
            EnergyPoints = fighterData.EnergyPoints;
        }
    }
    public Creature(CreatureData creatureData)
    {
        if (MainWikiManager.Instance.GetCreatureInfo(creatureData.Specie_ID, out CreatureSO creatureInfo))
        {
            CreatureInfo = creatureInfo;
        }
        ID = creatureData.Creature_ID;
        Nickname = creatureData.Nickname;
        Lvl = creatureData.Lvl;
        MaxHealthPoints = creatureData.MaxHealthPoints;
        MaxEnergyPoints = creatureData.MaxEnergyPoints;
        FisicalPower = creatureData.FisicalPower;
        RangePower = creatureData.RangePower;
        Defense = creatureData.Defense;
        Speed = creatureData.Speed;
        HealthPoints = creatureData.HealthPoints;
        EnergyPoints = creatureData.EnergyPoints;
    }
}
