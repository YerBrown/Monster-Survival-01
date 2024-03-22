using UnityEngine;
[CreateAssetMenu(fileName = "CombatItemSO_Heal_Potion", menuName = "ScriptableObjects/Item/Combat Item/Combat Item Heal Potion")]
public class HealPotion : CombatItemSO
{
    public int HealPoints;
    public override void Use(Fighter targetFighter)
    {
        targetFighter.Heal(HealPoints);
    }
    public override void Use(FighterData targetFighter)
    {
        targetFighter.Heal(HealPoints);
    }
}
