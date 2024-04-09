using UnityEngine;
[CreateAssetMenu(fileName = "CombatItemSO_Heal_Potion", menuName = "ScriptableObjects/Item/Combat Item/Combat Item Heal Potion")]
public class HealPotion : CombatItemSO
{
    public int HealPoints;
    public ElementType HealElement = ElementType.NO_TYPE;
    public override void Use(Fighter targetFighter)
    {
        targetFighter.Heal(HealPoints, HealElement);
    }
    public override void Use(FighterData targetFighter)
    {
        targetFighter.Heal(HealPoints, HealElement);
    }
}
