using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemSlotController : MonoBehaviour, IPointerClickHandler
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
    public Image SelectedFrame;
    public Image FilterBackground;
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
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void EnableFilter(bool enable)
    {
        FilterBackground.gameObject.SetActive(enable);
    }
    public void EnableBack(bool enable)
    {
        if (enable)
        {
            SelectedFrame.color = Color.white;
        }
        else
        {
            SelectedFrame.color = Color.clear;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!FilterBackground.gameObject.activeSelf)
        {
            SelectItemSlot();
        }
    }
}

