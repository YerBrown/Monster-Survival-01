using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using MonsterSurvival.Data;
public class SurvivalBaseStorageManager : MonoSingleton<SurvivalBaseStorageManager>
{
    public Inventory StorageInventory;
    public PairInventoriesEventChannelSO OnOpenManageInventoriesMenu;
    public StorageData CurrentStorageData;
    public bool SaveData = true;
    private void OnEnable()
    {
        StorageInventory.OnItemAdded.OnEventRaised += SaveStorageInventoryData;
        StorageInventory.OnItemRemoved.OnEventRaised += SaveStorageInventoryData;
    }
    private void OnDisable()
    {
        StorageInventory.OnItemAdded.OnEventRaised -= SaveStorageInventoryData;
        StorageInventory.OnItemRemoved.OnEventRaised -= SaveStorageInventoryData;
    }
    private void Start()
    {
        LoadStorageInventoryData();
    }
    public int GetItemAmount(ItemsSO item)
    {
        return StorageInventory.GetAmountOfType(item);
    }
    public bool IsPosibleToConsume(List<ItemSlot> consumedItems)
    {
        bool isPosible = true;
        foreach (ItemSlot itemSlot in consumedItems)
        {
            if (StorageInventory.GetAmountOfType(itemSlot.ItemInfo) < itemSlot.Amount)
            {
                isPosible = false;
                Debug.LogWarning($"Not enough {itemSlot.ItemInfo.i_Name} to consume in the storage.");
            }
        }
        return isPosible;
    }
    public void ConsumeItems(List<ItemSlot> consumedItems)
    {
        foreach (ItemSlot itemSlot in consumedItems)
        {
            StorageInventory.RemoveItemOfType(itemSlot.ItemInfo, itemSlot.Amount);
        }
    }
    public void OpenPlayerInventory()
    {
        OnOpenManageInventoriesMenu.RaiseEvent(new PairInventories(PlayerManager.Instance.P_Inventory, StorageInventory));
    }

    public void SaveStorageInventoryData(ItemSlot changedItemSlot)
    {
        if (SaveData)
        {
            CurrentStorageData = new StorageData(StorageInventory);

            string storageDataJson = JsonUtility.ToJson(CurrentStorageData, true);

            string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string fileName = "storage_data.json";
            string completeRute = Path.Combine(folderPath, fileName);
            File.WriteAllText(completeRute, storageDataJson);
        }
    }
    public void LoadStorageInventoryData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        string fileName = "storage_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        if (Directory.Exists(folderPath) && File.Exists(completeRute))
        {
            string json = File.ReadAllText(completeRute);
            CurrentStorageData = JsonUtility.FromJson<StorageData>(json);
            AddStorageData(CurrentStorageData);
        }
    }
    public void DeleteStorageData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        string fileName = "storage_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        if (Directory.Exists(folderPath) && File.Exists(completeRute))
        {
            File.Delete(completeRute);
        }
    }
    private void AddStorageData(StorageData storageData)
    {
        StorageInventory.MaxSlots = storageData.MaxSlots;
        StorageInventory.Slots.Clear();
        foreach (var item in storageData.AllItems)
        {
            StorageInventory.Slots.Add(new ItemSlot(MainWikiManager.Instance.GetItemByID(item.ItemID), item.Amount));
        }
    }
}
