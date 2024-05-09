using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInfoWiki : MonoSingleton<BuildingInfoWiki>
{
    public List<BuildingSO> BuildingsLibrary = new List<BuildingSO>();
    public Dictionary<string, BuildingSO> BuildingsDictionary = new Dictionary<string, BuildingSO>();
    private void Start()
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
}
