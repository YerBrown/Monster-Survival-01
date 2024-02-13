using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;
    public List<InteractiveElement> InitialElements = new List<InteractiveElement>();

    public CharacterInfo Character;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            AddAllOverlayTiles();
        }
    }
    private void Start()
    {
        InitialElements = AddAllInteractiveELementsInScene();
        Invoke(nameof(InitializeInteractiveElements), 0.5f);
    }
    private void AddAllOverlayTiles()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        map = new Dictionary<Vector2Int, OverlayTile>();
        BoundsInt bounds = tileMap.cellBounds;
        //looping throug all of our tiles
        for (int z = bounds.max.z; z > bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);
                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gridLocation = tileLocation;
                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }
    }
    private List<InteractiveElement> AddAllInteractiveELementsInScene()
    {
        List<InteractiveElement> interactiveElements = new List<InteractiveElement>();

        // Buscamos todos los GameObjects en la escena.
        InteractiveElement[] allInteractive = FindObjectsOfType<InteractiveElement>();

        // A�adimos a la lista todos InteractiveElement del array
        foreach (InteractiveElement in_element in allInteractive)
        {
            interactiveElements.Add(in_element);
        }
        return interactiveElements;
    }
    private void InitializeInteractiveElements()
    {
        foreach (var interactiveElement in InitialElements)
        {
            interactiveElement.InitializeElement();
        }
    }
}

