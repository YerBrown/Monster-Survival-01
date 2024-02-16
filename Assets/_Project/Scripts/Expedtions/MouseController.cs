using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float speed;
    public GameObject characterPrefab;
    private CharacterInfo Character;
    public SpriteRenderer CursorRenderer;

    private PathFinder pathFinder;

    private List<OverlayTile> path = new List<OverlayTile>();

    private OverlayTile _currentOverlayClicked;

    public VoidEventChannelSO OnMovementGridPointed;
    private void Start()
    {
        pathFinder = new PathFinder();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        var focusedTileHit = GetFocusedTile();

        if (focusedTileHit.HasValue)
        {
            OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            transform.position = overlayTile.transform.position + new Vector3(0, 0.001f, 0);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
            if (overlayTile.I_Element != null)
            {
                if (CursorRenderer.color != overlayTile.I_Element.CursorColor)
                    CursorRenderer.color = overlayTile.I_Element.CursorColor;
            }
            else
            {
                if (CursorRenderer.color != Color.white)
                    CursorRenderer.color = Color.white;
            }
            if (Input.GetMouseButtonDown(0) && !GeneralUIController.Instance.MenuOpened)
            {
                //overlayTile.ShowTile();

                if (Character == null)
                {
                    Character = Instantiate(characterPrefab).GetComponent<CharacterInfo>();
                    MapManager.Instance.Character = Character;
                    PositionCharacterOnTile(overlayTile);
                }
                else
                {
                    if (_currentOverlayClicked != overlayTile)
                    {
                        OnMovementGridPointed.RaiseEvent();
                    }
                    _currentOverlayClicked = overlayTile;
                    List<OverlayTile> newPath = pathFinder.FindPath(Character.ActiveTile, overlayTile);
                    if (newPath != null && newPath.Count > 0)
                    {
                        path = newPath;
                        if (overlayTile.I_Element != null)
                        {
                            if (overlayTile.I_Element.BlockMovement)
                            {
                                path.RemoveAt(path.Count - 1);
                            }
                            if (path.Count == 0)
                            {
                                FinishPath();
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("It is not possible to reach this location");
                    }

                }
            }
        }

        if (path.Count > 0)
        {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime;

        var zIndex = path[0].transform.position.z;
        Character.transform.position = Vector2.MoveTowards(Character.transform.position, path[0].transform.position, step);

        Character.transform.position = new Vector3(Character.transform.position.x, Character.transform.position.y, zIndex);

        if (Vector2.Distance(Character.transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
            if (path.Count == 0)
            {
                //Path Finished
                FinishPath();
            }
        }
        else
        {
            Character.SetMovement((path[0].transform.position - Character.transform.position).normalized);
        }
    }

    public RaycastHit2D? GetFocusedTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }
        return null;
    }
    private void PositionCharacterOnTile(OverlayTile tile)
    {
        Character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + .0001f, tile.transform.position.z);
        Character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        Character.ActiveTile = tile;
    }

    private void FinishPath()
    {
        if (_currentOverlayClicked != null && _currentOverlayClicked.I_Element != null)
        {
            Character.SetMovement((_currentOverlayClicked.transform.position - Character.transform.position).normalized);
            _currentOverlayClicked.I_Element.Interact(Character);
        }
        Character.SetMovement(Vector2.zero);
    }
}
