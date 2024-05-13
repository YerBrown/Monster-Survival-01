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
    public TMP_Text[] ItemCostText = new TMP_Text[3];
    public Color NotEnoughAmountColor = Color.white;
    public Image[] ItemCostImages = new Image[3];
    public Button BuildButton;
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

        for (int i = 0; i < ItemCostText.Length; i++)
        {
            if (CurrentBuilding.Costs[0].Costs[i].ItemInfo != null)
            {
                string amountTextColor;
                int currentAmount = SurvivalBaseStorageManager.Instance.GetItemAmount(CurrentBuilding.Costs[0].Costs[i].ItemInfo);
                if (currentAmount >= CurrentBuilding.Costs[0].Costs[i].Amount)
                {
                    amountTextColor = ColorUtility.ToHtmlStringRGB(ItemCostText[i].color);
                }
                else
                {
                    amountTextColor = ColorUtility.ToHtmlStringRGB(NotEnoughAmountColor);
                }
                ItemCostText[i].text = $"<color=#{amountTextColor}>{currentAmount.ToString("00")}</color>/{CurrentBuilding.Costs[0].Costs[i].Amount.ToString("00")}";
                ItemCostImages[i].sprite = CurrentBuilding.Costs[0].Costs[i].ItemInfo.i_Sprite;
                ItemCostText[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                ItemCostText[i].transform.parent.gameObject.SetActive(false);
            }
        }
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
