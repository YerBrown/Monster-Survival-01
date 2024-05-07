using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BuildAreaController : MonoBehaviour
{
    public bool IsEmpty = true;
    public BuildingSO.BuildingSize Size;
    public void SelectArea()
    {
        Debug.Log($"Build area {gameObject.name} clicked");
        if (IsEmpty)
        {
            CampManager.Instance.SelectBuildArea(this);
        }
    }
}
