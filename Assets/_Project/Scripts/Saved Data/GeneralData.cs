using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MonsterSurvival.Data
{
    [Serializable]
    public class CreatureData
    {
        public string Creature_ID;
        public string Specie_ID;
        public string Nickname;

        public int Lvl;
        public int MaxHealthPoints;
        public int MaxEnergyPoints;
        public int FisicalPower;
        public int RangePower;
        public int Defense;
        public int Speed;

        public int HealthPoints;
        public int EnergyPoints;
        public CreatureData(Creature creature)
        {
            Creature_ID = creature.ID;
            Specie_ID = creature.CreatureInfo.c_Name;
            Nickname = creature.Nickname;
            Lvl = creature.Lvl;
            MaxHealthPoints = creature.MaxHealthPoints;
            MaxEnergyPoints = creature.MaxEnergyPoints;
            FisicalPower = creature.FisicalPower;
            RangePower = creature.RangePower;
            Defense = creature.Defense;
            Speed = creature.Speed;
            HealthPoints = creature.HealthPoints;
            EnergyPoints = creature.EnergyPoints;
        }
    }
    [Serializable]
    public class SurvivalBaseData
    {
        public List<BuildingData> BuildingsData = new List<BuildingData>();
        //public List<WorkersData> CorstructorsData = new List<WorkersData>();
        public void UpdateBuildAreaData(BuildAreaController buildAreaController)
        {
            foreach (var data in BuildingsData)
            {
                if (!string.IsNullOrEmpty(data.Area_ID) && data.Area_ID == buildAreaController.ID)
                {
                    data.UpdateData(buildAreaController);
                    return;
                }
            }
            // In case of no having any information in saved data
            BuildingData newBuildAreaInfo = new();
            newBuildAreaInfo.UpdateData(buildAreaController);
            BuildingsData.Add(newBuildAreaInfo);
        }
        //public void UpdateConstructorData(WorkerCreature worker)
        //{
        //    foreach (var data in CorstructorsData)
        //    {
        //        if (!string.IsNullOrEmpty(data.Worker_ID) && data.Worker_ID == worker.Data.ID)
        //        {
        //            data.UpdateData(worker);
        //            return;
        //        }
        //    }
        //    // In case of no having any information in saved data
        //    WorkersData newWorkerInfo = new();
        //    newWorkerInfo.UpdateData(worker);
        //    CorstructorsData.Add(newWorkerInfo);
        //}
    }
    [Serializable]
    public class BuildingData
    {
        public string Area_ID;
        public string Building_ID;
        public string Worker_ID;
        public int Level = 0;
        public bool InProgress = false;
        public string StartBuildingDateTime;
        public string FinishBuildingDateTime;
        public BuildingData()
        {

        }
        public BuildingData(string areaID, string buildingID, string workerID, int level, bool inPorgress, DateTime startBuildingTime, DateTime finishBuildingTime)
        {
            Area_ID = areaID;
            Building_ID = buildingID;
            Worker_ID = workerID;
            Level = level;
            InProgress = inPorgress;
            StartBuildingDateTime = startBuildingTime.ToString("o");
            FinishBuildingDateTime = finishBuildingTime.ToString("o");
        }

        public void UpdateData(BuildAreaController buildArea)
        {
            Area_ID = buildArea.ID;
            if (buildArea.ChildBuildingController != null)
            {
                BuildingController buildingController = buildArea.ChildBuildingController;
                Building_ID = buildingController.BuildingInfo.Name;
                if (buildingController.ConstructionWorker != null)
                {
                    Worker_ID = buildingController.ConstructionWorker.Data.ID;

                }
                else
                {
                    Worker_ID = "";

                }
                Level = buildingController.Level;
                InProgress = buildingController.InProgress;
                StartBuildingDateTime = buildingController.StartBuildingDateTime.ToString("o");
                FinishBuildingDateTime = buildingController.FinishBuildingDateTime.ToString("o");
            }
        }
    }
    [Serializable]
    public class StorageData
    {
        public int MaxSlots;
        public List<ItemContData> AllItems = new List<ItemContData>();

        public StorageData(Inventory inventory)
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
    [Serializable]
    public class PlayerInventoryData
    {
        public int MaxSlots;
        public List<ItemContData> AllItems = new List<ItemContData>();


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
    [Serializable]
    public class BiomesData
    {
        public List<CreatureBiomeData> AllBiomes = new();
        public BiomesData(List<BiomesManager.CreatureBiome> allBiomes)
        {
            AllBiomes.Clear();
            foreach (var biome in allBiomes)
            {
                AllBiomes.Add(new CreatureBiomeData(biome));
            }
        }
        [Serializable]
        public class CreatureBiomeData
        {
            public string Biome_ID;
            public int Level;
            public List<CreatureData> Creatures = new();
            public CreatureBiomeData(BiomesManager.CreatureBiome creatureBiome)
            {
                Biome_ID = creatureBiome.BiomeInfo.Name;
                Level = creatureBiome.Level;
                AddCreatureList(creatureBiome.CreatureSlots);
            }
            public void AddCreatureList(List<Creature> allCreatures)
            {
                Creatures.Clear();
                foreach (Creature creature in allCreatures)
                {
                    Creatures.Add(new CreatureData(creature));
                }
            }
        }
    }
    [Serializable]
    public class PlayerTeamData
    {
        public PlayerFighterData PlayerData;
        public List<CreatureData> TeamData = new();
        public PlayerTeamData(FighterData playerData, EquipableItemSO armor, EquipableItemSO weapon, List<FighterData> teamData)
        {
            PlayerData = new PlayerFighterData(playerData, armor, weapon);

            TeamData.Clear();
            foreach (var fighter in teamData)
            {
                if (!string.IsNullOrEmpty(fighter.TypeID))
                {
                    TeamData.Add(new CreatureData(new Creature(fighter)));
                }
            }
        }
    }
    [Serializable]
    public class PlayerFighterData
    {
        public CreatureData FighterCreatureData;
        public string ArmorItemName;
        public string WeaponItemName;

        public PlayerFighterData(FighterData playerData, EquipableItemSO armor, EquipableItemSO weapon)
        {
            FighterCreatureData = new CreatureData(new Creature(playerData));
            if (armor != null)
            {
                ArmorItemName = armor.i_Name;
            }
            else
            {
                ArmorItemName = "";
            }
            if (weapon != null)
            {
                WeaponItemName = weapon.i_Name;
            }
            else
            {
                WeaponItemName = "";
            }
        }
    }
}
