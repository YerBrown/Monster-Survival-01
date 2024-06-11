using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
[CreateAssetMenu(fileName = "EquipmentSO", menuName = "ScriptableObjects/Item/Equipment")]
public class EquipableItemSO : ItemsSO
{
    public EquipType EquipmentType;
    public ElementType EquipmentElement;
    public BasicStats AddedStats;
    public List<Skills> GainedSkills;
}
