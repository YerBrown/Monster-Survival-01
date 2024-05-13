using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerInventoryMenuController : PlayerInventoryManagementController
{
    public GameObject UIParent;
    public VoidEventChannelSO OnOpenPlayerInventory;

    private void OnEnable()
    {
        OnOpenPlayerInventory.OnEventRaised += OpenPopup;
    }
    private void OnDisable()
    {
        OnOpenPlayerInventory.OnEventRaised -= OpenPopup;
    }
    public override void OpenPopup()
    {
        base.OpenPopup();
        if (PlayerManager.Instance != null) UIPlayerInventory.UI_Inventory = PlayerManager.Instance.P_Inventory;
        UIParent.gameObject.SetActive(true);
        if (GeneralUIController.Instance != null)
        {
            GeneralUIController.Instance.OpenMenu(true);
        }
    }
    public override void ClosePopup()
    {
        base.OpenPopup();
        UIParent.gameObject.SetActive(false);
        if (GeneralUIController.Instance != null)
        {
            GeneralUIController.Instance.OpenMenu(false);
        }
    }
}
