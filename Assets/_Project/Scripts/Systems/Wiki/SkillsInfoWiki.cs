using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsInfoWiki : MonoBehaviour
{
    public List<SkillInfo> AllSkils = new();
    public Dictionary<Skills, SkillInfo> SkillsDictionary = new();
    [Serializable]
    public class SkillInfo
    {
        public Skills Skill;
        public Color FrameColor;
        public Color IconColor;
        public Sprite IconSprite;
    }
    private void Awake()
    {
        foreach (var skill in AllSkils)
        {
            if (!SkillsDictionary.ContainsKey(skill.Skill))
            {
                SkillsDictionary.Add(skill.Skill, skill);
            }
        }
    }
    public (Color, Color, Sprite) GetSkillInfo(Skills skill)
    {
        if (SkillsDictionary.TryGetValue(skill, out SkillInfo skillInfo))
        {
            return (skillInfo.FrameColor, skillInfo.IconColor, skillInfo.IconSprite);
        }
        return (Color.white, Color.white, null);
    }
}
