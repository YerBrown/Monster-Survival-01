using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class InteractiveElement : MonoBehaviour
{
    public string ID; // Element identificator for save and load data.
    public string Interactive_Element_ID; // Type of elemnt identificator.
    public List<InteractiveElementChild> ElementChilds = new();
    public bool IsBlockingMovement = false; // If is true, blocks the movement in the overlayTile calculation of the pathfinder.
    public Color CursorColor = Color.white; // The color of the cursor when interacts with this element.
    public ScreenPositon_String_EventChannelSO NotificateEvent; // Notification event channel
    public bool IsActiveElement;
    public void AddOverlayTileUnderLink()
    {
        if (ElementChilds.Count <= 0)
        {
            ElementChilds = GetComponentsInChildren<InteractiveElementChild>().ToList();
        }
        foreach (var child in ElementChilds)
        {
            child.AddOverlayTileUnderRef();
        }
        IsActiveElement = true;
    }
    public void RemoveOverlayTileUnderLink()
    {
        if (ElementChilds.Count <= 0)
        {
            ElementChilds = GetComponentsInChildren<InteractiveElementChild>().ToList();
        }
        foreach (var child in ElementChilds)
        {
            child.RemoveOverlayTileUnderRef();
        }
        IsActiveElement = false;
    }

    // Called when the player interacts with this element.
    public virtual void Interact(CharacterInfo character = null)
    {
        Debug.Log($"Player interacted with {name}");
    }
    protected void ChangeCursorColor(string colorHex)
    {
        // Try parse the hexadecimal color to color.
        if (ColorUtility.TryParseHtmlString(colorHex, out Color color))
        {
            CursorColor = color;
        }
        else
        {
            // If the value is not parseable trigger a log.
            Debug.LogWarning("Could not convert hexadecimal color: " + colorHex);
        }
    }
    //Update interactive element properties by loaded data
    public virtual void UpdateElement(ExpeditionData.ParentData data)
    {
        ID = data.ID;
    }
    // Enable interactive element following the needs
    public void EnableElement(bool enable)
    {
        if (enable && !IsActiveElement)
        {
            AddOverlayTileUnderLink();
        }
        else if (!enable && IsActiveElement)
        {
            RemoveOverlayTileUnderLink();
        }
    }
    protected void Notify(Vector3 newPos, string text)
    {
        if (NotificateEvent == null) { return; };
        (Vector3, string) notificationData = (newPos, text);
        NotificateEvent.RaiseEvent(notificationData);
    }
}
