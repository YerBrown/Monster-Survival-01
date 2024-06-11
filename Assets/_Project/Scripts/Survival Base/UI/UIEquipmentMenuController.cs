using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentMenuController : MonoBehaviour
{
    public GameObject MenuParent;
    public GameObject EquipmentMenu;
    public GameObject InventoryMenu;
    public GameObject InventoryMenuButtonsLayout;
    public EquipmentPanelController EquipmentController;
    public UIInventoriesManagementController InventoriesTransferController;
    public UITrashController TrashController;
    public Button EquipmentButton;
    public Button InventoriesButton;
    private void Start()
    {
        InventoriesTransferController.Inventory_L.UI_Inventory = PlayerManager.Instance.P_Inventory;
        InventoriesTransferController.Inventory_R.UI_Inventory = SurvivalBaseStorageManager.Instance.StorageInventory;

        EquipmentController.EquipInventoryController.UI_Inventory = PlayerManager.Instance.P_Inventory;
    }
    public void OpenMenu()
    {
        MenuParent.SetActive(true);
        OpenEquipment();
    }
    public void CloseMenu()
    {
        MenuParent.SetActive(false);
    }
    public void OpenEquipment()
    {
        EquipmentMenu.SetActive(true);
        InventoryMenu.SetActive(false);
        InventoryMenuButtonsLayout.SetActive(false);

        EquipmentButton.transform.GetChild(1).gameObject.SetActive(true);
        InventoriesButton.transform.GetChild(1).gameObject.SetActive(false);
        
    }
    public void OpenInventories()
    {
        EquipmentMenu.SetActive(false);
        InventoryMenu.SetActive(true);
        InventoryMenuButtonsLayout.SetActive(true);

        EquipmentButton.transform.GetChild(1).gameObject.SetActive(false);
        InventoriesButton.transform.GetChild(1).gameObject.SetActive(true);

    }
}
