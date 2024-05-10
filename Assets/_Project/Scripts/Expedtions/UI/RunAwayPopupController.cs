using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayPopupController : MonoBehaviour
{
    public GameObject PopupParent;
    public VoidEventChannelSO OnRunAwayFromExpedition;
    public void OpenPopup()
    {
        if (GeneralUIController.Instance != null)
        {
            GeneralUIController.Instance.OpenMenu(true);
        }
        PopupParent.SetActive(true);
    }
    public void ClosePopup()
    {
        if (GeneralUIController.Instance != null)
        {
            GeneralUIController.Instance.OpenMenu(false);
        }
        PopupParent.SetActive(false);
    }
    public void ConfirmRunAway()
    {
        OnRunAwayFromExpedition.RaiseEvent();
    }
}
