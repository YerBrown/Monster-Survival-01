using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSlotPanel : MonoBehaviour
{
    public UIBuildMenuController BuildMenuController;
    public BuildingSO CurrentBuilding;
    [Header("UI")]
    public TMP_Text NameText;
    public TMP_Text TimeText;
    public Image BuildingImage;
    public Button BuildButton;
    public CostPanelControllerUI CostPanelController;
    public void UpdatePanel(BuildingSO newBuilding)
    {
        CurrentBuilding = newBuilding;

        NameText.text = CurrentBuilding.Name;
        BuildingSO.BuildTime buildTime = CurrentBuilding.FinishTimes[0];
        TimeText.text = $"";
        if (buildTime.Days > 0)
        {
            TimeText.text += $" {buildTime.Days}d";
        }
        if (buildTime.Hours > 0)
        {
            TimeText.text += $" {buildTime.Hours}h";
        }
        if (buildTime.Minutes > 0)
        {
            TimeText.text += $" {buildTime.Minutes}m";
        }
        if (buildTime.Seconds > 0)
        {
            TimeText.text += $" {buildTime.Seconds}s";
        }
        CostPanelController.UpdateCost(CurrentBuilding.Costs[0].Costs.ToList());
        BuildingImage.sprite = CurrentBuilding.Sprites[0];
        bool isPosibleToBuild = SurvivalBaseStorageManager.Instance.IsPosibleToConsume(CurrentBuilding.Costs[0].Costs.ToList());
        BuildButton.interactable = isPosibleToBuild;
    }
    public void ClearBuilding()
    {
        CurrentBuilding = null;
    }
    public void StartBuilding()
    {
        SurvivalBaseStorageManager.Instance.ConsumeItems(CurrentBuilding.Costs[0].Costs.ToList());
        BuildMenuController.StartBuidling(CurrentBuilding);
    }

}
