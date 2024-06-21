using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIBiomesMenuController : MonoBehaviour
{
    public GameObject MenuParent;
    private Creature _CurrentCreatureSelected;
    public BoolEventChannelSO OnOpenMenuPopup;
    [Header("UI")]
    public GameObject ConfirmReleasePopup;
    public GameObject ConfirmUpgradePopup;
    public CostPanelControllerUI UpgradeCostController;
    public Button UpgradeConfirmButon;
    [Header("Player Team Panel")]
    public BiomeTeamPanelControllerUI TeamManager;
    public TeamSwipeOrderControllerUI TeamSwipeOrderController;
    [Header("Biomes Panel")]
    public BiomePanelControllerUI BiomeUIController;
    public Button UpgradeBiomeButton;
    public Button AddBiomeButton;
    [Header("Selected Creature Panel")]
    public Image CreatureImage;
    public TMP_Text CreatureInfoText;
    public TMP_Text CreatureRaceText;
    public TMP_Text CreatureElementText;
    public TMP_Text CreatureStatsText;
    public UISkillShowerController SkillShower;
    public GameObject ActionButtonsParent;
    public Button AddToTeamButton;
    public Button SendToBiomeButton;
    public Button ReleaseButton;
    // Add creature to biome
    private bool IsTeamCreature = false;
    private int SelectedTeamIndex = 0;
    public void OpenMenu()
    {
        MenuParent.SetActive(true);
        OnOpenMenuPopup.RaiseEvent(true);
        _CurrentCreatureSelected = null;
        ShowSelectedCreatureInfo();
        TeamManager.DisableAllFrames();
        BiomeUIController.DisableAllFrames();
    }
    public void CloseMenu()
    {
        MenuParent.SetActive(false);
        OnOpenMenuPopup.RaiseEvent(false);
        CloseSwipeOrderMenu();
    }
    public void SelectCreatureFromBiome(Creature selectedCreature)
    {
        if (selectedCreature == null && IsTeamCreature)
        {
            return;
        }
        _CurrentCreatureSelected = selectedCreature;
        ShowSelectedCreatureInfo();
        IsTeamCreature = false;
        AddToTeamButton.gameObject.SetActive(true);
        SendToBiomeButton.gameObject.SetActive(false);
        ReleaseButton.gameObject.SetActive(true);
        CheckSendToTeamFromBiome();
        if (selectedCreature != null)
        {
            BiomeUIController.EnableFrameByCreatureID(selectedCreature.ID);
        }
        else
        {
            BiomeUIController.DisableAllFrames();
        }
        TeamManager.DisableAllFrames();
    }
    public void SelectPlayer()
    {
        _CurrentCreatureSelected = new Creature(PlayerManager.Instance.P_Fighter);
        ShowSelectedCreatureInfo();
        IsTeamCreature = true;
        AddToTeamButton.gameObject.SetActive(false);
        SendToBiomeButton.gameObject.SetActive(false);
        ReleaseButton.gameObject.SetActive(false);
        TeamManager.EnableFrameByIndex(0);
        BiomeUIController.DisableAllFrames();
    }
    public void SelectTeamCreature(int index)
    {
        if (!string.IsNullOrEmpty(PlayerManager.Instance.Team[index].ID))
        {
            _CurrentCreatureSelected = new Creature(PlayerManager.Instance.Team[index]);
            ShowSelectedCreatureInfo();
            IsTeamCreature = true;
            SelectedTeamIndex = index;
            CheckSendToBiomeFromTeam();
            AddToTeamButton.gameObject.SetActive(false);
            SendToBiomeButton.gameObject.SetActive(true);
            ReleaseButton.gameObject.SetActive(true);
            TeamManager.EnableFrameByIndex(index);
            BiomeUIController.DisableAllFrames();
        }
    }
    public void ShowSelectedCreatureInfo()
    {
        if (_CurrentCreatureSelected != null)
        {
            if (_CurrentCreatureSelected.CreatureInfo.c_Sprite != null)
            {
                CreatureImage.sprite = _CurrentCreatureSelected.CreatureInfo.c_Sprite;
            }
            else
            {
                CreatureImage.sprite = MainWikiManager.Instance.MissingSprite;
            }
            CreatureImage.enabled = true;
            CreatureInfoText.text = $"{_CurrentCreatureSelected.Nickname}\n{_CurrentCreatureSelected.CreatureInfo.c_Name}";
            CreatureStatsText.text = $"Health Point {_CurrentCreatureSelected.MaxHealthPoints} - Energy Points {_CurrentCreatureSelected.MaxEnergyPoints} \n Fisical Attack {_CurrentCreatureSelected.FisicalPower} - Range Attack {_CurrentCreatureSelected.RangePower}\n Defense {_CurrentCreatureSelected.Defense} - Speed {_CurrentCreatureSelected.Speed}";
            SkillShower.ShowCreatureSkills(_CurrentCreatureSelected.CreatureInfo);
            SkillShower.gameObject.SetActive(true);
            ActionButtonsParent.SetActive(true);
        }
        else
        {
            CreatureImage.enabled = false;
            CreatureInfoText.text = "";
            CreatureStatsText.text = "";
            SkillShower.gameObject.SetActive(false);
            ActionButtonsParent.SetActive(false);
        }
    }
    public void CheckSendToBiomeFromTeam()
    {
        if (IsTeamCreature && _CurrentCreatureSelected != null)
        {
            SendToBiomeButton.interactable = BiomeUIController.CurrentBiome.CheckCreatureAddPossibility(_CurrentCreatureSelected.CreatureInfo) && BiomeUIController.CurrentBiome.UnlockedSlots > BiomeUIController.CurrentBiome.CreatureSlots.Count;
        }
    }
    public void CheckSendToTeamFromBiome()
    {
        AddToTeamButton.interactable = (PlayerManager.Instance.GetEmptyTeamSlotIndex() >= 0);
    }
    public void SendToBiome()
    {
        BiomeUIController.AddTeamCreatureToBiome(SelectedTeamIndex);
    }
    public void AddToTeam()
    {
        BiomeUIController.AddCreatureToTeam(_CurrentCreatureSelected);
    }
    public void CheckUpgradeBiomePossibility()
    {
        UpgradeBiomeButton.interactable = BiomeUIController.CurrentBiome.Level < GeneralValues.StaticCombatGeneralValues.Biomes_Max_Level;
    }
    public void ReleaseSelectedCreature()
    {
        if (_CurrentCreatureSelected !=null)
        {
            if (IsTeamCreature)
            {
                PlayerManager.Instance.RemoveTeamCreature(SelectedTeamIndex);
                TeamManager.UpdateUI();
                TeamManager.DisableAllFrames();
                IsTeamCreature = false;
                SelectCreatureFromBiome(null);
            }
            else
            {
                BiomeUIController.RemoveCreatureFromBiome(_CurrentCreatureSelected);
            }
            CloseReleasePopup();
        }
    }
    public void OpenReleasePopup()
    {
        ConfirmReleasePopup.SetActive(true);
    }
    public void CloseReleasePopup()
    {
        ConfirmReleasePopup.SetActive(false);
    }
    public void OpenSwipeOrderMenu()
    {
        TeamSwipeOrderController.gameObject.SetActive(true);
        IsTeamCreature = false;
        SelectCreatureFromBiome(null);
        AddBiomeButton.gameObject.SetActive(false);
        UpgradeBiomeButton.gameObject.SetActive(false);
    }
    public void CloseSwipeOrderMenu()
    {
        TeamManager.UpdateUI();
        TeamSwipeOrderController.gameObject.SetActive(false);
        AddBiomeButton.gameObject.SetActive(true);
        UpgradeBiomeButton.gameObject.SetActive(true);
    }
    public void UpgradeBiome()
    {
        BiomeUIController.UpgradeBiome();
        CheckUpgradeBiomePossibility();
        CloseConfirmUpgradeBiomePopup();
    }
    public void OpenConfirmUpgradeBiomePopup()
    {
        UpgradeCostController.UpdateCost(BiomeUIController.CurrentBiome.BiomeInfo.UpgradeCosts[BiomeUIController.CurrentBiome.Level - 1].Costs);
        UpgradeConfirmButon.interactable = SurvivalBaseStorageManager.Instance.IsPosibleToConsume(BiomeUIController.CurrentBiome.BiomeInfo.UpgradeCosts[BiomeUIController.CurrentBiome.Level - 1].Costs);
        ConfirmUpgradePopup.SetActive(true);
    }
    public void CloseConfirmUpgradeBiomePopup()
    {
        ConfirmUpgradePopup.SetActive(false);

    }

}
