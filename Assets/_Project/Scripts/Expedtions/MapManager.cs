using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoSingleton<MapManager>
{
    [Header("MAp Manager")]
    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;
    public Grid MainGrid;
    public Dictionary<Vector2Int, OverlayTile> map;
    public List<OverlayTile> AllTiles = new();
    public List<InteractiveElement> InitialElements = new List<InteractiveElement>();

    public CharacterInfo Character;
    public MouseController Cursor;

    public FieldTilemapInfo CurrentField;
    public Vector2Int CurrentCoordinates;
    public ExpeditionMapSO FullMap;
    public GameObject DropedItemContainerPrefab;
    ExpeditionData CurrentFieldData = new ExpeditionData();

    private void Start()
    {
        if (SceneLoadManager.Instance != null)
        {
            if (SceneLoadManager.Instance.CurrentMap != null)
            {
                ExpeditionMapSO loadMap = SceneLoadManager.Instance.CurrentMap;
                Vector2Int loadCoordinates = SceneLoadManager.Instance.MapFieldCoordinates;
                Vector2 playerStartPosition = SceneLoadManager.Instance.PlayerLastPosition;
                StartCoroutine(LoadLastFieldStatus(loadMap, loadCoordinates, playerStartPosition));
            }
            else
            {
                StartCoroutine(InitializeExpedition());
            }
        }
        else
        {
            StartCoroutine(InitializeExpedition());
        }
    }
    public void ChangeField(Vector2Int travelDistance, int pathSide)
    {
        StartCoroutine(TravelToFieldCoroutine(travelDistance, pathSide));
    }
    private void AddAllOverlayTiles(FieldTilemapInfo newField)
    {
        if (newField == null) return;
        map = new Dictionary<Vector2Int, OverlayTile>();
        BoundsInt bounds = newField.FieldTileMap.cellBounds;
        //looping throug all of our tiles
        for (int z = bounds.max.z; z > bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);
                    if (newField.FieldTileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        OverlayTile overlayTile = ReuseOverlayTile(map);
                        if (overlayTile == null)
                        {
                            overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                            AllTiles.Add(overlayTile);
                        }
                        var cellWorldPosition = newField.FieldTileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = newField.FieldTileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gridLocation = tileLocation;
                        overlayTile.I_Element = null;
                        overlayTile.previous = null;
                        overlayTile.name = $"{tileLocation}";
                        overlayTile.transform.SetAsFirstSibling();
                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }
    }
    private List<InteractiveElement> AddAllInteractiveElementsInScene()
    {
        List<InteractiveElement> interactiveElements = new List<InteractiveElement>();

        // Buscamos todos los GameObjects en la escena.
        InteractiveElement[] allInteractive = CurrentField.GetComponentsInChildren<InteractiveElement>();

        // Anadimos a la lista todos InteractiveElement del array
        foreach (InteractiveElement in_element in allInteractive)
        {
            interactiveElements.Add(in_element);
        }
        return interactiveElements;
    }
    private void GoNextField(Vector2Int travelDistance)
    {

        Vector2Int newCoordinates = CurrentCoordinates + travelDistance;
        if (CurrentField != null)
        {
            SaveAreaState();
            Destroy(CurrentField.gameObject);
            CurrentField = null;
        }
        CurrentCoordinates = newCoordinates;
        CurrentField = Instantiate(FullMap.GetField(CurrentCoordinates), MainGrid.transform).GetComponent<FieldTilemapInfo>();
    }

    private void MovePlayerToInitialPos(Vector2Int travelDistance, int pathSide)
    {
        OverlayTile startPosTile = null;
        if (travelDistance.x > 0 || travelDistance.x < 0)
        {
            if (travelDistance.x == 1)
            {
                startPosTile = GetOverlayTile(CurrentField.StartPoints_L[pathSide].transform.position);
                Character.SetMovementIdle(new Vector2(1, -1));
            }
            else
            {
                startPosTile = GetOverlayTile(CurrentField.StartPoints_R[pathSide].transform.position);
                Character.SetMovementIdle(new Vector2(-1, 1));
            }
        }
        if (travelDistance.y > 0 || travelDistance.y < 0)
        {
            if (travelDistance.y == 1)
            {
                startPosTile = GetOverlayTile(CurrentField.StartPoints_D[pathSide].transform.position);
                Character.SetMovementIdle(new Vector2(1, 1));
            }
            else
            {
                startPosTile = GetOverlayTile(CurrentField.StartPoints_U[pathSide].transform.position);
                Character.SetMovementIdle(new Vector2(-1, -1));
            }
        }

        if (startPosTile != null)
        {
            Character.transform.position = startPosTile.transform.position;
            Character.ActiveTile = startPosTile;
        }
    }

    private OverlayTile GetOverlayTile(Vector2 checkPos)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(checkPos, Vector2.zero);
        RaycastHit2D? lastHit = null;

        if (hits.Length > 0)
        {
            lastHit = hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }
        if (lastHit.HasValue)
        {
            OverlayTile startTile = lastHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            Debug.Log(startTile.name);
            return startTile;

        }
        return null;
    }

    private OverlayTile ReuseOverlayTile(Dictionary<Vector2Int, OverlayTile> mapDictionary)
    {
        foreach (var tile in AllTiles)
        {
            if (!mapDictionary.ContainsValue(tile))
            {
                return tile;
            }
        }
        return null;
    }
    private void SpawnPlayerFirst(Vector2 spawnPosition)
    {
        Cursor.SpawnPlayer(GetOverlayTile(spawnPosition));
    }
    IEnumerator InitializeExpedition()
    {
        GeneralUIController.Instance.EnableBlackBackground(true);
        Debug.Log("Fundido a negro");
        yield return new WaitForSeconds(.2f);
        GoNextField(Vector2Int.zero);
        AddAllOverlayTiles(CurrentField);
        Debug.Log("A�adidas todas las overlay tiles");
        yield return new WaitForSeconds(.2f);
        LoadAreaState();
        Debug.Log("Sincronizados todos los elementos interactivos");
        yield return new WaitForSeconds(.2f);
        SpawnPlayerFirst(CurrentField.transform.position);
        Debug.Log("Instanciado el player");
        yield return new WaitForSeconds(.2f);
        GeneralUIController.Instance.EnableBlackBackground(false);
        Debug.Log("Fundido a blanco");
    }
    IEnumerator TravelToFieldCoroutine(Vector2Int travelDistance, int pathSide)
    {
        GeneralUIController.Instance.EnableBlackBackground(true);
        Debug.Log("Fundido a negro");
        yield return new WaitForSeconds(.2f);
        GoNextField(travelDistance);
        AddAllOverlayTiles(CurrentField);
        Debug.Log("A�adidas todas las overlay tiles");
        yield return new WaitForSeconds(.2f);
        LoadAreaState();
        Debug.Log("Sincronizados todos los elementos interactivos");
        yield return new WaitForSeconds(.2f);
        MovePlayerToInitialPos(travelDistance, pathSide);
        Debug.Log("Mover al player");
        yield return new WaitForSeconds(.2f);
        GeneralUIController.Instance.EnableBlackBackground(false);
        Debug.Log("Fundido a blanco");
    }
    IEnumerator LoadLastFieldStatus(ExpeditionMapSO lastExpeditionMap, Vector2Int lastFieldCordinates, Vector2 playerLastPosition)
    {
        FullMap = lastExpeditionMap;
        CurrentCoordinates = lastFieldCordinates;
        GeneralUIController.Instance.EnableBlackBackground(true);
        Debug.Log("Fundido a negro");
        yield return new WaitForSeconds(.2f);
        GoNextField(Vector2Int.zero);
        AddAllOverlayTiles(CurrentField);
        Debug.Log("A�adidas todas las overlay tiles");
        yield return new WaitForSeconds(.2f);
        LoadAreaState();
        Debug.Log("Sincronizados todos los elementos interactivos");
        yield return new WaitForSeconds(.2f);
        SpawnPlayerFirst(playerLastPosition);
        Debug.Log("Instanciado el player");
        yield return new WaitForSeconds(.2f);
        GeneralUIController.Instance.EnableBlackBackground(false);
        Debug.Log("Fundido a blanco");
    }
    public DropedItemsContainerElement AddLootBag(Vector3 spawnPos)
    {
        DropedItemsContainerElement newBag = Instantiate(DropedItemContainerPrefab, CurrentField.EnvironmentParent).GetComponent<DropedItemsContainerElement>();
        newBag.transform.position = spawnPos;
        newBag.EnableElement(true);
        return newBag;
    }
    private void LoadAreaState()
    {
        if (CurrentField != null)
        {
            string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
            string fileName = "map_data.json";
            string completeRute = Path.Combine(folderPath, fileName);
            if (Directory.Exists(folderPath) && File.Exists(completeRute))
            {
                string json = File.ReadAllText(completeRute);
                CurrentFieldData = JsonUtility.FromJson<ExpeditionData>(json);
            }
            ExpeditionData.FieldData currentFieldData = CurrentFieldData.GetField(CurrentCoordinates);
            CurrentField.AddInteractiveElements();
            CurrentField.AsignIds();
            CurrentField.InitializeInteractiveElements();
            if (currentFieldData != null)
            {
                LoadAllInteractiveElements(currentFieldData);
            }
            if (SceneLoadManager.Instance != null)
            {
                string enemyID = SceneLoadManager.Instance.TeamID;
                if (!string.IsNullOrEmpty(enemyID))
                {
                    (string, FighterData[]) combatResult = SceneLoadManager.Instance.LoadCombatResult();
                    foreach (var creature in currentFieldData.Creatures)
                    {
                        CreatureElement newCreature = (CreatureElement)GetElement(creature.ID, creature.Element_ID, creature.Pos);
                        if (newCreature.ID == combatResult.Item1)
                        {
                            newCreature.UpdateTeamAfterCombat(combatResult.Item2);
                            break;
                        }
                    }
                }
            }
        }
    }
    private void LoadAllInteractiveElements(ExpeditionData.FieldData fieldData)
    {
        if (MainWikiManager.Instance == null) return;
        foreach (var blocker in fieldData.Blockers)
        {
            BlockerElement newBlocker = (BlockerElement)GetElement(blocker.ID, blocker.Element_ID, blocker.Pos);
            newBlocker.UpdateElement(blocker);
        }

        foreach (var container in fieldData.Containers)
        {
            ContainerElement newContainer = (ContainerElement)GetElement(container.ID, container.Element_ID, container.Pos);
            newContainer.UpdateElement(container);
        }
        foreach (var creature in fieldData.Creatures)
        {
            CreatureElement newCreature = (CreatureElement)GetElement(creature.ID, creature.Element_ID, creature.Pos);
            newCreature.UpdateElement(creature);
        }
        foreach (var item in fieldData.Items)
        {
            ItemElement newItem = (ItemElement)GetElement(item.ID, item.Element_ID, item.Pos);
            newItem.UpdateElement(item);
        }

        foreach (var resource in fieldData.Resources)
        {
            ResourceElement newResource = (ResourceElement)GetElement(resource.ID, resource.Element_ID, resource.Pos);
            newResource.UpdateElement(resource);
        }
    }

    private InteractiveElement GetElement(string id, string element_id, Vector3 newPos)
    {
        foreach (var element in CurrentField.InitialInteractiveElements)
        {
            if (element.ID == id)
            {
                if (element.transform.position != newPos)
                {
                    element.transform.position = newPos;
                }

                return element;
            }
        }
        InteractiveElement spawnedElement = Instantiate(MainWikiManager.Instance.GetInteractiveElementByID(element_id), CurrentField.EnvironmentParent).GetComponent<InteractiveElement>();
        spawnedElement.transform.position = newPos;
        return spawnedElement;
    }
    private void SaveAreaState()
    {
        if (CurrentField != null)
        {
            CurrentField.AddInteractiveElements();
            CurrentField.AsignIds();
            CurrentFieldData.GetField(CurrentCoordinates).ClearData();
            foreach (var element in CurrentField.InitialInteractiveElements)
            {
                CurrentFieldData.GetField(CurrentCoordinates).UpdateData(element);
            }

            string fieldDataJson = JsonUtility.ToJson(CurrentFieldData, true);

            string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string fileName = "map_data.json";
            string completeRute = Path.Combine(folderPath, fileName);
            File.WriteAllText(completeRute, fieldDataJson);
        }
    }

    public void GoToCombatScene(string teamId, FighterData[] enemyTeam)
    {
        SaveAreaState();
        SceneLoadManager.Instance.LoadCombatFromExpeditionEnemyEncounter(teamId, enemyTeam);
    }

    private void OnApplicationQuit()
    {
        // Delete current expedition data
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        if (Directory.Exists(folderPath))
        {
            string fileName = "map_data.json";
            string completeRute = Path.Combine(folderPath, fileName);
            File.Delete(completeRute);
        }
    }
}

