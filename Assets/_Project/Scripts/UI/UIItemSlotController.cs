using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIItemSlotController : MonoBehaviour
{
    [SerializeField]
    private ItemSlot _Slot;
    public ItemSlot Slot
    {
        get { return _Slot; }
        set
        {
            if (value != _Slot)
            {
                _Slot = value;
                UpdateUI();
            }
        }
    }
    public UnityEvent<UIItemSlotController> SelectSlotEvent;

    public Image ItemImage;
    public TMP_Text AmountText;
    public Image ItemBack;
    public void SelectItemSlot()
    {
        SelectSlotEvent?.Invoke(this);
    }
    public void UpdateUI()
    {
        if (Slot != null)
        {
            ItemImage.sprite = ItemsRelatedUtilities.CheckItemIcon(Slot.ItemInfo);
            AmountText.text = $"x{Slot.Amount}";
        }
    }

    private void ChangeItemImage(ItemsSO itemInfo)
    {
        ItemImage.sprite = ItemsRelatedUtilities.CheckItemIcon(itemInfo);
    }
    private void ChangeAmountText(int newAmount)
    {
        AmountText.text = $"x{newAmount}";
    }

    public void EnableBack(bool enable)
    {
        if (enable)
        {
            ItemBack.color = Color.white;
        }
        else
        {
            ItemBack.color = Color.clear;
        }
    }
}

