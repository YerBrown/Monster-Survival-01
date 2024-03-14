using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWinAnimationController : MonoBehaviour
{
    public Color PlayerColor;
    public Color EnemyColor;

    public Image WinFrame1;
    public Image WinFrame2;
    public TMP_Text WinText;

    Sequence _MySequence;
    public void PlayPlayerWin()
    {
        WinFrame1.color = new Color(PlayerColor.r, PlayerColor.g, PlayerColor.b, 0.8f);
        WinFrame2.color = PlayerColor;
        WinText.text = "Player Win!";
        AnimateWinIn();
    }
    public void PlayEnemyWin()
    {
        WinFrame1.color = new Color(EnemyColor.r, EnemyColor.g, EnemyColor.b, 0.8f);
        WinFrame2.color = EnemyColor;
        WinText.text = "Enemy Win!";
        AnimateWinIn();
    }

    public void AnimateWinIn()
    {
        if (_MySequence != null)
        {
            _MySequence.Kill();
        }

        _MySequence = DOTween.Sequence();
        _MySequence.Append(WinFrame1.rectTransform.DOAnchorPos (Vector2.zero, 1f).SetEase(Ease.InOutBack));
    }
    public void AnimateWinShowRewards()
    {
        if (_MySequence != null)
        {
            _MySequence.Kill();
        }

        _MySequence = DOTween.Sequence();
        _MySequence.Append(WinFrame1.transform.DOMoveX(0, 1f).SetEase(Ease.InOutBack));
    }
}
