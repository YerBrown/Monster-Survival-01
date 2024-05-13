using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainScreenController : MonoBehaviour
{
    public CanvasGroup MainScreenParent;
    public BoolEventChannelSO OnOpenMenuPopup;
    public Sequence FadeSequence;

    private void OnEnable()
    {
        OnOpenMenuPopup.OnEventRaised += EnableMainScreen;
    }
    private void OnDisable()
    {
        OnOpenMenuPopup.OnEventRaised -= EnableMainScreen;
        
    }
    public void EnableMainScreen(bool enable)
    {
        if (FadeSequence != null)
        {
            FadeSequence.Kill();
        }
        FadeSequence = DOTween.Sequence();
        if (!enable)
        {
            FadeSequence.Append(MainScreenParent.DOFade(1f, 0.25f).OnComplete(() =>
            {
                MainScreenParent.interactable = true;
                MainScreenParent.blocksRaycasts = true;
            }));
        }
        else
        {
            FadeSequence.Append(MainScreenParent.DOFade(0f, 0.25f).OnComplete(() =>
            {
                MainScreenParent.interactable = false;
                MainScreenParent.blocksRaycasts = false;
            }));
        }
    }
}
