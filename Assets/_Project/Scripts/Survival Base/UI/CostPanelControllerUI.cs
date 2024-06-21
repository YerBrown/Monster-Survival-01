using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostPanelControllerUI : MonoBehaviour
{
    public List<CostSlot> AllCosts = new();
    [Serializable]
    public class CostSlot
    {
        public GameObject CostParent;
        public Image CostImage;
        public TMP_Text CostText;
    }
    public void UpdateCost(List<ItemSlot> costs)
    {
        for (int i = 0; i < AllCosts.Count; i++)
        {
            if (i < costs.Count && costs[i].ItemInfo != null)
            {
                string amountTextColor;
                int currentAmount = SurvivalBaseStorageManager.Instance.GetItemAmount(costs[i].ItemInfo);
                if (currentAmount >= costs[i].Amount)
                {
                    amountTextColor = ColorUtility.ToHtmlStringRGB(AllCosts[i].CostText.color);
                }
                else
                {
                    amountTextColor = "F53535";
                }
                AllCosts[i].CostText.text = $"<color=#{amountTextColor}>{currentAmount.ToString("00")}</color>/{costs[i].Amount.ToString("00")}";
                AllCosts[i].CostImage.sprite = costs[i].ItemInfo.i_Sprite;
                AllCosts[i].CostParent.SetActive(true);
            }
            else
            {
                AllCosts[i].CostParent.SetActive(false);
            }
        }
    }
}
