using UnityEngine;
[CreateAssetMenu(fileName = "CombatItemSO_Damage_State", menuName = "ScriptableObjects/Item/Combat Item/Combat Item Damage And State")]
public class DamageAndStateItem : CombatItemSO
{
    public int Damage = 5;
    public float StatusRate = 100f;
    public StatusProblemType StatusProblem;

    public override void Use(Fighter targetFighter)
    {
        targetFighter.ReceiveDamage(Damage);
        if (Random.Range(0, 100) < StatusRate)
        {
            targetFighter.AddStatusProblem(StatusProblem);
        }
    }
}
