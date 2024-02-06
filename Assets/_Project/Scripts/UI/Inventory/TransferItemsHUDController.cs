using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferItemsHUDController : MonoBehaviour
{
    public UIInventoriesManagementController InventoriesManagementController;
    public Inventory Test1;
    public Inventory Test2;
    public void OpenMenu(Inventory inventoryL, Inventory inventoryR)
    {
        InventoriesManagementController.Inventory1.UI_Inventory = inventoryL;
        InventoriesManagementController.Inventory2.UI_Inventory = inventoryR;
        InventoriesManagementController.gameObject.SetActive(true);
    }

    public void OpenTestMenu()
    {
        OpenMenu(Test1, Test2);
    }
}
