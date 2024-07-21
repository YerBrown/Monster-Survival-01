using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIFighterHealthChangeNotificactionController : MonoSingleton<UIFighterHealthChangeNotificactionController>
{
    public TMP_Text HealthChangeText;
    Sequence _sequence;
    public void NotificateHealthChange(Vector3 fighterPosition, int amount, Effectiveness effectiveness)
    {
        if (_sequence != null)
        {
            _sequence.Kill();
        }
        if (amount > 0)
        {
            HealthChangeText.color = Color.green;
            HealthChangeText.text = $"+{Math.Abs(amount)}";
        }
        else if (amount < 0)
        {
            HealthChangeText.color = Color.red;
            HealthChangeText.text = $"-{Math.Abs(amount)}";
        }
        else
        {
            return;
        }
        HealthChangeText.rectTransform.position = Camera.main.WorldToScreenPoint(fighterPosition);
        _sequence = DOTween.Sequence();
        _sequence.Append(HealthChangeText.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic));
        _sequence.Join(HealthChangeText.DOFade(1, 0.25f));
        _sequence.AppendInterval(0.5f);
        _sequence.Append(HealthChangeText.DOFade(0, 2f));

    }
}
