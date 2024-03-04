using System;
using System.Linq;
using UnityEngine;
public class InteractiveElement : MonoBehaviour
{
    public string ID; // Element identificator for save and load data.
    public string Interactive_Element_ID; // Type of elemnt identificator.
    public OverlayTile OverlayTileUnder; // Overlay tile under the interactive element.
    public bool IsBlockingMovement = false; // If is true, blocks the movement in the overlayTile calculation of the pathfinder.
    public Color CursorColor = Color.white; // The color of the cursor when interacts with this element.
    public ScreenPositon_String_EventChannelSO NotificateEvent; // Notification event channel
    public virtual void OnDisable()
    {
        RemoveOverlayTileUnderRef();
    }
    // Called when the player interacts with this element.
    public virtual void Interact(CharacterInfo character = null)
    {
        Debug.Log($"Player interacted with {name}");
    }
    public void AddOverlayTileUnderRef()
    {
        OverlayTile overlayTileDetected = DetectOverlayTileUnder();
        if (overlayTileDetected != null)
        {
            OverlayTileUnder = overlayTileDetected;
            OverlayTileUnder.I_Element = this;
        }
    }
    private void RemoveOverlayTileUnderRef()
    {
        if (OverlayTileUnder != null)
        {
            // Remove reference to this elemnt in the overlay tile.
            OverlayTileUnder.I_Element = null;
            OverlayTileUnder = null;
        }
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
    private OverlayTile DetectOverlayTileUnder()
    {
        // Throw ray in this element position.
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);

        if (hits.Length > 0)
        {
            // Select first hited object.
            RaycastHit2D hitTile = hits.OrderByDescending(i => i.collider.transform.position.z).First();
            if (hitTile.collider.gameObject.TryGetComponent(out OverlayTile overlayTileDetected))
            {
                return overlayTileDetected;
            }
        }
        return null;
    }
    protected void Notify(Vector3 newPos, string text)
    {
        if (NotificateEvent == null) { return; };
        (Vector3, string) notificationData = (newPos, text);
        NotificateEvent.RaiseEvent(notificationData);
    }
}
