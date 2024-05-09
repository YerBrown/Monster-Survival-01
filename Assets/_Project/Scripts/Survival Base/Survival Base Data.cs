using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
[Serializable]
public class SurvivalBaseData
{
    public List<BuildingData> BuildingsData = new List<BuildingData>();
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
    [Serializable]
    public class BuildingData
    {
        public string Area_ID;
        public string Building_ID;
        public int Level = 0;
        public bool InProgress = false;
        public string StartBuildingDateTime;
        public string FinishBuildingDateTime;
        public BuildingData()
        {

        }
        public BuildingData(string areaID, string buildingID, int level, bool inPorgress, DateTime startBuildingTime, DateTime finishBuildingTime)
        {
            Area_ID = areaID;
            Building_ID = buildingID;
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
                Level = buildingController.Level;
                InProgress = buildingController.InProgress;
                StartBuildingDateTime = buildingController.StartBuildingDateTime.ToString("o");
                FinishBuildingDateTime = buildingController.FinishBuildingDateTime.ToString("o");
            }
        }
    }
}
