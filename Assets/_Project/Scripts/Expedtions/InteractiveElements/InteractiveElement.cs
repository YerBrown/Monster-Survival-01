using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Serializable]
public class InteractiveElement : MonoBehaviour
{
    [SerializeField]
    public string ID;
    public string Interactive_Element_ID;
    public OverlayTile OverlayTileUnder;
    [SerializeField]
    public bool BlockMovement = false;
    public Color CursorColor = Color.white;
    public virtual void OnEnable()
    {
        InitializeElement();
    }
    public virtual void OnDisable()
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
    protected void ChangeCursorColor(string colorHex)
    {
        // Color resultante
        Color color;
        // Intentar convertir el color hexadecimal en un objeto Color
        if (ColorUtility.TryParseHtmlString(colorHex, out color))
        {
            CursorColor = color;
        }
        else
        {
            // La conversión falló, el formato del color hexadecimal podría ser incorrecto
            Debug.LogWarning("No se pudo convertir el color hexadecimal: " + colorHex);
        }
    }

    public virtual void UpdateElement(ExpeditionData.ParentData data)
    {
        //DEFAULT
        ID = data.ID;       
    }
}
