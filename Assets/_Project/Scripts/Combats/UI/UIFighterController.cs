using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIFighterController : MonoBehaviour
{
    public Slider HealthPointsSlider;
    public TMP_Text HealthPointsText;
    public Slider EnergyPointsSlider;
    public Fighter CurrentFighter;
    public GameObject DefenseIcon;
    [SerializeField] private float _AnimDuration = 1f;
    private int _Current, _Target;
    public void UpdateGeneralUI(bool animate = true)
    {
        _Target = CurrentFighter.HealthPoints;
        if (animate)
        {
            HealthPointsSlider.DOValue((float)CurrentFighter.HealthPoints / CurrentFighter.Stats.MaxHealtPoints, _AnimDuration);
            DOVirtual.Int(_Current, _Target, _AnimDuration, (x) =>
            {
                _Current = x;
                HealthPointsText.text = $"{_Current}";
            });
            //HealthPointsText.text = $"{CurrentFighter.HealthPoints}";
            EnergyPointsSlider.DOValue((float)CurrentFighter.EnergyPoints / CurrentFighter.Stats.MaxEnergyPoints, _AnimDuration);
        }
        else
        {
            HealthPointsSlider.value = (float)CurrentFighter.HealthPoints / CurrentFighter.Stats.MaxHealtPoints;
            HealthPointsText.text = $"{CurrentFighter.HealthPoints}";
            EnergyPointsSlider.value = (float)CurrentFighter.EnergyPoints / CurrentFighter.Stats.MaxEnergyPoints;
        }
    }
    public void EnableDefenseIcon(bool enable)
    {
        DefenseIcon.SetActive(enable);
    }
}
