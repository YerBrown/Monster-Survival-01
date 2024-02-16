using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractiveElement : MonoBehaviour
{
    public OverlayTile OverlayTileUnder;
    public bool BlockMovement = false;
    public Color CursorColor = Color.white;
    private void OnEnable()
    {
        InitializeElement();
    }
    private void OnDisable()
    {
        if (OverlayTileUnder != null)
        {
            OverlayTileUnder.I_Element = null;
            OverlayTileUnder = null;
        }
    }
    public virtual void Interact(CharacterInfo character = null)
    {
        Debug.Log($"Player interacted with {name}");
    }

    private RaycastHit2D? DetectOverlayTile()
    {

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }
        return null;
    }
    public void InitializeElement()
    {
        var overlayDetected = DetectOverlayTile();
        if (overlayDetected.HasValue)
        {
            OverlayTileUnder = overlayDetected.Value.collider.gameObject.GetComponent<OverlayTile>();
            OverlayTileUnder.I_Element = this;
        }
    }
}
