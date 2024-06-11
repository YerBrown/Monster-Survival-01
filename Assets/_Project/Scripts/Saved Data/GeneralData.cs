using System;
using System.Collections;
using System.Collections.Generic;
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
