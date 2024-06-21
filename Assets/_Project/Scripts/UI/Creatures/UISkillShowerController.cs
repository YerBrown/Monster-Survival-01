using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillShowerController : MonoBehaviour
{
    public List<Image> SkillFrames = new();
    public List<Image> SkillIcons = new();

    public void ShowCreatureSkills(CreatureSO creatureInfo)
    {
        for (int i = 0; i < SkillFrames.Count; i++)
        {
            if (i < creatureInfo.c_Skills.Count)
            {
                (Color, Color, Sprite) skillInfo = MainWikiManager.Instance.GetSkillInfo(creatureInfo.c_Skills[i]);
                if (skillInfo.Item3 != null)
                {
                    SkillFrames[i].color = skillInfo.Item1;
                    SkillIcons[i].color = skillInfo.Item2;
                    SkillIcons[i].sprite = skillInfo.Item3;
                    SkillFrames[i].gameObject.SetActive(true);
                }
                else
                {
                    SkillFrames[i].gameObject.SetActive(false);
                }
            }
            else
            {
                SkillFrames[i].gameObject.SetActive(false);
            }
        }
    }

}
