using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropedItemsContainerElement : ContainerElement
{
    public VoidEventChannelSO OnClosePopup;

    public void OnEnable()
    {
        OnClosePopup.OnEventRaised += CheckInventoryItems;
    }
    public  void OnDisable()
    {
        OnClosePopup.OnEventRaised -= CheckInventoryItems;
    }

    private void CheckInventoryItems()
    {
        if (ContainerInventory == null) return;
        if (ContainerInventory.Slots.Count > 0)
        {
            //nothing
        }
        else
        {
            EnableElement(false);
        }
    }

    public void AddNewItem(ItemSlot addedItem)
    {
        ContainerInventory.AddNewItem(addedItem, false);
    }
}
