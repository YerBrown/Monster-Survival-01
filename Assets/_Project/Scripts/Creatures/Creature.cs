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
}
