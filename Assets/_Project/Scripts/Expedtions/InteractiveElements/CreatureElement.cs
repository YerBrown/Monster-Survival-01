using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        MapManager.Instance.GoToCombatScene(ID, FullTeam);
    }
    private void SetSprites()
    {
        int CreaturesInWorld = 0;
        List<FighterData> livingCreatures = FullTeam.Where(fighter => !fighter.IsDead()).Take(3).ToList();
        if (SpriteRenderers.Length > 1)
        {
            switch (livingCreatures.Count)
            {
                case 1:
                    SpriteRenderers[0].transform.localPosition = new Vector3(0, -0.01f, 0);
                    break;
                case 2:
                    SpriteRenderers[0].transform.localPosition = new Vector3(-0.25f, 0.125f, 0);
                    SpriteRenderers[1].transform.localPosition = new Vector3(0.25f, -0.125f, 0);
                    break;
                case 3:
                    SpriteRenderers[0].transform.localPosition = new Vector3(-0.25f, 0.25f, 0);
                    SpriteRenderers[1].transform.localPosition = new Vector3(0, -0.01f, 0);
                    SpriteRenderers[2].transform.localPosition = new Vector3(0.5f, -0.125f, 0);
                    break;
                default:
                    break;
            }
        }
        for (int i = 0; i < SpriteRenderers.Length; i++)
        {
            if (i < livingCreatures.Count)
            {
                if (FightersInfoWiki.Instance != null && FightersInfoWiki.Instance.FightersDictionary.TryGetValue(livingCreatures[i].TypeID, out CreatureSO creatureInfo))
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
            else
            {
                SpriteRenderers[i].GetComponent<Animator>().enabled = false;

                SpriteRenderers[i].enabled = false;
            }

        }
    }
    private void UpdateCreatures(FighterData[] team)
    {
        FullTeam = team;
        SetSprites();
        if (IsAllTeamDefeated())
        {
            // TODO: Disable element
            Debug.Log("Creatures defeated");
            EnableElement(false);
        }
        else
        {
            EnableElement(true);
        }
    }
    public override void UpdateElement(ExpeditionData.ParentData data)
    {
        //Check if the instance is of the same type.
        if (data is ExpeditionData.CreatureData)
        {
            base.UpdateElement(data);
            UpdateCreatures(((ExpeditionData.CreatureData)data).Fighters);            
        }
        else
        {
            Debug.LogWarning("Can not be updated from another type of element.");
        }
    }
    public void UpdateTeamAfterCombat(FighterData[] team)
    {
        UpdateCreatures(team);   
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
