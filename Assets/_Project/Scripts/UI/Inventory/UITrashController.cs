using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITrashController : UI_SetAmountController
{
    private static UITrashController _instance;
    public static UITrashController Instance { get { return _instance; } }

    public UIInventoriesManagementController InventoriesController;
    public PlayerInventoryManagementController PlayerInventoryController;
    [Header("+ UI Elements")]
    public Button RemoveAllButton;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
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
        if (InventoriesController != null) InventoriesController.RemoveItem(Amount);
        if (PlayerInventoryController != null) PlayerInventoryController.RemoveItem(Amount);
        ClosePopup();
    }
    public void RemoveAll()
    {
        if (InventoriesController != null) InventoriesController.RemoveItem(CurrentAmount);
        if (PlayerInventoryController != null) PlayerInventoryController.RemoveItem(CurrentAmount);
        ClosePopup();
    }
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
