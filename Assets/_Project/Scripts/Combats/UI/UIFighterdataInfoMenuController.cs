using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Rendering.FilterWindow;

public class UIFighterdataInfoMenuController : MonoBehaviour
{
    public RectTransform ParentPopup;
    public Animator FighterForRenderAnimator;
    public Image ElementType;
    public Image AvatarImage;
    public Image HPImage;
    public Image EPImage;
    public TMP_Text NicknameText;
    public TMP_Text RaceText;
    public TMP_Text HealthPointsText;
    public TMP_Text EnergyPointsText;
    public TMP_Text LvlText;
    public TMP_Text FisicalPowerText;
    public TMP_Text RangePowerText;
    public TMP_Text DefensePowerText;
    public TMP_Text SpeedPowerText;
    public Button PrevFighterButton;
    public Button NextFighterButton;

    private List<FighterData> _FightersList = new();
    private int _CurrentFighterIndex;
    private FighterData _CurrentFighter;
    public void OpenPopup(FighterData fighterData)
    {
        _FightersList.Clear();
        _CurrentFighter = fighterData;
        UpdateUI();
        ParentPopup.gameObject.SetActive(true);
        PrevFighterButton.gameObject.SetActive(false);
        NextFighterButton.gameObject.SetActive(false);
    }
    public void OpenPopup(List<FighterData> fighters, int fighterIndex)
    {
        _FightersList = fighters;
        _CurrentFighterIndex = fighterIndex;
        _CurrentFighter = _FightersList[fighterIndex];
        UpdateUI();
        ParentPopup.gameObject.SetActive(true);
        if (fighters.Count > 1)
        {
            PrevFighterButton.gameObject.SetActive(true);
            NextFighterButton.gameObject.SetActive(true);
        }
        else
        {
            PrevFighterButton.gameObject.SetActive(false);
            NextFighterButton.gameObject.SetActive(false);
        }
    }
    public void ClosePopup()
    {
        ParentPopup.gameObject.SetActive(false);
    }
    public void PrevFighter()
    {
        if (_CurrentFighterIndex - 1 < 0)
        {
            _CurrentFighterIndex = _FightersList.Count - 1;
        }
        else
        {
            _CurrentFighterIndex--;
        }
        _CurrentFighter = _FightersList[_CurrentFighterIndex];
        UpdateUI();
    }
    public void NextFighter()
    {
        if (_CurrentFighterIndex + 1 >= _FightersList.Count)
        {
            _CurrentFighterIndex = 0;
        }
        else
        {
            _CurrentFighterIndex++;
        }
        _CurrentFighter = _FightersList[_CurrentFighterIndex];
        UpdateUI();
    }
    private void UpdateUI()
    {
        CreatureSO fighter = _CurrentFighter.GetCreatureInfo();
        if (fighter != null)
        {
            if (MainWikiManager.Instance.GetElementSprite(fighter.c_Element, out Sprite elementSprite))
            {
                ElementType.sprite = elementSprite;
            }
            else
            {
                ElementType.sprite = MainWikiManager.Instance.MissingSprite;
            }
            NicknameText.text = _CurrentFighter.Nickname;
            RaceText.text = fighter.c_Name;
            HealthPointsText.text = $"HP {_CurrentFighter.HealthPoints}/{_CurrentFighter.MaxHealthPoints}";
            HPImage.fillAmount = (float)_CurrentFighter.HealthPoints / _CurrentFighter.MaxHealthPoints;
            EnergyPointsText.text = $"EP {_CurrentFighter.EnergyPoints}/{_CurrentFighter.MaxEnergyPoints}";
            EPImage.fillAmount = (float)_CurrentFighter.EnergyPoints / _CurrentFighter.MaxEnergyPoints;

            FighterForRenderAnimator.runtimeAnimatorController = fighter.c_Animator;
            AvatarImage.sprite = fighter.c_AvatarSprite;
            LvlText.text = $"Lvl.{_CurrentFighter.Lvl}";
            FisicalPowerText.text = _CurrentFighter.FisicalPower.ToString();
            RangePowerText.text = _CurrentFighter.RangePower.ToString();
            DefensePowerText.text = _CurrentFighter.Defense.ToString();
            SpeedPowerText.text = _CurrentFighter.Speed.ToString();
        }

    }

}
