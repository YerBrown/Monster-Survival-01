using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class TransferItemsHUDController : MonoBehaviour
{
    public UIInventoriesManagementController InventoriesManagementController;

    public PairInventoriesEventChannelSO OnOpenManageInventoriesMenu;
    private void OnEnable()
    {
        if (OnOpenManageInventoriesMenu != null)
            OnOpenManageInventoriesMenu.OnEventRaised += OpenMenu;
    }
    private void OnDisable()
    {
        if (OnOpenManageInventoriesMenu != null)
            OnOpenManageInventoriesMenu.OnEventRaised -= OpenMenu;
    }
    public void OpenMenu(PairInventories pairInventories)
    {
        InventoriesManagementController.Inventory1.UI_Inventory = pairInventories.LeftInventory;
        InventoriesManagementController.Inventory2.UI_Inventory = pairInventories.RightInventory;
        InventoriesManagementController.gameObject.SetActive(true);
    }
}
[Serializable]
public class PairInventories
{
    public Inventory LeftInventory;
    public Inventory RightInventory;

    public PairInventories()
    {
    }

    public PairInventories(Inventory leftInv, Inventory righInv)
    {
        LeftInventory = leftInv;
        RightInventory = righInv;
    }
}
