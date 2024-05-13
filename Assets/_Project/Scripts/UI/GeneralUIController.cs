using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GeneralUIController : MonoSingleton<GeneralUIController>
{
    public BoolEventChannelSO OnOpenMenuPopup;
    public bool MenuOpened = false;

    public Image BlackBackground;
    private void Start()
    {
        BlackBackground.color = new Color(BlackBackground.color.r, BlackBackground.color.g, BlackBackground.color.b, 1f);
    }
    public void OpenMenu(bool enable)
    {
        MenuOpened = enable;
        OnOpenMenuPopup.RaiseEvent(MenuOpened);
    }

    public void EnableBlackBackground(bool enable)
    {
        if (enable)
        {
            OpenMenu(true);
            BlackBackground.DOFade(1, .15f).SetEase(Ease.OutSine);
        }
        else
        {
            BlackBackground.DOFade(0, 0.5f).SetEase(Ease.InQuint).OnComplete(() =>
            {
                OpenMenu(false);
            });

        }
    }
}
