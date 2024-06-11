using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuildMenuController : UIBuildingMenuController
{
    public WorkerCreature SelectedWorker;
    [Header("UI+")]
    public GameObject MenuParent;
    public List<BuildingSlotPanel> BuildingSlotPanels = new();
    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        
    }
    public void OpenBuildMenu()
    {
        if (CampManager.Instance.SelectedArea != null)
        {
            MenuParent.SetActive(true);
            UpdateBuildMenu();
        }
    }
    public void CloseBuildMenu()
    {
        MenuParent.SetActive(false);
    }
    public void UpdateBuildMenu()
    {
        List<BuildingSO> sameSizeBuildings = new();
        foreach (var building in MainWikiManager.Instance.GetBuidlingsDictionary())
        {
            if (building.Value.Size == CampManager.Instance.SelectedArea.Size)
            {
                sameSizeBuildings.Add(building.Value);
            }
        }

        for (int i = 0; i < BuildingSlotPanels.Count; i++)
        {
            if (i < sameSizeBuildings.Count)
            {
                BuildingSlotPanels[i].UpdatePanel(sameSizeBuildings[i]);
                BuildingSlotPanels[i].gameObject.SetActive(true);
            }
            else
            {
                BuildingSlotPanels[i].ClearBuilding();
                BuildingSlotPanels[i].gameObject.SetActive(false);
            }
        }
    }
    public void StartBuidling(BuildingSO selectedBuilding)
    {
        CampManager.Instance.PlaceNewBuilding(selectedBuilding, SelectedWorker);
        CloseBuildMenu();
    }
}
