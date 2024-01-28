using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SetAmountController : MonoBehaviour
{

    [SerializeField] private int _amount;
    public int Amount
    {
        get { return _amount; }
        set
        {
            if (value != _amount)
            {
                _amount = CheckAmountMax(value);
                TransferAmountField.text = _amount.ToString();
                ConfirmButton.interactable = _amount > 0;
            }
        }
    }
    [Header("Item Info")]
    public ItemsSO ItemToTransfer;
    public int CurrentAmount;
    [Header("UI Elements")]
    public Image ItemIcon;
    public TMP_Text CurrentAmountText;
    public TMP_Text TransferAmountField;
    public Button ConfirmButton;
    protected virtual void OnEnable()
    {
        if (ItemToTransfer != null)
        {
            ItemIcon.sprite = ItemsRelatedUtilities.CheckItemIcon(ItemToTransfer);
            CurrentAmountText.text = $"x{CurrentAmount}";
        }
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
        string newNumberString = Amount.ToString() + number.ToString();
        int newNumber = int.Parse(newNumberString);
        Amount = newNumber;
    }
    public void RemoveLastNumber()
    {
        string newNumberString = Amount.ToString();
        if (newNumberString.Length > 1)
        {
            newNumberString = newNumberString.Substring(0, newNumberString.Length - 1);
        }
        else
        {
            newNumberString = "0";
        }
        int newNumber = int.Parse(newNumberString);
        Amount = newNumber;
    }
    public void ResetNumber()
    {
        Amount = 0;
    }
}
