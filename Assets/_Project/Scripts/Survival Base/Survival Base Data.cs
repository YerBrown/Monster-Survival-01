using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalBaseData : MonoBehaviour
{
    public List<BuildingData> BuildingsData = new List<BuildingData>();

    public class BuildingData
    {
        public string Area_ID;
        public string Building_ID;
        public int Level;
        public bool InProgress;
        public string StartBuildingDateTime;
        public string FinishBuildingDateTime;
    }
}
