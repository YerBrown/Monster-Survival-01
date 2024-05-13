using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPanelController : PlayerInventoryManagementController
{
    private void OnEnable()
    {
        OpenPopup();
    }
    private void OnDisable()
    {
        ClosePopup();
    }
    public override void OpenPopup()
    {
        base.OpenPopup();
    }
    public override void ClosePopup() 
    { 
        base.ClosePopup();

    }
}
