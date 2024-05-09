using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuildingMenuController : MonoBehaviour
{
    public Sequence EnableActionButtonsSequence;
    [Header("UI")]
    public CanvasGroup ActionButtonsParent;

    public void EnableActionButtons()
    {
        if (EnableActionButtonsSequence != null)
        {
            EnableActionButtonsSequence.Kill();
        }
        EnableActionButtonsSequence.Append(ActionButtonsParent.DOFade(1f, 0.25f));
    }
    public void DisableActionButtons()
    {
        if (EnableActionButtonsSequence != null)
        {
            EnableActionButtonsSequence.Kill();
        }
        EnableActionButtonsSequence.Append(ActionButtonsParent.DOFade(0f, 0.25f));
    }
}
