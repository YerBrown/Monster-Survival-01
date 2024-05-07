using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICaptueMenuController : MonoBehaviour
{
    public RectTransform PopupParent;
    public RectTransform NotEnoughEnergyPopup;
    public List<Image> SlotsImages = new();
    public List<Image> IntensityLocks = new();
    public Slider PlayerEnergySlider;
    public Image PlayerEnergyAfterCaptureImage;
    public Slider IntensitySlider;
    public TMP_Text PlayerEnergyText;
    public TMP_Text IntensityText;
    public Button DecreaseIntensityButton;
    public Button IncreaseIntensityButton;
    public Button CancelActionButton;
    public Sprite CapsuleEmptySprite;

    public int CurrentIntensity = 0;
    private int _PlayerMaxIntensityLevel = 1;

    public UIActionsController ActionsController;
    public void OpenPopup()
    {
        FighterData playerFighter = PlayerManager.Instance.P_Fighter;
        PlayerEnergySlider.value = (float)playerFighter.EnergyPoints / playerFighter.MaxEnergyPoints;
        for (int i = 0; i < SlotsImages.Count; i++)
        {
            bool isUnlockedSlot = i < PlayerManager.Instance.Captures.CaptureSlots.Count;
            if (isUnlockedSlot)
            {
                FighterData fighterSlot = PlayerManager.Instance.Captures.CaptureSlots[i];
                if (fighterSlot == null || string.IsNullOrEmpty(fighterSlot.ID))
                {
                    SlotsImages[i].transform.GetChild(0).GetComponent<Image>().sprite = CapsuleEmptySprite;
                }
                else
                {
                    CreatureSO fighter = fighterSlot.GetCreatureInfo();
                    if (fighter != null)
                    {
                        SlotsImages[i].transform.GetChild(0).GetComponent<Image>().sprite = fighter.c_AvatarSprite;
                    }
                    else
                    {
                        SlotsImages[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    }
                }
            }
            SlotsImages[i].transform.GetChild(0).gameObject.SetActive(isUnlockedSlot);
            SlotsImages[i].transform.GetChild(1).gameObject.SetActive(!isUnlockedSlot);
        }
        _PlayerMaxIntensityLevel = PlayerManager.Instance.Captures.CurrentMaxIntensity;
        for (int i = 0; i < IntensityLocks.Count; i++)
        {
            bool isUnlockedIntensity = i < _PlayerMaxIntensityLevel;
            IntensityLocks[i].gameObject.SetActive(!isUnlockedIntensity);
        }
        CancelActionButton.gameObject.SetActive(true);
        PopupParent.gameObject.SetActive(true);
        UpdateIntensityLevel();
    }
    private void UpdateIntensityLevel()
    {
        IntensityText.text = $" -{GeneralValues.StaticCombatGeneralValues.Capture_CaptureIntensity_EnergyCosts[CurrentIntensity]}";
        IntensitySlider.value = CurrentIntensity + 1;
        PlayerEnergyAfterCaptureImage.fillAmount = (float)(PlayerManager.Instance.P_Fighter.EnergyPoints - GeneralValues.StaticCombatGeneralValues.Capture_CaptureIntensity_EnergyCosts[CurrentIntensity]) / PlayerManager.Instance.P_Fighter.MaxEnergyPoints;
        PlayerEnergyText.text = $"{PlayerManager.Instance.P_Fighter.EnergyPoints}({PlayerManager.Instance.P_Fighter.EnergyPoints - GeneralValues.StaticCombatGeneralValues.Capture_CaptureIntensity_EnergyCosts[CurrentIntensity]})/{PlayerManager.Instance.P_Fighter.MaxEnergyPoints}";
        DecreaseIntensityButton.interactable = CurrentIntensity > 0;
        IncreaseIntensityButton.interactable = CurrentIntensity < _PlayerMaxIntensityLevel - 1;
        if (PlayerManager.Instance.P_Fighter.EnergyPoints > GeneralValues.StaticCombatGeneralValues.Capture_CaptureIntensity_EnergyCosts[CurrentIntensity] && PlayerManager.Instance.Captures.GetRemainingCapsules() > 0)
        {
            ActionsController.TargetController.EnableCaptureRates(CurrentIntensity);
            ActionsController.TargetController.EnableEnemyTargets();
            NotEnoughEnergyPopup.gameObject.SetActive(false);
        }
        else
        {
            ActionsController.TargetController.DisableAllTargets();
            StringBuilder warningText = new StringBuilder();
            if (PlayerManager.Instance.P_Fighter.EnergyPoints < GeneralValues.StaticCombatGeneralValues.Capture_CaptureIntensity_EnergyCosts[CurrentIntensity])
            {
                warningText.AppendLine("Not enough energy");
            }
            if (PlayerManager.Instance.Captures.GetRemainingCapsules() <= 0)
            {
                warningText.AppendLine("No empty capsules");
            }
            NotEnoughEnergyPopup.GetComponentInChildren<TMP_Text>().text = warningText.ToString();
            NotEnoughEnergyPopup.gameObject.SetActive(true);
        }
    }
    public void IncreaseCurrentIntensity()
    {
        if (CurrentIntensity + 1 < _PlayerMaxIntensityLevel)
        {
            CurrentIntensity++;
        }
        UpdateIntensityLevel();
    }
    public void DecreaseCurrentIntensity()
    {
        if (CurrentIntensity - 1 >= 0)
        {
            CurrentIntensity--;
        }
        UpdateIntensityLevel();
    }
    public void CancelAction()
    {
        ClosePopup();
        ActionsController.OnCancelAction();
    }
    public void ClosePopup()
    {
        CancelActionButton.gameObject.SetActive(false);
        PopupParent.gameObject.SetActive(false);
        NotEnoughEnergyPopup.gameObject.SetActive(false);
    }

    public void OpenCapturedFighterInfo(int index)
    {
        FighterData selectedFighter = null;
        if (index < PlayerManager.Instance.Captures.CaptureSlots.Count && !string.IsNullOrEmpty(PlayerManager.Instance.Captures.CaptureSlots[index].ID))
        {
            selectedFighter = PlayerManager.Instance.Captures.CaptureSlots[index];
        }
        if (selectedFighter != null)
        {
            CombatManager.Instance.UIManager.FighterDataInfoController.OpenPopup(selectedFighter);
        }
    }
}
