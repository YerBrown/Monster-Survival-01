using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using MonsterSurvival.Data;

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
    public FighterData[] Team = new FighterData[6];
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
}