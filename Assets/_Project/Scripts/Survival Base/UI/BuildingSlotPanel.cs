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
    public Image[] ItemCostImages = new Image[3];

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
                ItemCostText[i].text = $"00/{CurrentBuilding.Costs[0].Costs[i].Amount.ToString("00")}";
                ItemCostImages[i].sprite = CurrentBuilding.Costs[0].Costs[i].ItemInfo.i_Sprite;
                ItemCostText[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                ItemCostText[i].transform.parent.gameObject.SetActive(false);
            }
        }
        BuildingImage.sprite = CurrentBuilding.Sprites[0];
    }
    public void ClearBuilding()
    {
        CurrentBuilding = null;
    }
    public void StartBuilding()
    {
        BuildMenuController.StartBuidling(CurrentBuilding);
    }

}
