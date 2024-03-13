using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class CreatureElement : InteractiveElement
{
    public FighterData[] FullTeam = new FighterData[6];
    public SpriteRenderer[] SpriteRenderers = new SpriteRenderer[3];
    private void Start()
    {
        ChangeCursorColor("#FF0000");
        SetSprites();
    }
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        //Iniciar combate
    }
    private void SetSprites()
    {
        int CreaturesInWorld = 0;
        for (int i = 0; i < SpriteRenderers.Length; i++)
        {
            if (FightersInfoWiki.Instance != null && FightersInfoWiki.Instance.FightersDictionary.TryGetValue(FullTeam[i].TypeID, out CreatureSO creatureInfo))
            {
                if (creatureInfo != null && SpriteRenderers[i] != null)
                {
                    CreaturesInWorld++;

                    SpriteRenderers[i].sprite = creatureInfo.c_Sprite;
                    if (creatureInfo.c_Animator != null)
                    {
                        SpriteRenderers[i].GetComponent<Animator>().runtimeAnimatorController = creatureInfo.c_Animator;
                        SpriteRenderers[i].GetComponent<Animator>().enabled = true;
                    }
                    else
                    {
                        Debug.LogWarning("CreaturesTeam[i].CreatureInfo.c_Animator missing");
                    }
                    SpriteRenderers[i].enabled = true;
                }
                else
                {
                    SpriteRenderers[i].GetComponent<Animator>().enabled = false;

                    SpriteRenderers[i].enabled = false;
                }
            }

        }
    }
    public override void UpdateElement(ExpeditionData.ParentData data)
    {
        //Check if the instance is of the same type.
        if (data is ExpeditionData.CreatureData)
        {
            base.UpdateElement(data);
            FullTeam = ((ExpeditionData.CreatureData)data).Fighters;
            if (IsAllTeamDefeated())
            {
                // TODO: Disable element
                Debug.Log("Creatures defeated");
            }
        }
        else
        {
            Debug.LogWarning("Can not be updated from another type of element.");
        }
    }

    private bool IsAllTeamDefeated()
    {
        foreach (var fighter in FullTeam)
        {
            if (fighter.ID != "" && fighter.HealthPoints > 0)
            {
                return false;
            }
        }
        return true;
    }
}
