using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Properties;
using GeneralValues;
public class UIFighterController : MonoBehaviour
{
    public RectTransform StatsInfoPanel;
    public Slider HealthPointsSlider;
    public TMP_Text HealthPointsText;
    public Slider EnergyPointsSlider;
    public Fighter CurrentFighter;
    public GameObject DefenseIcon;
    public Image FrienshipPointsFillImage;
    public CanvasGroup FriendshipPointsParent;
    public Image EffectiveAddedFriendshipPointsIcon;
    public TMP_Text NextText;
    public RectTransform PlayerStatsPos;
    public RectTransform EnemyStatsPos;
    public List<GameObject> StatusProblemIcons = new();
    public TMP_Text CurrentStatusTurnsActiveText;
    [SerializeField] private float _AnimDuration = 1f;
    private int _Current, _Target;
    Sequence _SequenceGeneral;
    Sequence _SequenceHP;
    Sequence _SequenceFriendPoints;
    private void Start()
    {
        CurrentFighter = GetComponentInParent<Fighter>();
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
            if (_Current != _Target)
            {
                _SequenceGeneral.Append(HealthPointsSlider.DOValue((float)CurrentFighter.HealthPoints / CurrentFighter.CurrentStats.MaxHealthPoints, _AnimDuration));
                _SequenceGeneral.Join(DOVirtual.Int(_Current, _Target, _AnimDuration, (x) =>
                {
                    _Current = x;
                    HealthPointsText.text = $"{_Current}";
                }));
            }
            //HealthPointsText.text = $"{CurrentFighter.HealthPoints}";
            _SequenceGeneral.Join(EnergyPointsSlider.DOValue((float)CurrentFighter.EnergyPoints / CurrentFighter.CurrentStats.MaxEnergyPoints, _AnimDuration));
        }
        else
        {
            _Current = CurrentFighter.HealthPoints;
            HealthPointsSlider.value = (float)CurrentFighter.HealthPoints / CurrentFighter.CurrentStats.MaxHealthPoints;
            HealthPointsText.text = $"{CurrentFighter.HealthPoints}";
            EnergyPointsSlider.value = (float)CurrentFighter.EnergyPoints / CurrentFighter.CurrentStats.MaxEnergyPoints;
        }
        int statusIndex = -1;
        switch (CurrentFighter.CurrentStatusProblem)
        {
            case StatusProblemType.NONE:
                statusIndex = -1;
                break;
            case StatusProblemType.BURNED:
                statusIndex = 0;
                break;
            case StatusProblemType.POISONED:
                statusIndex = 1;
                break;
            case StatusProblemType.PARALIZED:
                statusIndex = 2;
                break;
            case StatusProblemType.FROZEN:
                statusIndex = 3;
                break;
            case StatusProblemType.TRAPED:
                statusIndex = 4;
                break;
            default:
                break;
        }
        for (int i = 0; i < StatusProblemIcons.Count; i++)
        {
            if (StatusProblemIcons[i].gameObject != null)
            {
                if (i == statusIndex)
                {
                    StatusProblemIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    StatusProblemIcons[i].gameObject.SetActive(false);
                }
                if (i == 0 && statusIndex == 0)
                {
                    CurrentStatusTurnsActiveText.text = (StaticCombatGeneralValues.Fighter_StatusProblem_BurnedMaxTurns - CurrentFighter.CurrentStatusProblemActiveTurns).ToString();
                    CurrentStatusTurnsActiveText.enabled = true;
                }
                else
                {
                    CurrentStatusTurnsActiveText.enabled = false;
                }
            }
        }
    }
    public void HealthPointsChanged(int amount, Effectiveness effectiveness)
    {
        CombatManager.Instance.UIManager.NotificationController.OnHealthChanged(transform.position, amount, effectiveness);
    }
    public void FrienshipPointsChanged(bool effectiveAdded)
    {
        if (_SequenceFriendPoints != null)
        {
            _SequenceFriendPoints.Kill();
        }
        CreatureSO creatureInfo = CurrentFighter.GetCreatureInfo();
        _SequenceFriendPoints = DOTween.Sequence();
        if (!FriendshipPointsParent.gameObject.activeSelf)
        {
            FriendshipPointsParent.alpha = 0;
            FriendshipPointsParent.gameObject.SetActive(true);
            _SequenceFriendPoints.Append(FriendshipPointsParent.DOFade(1, 0.25f));
        }
        if (creatureInfo != null)
        {
            _SequenceFriendPoints.Append(FrienshipPointsFillImage.DOFillAmount((float)CurrentFighter.CurrentFriendshipPoints / creatureInfo.c_MaxFriendshipPoints, 1f));
        }
        if (effectiveAdded)
        {
            EffectiveAddedFriendshipPointsIcon.rectTransform.anchoredPosition = new Vector2(EffectiveAddedFriendshipPointsIcon.rectTransform.anchoredPosition.x, 65);
            _SequenceFriendPoints.Join(EffectiveAddedFriendshipPointsIcon.rectTransform.DOAnchorPosY(75, 0.5f).SetEase(Ease.OutElastic));
            _SequenceFriendPoints.Join(EffectiveAddedFriendshipPointsIcon.DOFade(1f, 0.25F));
            _SequenceFriendPoints.AppendInterval(1f);
            _SequenceFriendPoints.Append(EffectiveAddedFriendshipPointsIcon.DOFade(0, 0.25F));
        }
    }
    public void EnableDefenseIcon(bool enable)
    {
        DefenseIcon.SetActive(enable);
    }

    public void MoveStatsInfoToCorrectPosition(bool isPlayerTeam)
    {
        if (isPlayerTeam)
        {
            StatsInfoPanel.transform.position = PlayerStatsPos.position;
        }
        else
        {
            StatsInfoPanel.transform.position = EnemyStatsPos.position;
        }
    }
}
