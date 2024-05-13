using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SurvivalBaseStorageManager;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [Header("Player Manager")]
    public string Nickname;
    public int PlayerLevel;
    public FighterData P_Fighter;
    public Inventory P_Inventory;
    public FighterData[] Team = new FighterData[6];
    public CapturesController Captures;
    private void OnEnable()
    {
        P_Inventory.OnItemAdded.OnEventRaised += SavePlayerInventoryData;
        P_Inventory.OnItemRemoved.OnEventRaised += SavePlayerInventoryData;
    }
    private void OnDisable()
    {
        P_Inventory.OnItemAdded.OnEventRaised -= SavePlayerInventoryData;
        P_Inventory.OnItemRemoved.OnEventRaised -= SavePlayerInventoryData;
    }
    private void Start()
    {
        LoadPlayerInfo();
        LoadPlayerInventoryData();
    }
    private void LoadPlayerInfo()
    {
        for (int i = 0; i < Team.Length; i++)
        {
            if (Team[i].ID == P_Fighter.ID)
            {
                Team[i] = P_Fighter;

            }
            Team[i].Lvl = PlayerLevel;
        }
    }
    public void LoadCombatScene()
    {
        SceneManager.LoadScene("Combat Field");
    }
    public void LoadExpeditionScene()
    {
        SceneManager.LoadScene("Expedition");
    }
    public FighterData[] GetTeamCreatures()
    {
        return Team.Where(fighter => fighter.ID != P_Fighter.ID).ToArray();
    }
    public FighterData GetCreature(string id)
    {
        foreach (FighterData creature in Team)
        {
            if (creature.ID == id)
            {
                return creature;
            }
        }
        return null;
    }

    public void SavePlayerInventoryData(ItemSlot changedItemSlot)
    {
        PlayerInventoryData playerInventoryData = new PlayerInventoryData(P_Inventory);

        string storageDataJson = JsonUtility.ToJson(playerInventoryData, true);

        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string fileName = "player_inventory_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        File.WriteAllText(completeRute, storageDataJson);
    }
    public void LoadPlayerInventoryData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        string fileName = "player_inventory_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        if (Directory.Exists(folderPath) && File.Exists(completeRute))
        {
            string json = File.ReadAllText(completeRute);
            PlayerInventoryData newData = JsonUtility.FromJson<PlayerInventoryData>(json);
            AddInventoryData(newData);
        }
    }

    public void DeletePlayerData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        string fileName = "player_inventory_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        if (Directory.Exists(folderPath) && File.Exists(completeRute))
        {
            File.Delete(completeRute);
        }
    }
    private void AddInventoryData(PlayerInventoryData inventoryData)
    {
        P_Inventory.MaxSlots = inventoryData.MaxSlots;
        P_Inventory.Slots.Clear();
        foreach (var item in inventoryData.AllItems)
        {
            P_Inventory.Slots.Add(new ItemSlot(MainWikiManager.Instance.GetItemByID(item.ItemID), item.Amount));
        }
    }
    [Serializable]
    public class PlayerInventoryData
    {
        public int MaxSlots;
        public List<ItemContData> AllItems = new List<ItemContData>();
        [Serializable]
        public class ItemContData
        {
            public string ItemID;
            public int Amount;
            public ItemContData(string itemID, int amount)
            {
                ItemID = itemID;
                Amount = amount;
            }
        }

        public PlayerInventoryData(Inventory inventory)
        {
            MaxSlots = inventory.MaxSlots;
            foreach (var slot in inventory.Slots)
            {
                Add(slot);
            }
        }

        public void Add(ItemSlot slot)
        {
            AllItems.Add(new ItemContData(slot.ItemInfo.i_Name, slot.Amount));
        }
    }
}