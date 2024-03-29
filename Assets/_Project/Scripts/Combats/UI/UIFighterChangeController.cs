using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class UIFighterChangeController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject ParentPopup; // Popup parent.
    [SerializeField] private GameObject ParentSelectSwipe; // Popup parent.
    [SerializeField] private List<FighterSlotPanel> FighterSlots = new(); // All the slots for the player team fighters in popup.
    [SerializeField] private List<Button> SelectFighterButtons = new(); // All buttons of fighter to be changed selection.
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button InfoButton;
    [SerializeField] private Button SwipeButton;
    [SerializeField] private Button ItemUseButton;
    [Header("Change Data")]
    [SerializeField] private int CurrentChangeTurn; // Current fighter change turn.
    [SerializeField] private List<int> ChangesNums = new(); // All changes selected nums.
    [SerializeField] private bool IsChangeMandatory = false; // If is a change by dead fighters, is mandatory to select all change posibles.
    [SerializeField] private FighterData CurrentFighterData; // Selected fighter data to replace other fighter.
    private Dictionary<int, Fighter> FightersSelectedToChange = new Dictionary<int, Fighter>(); // Dictionary of all fighters selected to replace in the field.
    private Dictionary<int, FighterData> NewFighterToAdd = new Dictionary<int, FighterData>(); // Dictionary of the replacements for the selected fighters in field.
    [Header("Slot Panel Type Colors")]
    [SerializeField] private Color InFieldColor;
    [SerializeField] private Color OutOfFieldColor;
    [SerializeField] private Color DeadColor;
    [Header("Other")]
    [SerializeField] private UIPlayerInventoryInCombatController _PlayerInventoryController;
    private SelectFighterMode _SelectMode;
    public enum SelectFighterMode
    {
        CHANGE,
        SELECT_ITEM_TARGET,
    }
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
                FighterImage.sprite = FightersInfoWiki.Instance.FightersDictionary[fighterData.TypeID].c_AvatarSprite;
                HealthSlider.value = (float)fighterData.HealthPoints / fighterData.MaxHealthPoints;
                HealthText.text = $"{fighterData.HealthPoints} / {fighterData.MaxHealthPoints}";
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
    // Enables the popup and passes the changes needed and if the changes are mandatory.
    public void OpenPopup(List<Fighter> fightersToChange, bool mandatory)
    {
        CurrentChangeTurn = 0;
        NewFighterToAdd.Clear();
        ChangesNums.Clear();
        FightersSelectedToChange.Clear();
        IsChangeMandatory = mandatory;
        _SelectMode = SelectFighterMode.CHANGE;
        for (int i = 0; i < fightersToChange.Count; i++)
        {
            int num = CombatManager.Instance.TeamsController.GetFighterInFieldNum(fightersToChange[i]);
            FightersSelectedToChange.Add(num, fightersToChange[i]);
        }
        SwipeButton.gameObject.SetActive(true);
        ItemUseButton.gameObject.SetActive(false);
        // Update the popup.
        CheckCurrentChangeTurn();
        ParentPopup.SetActive(true);
    }
    public void OpenPopupToSetItemTarget()
    {
        CurrentChangeTurn = 0;
        NewFighterToAdd.Clear();
        ChangesNums.Clear();
        FightersSelectedToChange.Clear();
        IsChangeMandatory = false;
        _SelectMode = SelectFighterMode.SELECT_ITEM_TARGET;
        // Update the popup.
        CheckCurrentChangeTurn();
        SwipeButton.gameObject.SetActive(false);
        ItemUseButton.gameObject.SetActive(true);
        ParentPopup.SetActive(true);
    }
    // Disable teh popup.
    public void ClosePopup()
    {
        ParentPopup.SetActive(false);
    }
    // Update the UI with the current turn change needed.
    public void UpdateUI()
    {
        for (int i = 0; i < FighterSlots.Count; i++)
        {
            FighterData newFighter = CombatManager.Instance.TeamsController.PlayerTeam.Fighters[i];
            if (newFighter != null)
            {
                // Update the slot panel with fighter data.
                FighterSlots[i].UpdateSlot(newFighter);
                // Check if the current slot is the fighter that we want to replace.

                FighterSlots[i].ChangeThisFrame.gameObject.SetActive(CheckIfFighterDataIsFighterSelectedToReplace(newFighter) && !IsRaplacementAlreadySelectedForFighter(newFighter));

                // Change the color of the panels acording to the needs.
                // Check if the fiighter is dead.
                if (!string.IsNullOrEmpty(newFighter.ID) && newFighter.IsDead())
                {
                    FighterSlots[i].MainButton.GetComponent<Image>().color = DeadColor;
                }
                else if (!string.IsNullOrEmpty(newFighter.ID))
                {
                    // Check if the fighter is on field or is already selected to be a replacement.
                    if (CombatManager.Instance.TeamsController.GetFighterInFieldByID(newFighter.ID) != null || NewFighterToAdd.ContainsValue(newFighter))
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
    private bool CheckIfFighterDataIsFighterSelectedToReplace(FighterData fighterData)
    {
        foreach (var fighter in FightersSelectedToChange)
        {
            if (fighter.Value.ID == fighterData.ID)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsRaplacementAlreadySelectedForFighter(FighterData fighterData)
    {
        foreach (var fighter in FightersSelectedToChange)
        {
            if (fighterData.ID == fighter.Value.ID)
            {
                int key = fighter.Key;
                return NewFighterToAdd.ContainsKey(key);
            }
        }
        return false;
    }
    // Select the slot to be a replacement or whatch her data/info. 
    public void SelectFighter(Button fighterSLot)
    {
        if (fighterSLot == null)
        {
            CurrentFighterData = null;
            return;
        }
        // Update slected frame to all slots
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
        // Check if the fighter selected can be a replacement, it's not possible if they are currently on the battlefield, nor if they are dead, and if they are already a replacment, then as well.
        if (CombatManager.Instance.TeamsController.GetFighterInFieldByID(CurrentFighterData.ID) == null && !NewFighterToAdd.ContainsValue(CurrentFighterData) && CurrentFighterData.HealthPoints > 0)
        {
            SwipeButton.interactable = true;
        }
        else
        {
            SwipeButton.interactable = false;
        }
        InfoButton.interactable = true;
    }
    // Confirm the selected fighter change.
    public void ConfirmSwipe()
    {
        CurrentChangeTurn++;
        CheckIfChangeIsComplete();
    }
    private void CheckIfChangeIsComplete()
    {
        // Check if the replacements list has all the replacements needed or if there arent more replacements posibles.
        if (CurrentChangeTurn >= FightersSelectedToChange.Count || CombatManager.Instance.TeamsController.PlayerTeam.GetFightersNotInField(FightersSelectedToChange.Count).Count == CurrentChangeTurn)
        {
            // Start change fighters flow.
            if (IsChangeMandatory)
            {
                ChangeVariousFighters();

            }
            else
            {
                ChangeFighter();
            }
        }
        else
        {

            ParentPopup.SetActive(true);
            // Pass to next replacemnt selection.
            CheckCurrentChangeTurn();
        }
    }
    // The logic behind the back/close button.
    public void GoBack()
    {
        // Check if the current state is a multiple change mandatory or single change optional
        if (IsChangeMandatory)
        {
            // Go back to previous replacemnt selection.
            CurrentChangeTurn--;
            NewFighterToAdd.Remove(ChangesNums[ChangesNums.Count - 1]);
            ChangesNums.RemoveAt(ChangesNums.Count - 1);
            CheckCurrentChangeTurn();
        }
        else
        {
            // Close the popup because the change is optional.
            ClosePopup();
            if (_SelectMode == SelectFighterMode.CHANGE)
            {
                CombatManager.Instance.UIManager.ActionsController.EnableAction(true);
            }
            else
            {
                _PlayerInventoryController.OpenSelectTargets();
            }
        }
    }
    // Update the ui depending of the needs of replacemnt selection.
    private void CheckCurrentChangeTurn()
    {
        // Update the ui
        if (CombatManager.Instance != null)
        {
            UpdateUI();
            SelectFighter(FighterSlots[0].MainButton);
        }
        if (IsChangeMandatory)
        {
            // If is a mandatory change, enable the go back button if the current change turn isn't the first one.
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
            // Enable the close button is is optional change.
            CloseButton.gameObject.SetActive(true);
        }
    }
    public void OpenSelectFighterToBeChanged()
    {
        // Disable all select actions.
        foreach (var button in SelectFighterButtons)
        {
            button.gameObject.SetActive(false);
        }
        // Enable needed select buttons.
        foreach (var fighter in FightersSelectedToChange)
        {
            if (!NewFighterToAdd.ContainsKey(fighter.Key))
            {
                SelectFighterButtons[fighter.Key].GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(CombatManager.Instance.TeamsController.PlayerTeam.FightersPos[fighter.Key].transform.position + Vector3.up * 0.15f);
                SelectFighterButtons[fighter.Key].gameObject.SetActive(true);
            }
        }
        ParentPopup.SetActive(false);
        ParentSelectSwipe.SetActive(true);
    }
    public void CancelSelect()
    {
        ParentPopup.SetActive(true);
        ParentSelectSwipe.SetActive(false);
    }
    public void SelectReplacementFighter(int num)
    {
        NewFighterToAdd[num] = CurrentFighterData;
        ChangesNums.Add(num);
        ParentSelectSwipe.SetActive(false);
        ConfirmSwipe();
    }
    public void OnSwipe()
    {
        OpenSelectFighterToBeChanged();
    }
    public void OnInfo()
    {

    }
    public void OnSelectItemTarget()
    {
        _PlayerInventoryController.SelectFighterDataTarget(CurrentFighterData);
        ClosePopup();
        CombatManager.Instance.UIManager.NotificationController.DisableActionInfoPopup();
    }
    // Start flow of optional and single change.
    public void ChangeFighter()
    {
        CombatManager.Instance.ChangePlayerFighter(CurrentFighterData);
        ClosePopup();
    }
    // Start flow of multiple mandatory change.
    public void ChangeVariousFighters()
    {
        List<FighterData> newFighters = new() { new FighterData(), new FighterData(), new FighterData() };
        foreach (var fighter in NewFighterToAdd)
        {
            newFighters[fighter.Key] = fighter.Value;
        }
        CombatManager.Instance.ChangePlayerDeadFighters(newFighters);
        ClosePopup();
    }
}
