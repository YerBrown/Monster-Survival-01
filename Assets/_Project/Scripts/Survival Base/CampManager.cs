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
        GameObject build = Instantiate(newBuild.Prefab, selectedArea.transform);
        build.transform.localPosition = Vector3.zero;
        selectedArea.IsEmpty = false;
    }
    public void SelectBuildArea(BuildAreaController selectedArea)
    {
        if (!MainCameraController.WasDraggingInLastMovement)
        {
            BuildMenuController.SelectNewBuildArea(selectedArea);
        }
    }

}


