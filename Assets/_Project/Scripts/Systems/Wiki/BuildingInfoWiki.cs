using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInfoWiki : MonoBehaviour
{
    public List<BuildingSO> BuildingsLibrary = new List<BuildingSO>();
    public Dictionary<string, BuildingSO> BuildingsDictionary = new Dictionary<string, BuildingSO>();
    private void Awake()
    {
        UpdateLibrary();
    }
    private void UpdateLibrary()
    {
        foreach (var building in BuildingsLibrary)
        {
            BuildingsDictionary.Add(building.name, building);
        }
    }
    public BuildingSO GetBuildingByID(string id)
    {
        return BuildingsDictionary[id];
    }
    public Dictionary<string, BuildingSO> GetBuidlingsDictionary()
    {
        return BuildingsDictionary;
    }
}
