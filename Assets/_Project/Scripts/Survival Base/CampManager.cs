using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MonsterSurvival.Data;
public class CampManager : MonoSingleton<CampManager>
{
    public List<BuildAreaController> BuildAreas = new();
    public CameraController MainCameraController;
    public BuildAreaController SelectedArea;
    public BuildingController SelectedBuilding;


    public SurvivalBaseData CurrentSurvivalBaseData = new();
    public WorkersManager CampWorkers;
    public VoidEventChannelSO OnPointerClickedFloor;
    public bool SaveData = true;
    [Header("UI")]
    public UIBuildMenuController BuildMenuController;
    private void OnEnable()
    {
        OnPointerClickedFloor.OnEventRaised += UnselectBoth;

    }
    private void OnDisable()
    {
        OnPointerClickedFloor.OnEventRaised -= UnselectBoth;
    }
    private void Start()
    {
        StartCoroutine(InitializeSurvivalBase());
    }
    public void PlaceNewBuilding(BuildingSO newBuild, WorkerCreature worker)
    {
        BuildingController build = Instantiate(newBuild.Prefab, SelectedArea.transform).GetComponent<BuildingController>();
        build.transform.localPosition = Vector3.zero;
        SelectedArea.ChildBuildingController = build;
        build.ParentArea = SelectedArea;
        DateTime finishDateTime = RealDateTimeManager.Instance.GetCurrentDateTime() + new TimeSpan(newBuild.FinishTimes[0].Days, newBuild.FinishTimes[0].Hours, newBuild.FinishTimes[0].Minutes, newBuild.FinishTimes[0].Seconds);
        build.StartUpdate(finishDateTime, worker);
        SelectBuilding(build);
    }
    public void PlaceBuildingInAreaFromSavedData(BuildAreaController area, BuildingData buildingData)
    {
        BuildingSO buildInfo = MainWikiManager.Instance.GetBuildingByID(buildingData.Building_ID);
        BuildingController build = Instantiate(buildInfo.Prefab, area.transform).GetComponent<BuildingController>();
        build.transform.localPosition = Vector3.zero;
        area.ChildBuildingController = build;
        build.ParentArea = area;
        build.CheckCurrentBuildingState(buildingData);
    }
    public void SelectBuildArea(BuildAreaController selectedArea)
    {
        if (SelectedArea != selectedArea)
        {
            UnselectBoth();
            SelectedArea = selectedArea;
        }
        else
        {
            UnselectBoth();
            return;
        }

        if (SelectedArea != null && !MainCameraController.WasDraggingInLastMovement)
        {
            // Open actions menu
            BuildMenuController.EnableActionButtons();
            SelectedArea.SelectArea();
        }
    }
    public void UnselectBoth()
    {
        UnselectBuildArea();
        UnselectBuilding();
    }
    public void UnselectBuildArea()
    {
        if (SelectedArea != null)
        {
            BuildMenuController.DisableActionButtons();
            SelectedArea.UnselectArea();
            SelectedArea = null;
        }
    }
    public void SelectBuilding(BuildingController selectedBuilding)
    {
        if (SelectedBuilding != selectedBuilding)
        {
            UnselectBoth();
            SelectedBuilding = selectedBuilding;
        }
        else
        {
            UnselectBoth();
            return;
        }

        if (SelectedBuilding != null && !MainCameraController.WasDraggingInLastMovement)
        {
            SelectedBuilding.SelectBuilding();
        }
    }
    public void UnselectBuilding()
    {
        if (SelectedBuilding != null)
        {
            SelectedBuilding.UnselectBuilding();
            SelectedBuilding = null;
        }
    }
    public void SaveBuildAreaData(BuildAreaController buildArea)
    {
        if (SaveData)
        {
            CurrentSurvivalBaseData.UpdateBuildAreaData(buildArea);

            string survivalBaseDataJson = JsonUtility.ToJson(CurrentSurvivalBaseData, true);

            string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string fileName = "survival_base_data.json";
            string completeRute = Path.Combine(folderPath, fileName);
            File.WriteAllText(completeRute, survivalBaseDataJson);
        }
    }
    public void LoadSurvivalBaseData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        string fileName = "survival_base_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        if (Directory.Exists(folderPath) && File.Exists(completeRute))
        {
            string json = File.ReadAllText(completeRute);
            CurrentSurvivalBaseData = JsonUtility.FromJson<SurvivalBaseData>(json);
            AddSavedDataBuildings();
        }
    }
    public void DeleteSurvivalBaseData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        string fileName = "survival_base_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        if (Directory.Exists(folderPath) && File.Exists(completeRute))
        {
            File.Delete(completeRute);
        }
    }
    public void AddSavedDataBuildings()
    {
        foreach (var buildingData in CurrentSurvivalBaseData.BuildingsData)
        {
            foreach (var area in BuildAreas)
            {
                if (area.ID == buildingData.Area_ID)
                {
                    PlaceBuildingInAreaFromSavedData(area, buildingData);
                    break;
                }
            }
        }
    }

    IEnumerator InitializeSurvivalBase()
    {
        GeneralUIController.Instance.EnableBlackBackground(true);
        Debug.Log("Fundido a negro");
        yield return new WaitForSeconds(.2f);
        LoadSurvivalBaseData();
        Debug.Log("Load all buildings data from saved file");
        yield return new WaitForSeconds(.2f);
        GeneralUIController.Instance.EnableBlackBackground(false);
        Debug.Log("Fundido a blanco");

    }
}


