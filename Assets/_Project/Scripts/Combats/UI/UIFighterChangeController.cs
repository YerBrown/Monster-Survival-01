using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class UIFighterChangeController : MonoBehaviour
{
    public GameObject ParentPopup;
    public List<FighterSlotPanel> FighterSlots = new();
    public FighterData CurrentFighterData;
    public List<Fighter> FightersSelectedToChange;
    public List<FighterData> NewFighterToAdd;
    private int CurrentChangeTurn;
    public bool IsChangeMandatory = false;
    public Button CloseButton;
    public Button InfoButton;
    public Button SwipeButton;
    public Color InFieldColor;
    public Color OutOfFieldColor;
    public Color DeadColor;
    [Serializable]
    public class FighterSlotPanel
    {
        public Button MainButton;
        public CanvasGroup Parent;
        public Image FighterImage;
        public Slider HealthSlider;
        public TMP_Text HealthText;
        public Slider EnergySlider;
        public TMP_Text EnergyText;
        public TMP_Text FisicalPower;
        public TMP_Text RangePower;
        public TMP_Text Defense;
        public Image ElementType;
        public Image SlotSelectedFrame;
        public Image ChangeThisFrame;
        public FighterData FighterInSlot;

        public void UpdateSlot(FighterData fighterData)
        {
            if (!string.IsNullOrEmpty(fighterData.ID))
            {
                FighterImage.sprite = FightersInfoWiki.Instance.FightersDictionary[fighterData.TypeID].c_Sprite;
                HealthSlider.value = (float)fighterData.HealthPoints / fighterData.MaxHealtPoints;
                HealthText.text = $"{fighterData.HealthPoints} / {fighterData.MaxHealtPoints}";
                EnergySlider.value = (float)fighterData.EnergyPoints / fighterData.MaxEnergyPoints;
                EnergyText.text = $"{fighterData.EnergyPoints} / {fighterData.MaxEnergyPoints}";
                FisicalPower.text = fighterData.FisicalPower.ToString();
                RangePower.text = fighterData.RangePower.ToString();
                Defense.text = fighterData.Defense.ToString();
                // TODO: Set elemnt sprite
                Parent.alpha = 1;
                FighterInSlot = fighterData;
            }
            else
            {
                FighterImage.sprite = null;
                HealthSlider.value = 0;
                HealthText.text = "0 / 0";
                EnergySlider.value = 0;
                EnergyText.text = "0 / 0";
                FisicalPower.text = "0";
                RangePower.text = "0";
                Defense.text = "0";
                // TODO: Set elemnt sprite
                Parent.alpha = 0;
                FighterInSlot = null;
            }

        }

    }

    public void OpenPopup(List<Fighter> fightersToChange, bool mandatory)
    {
        CurrentChangeTurn = 0;
        FightersSelectedToChange = fightersToChange;
        NewFighterToAdd.Clear();
        IsChangeMandatory = mandatory;
        CheckCurrentChangeTurn();
        ParentPopup.SetActive(true);
    }
    public void ClosePopup()
    {
        ParentPopup.SetActive(false);
    }
    public void UpdateUI()
    {
        for (int i = 0; i < FighterSlots.Count; i++)
        {
            FighterData newFighter = CombatManager.Instance.TeamsController.PlayerTeam.Fighters[i];
            if (newFighter != null)
            {
                FighterSlots[i].UpdateSlot(newFighter);
                FighterSlots[i].ChangeThisFrame.gameObject.SetActive(FightersSelectedToChange[CurrentChangeTurn].ID == newFighter.ID);
                if (!string.IsNullOrEmpty(newFighter.ID) && newFighter.IsDead())
                {
                    FighterSlots[i].MainButton.GetComponent<Image>().color = DeadColor;
                }
                else if (!string.IsNullOrEmpty(newFighter.ID))
                {
                    if (CombatManager.Instance.TeamsController.GetFighterInFieldByID(newFighter.ID) != null)
                    {
                        FighterSlots[i].MainButton.GetComponent<Image>().color = InFieldColor;
                    }
                    else
                    {
                        FighterSlots[i].MainButton.GetComponent<Image>().color = OutOfFieldColor;
                    }
                }
            }
        }
    }
    public void SelectFighter(Button fighterSLot)
    {
        if (fighterSLot == null)
        {
            CurrentFighterData = null;
            return;
        }
        foreach (var slot in FighterSlots)
        {
            if (slot.MainButton == fighterSLot)
            {
                CurrentFighterData = slot.FighterInSlot;
                slot.SlotSelectedFrame.gameObject.SetActive(true);
            }
            else
            {
                slot.SlotSelectedFrame.gameObject.SetActive(false);
            }
        }
        if (CombatManager.Instance.TeamsController.GetFighterInFieldByID(CurrentFighterData.ID) == null && !NewFighterToAdd.Contains(CurrentFighterData) && CurrentFighterData.HealthPoints > 0)
        {
            SwipeButton.interactable = true;
        }
        else
        {
            SwipeButton.interactable = false;
        }
        InfoButton.interactable = true;
    }
    public void ConfirmSwipe()
    {
        if (IsChangeMandatory)
        {
            NewFighterToAdd.Add(CurrentFighterData);
            CurrentChangeTurn++;
            if (CurrentChangeTurn >= FightersSelectedToChange.Count || CombatManager.Instance.TeamsController.PlayerTeam.GetFightersNotInField(FightersSelectedToChange.Count).Count == CurrentChangeTurn)
            {
                ChangeVariousFighters();
            }
            else
            {
                CheckCurrentChangeTurn();
            }
        }
        else
        {
            ChangeFighter();
        }
    }
    public void GoBack()
    {
        if (IsChangeMandatory)
        {
            CurrentChangeTurn--;
            NewFighterToAdd.RemoveAt(NewFighterToAdd.Count - 1);
            CheckCurrentChangeTurn();
        }
        else
        {
            ClosePopup();
        }
    }
    private void CheckCurrentChangeTurn()
    {
        if (CombatManager.Instance != null)
        {
            UpdateUI();
            SelectFighter(FighterSlots[0].MainButton);
        }
        if (IsChangeMandatory)
        {
            if (CurrentChangeTurn == 0)
            {
                CloseButton.gameObject.SetActive(false);
            }
            else
            {
                CloseButton.gameObject.SetActive(true);
            }
        }
        else
        {
            CloseButton.gameObject.SetActive(true);
        }
    }
    public void ChangeFighter()
    {
        CombatManager.Instance.ChangePlayerFighter(CurrentFighterData);
        ClosePopup();
    }
    public void ChangeVariousFighters()
    {
        CombatManager.Instance.ChangePlayerDeadFighters(NewFighterToAdd);
        ClosePopup();
    }
}
