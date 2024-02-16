using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
/// <summary>
/// A Scriptable Object with the basic info of a Creature race
/// </summary>
[CreateAssetMenu(fileName = "CreatureSO", menuName = "ScriptableObjects/Creature")]
public class CreatureSO : ScriptableObject
{
    public string c_Name;
    public string c_Description;
    public Sprite c_Sprite;
    public AnimatorOverrideController c_Animator;
    public List<Skills> c_Skills = new();
}
[Serializable]
public class Creature
{
    public CreatureSO CreatureInfo;
    public string C_Nickname;
    public int C_HP;
    public int C_Damage;
    

}
