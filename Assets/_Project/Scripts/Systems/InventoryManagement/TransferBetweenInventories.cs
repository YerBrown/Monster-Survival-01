using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferBetweenInventories
{
    public static void TransferItem(Inventory sourceInventory, Inventory targetInventory, ItemsSO itemType, int amount)
    {
        int amountNotTrasfered = targetInventory.AddNewItem(new ItemSlot(itemType, amount));
        if (amountNotTrasfered != amount)
        {
            sourceInventory.RemoveItemOfType(itemType, amount - amountNotTrasfered);
            if (amountNotTrasfered > 0)
            {
                Debug.Log($"Transfer completed partially, {amountNotTrasfered} remaining ");
            }
            else
            {
                Debug.Log("Transfer completed");
            }
        }
        else
        {
            Debug.Log("Transfer not completed");
        }
    }
}
