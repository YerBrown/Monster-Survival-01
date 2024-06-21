using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A Scriptable Object with the basic info of a biome
/// </summary>
[CreateAssetMenu(fileName = "CreatureSO", menuName = "ScriptableObjects/Biome")]
public class BiomeSO : ScriptableObject
{
    public string Name;
    public Sprite MenuBackgroundSprite;
    public List<CreatureSO> Creatures = new();
    public List<UpgradeCost> UpgradeCosts = new();
    public List<ItemSlot> BuyBiomePrice = new();
    [Serializable]
    public class UpgradeCost
    {
        public List<ItemSlot> Costs = new();
    }
}
