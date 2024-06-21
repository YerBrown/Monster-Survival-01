using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class BiomeTeamPanelControllerUI : MonoBehaviour
{
    public UIBiomesMenuController MenuController;
    public TeamCreatureSlotControllerUI PlayerSlot;
    public List<TeamCreatureSlotControllerUI> CreatureTeamSlots = new();

    protected virtual void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        Creature player = new Creature(PlayerManager.Instance.P_Fighter);
        (ElementType, ElementType) playerTypes = PlayerManager.Instance.GetPlayerElements();
        if (playerTypes.Item1 != playerTypes.Item2)
        {
            PlayerSlot.UpdateSlot(player, playerTypes.Item1, playerTypes.Item2);
        }
        else
        {
            PlayerSlot.UpdateSlot(player, playerTypes.Item1);
        }
        List<FighterData> FightersTeam = PlayerManager.Instance.Team;
        for (int i = 0; i < PlayerManager.Instance.Team.Count - 1; i++)
        {
            if (!string.IsNullOrEmpty(FightersTeam[i + 1].ID))
            {
                Creature teamCreature = new Creature(FightersTeam[i + 1]);
                CreatureTeamSlots[i].UpdateSlot(teamCreature, teamCreature.CreatureInfo.c_Element);
            }
            else
            {
                CreatureTeamSlots[i].EnableSlot(false);
            }
        }
    }
    public void EnableFrameByIndex(int index)
    {
        PlayerSlot.EnableFrame(index == 0);
        for (int i = 0; i < CreatureTeamSlots.Count; i++)
        {
            CreatureTeamSlots[i].EnableFrame(i + 1 == index);
        }
    }
    public void DisableAllFrames()
    {
        PlayerSlot.EnableFrame(false);
        foreach (var slot in CreatureTeamSlots)
        {
            slot.EnableFrame(false);
        }
    }        
}
