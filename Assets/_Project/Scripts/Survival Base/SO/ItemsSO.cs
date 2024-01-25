using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObjects/Item")]
public class ItemsSO : ScriptableObject
{
    public string i_Name;
    public string i_Description;
    public Sprite i_Sprite;
    public int i_StackMax;
    public float i_Weight;
}
[Serializable]
public class ItemSlot
{
    public ItemsSO ItemInfo;
    public int Amount;

    public ItemSlot(ItemsSO itemInfo, int amount)
    {
        ItemInfo = itemInfo;
        Amount = amount;
    }
}