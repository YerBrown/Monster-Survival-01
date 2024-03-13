using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICombatManager : MonoBehaviour
{
    public List<PlayerFighterPanel> PlayerFighters = new();
    public UIActionsController ActionsController;
    public UIFighterChangeController ChangeFighterController;
    public UIWinAnimationController WinAnimationController;
    [Serializable]
    public class PlayerFighterPanel
    {
        public Fighter FighterAsigned;
        public TMP_Text NicknameText;
        public GameObject SelectedFrame;
        public Image FighterIcon;
        public Slider HealthPointsSlider;
        public TMP_Text HealthPointsText;
        public Slider EnergyPointsSlider;
        public TMP_Text EnergyPointsText;
    }
    public void AsignFighter(Fighter newFighter, int id)
    {
        if (newFighter != null)
        {
            PlayerFighters[id].FighterAsigned = newFighter;
            UpdatePlayerFighterPanel(newFighter);
        }
        else
        {
            DisablePanel(id);
        }
    }
    public void UpdatePlayerFighterPanel(Fighter updateFighter)
    {
        foreach (var fighterPanel in PlayerFighters)
        {
            if (fighterPanel.FighterAsigned == updateFighter)
            {
                fighterPanel.NicknameText.text = updateFighter.Nickname;
                fighterPanel.FighterIcon.sprite = updateFighter.AvatarSprite;
                fighterPanel.FighterIcon.enabled = true;
                fighterPanel.HealthPointsSlider.value = (float)updateFighter.HealthPoints / updateFighter.Stats.MaxHealtPoints;
                fighterPanel.HealthPointsText.text = $"{updateFighter.HealthPoints}/{updateFighter.Stats.MaxHealtPoints}";
                fighterPanel.EnergyPointsSlider.value = (float)updateFighter.EnergyPoints / updateFighter.Stats.MaxEnergyPoints;
                fighterPanel.EnergyPointsText.text = $"{updateFighter.EnergyPoints}/{updateFighter.Stats.MaxEnergyPoints}";
                return;
            }
        }
    }
    private void DisablePanel(int id)
    {
        PlayerFighters[id].FighterAsigned = null;
        PlayerFighters[id].NicknameText.text = "";
        PlayerFighters[id].FighterIcon.sprite = null;
        PlayerFighters[id].FighterIcon.enabled = false;
        PlayerFighters[id].HealthPointsSlider.value = 0;
        PlayerFighters[id].HealthPointsText.text = $"??/??";
        PlayerFighters[id].EnergyPointsSlider.value = 0;
        PlayerFighters[id].EnergyPointsText.text = $"??/??";
    }
    public void HiglightFighter(Fighter currentFighter)
    {
        foreach (var panel in PlayerFighters)
        {
            if (panel.FighterAsigned == currentFighter)
            {
                panel.SelectedFrame.SetActive(true);
            }
            else
            {
                panel.SelectedFrame.SetActive(false);
            }
        }
    }
    public void EnableAction(bool enable)
    {
        if (ActionsController != null)
        {
            ActionsController.EnableAction(enable);
        }
    }
    public void PlayPlayerWinAnim()
    {
        WinAnimationController.PlayPlayerWin();
    }
    public void PlayEnemyWinAnim()
    {
        WinAnimationController.PlayEnemyWin();
    }
}
