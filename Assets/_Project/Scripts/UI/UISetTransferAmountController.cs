using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISetTransferAmountController : MonoBehaviour
{
    private int _transferAmount;
    public int TransferAmount
    {
        get { return _transferAmount; }
        set
        {
            if (value != _transferAmount)
            {
                _transferAmount = CheckAmountMax(value);
                TransferAmountField.text = _transferAmount.ToString();
            }
        }
    }
    public TMP_Text TransferAmountField;
    public ItemSlot ItemToTransfer;
    public Image ItemIcon;
    public TMP_Text CurrentAmountText;
    private void OnEnable()
    {
        if (ItemToTransfer.ItemInfo != null)
        {
            ItemIcon.sprite = ItemsRelatedUtilities.CheckItemIcon(ItemToTransfer.ItemInfo);
            CurrentAmountText.text = $"x{ItemToTransfer.Amount}";
        }
    }
    private int CheckAmountMax(int value)
    {
        if (value > ItemToTransfer.Amount)
        {
            return ItemToTransfer.Amount;
        }
        else
        {
            return value;
        }
    }
    public void AddNumber(int number)
    {
        string newNumberString = TransferAmount.ToString() + number.ToString();
        int newNumber = int.Parse(newNumberString);
        TransferAmount = newNumber;
    }
    public void RemoveLastNumber()
    {
        string newNumberString = TransferAmount.ToString();
        if (newNumberString.Length > 1)
        {
            newNumberString = newNumberString.Substring(0, newNumberString.Length - 1);
        }
        else
        {
            newNumberString = "0";
        }
        int newNumber = int.Parse(newNumberString);
        TransferAmount = newNumber;
    }
    public void ResetNumber()
    {
        TransferAmount = 0;
    }
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
