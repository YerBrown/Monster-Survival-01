using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInventoryUpdatesController : MonoBehaviour
{
    public ItemSlotEventChannelSO OnItemAdded;
    public ItemSlotEventChannelSO OnItemRemoved;
    public List<UIItemUpdateController> UIItemUpdates;
    private int _currentSlot = 0;

    private void OnEnable()
    {
        OnItemAdded.OnEventRaised += UpdateAddedItem;
        OnItemRemoved.OnEventRaised += UpdateRemovedItem;
    }
    private void OnDisable()
    {
        OnItemAdded.OnEventRaised -= UpdateAddedItem;
        OnItemRemoved.OnEventRaised -= UpdateRemovedItem;
    }
    private void UpdateAddedItem(ItemSlot newItem)
    {
        UIItemUpdateController nextItemUpdate = GetNextSlot();
        if (nextItemUpdate != null)
        {
            nextItemUpdate.transform.SetAsFirstSibling();
            nextItemUpdate.ItemIcon.sprite = newItem.ItemInfo.i_Sprite;
            nextItemUpdate.ItemAmountFrame.color = Color.green;
            nextItemUpdate.ItemAmountText.text = $"+{newItem.Amount}";
            nextItemUpdate.gameObject.SetActive(true);
            nextItemUpdate.StartDisableCount(5);
        }
    }
    private void UpdateRemovedItem(ItemSlot newItem)
    {
        UIItemUpdateController nextItemUpdate = GetNextSlot();
        if (nextItemUpdate != null)
        {
            nextItemUpdate.transform.SetAsFirstSibling();
            nextItemUpdate.ItemIcon.sprite = newItem.ItemInfo.i_Sprite;
            nextItemUpdate.ItemAmountFrame .color = Color.red;
            nextItemUpdate.ItemAmountText.text = $"-{newItem.Amount}";
            nextItemUpdate.gameObject.SetActive(true);
            nextItemUpdate.StartDisableCount(5);
        }
    }
    private UIItemUpdateController GetNextSlot()
    {
        _currentSlot++;
        if (_currentSlot >= UIItemUpdates.Count)
        {
            _currentSlot = 0;
        }
        return UIItemUpdates[_currentSlot];
    }
}
