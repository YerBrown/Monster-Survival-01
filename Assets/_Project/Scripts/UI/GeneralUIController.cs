using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GeneralUIController : MonoBehaviour
{
    public static GeneralUIController _instance;
    public static GeneralUIController Instance { get { return _instance; } }

    public bool MenuOpened = false;

    public Image BlackBackground;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void OpenMenu(bool enable)
    {
        MenuOpened = enable;
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
