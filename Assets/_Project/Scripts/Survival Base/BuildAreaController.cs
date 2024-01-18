using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BuildAreaController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SelectArea();
    }
    public void SelectArea()
    {
        Debug.Log($"Build area {gameObject.name} clicked");
    }
}
