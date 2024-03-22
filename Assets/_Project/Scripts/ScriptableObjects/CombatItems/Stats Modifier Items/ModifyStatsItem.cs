using UnityEngine;
[CreateAssetMenu(fileName = "CombatItemSO_Stat_Modifier", menuName = "ScriptableObjects/Item/Combat Item/Combat Item Stat Modifier")]
public class ModifyStatsItem : CombatItemSO
{
    public BasicStats ModifierStats = new();
    public int TurnsDuration = 1;
    public override void Use(Fighter targetFighter)
    {
        targetFighter.AddStatModifier(ModifierStats, TurnsDuration);
    }
    public override void Use(FighterData targetFighter)
    {
        base.Use(targetFighter);
    }
}
