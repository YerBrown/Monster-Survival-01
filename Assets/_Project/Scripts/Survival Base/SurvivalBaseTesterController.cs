using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalBaseTesterController : MonoBehaviour
{
    public PairInventoriesEventChannelSO OnOpenInventoriesMenu;
    public PairInventories TestInventories;
    private void OnGUI()
    {
        if (GUILayout.Button("Test Open Inventory Management"))
            OnOpenInventoriesMenu.RaiseEvent(TestInventories);
    }

}
