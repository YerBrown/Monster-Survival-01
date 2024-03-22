using System;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObjects/Item")]
public class ItemsSO : ScriptableObject
{
    public string i_Name;
    public string i_Description;
    public Sprite i_Sprite;
    public int i_StackMax;
    public ItemType i_ItemType;
    public CombatItemType i_CombatType;
    public TargetSelectType i_TargetType;
}
[Serializable]
public class ItemSlot
{
    [SerializeField]
    private ItemsSO _ItemInfo;
    [SerializeField]
    private int _Amount;
    public ItemSlot()
    {

    }

    public ItemsSO ItemInfo
    {
        get { return _ItemInfo; }
        set
        {
            if (_ItemInfo != value)
            {
                _ItemInfo = value;
            }
        }
    }

    public int Amount
    {
        get { return _Amount; }
        set
        {
            if (_Amount != value)
            {
                _Amount = value;
            }
        }
    }
    public ItemSlot(ItemsSO itemInfo, int amount)
    {
        ItemInfo = itemInfo;
        Amount = amount;
    }
}