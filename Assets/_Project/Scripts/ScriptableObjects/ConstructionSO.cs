using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ConstructionSO", menuName = "ScriptableObjects/Construction")]
public class ConstructionSO : ScriptableObject
{
    public string c_Name;
    public string c_Description;
    public List<Sprite> c_Sprites;
    //In seconds
    public List<int> c_UpgradeTimes = new List<int>();
}
