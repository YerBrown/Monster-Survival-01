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
    public TMP_Text HealthChangeText;
    public TMP_Text NextText;
    [SerializeField] private float _AnimDuration = 1f;
    private int _Current, _Target;
    Sequence _SequenceGeneral;
    Sequence _SequenceHP;
    private void Start()
    {
        CurrentFighter=GetComponentInParent<Fighter>();
    }
    public void UpdateGeneralUI(bool animate = true)
    {
        if (_SequenceGeneral != null)
        {
            _SequenceGeneral.Kill();
        }

        _Target = CurrentFighter.HealthPoints;
        if (animate)
        {
            _SequenceGeneral = DOTween.Sequence();
            _SequenceGeneral.Append(HealthPointsSlider.DOValue((float)CurrentFighter.HealthPoints / CurrentFighter.Stats.MaxHealtPoints, _AnimDuration));
            _SequenceGeneral.Join(DOVirtual.Int(_Current, _Target, _AnimDuration, (x) =>
            {
                _Current = x;
                HealthPointsText.text = $"{_Current}";
            }));
            //HealthPointsText.text = $"{CurrentFighter.HealthPoints}";
            _SequenceGeneral.Join(EnergyPointsSlider.DOValue((float)CurrentFighter.EnergyPoints / CurrentFighter.Stats.MaxEnergyPoints, _AnimDuration));
        }
        else
        {
            HealthPointsSlider.value = (float)CurrentFighter.HealthPoints / CurrentFighter.Stats.MaxHealtPoints;
            HealthPointsText.text = $"{CurrentFighter.HealthPoints}";
            EnergyPointsSlider.value = (float)CurrentFighter.EnergyPoints / CurrentFighter.Stats.MaxEnergyPoints;
        }
    }
    public void HealthPointsChanged(int amount)
    {
        if (_SequenceHP != null)
        {
            _SequenceHP.Kill();
        }
        if (amount > 0)
        {
            HealthChangeText.color = Color.green;
        }
        else
        {
            HealthChangeText.color = Color.red;
        }
        HealthChangeText.rectTransform.localScale = new Vector2(0.1f, 0.1f);
        HealthChangeText.text = amount.ToString();
        _SequenceHP = DOTween.Sequence();
        _SequenceHP.Append(HealthChangeText.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic));
        _SequenceHP.Join(HealthChangeText.DOFade(1, 0.25f));
        _SequenceHP.Append(HealthChangeText.DOFade(0, 1f));
    }
    public void EnableDefenseIcon(bool enable)
    {
        DefenseIcon.SetActive(enable);
    }
}
