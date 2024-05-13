using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIActionInFighterDataController : MonoBehaviour
{
    [Header("UI")]
    public RectTransform InfoPopup;
    public Image ActionImage;
    public TMP_Text ActionNameText;
    public Slider HPSlider;
    public TMP_Text HPText;
    public Slider EnergySlider;
    public TMP_Text EnergyText;
    [Header("Other")]
    private int _CurrentHP, _TargetHP;
    private int _CurrentEnergy, _TargetEnergy;
    Sequence _Sequence;
    public void UpdateFighterDataPanel(FighterData fighterData, int currentHP, int targetHP, int currentEnergy, int targetEnergy)
    {
        CreatureSO fighterInfo = fighterData.GetCreatureInfo();
        if (fighterInfo!=null)
        {
            ActionImage.sprite = fighterInfo.c_AvatarSprite;
        }
        else
        {
            ActionImage.sprite = null;
        }
        
        ActionNameText.text = fighterData.Nickname;
        HPSlider.value = (float)currentHP / fighterData.MaxHealthPoints;
        EnergySlider.value = (float)currentEnergy / fighterData.MaxEnergyPoints;
        HPText.text = currentHP.ToString();
        EnergyText.text = currentEnergy.ToString();

        _CurrentHP = currentHP;
        _TargetHP = targetHP;
        _CurrentEnergy = currentEnergy;
        _TargetEnergy = targetEnergy;

        InfoPopup.gameObject.SetActive(true);
        _Sequence = DOTween.Sequence();
        _Sequence.Append(InfoPopup.DOAnchorPosX(0, 0.5f));
        if (currentHP != targetHP)
        {
            _Sequence.Append(HPSlider.DOValue((float)_TargetHP / fighterData.MaxHealthPoints, 1f)).
                Join(DOVirtual.Int(_CurrentHP, _TargetHP, 1f, (x) =>
            {
                _CurrentHP = x;
                HPText.text = $"{_CurrentHP}";
            }));
        }
        if (currentEnergy != targetEnergy)
        {
            _Sequence.Append(EnergySlider.DOValue((float)_TargetEnergy / fighterData.MaxEnergyPoints, 1f)).
                Join(DOVirtual.Int(_CurrentEnergy, _TargetEnergy, 1f, (x) =>
            {
                _CurrentEnergy = x;
                HPText.text = $"{_CurrentEnergy}";
            }));
        }
        _Sequence.AppendInterval(1f);
        _Sequence.Append(InfoPopup.DOAnchorPosX(500, 0.5f));
        _Sequence.AppendInterval(1f);
        _Sequence.OnComplete(() =>
        {
            InfoPopup.gameObject.SetActive(false);
        });
        _Sequence.Play();
    }
}
