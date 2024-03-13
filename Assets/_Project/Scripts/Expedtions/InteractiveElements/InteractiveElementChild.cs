using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractiveElementChild : MonoBehaviour
{
    public OverlayTile OverlayTileUnder; // Overlay tile under the interactive element.
    public InteractiveElement ParentElement; // The interactive element to which this child belongs.
    public void AddOverlayTileUnderRef()
    {
        OverlayTile overlayTileDetected = DetectOverlayTileUnder();
        if (overlayTileDetected != null)
        {
            OverlayTileUnder = overlayTileDetected;
            if (ParentElement == null)
            {
                ParentElement = GetComponentInParent<InteractiveElement>();
            }
            OverlayTileUnder.I_Element = ParentElement;
        }
    }
    public void RemoveOverlayTileUnderRef()
    {
        if (OverlayTileUnder != null)
        {
            // Remove reference to this elemnt in the overlay tile.
            OverlayTileUnder.I_Element = null;
            OverlayTileUnder = null;
        }
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
}
