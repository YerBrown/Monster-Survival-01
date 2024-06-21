using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BiomePanelControllerUI : MonoBehaviour
{
    public UIBiomesMenuController MenuController;
    public List<BiomeSlotControllerUI> Slots = new();
    public BiomesManager.CreatureBiome CurrentBiome;
    public int CurrentBiomeIndex = 0;
    public UnityEvent<BiomeSlotControllerUI> OnBiomeSlotSelected;
    public List<Image> BiomesDots = new();
    public Image BiomeBackground;
    private void OnEnable()
    {
        OnBiomeSlotSelected.AddListener(SelectCreatureFromBiome);
        UpdateBiome();
    }
    private void OnDisable()
    {
        OnBiomeSlotSelected.RemoveListener(SelectCreatureFromBiome);
    }

    public void SelectCreatureFromBiome(BiomeSlotControllerUI biomeSlot)
    {
        MenuController.SelectCreatureFromBiome(biomeSlot.CreatureSlot);
    }

    public void UpdateBiome()
    {
        CurrentBiome = BiomesManager.Instance.CurrentBiomes[CurrentBiomeIndex];
        BiomeBackground.sprite = CurrentBiome.BiomeInfo.MenuBackgroundSprite;
        for (int i = 0; i < BiomesDots.Count; i++)
        {
            if (i < BiomesManager.Instance.CurrentBiomes.Count)
            {
                if (i == CurrentBiomeIndex)
                {
                    BiomesDots[i].color = Color.white;
                }
                else
                {
                    BiomesDots[i].color = Color.gray;
                }
                BiomesDots[i].gameObject.SetActive(true);
            }
            else
            {
                BiomesDots[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < Slots.Count; i++)
        {
            if (CurrentBiome.UnlockedSlots > i)
            {
                Slots[i].UnlockSlot();
            }
            else
            {
                Slots[i].LockSlot();
            }
            if (CurrentBiome.CreatureSlots.Count > i && CurrentBiome.CreatureSlots[i].CreatureInfo != null)
            {
                Slots[i].UpdateSlot(CurrentBiome.CreatureSlots[i]);
            }
            else
            {
                Slots[i].UpdateSlot(null);
            }
        }
        MenuController.CheckSendToBiomeFromTeam();
    }
    public void NextBiome()
    {
        int maxBiomes = BiomesManager.Instance.CurrentBiomes.Count;
        int lastIndex = CurrentBiomeIndex;
        if (CurrentBiomeIndex == maxBiomes - 1)
        {
            CurrentBiomeIndex = 0;
        }
        else
        {
            CurrentBiomeIndex++;
        }
        UpdateBiome();
        if (lastIndex != CurrentBiomeIndex)
        {
            MenuController.SelectCreatureFromBiome(null);
            MenuController.CheckUpgradeBiomePossibility();
        }
    }
    public void PreviousBiome()
    {
        int maxBiomes = BiomesManager.Instance.CurrentBiomes.Count;
        int lastIndex = CurrentBiomeIndex;
        if (CurrentBiomeIndex == 0)
        {
            CurrentBiomeIndex = maxBiomes - 1;
        }
        else
        {
            CurrentBiomeIndex--;
        }
        UpdateBiome();
        if (lastIndex != CurrentBiomeIndex)
        {
            MenuController.SelectCreatureFromBiome(null);
            MenuController.CheckUpgradeBiomePossibility();
        }
    }
    public void AddTeamCreatureToBiome(int index)
    {
        Creature teamCreature = new Creature(PlayerManager.Instance.Team[index]);
        BiomesManager.Instance.AddCreatureToBiome(CurrentBiome, teamCreature);
        UpdateBiome();
        PlayerManager.Instance.RemoveTeamCreature(index);
        MenuController.TeamManager.UpdateUI();
        foreach (var slot in Slots)
        {
            if (slot.CreatureSlot != null && slot.CreatureSlot.ID == teamCreature.ID)
            {
                SelectCreatureFromBiome(slot);
                return;
            }
        }
    }
    public void AddCreatureToTeam(Creature addedCreature)
    {
        int emptyIndex = PlayerManager.Instance.GetEmptyTeamSlotIndex();
        if (emptyIndex >= 0)
        {
            PlayerManager.Instance.AddTeamCreature(emptyIndex, new FighterData(addedCreature));
            MenuController.TeamManager.UpdateUI();
            BiomesManager.Instance.RemoveCreatureFromBiome(CurrentBiome, addedCreature);
            UpdateBiome();
            MenuController.SelectTeamCreature(emptyIndex);
        }
    }
    public void RemoveCreatureFromBiome(Creature creatureRemoved)
    {
        BiomesManager.Instance.RemoveCreatureFromBiome(CurrentBiome, creatureRemoved);
        UpdateBiome();
        DisableAllFrames();
        MenuController.SelectCreatureFromBiome(null);
    }
    public void EnableFrameByCreatureID(string id)
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].CreatureSlot != null)
            {
                Slots[i].EnableFrame(Slots[i].CreatureSlot.ID == id);
            }
            else
            {
                Slots[i].EnableFrame(false);
            }
        }
    }
    public void DisableAllFrames()
    {
        foreach (var slot in Slots)
        {
            slot.EnableFrame(false);
        }
    }
    public void UpgradeBiome()
    {
        BiomesManager.Instance.UpgradeBiome(CurrentBiome);
        UpdateBiome();
    }
}
