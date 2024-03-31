using System;
using System.Collections.Generic;
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
    public Sprite c_AvatarSprite;
    public AnimatorOverrideController c_Animator;
    public List<Skills> c_Skills = new();
    public GameObject CombatFighterPrefab;
}
[Serializable]
public class CreatureFighter : Fighter
{
    public CreatureSO CreatureInfo;
}
