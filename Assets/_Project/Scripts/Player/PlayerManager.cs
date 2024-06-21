using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using MonsterSurvival.Data;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [Header("Player Manager")]
    public string Nickname;
    public int PlayerLevel;
    public FighterData P_Fighter;
    public BasicStats StatsWithoutEquipment;
    public Inventory P_Inventory;
    public EquipableItemSO WeaponEquiped;
    public EquipableItemSO ArmorEquiped;
    public List<FighterData> Team = new();
    public CapturesController Captures;
    public bool SaveData = true;
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
        LoadPlayerTeamData();
        LoadPlayerInventoryData();
        LoadPlayerInfo();
    }
    private void LoadPlayerInfo()
    {
        for (int i = 0; i < Team.Count; i++)
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
    public void AddTeamCreature(int index, FighterData fighter)
    {
        Team[index] = fighter;
        SavePlayerTeamData();
    }
    public void RemoveTeamCreature(int index)
    {
        Team.RemoveAt(index);
        while (Team.Count < GeneralValues.StaticCombatGeneralValues.Team_Max_Fighters)
        {
            Team.Add(new FighterData());
        }
        SavePlayerTeamData();
    }
    public void SwipeTeamOrder(int firstIndex, int secondIndex)
    {
        FighterData firstFighter = Team[firstIndex];
        FighterData secondFighter = Team[secondIndex];
        Team[secondIndex] = firstFighter;
        Team[firstIndex] = secondFighter;
        SavePlayerTeamData();
    }
    public int GetEmptyTeamSlotIndex()
    {
        for (int i = 0; i < Team.Count; i++)
        {
            if (Team[i] == null || string.IsNullOrEmpty(Team[i].ID))
            {
                return i;
            }
        }
        return -1;
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
    public BasicStats GetPlayerTotalStats()
    {
        BasicStats playerStatsWithEquipment = new(StatsWithoutEquipment);
        if (WeaponEquiped != null)
        {
            playerStatsWithEquipment.AddStats(WeaponEquiped.AddedStats);
        }
        if (ArmorEquiped != null)
        {
            playerStatsWithEquipment.AddStats(ArmorEquiped.AddedStats);
        }
        return playerStatsWithEquipment;
    }
    public (ElementType, ElementType) GetPlayerElements()
    {
        (ElementType, ElementType) playerElements = (ElementType.NO_TYPE, ElementType.NO_TYPE);
        if (ArmorEquiped != null)
        {
            playerElements.Item1 = ArmorEquiped.EquipmentElement;
        }
        if (WeaponEquiped != null)
        {
            playerElements.Item2 = WeaponEquiped.EquipmentElement;
        }
        return playerElements;
    }
    public void EquipArmor()
    {

    }
    public void UnequipArmor()
    {

    }
    public void EquipWeapon()
    {

    }
    public void UnequipWeapon()
    {

    }
    public void SavePlayerInventoryData(ItemSlot changedItemSlot)
    {
        if (SaveData)
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
    public void DeletePlayerInventoryData()
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
    public void SavePlayerTeamData()
    {
        if (SaveData)
        {
            PlayerTeamData playerTeamData = new PlayerTeamData(P_Fighter, ArmorEquiped, WeaponEquiped, Team);

            string playerTeamDataJson = JsonUtility.ToJson(playerTeamData, true);

            string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string fileName = "player_team_data.json";
            string completeRute = Path.Combine(folderPath, fileName);
            File.WriteAllText(completeRute, playerTeamDataJson);
        }
    }
    public void LoadPlayerTeamData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        string fileName = "player_team_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        if (Directory.Exists(folderPath) && File.Exists(completeRute))
        {
            string json = File.ReadAllText(completeRute);
            PlayerTeamData newData = JsonUtility.FromJson<PlayerTeamData>(json);
            AddTeamData(newData);
        }
    }
    public void DeletePlayerTeamData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        string fileName = "player_team_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        if (Directory.Exists(folderPath) && File.Exists(completeRute))
        {
            File.Delete(completeRute);
        }
    }
    private void AddTeamData(PlayerTeamData teamData)
    {
        P_Fighter = new FighterData(new Creature(teamData.PlayerData.FighterCreatureData));
        if (!string.IsNullOrEmpty(teamData.PlayerData.ArmorItemName))
        {
            ArmorEquiped = (EquipableItemSO)MainWikiManager.Instance.GetItemByID(teamData.PlayerData.ArmorItemName);
        }
        if (!string.IsNullOrEmpty(teamData.PlayerData.WeaponItemName))
        {
            WeaponEquiped = (EquipableItemSO)MainWikiManager.Instance.GetItemByID(teamData.PlayerData.WeaponItemName);
        }
        Team.Clear();
        foreach (var creatureData in teamData.TeamData)
        {
            Team.Add(new FighterData(new Creature(creatureData)));
        }
        while (Team.Count < GeneralValues.StaticCombatGeneralValues.Team_Max_Fighters)
        {
            Team.Add(new FighterData());
        }
    }
    public void LoadPlayerData()
    {
        LoadPlayerInventoryData();
        LoadPlayerTeamData();
    }
    public void DeletePlayerData()
    {
        DeletePlayerInventoryData();
        DeletePlayerTeamData();
    }
}