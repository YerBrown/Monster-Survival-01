using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITrashController : UI_SetAmountController
{
    public UIInventoriesManagementController InventoriesController;
    [Header("+ UI Elements")]
    public Button RemoveAllButton;
    protected override void OnEnable()
    {
        base.OnEnable();
        ConfirmButton.onClick.AddListener(ConfirmRemove);
        RemoveAllButton.onClick.AddListener(RemoveAll);
    }
    private void OnDisable()
    {
        ConfirmButton.onClick.RemoveListener(ConfirmRemove);
        RemoveAllButton.onClick.RemoveListener(RemoveAll);
    }
    public void ConfirmRemove()
    {
        InventoriesController.RemoveItem(Amount);
        ClosePopup();
    }
    public void RemoveAll()
    {
        InventoriesController.RemoveItem(CurrentAmount);
        ClosePopup();
    }
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
