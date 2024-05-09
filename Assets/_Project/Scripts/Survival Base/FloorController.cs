using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloorController : MonoBehaviour, IPointerClickHandler
{
    public VoidEventChannelSO OnClickOnFloor;
    public CameraController MainCameraController;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!MainCameraController.WasDraggingInLastMovement)
        {
            Vector3 inputPosition;
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                inputPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
            else
            {
                inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            RaycastHit2D hit = Physics2D.Raycast(inputPosition, Camera.main.transform.forward, 100f);
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<FloorController>() != null)
                {
                    Debug.Log($"Floor clicked");
                    OnClickOnFloor.RaiseEvent();
                }
                else if (hit.collider.GetComponent<BuildAreaController>() != null)
                {
                    Debug.Log($"Build area {hit.collider.name} clicked");
                    hit.collider.GetComponent<BuildAreaController>().TriggerSelectArea();
                }
                else if (hit.collider.GetComponent<BuildingController>() != null)
                {
                    Debug.Log($"Building {hit.collider.name} clicked");
                    hit.collider.GetComponent<BuildingController>().TriggerSelectBuilding();
                }
                
            }
        }
    }
}
