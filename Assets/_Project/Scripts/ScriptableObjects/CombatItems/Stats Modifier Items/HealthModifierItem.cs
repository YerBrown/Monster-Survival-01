using UnityEngine;
[CreateAssetMenu(fileName = "CombatItemSO_Health_Stat_Modifier", menuName = "ScriptableObjects/Item/Combat Item/Combat Item Health Stat Modifier")]
public class HealthModifierItem : ModifyStatsItem
{
    public int HealAmount = 1;
    public override void Use(Fighter targetFighter)
    {
        base.Use(targetFighter);
        targetFighter.Heal(HealAmount, ElementType.NO_TYPE);
    }
}
