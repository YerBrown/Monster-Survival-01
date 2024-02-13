using System;
using UnityEngine;
[CreateAssetMenu(fileName = "ResourceSO", menuName = "ScriptableObjects/Resource")]

public class ResourceSO : ScriptableObject
{
    public string R_Name;
    public Sprite R_FullSprite;
    public Sprite R_EmptySprite;
    public Skills R_SkillNeeded;
}
