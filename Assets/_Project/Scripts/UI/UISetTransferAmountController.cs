using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISetTransferAmountController : MonoBehaviour
{
    public StashManagementController InventoryManagmentController;
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
                TransferAmountButton.interactable = _transferAmount > 0;
            }
        }
    }
    public ItemsSO ItemToTransfer;
    public int CurrentAmount;
    public Image ItemIcon;
    public TMP_Text CurrentAmountText;
    public TMP_Text TransferAmountField;
    public Button TransferAmountButton;
    private void OnEnable()
    {
        if (ItemToTransfer != null)
        {
            ItemIcon.sprite = ItemsRelatedUtilities.CheckItemIcon(ItemToTransfer);
            CurrentAmountText.text = $"x{CurrentAmount}";
        }
        TransferAmountButton.onClick.AddListener(TransferAmountSet);
    }
    private int CheckAmountMax(int value)
    {
        if (value > CurrentAmount)
        {
            return CurrentAmount;
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
    public void TransferAmountSet()
    {
        InventoryManagmentController.TransferPart(TransferAmount);
        ResetNumber();
        ClosePopup();
    }
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
