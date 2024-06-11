using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanelController : MonoBehaviour
{
    public UIEquipmentInventoryController EquipInventoryController;
    private EquipableItemSO _currentSelectedItem;
    public EquipableItemSO CurrentSelectedItem
    {
        get { return _currentSelectedItem; }
        set
        {
            if (value != null)
            {
                CurrentSelectedEquipedItem = null;
            }
            _currentSelectedItem = value;
            EnableEquipButton(value != null);
            UpdateItemsScrollView(value != null);
        }
    }
    private EquipableItemSO _currentSelectedEquipedItem;
    public EquipableItemSO CurrentSelectedEquipedItem
    {
        get { return _currentSelectedEquipedItem; }
        set
        {
            if (value != null)
            {
                CurrentSelectedItem = null;
            }
            _currentSelectedEquipedItem = value;
            EnableEquipButton(value != null);
            EnableEquipedLayout(value != null);
        }
    }
    [Header("UI")]
    // Player stats
    public TMP_Text HealthText;
    public TMP_Text EnergyText;
    public TMP_Text FisicalPowerText;
    public TMP_Text RangePowerText;
    public TMP_Text DefenseText;
    public TMP_Text SpeedText;

    public TMP_Text HealthChangeText;
    public TMP_Text EnergyChangeText;
    public TMP_Text FisicalPowerChangeText;
    public TMP_Text RangePowerChangeText;
    public TMP_Text DefenseChangeText;
    public TMP_Text SpeedChangeText;
    public Image AttackElementChangeImage;
    public Image DefenseElementChangeImage;

    public Color IncreasedStatColor;
    public Color DecreasedStatColor;
    // Weapon and Armor equiped
    public Image WeaponEquipedImage;
    public Image WeaponSelectedImage;
    public Image WeaponElementImage;
    public Image ArmorEquipedImage;
    public Image ArmorSelectedImage;
    public Image ArmorElementImage;
    public TMP_Text WeaponStatsText;
    public TMP_Text ArmorStatsText;
    public Button WeaponEquipedButton;
    public Button ArmorEquipedButton;
    public Color SelectedEquipmentColor;
    public Color UnselectedEquipmentColor;
    // Items in inventory
    public Button EquipButton;
    public Action EquipLogic;
    // Filter buttons
    public List<Button> FilterButtons = new();
    public Color SelectedFilterColor;
    public Color NotSelectedFilterColor;
    private void OnEnable()
    {
        EquipInventoryController.ItemSlotSelected.AddListener(SelectItemSlot);
        WeaponEquipedButton.onClick.AddListener(SelectWeaponEquiped);
        ArmorEquipedButton.onClick.AddListener(SelectArmorEquiped);
        UpdateCurrentStats();
        CurrentSelectedItem = null;
        CurrentSelectedEquipedItem = null;
    }
    private void OnDisable()
    {
        EquipInventoryController.ItemSlotSelected.RemoveListener(SelectItemSlot);
        WeaponEquipedButton.onClick.RemoveListener(SelectWeaponEquiped);
        ArmorEquipedButton.onClick.RemoveListener(SelectArmorEquiped);
    }

    private void SelectItemSlot(UIInventoryController uiInventory, ItemsSO itemType)
    {
        EquipableItemSO selectedItem = itemType as EquipableItemSO;
        if (selectedItem != null)
        {
            if (selectedItem != CurrentSelectedItem)
            {
                CurrentSelectedItem = selectedItem;
            }
            else
            {
                CurrentSelectedItem = null;
            }
        }

    }
    private void UpdateItemsScrollView(bool enable)
    {
        DisableChangeTexts();
        if (enable)
        {
            UpdatePlayerStats(CurrentSelectedItem.EquipmentType, true);
            EquipInventoryController.SetSelectedUI(CurrentSelectedItem, true);
            EquipButton.GetComponentInChildren<TMP_Text>().text = "Equip";
            if (CurrentSelectedItem.EquipmentType == EquipType.WEAPON)
            {
                EquipLogic = () => EquipWeapon();
            }
            else
            {
                EquipLogic = () => EquipArmor();
            }
        }
        else
        {
            EquipInventoryController.SetSelectedUI(null, false);
        }
    }
    public void EnableEquipButton(bool enable)
    {
        EquipButton.interactable = enable;
    }
    public void EnableEquipedLayout(bool enable)
    {
        if (enable)
        {
            if (CurrentSelectedEquipedItem.EquipmentType == EquipType.WEAPON)
            {
                WeaponSelectedImage.color = SelectedEquipmentColor;
                ArmorSelectedImage.color = UnselectedEquipmentColor;
            }
            else
            {
                WeaponSelectedImage.color = UnselectedEquipmentColor;
                ArmorSelectedImage.color = SelectedEquipmentColor;
            }
        }
        else
        {
            WeaponSelectedImage.color = UnselectedEquipmentColor;
            ArmorSelectedImage.color = UnselectedEquipmentColor;
        }
    }
    public void SelectWeaponEquiped()
    {
        DisableChangeTexts();
        ArmorSelectedImage.color = UnselectedEquipmentColor;
        if (CurrentSelectedItem != null)
        {
            SelectItemSlot(null, CurrentSelectedItem);
            CurrentSelectedItem = null;
        }
        if (CurrentSelectedEquipedItem == PlayerManager.Instance.WeaponEquiped)
        {
            CurrentSelectedEquipedItem = null;
            WeaponSelectedImage.color = UnselectedEquipmentColor;
        }
        else
        {
            WeaponSelectedImage.color = SelectedEquipmentColor;
            CurrentSelectedEquipedItem = PlayerManager.Instance.WeaponEquiped;
            EquipLogic = () => UnequipWeapon();
            EquipButton.GetComponentInChildren<TMP_Text>().text = "Unequip";
            UpdatePlayerStats(EquipType.WEAPON, false);
        }
    }
    public void SelectArmorEquiped()
    {
        DisableChangeTexts();
        WeaponSelectedImage.color = UnselectedEquipmentColor;
        if (CurrentSelectedItem != null)
        {
            SelectItemSlot(null, CurrentSelectedItem);
            CurrentSelectedItem = null;
        }
        if (CurrentSelectedEquipedItem == PlayerManager.Instance.ArmorEquiped)
        {
            CurrentSelectedEquipedItem = null;
            DisableChangeTexts();
        }
        else
        {
            ArmorSelectedImage.color = SelectedEquipmentColor;
            CurrentSelectedEquipedItem = PlayerManager.Instance.ArmorEquiped;
            EquipLogic = () => UnequipArmor();
            EquipButton.GetComponentInChildren<TMP_Text>().text = "Unequip";
            UpdatePlayerStats(EquipType.ARMOR, false);
        }
    }
    private void UpdatePlayerStats(EquipType equipType, bool notEquipedItem)
    {
        BasicStats totalStats = new();

        switch (equipType)
        {
            case EquipType.NONE:
                break;
            case EquipType.WEAPON:
                if (PlayerManager.Instance.WeaponEquiped != null)
                {
                    totalStats.RemoveStats(PlayerManager.Instance.WeaponEquiped.AddedStats, false);
                }
                break;
            case EquipType.ARMOR:
                if (PlayerManager.Instance.ArmorEquiped != null)
                {
                    totalStats.RemoveStats(PlayerManager.Instance.ArmorEquiped.AddedStats, false);
                }
                break;
            default:
                break;
        }
        if (notEquipedItem)
        {
            totalStats.AddStats(CurrentSelectedItem.AddedStats, false);
            if (CurrentSelectedItem.EquipmentType == EquipType.WEAPON)
            {
                AttackElementChangeImage.sprite = MainWikiManager.Instance.GetElementSprite(CurrentSelectedItem.EquipmentElement);
                AttackElementChangeImage.gameObject.SetActive(true);
            }
            else
            {
                DefenseElementChangeImage.sprite = MainWikiManager.Instance.GetElementSprite(CurrentSelectedItem.EquipmentElement);
                DefenseElementChangeImage.gameObject.SetActive(true);
            }
        }
        if (totalStats.MaxHealthPoints != 0)
        {
            UpdateChangeText(HealthChangeText, totalStats.MaxHealthPoints);
        }
        if (totalStats.MaxEnergyPoints != 0)
        {
            UpdateChangeText(EnergyChangeText, totalStats.MaxEnergyPoints);
        }
        if (totalStats.HitPower != 0)
        {
            UpdateChangeText(FisicalPowerChangeText, totalStats.HitPower);
        }
        if (totalStats.RangePower != 0)
        {
            UpdateChangeText(RangePowerChangeText, totalStats.RangePower);
        }
        if (totalStats.Defense != 0)
        {
            UpdateChangeText(DefenseChangeText, totalStats.Defense);
        }
        if (totalStats.Speed != 0)
        {
            UpdateChangeText(SpeedChangeText, totalStats.Speed);
        }
        
    }
    private void UpdateChangeText(TMP_Text text, int statValue)
    {
        string addedStat;
        if (statValue > 0)
        {
            addedStat = "+" + statValue.ToString();
            text.color = IncreasedStatColor;
        }
        else
        {
            addedStat = statValue.ToString();
            text.color = DecreasedStatColor;
        }
        text.text = addedStat;
        text.gameObject.SetActive(true);
    }
    private void DisableChangeTexts()
    {
        HealthChangeText.gameObject.SetActive(false);
        EnergyChangeText.gameObject.SetActive(false);
        FisicalPowerChangeText.gameObject.SetActive(false);
        RangePowerChangeText.gameObject.SetActive(false);
        DefenseChangeText.gameObject.SetActive(false);
        SpeedChangeText.gameObject.SetActive(false);
        AttackElementChangeImage.gameObject.SetActive(false);
        DefenseElementChangeImage.gameObject.SetActive(false);
    }
    private void UpdateCurrentStats()
    {
        DisableChangeTexts();
        UpdateEquipedSlots();
        BasicStats playerStats = new(PlayerManager.Instance.GetPlayerTotalStats());
        HealthText.text = $"HP {playerStats.MaxHealthPoints}";
        EnergyText.text = $"EP {playerStats.MaxEnergyPoints}";
        FisicalPowerText.text = $"AP {playerStats.HitPower}";
        RangePowerText.text = $"RP {playerStats.RangePower}";
        DefenseChangeText.text = $"DF {playerStats.Defense}";
        SpeedText.text = $"SP {playerStats.Speed}";
    }
    public void UpdateEquipedSlots()
    {
        if (PlayerManager.Instance.WeaponEquiped != null)
        {
            WeaponEquipedImage.sprite = PlayerManager.Instance.WeaponEquiped.i_Sprite;
            WeaponElementImage.sprite = MainWikiManager.Instance.GetElementSprite(PlayerManager.Instance.WeaponEquiped.EquipmentElement);
            WeaponEquipedImage.enabled = true;
        }
        else
        {
            WeaponEquipedImage.sprite = null;
            WeaponElementImage.sprite = MainWikiManager.Instance.GetElementSprite(ElementType.NO_TYPE);
            WeaponEquipedImage.enabled = false;
        }
        if (PlayerManager.Instance.ArmorEquiped != null)
        {
            ArmorEquipedImage.sprite = PlayerManager.Instance.ArmorEquiped.i_Sprite;
            ArmorElementImage.sprite = MainWikiManager.Instance.GetElementSprite(PlayerManager.Instance.ArmorEquiped.EquipmentElement);
            ArmorEquipedImage.enabled = true;
        }
        else
        {
            ArmorEquipedImage.sprite = null;
            ArmorElementImage.sprite = MainWikiManager.Instance.GetElementSprite(ElementType.NO_TYPE);
            ArmorEquipedImage.enabled = false;
        }
    }
    public void Equip()
    {
        EquipLogic.Invoke();
    }
    private void EquipWeapon()
    {
        EquipInventoryController.RemoveItemFromInventory(CurrentSelectedItem, 1);
        if (PlayerManager.Instance.WeaponEquiped != null)
        {
            int remainingToAdd = EquipInventoryController.AddItemToInventory(new ItemSlot(PlayerManager.Instance.WeaponEquiped, 1));
        }
        PlayerManager.Instance.WeaponEquiped = CurrentSelectedItem;
        UpdateCurrentStats();
        CurrentSelectedItem = null;
    }
    private void UnequipWeapon()
    {
        int remainingToAdd = EquipInventoryController.AddItemToInventory(new ItemSlot(PlayerManager.Instance.WeaponEquiped, 1));
        if (remainingToAdd > 0)
        {
            int remainingStorageToAdd = SurvivalBaseStorageManager.Instance.StorageInventory.AddNewItem(new ItemSlot(PlayerManager.Instance.WeaponEquiped, 1));
            if (remainingStorageToAdd > 0)
            {
                Debug.Log("Not enough space in the storage and in player backpack");
            }
            else
            {
                PlayerManager.Instance.WeaponEquiped = null;
                Debug.Log("Not enough space in the backpack, so item added to storage");
            }
        }
        else
        {
            PlayerManager.Instance.WeaponEquiped = null;
        }
        if (PlayerManager.Instance.WeaponEquiped == null)
        {
            UpdateCurrentStats();
            CurrentSelectedEquipedItem = null;
        }
    }
    private void EquipArmor()
    {
        EquipInventoryController.RemoveItemFromInventory(CurrentSelectedItem, 1);
        if (PlayerManager.Instance.ArmorEquiped != null)
        {
            int remainingToAdd = EquipInventoryController.AddItemToInventory(new ItemSlot(PlayerManager.Instance.ArmorEquiped, 1));
            //if (remainingToAdd > 0)
            //{
            //    // TODO: Send not enough space notification
            //    Debug.Log("you dont have enough space in the inventory to store your current item")
            //    return;
            //}

        }
        PlayerManager.Instance.ArmorEquiped = CurrentSelectedItem;
        UpdateCurrentStats();
        CurrentSelectedItem = null;
    }
    private void UnequipArmor()
    {
        int remainingToAdd = EquipInventoryController.AddItemToInventory(new ItemSlot(PlayerManager.Instance.ArmorEquiped, 1));
        if (remainingToAdd > 0)
        {
            int remainingStorageToAdd = SurvivalBaseStorageManager.Instance.StorageInventory.AddNewItem(new ItemSlot(PlayerManager.Instance.ArmorEquiped, 1));
            if (remainingStorageToAdd > 0)
            {
                Debug.Log("Not enough space in the storage and in player backpack");
            }
            else
            {
                PlayerManager.Instance.ArmorEquiped = null;
                Debug.Log("Not enough space in the backpack, so item added to storage");
            }
        }
        else
        {
            PlayerManager.Instance.ArmorEquiped = null;
        }
        if (PlayerManager.Instance.ArmorEquiped == null)
        {
            UpdateCurrentStats();
            CurrentSelectedEquipedItem = null;
        }
    }
    public void SetEquipFilter(int index)
    {
        EquipInventoryController.SetFilter((EquipType)index);
        for (int i = 0; i < FilterButtons.Count; i++)
        {
            if (index == i)
            {
                FilterButtons[i].targetGraphic.color = SelectedFilterColor;
                if (FilterButtons[i].transform.GetChild(0).GetComponent<TMP_Text>() != null)
                {
                    FilterButtons[i].transform.GetChild(0).GetComponent<TMP_Text>().color = NotSelectedFilterColor;

                }
                else if (FilterButtons[i].transform.GetChild(0).GetComponent<Image>() != null)
                {
                    FilterButtons[i].transform.GetChild(0).GetComponent<Image>().color = NotSelectedFilterColor;
                }

            }
            else
            {
                FilterButtons[i].targetGraphic.color = NotSelectedFilterColor;
                if (FilterButtons[i].transform.GetChild(0).GetComponent<TMP_Text>() != null)
                {
                    FilterButtons[i].transform.GetChild(0).GetComponent<TMP_Text>().color = SelectedFilterColor;

                }
                else if (FilterButtons[i].transform.GetChild(0).GetComponent<Image>() != null)
                {
                    FilterButtons[i].transform.GetChild(0).GetComponent<Image>().color = SelectedFilterColor;
                }
            }
        }

    }
}
