using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISetTransferAmountController : UI_SetAmountController
{
    public UIInventoriesManagementController InventoriesManagmentController;
    protected override void OnEnable()
    {
        base.OnEnable();
        ConfirmButton.onClick.AddListener(TransferAmountSet);
    }
    private void OnDisable()
    {
        ConfirmButton.onClick.RemoveListener(TransferAmountSet);
    }
    public void TransferAmountSet()
    {
        InventoriesManagmentController.TransferPart(Amount);
        ResetNumber();
        ClosePopup();
    }
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
