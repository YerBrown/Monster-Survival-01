using JetBrains.Annotations;
using UnityEngine;
[CreateAssetMenu(fileName = "CombatItemSO_Food", menuName = "ScriptableObjects/Item/Combat Item/Combat Item Food")]
public class FoodItem : CombatItemSO
{
    public int HealPoints;
    public int FriendshipPoints;
    public ElementType FoodElement;
    public override void Use(Fighter targetFighter)
    {
        targetFighter.AddFriendshipPoints(this, FriendshipPoints);
        targetFighter.Heal(HealPoints, FoodElement);
    }
    public override void Use(FighterData targetFighter)
    {
        targetFighter.Heal(HealPoints, FoodElement);
    }

}
