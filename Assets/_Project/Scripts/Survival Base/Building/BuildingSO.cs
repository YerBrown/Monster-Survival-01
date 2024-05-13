using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BuildingSO", menuName = "ScriptableObjects/Building")]
public class BuildingSO : ScriptableObject
{
    public string Name;
    public string Description;
    public List<Sprite> Sprites;
    public GameObject Prefab;
    public List<BuildCost> Costs = new();
    public List<BuildTime> FinishTimes = new();
    [Serializable]
    public class BuildTime
    {
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;
    }
    [Serializable]
    public class BuildCost
    {
        public ItemSlot[] Costs = new ItemSlot[3];
    }
    public BuildingSize Size;
    public enum BuildingSize
    {
        OneByOne,
        ThreeByThree,
        FiveByFive,
    }
    public enum BuildingType
    {

    }
}
