using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFighterController : MonoBehaviour
{
    public Slider HealthPointsSlider;
    public TMP_Text HealthPointsText;
    public Fighter CurrentFighter;
    public GameObject DefenseIcon;
    public void UpdateGeneralUI()
    {
        HealthPointsSlider.value = (float)CurrentFighter.HealthPoints / CurrentFighter.Stats.MaxHealtPoints;
        HealthPointsText.text = $"{CurrentFighter.HealthPoints}";
    }
    public void EnableDefenseIcon(bool enable)
    {
        DefenseIcon.SetActive(enable);
    }
}
