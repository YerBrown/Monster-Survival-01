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
    private CharacterInfo character;
    public SpriteRenderer CursorRenderer;

    private PathFinder pathFinder;

    private List<OverlayTile> path = new List<OverlayTile>();

    private OverlayTile _currentOverlayClicked;

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
            transform.position = overlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
            if (overlayTile.I_Element != null)
            {
                if (overlayTile.I_Element is not ItemElement)
                {
                    if (CursorRenderer.color != Color.blue)
                        CursorRenderer.color = Color.blue;
                }
                else
                {
                    if (CursorRenderer.color != Color.yellow)
                        CursorRenderer.color = Color.yellow;
                }
            }
            else
            {
                if (CursorRenderer.color != Color.white)
                    CursorRenderer.color = Color.white;
            }
            if (Input.GetMouseButtonDown(0) && !GeneralUIController.Instance.MenuOpened)
            {
                //overlayTile.ShowTile();

                if (character == null)
                {
                    character = Instantiate(characterPrefab).GetComponent<CharacterInfo>();
                    PositionCharacterOnTile(overlayTile);
                }
                else
                {
                    _currentOverlayClicked = overlayTile;
                    path = pathFinder.FindPath(character.ActiveTile, overlayTile);
                    if (overlayTile.I_Element != null)
                    {
                        if (overlayTile.I_Element is not ItemElement)
                        {
                            path.RemoveAt(path.Count - 1);
                        }
                        if (path.Count == 0)
                        {
                            FinishPath();
                        }
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
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);

        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        if (Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
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
            character.SetMovement((path[0].transform.position - character.transform.position).normalized);
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
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + .0001f, tile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        character.ActiveTile = tile;
    }

    private void FinishPath()
    {
        if (_currentOverlayClicked != null && _currentOverlayClicked.I_Element != null)
        {
            character.SetMovement((_currentOverlayClicked.transform.position - character.transform.position).normalized);
            _currentOverlayClicked.I_Element.Interact(character);
        }
        character.SetMovement(Vector2.zero);
    }
}
