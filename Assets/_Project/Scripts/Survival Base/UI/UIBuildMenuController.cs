using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuildMenuController : MonoBehaviour
{
    public BuildAreaController SelectedBuildArea;
    public VoidEventChannelSO OnPointerClickedFloor;
    [Header("UI")]
    public RectTransform SelectBuildingPopup;
    public GameObject MenuParent;
    public List<BuildingSlotPanel> BuildingSlotPanels = new();
    private void OnEnable()
    {
        OnPointerClickedFloor.OnEventRaised += UnselectBuildArea;
    }
    private void OnDisable()
    {
        OnPointerClickedFloor.OnEventRaised -= UnselectBuildArea;
        
    }
    public void SelectNewBuildArea(BuildAreaController selectedArea)
    {
        SelectedBuildArea = selectedArea;
        StartCoroutine(PopupFollowBuildArea());
        SelectBuildingPopup.gameObject.SetActive(true);
    }
    public void UnselectBuildArea()
    {
        Debug.Log("Flor Clicked!");
        SelectedBuildArea = null;
        SelectBuildingPopup.gameObject.SetActive(false);
    }
    public void OpenBuildMenu()
    {
        if (SelectedBuildArea != null)
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
        foreach (var building in BuildingInfoWiki.Instance.BuildingsDictionary)
        {
            if (building.Value.Size == SelectedBuildArea.Size)
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
        CampManager.Instance.PlaceNewBuilding(selectedBuilding, SelectedBuildArea);
        CloseBuildMenu();
        UnselectBuildArea();
    }
    IEnumerator PopupFollowBuildArea()
    {
        while (SelectedBuildArea != null)
        {
            SelectBuildingPopup.transform.position = Camera.main.WorldToScreenPoint(SelectedBuildArea.transform.position);
            yield return null;
        }
    }
}
