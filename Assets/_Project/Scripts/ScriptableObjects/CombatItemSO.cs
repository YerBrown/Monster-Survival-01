using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CombatItemSO", menuName = "ScriptableObjects/Item/Combat Item")]
public class CombatItemSO : ItemsSO
{
    public bool IsUsableInTeamFighters;
    public virtual void Use(Fighter targetFighter)
    {

    }
    public virtual void Use(FighterData targetFighter)
    {

    }
}
