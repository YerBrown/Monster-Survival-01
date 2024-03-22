using UnityEngine;
[CreateAssetMenu(fileName = "CombatItemSO_Damage", menuName = "ScriptableObjects/Item/Combat Item/Combat Item Damage")]
public class DamageItem : CombatItemSO
{
    public int DamagePower = 5;
    public override void Use(Fighter targetFighter)
    {
        targetFighter.ReceiveDamage(DamagePower);
        targetFighter.AnimationController.PlayAttack("Fisical Attack");
    }
    public override void Use(FighterData targetFighter)
    {
        base.Use(targetFighter);
    }
}
