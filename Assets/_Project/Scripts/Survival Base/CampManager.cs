using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampManager : MonoSingleton<CampManager>
{
    public List<BuildAreaController> BuildAreas = new();
    public CameraController MainCameraController;
    public SurvivalBaseData CurrentData;
    [Header("UI")]
    public UIBuildMenuController BuildMenuController;
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    public void PlaceNewBuilding(BuildingSO newBuild, BuildAreaController selectedArea)
    {
        BuildingController build = Instantiate(newBuild.Prefab, selectedArea.transform).GetComponent<BuildingController>();
        build.transform.localPosition = Vector3.zero;
        selectedArea.IsEmpty = false;
        DateTime finishDateTime = RealDateTimeManager.Instance.GetCurrentDateTime() + new TimeSpan(newBuild.FinishTimes[0].Days, newBuild.FinishTimes[0].Hours, newBuild.FinishTimes[0].Minutes, newBuild.FinishTimes[0].Seconds);
        build.StartUpdate(finishDateTime);
    }
    public void SelectBuildArea(BuildAreaController selectedArea)
    {
        if (!MainCameraController.WasDraggingInLastMovement)
        {
            BuildMenuController.SelectNewBuildArea(selectedArea);
        }
    }

}


