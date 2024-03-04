using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldTilemapInfo : MonoBehaviour
{
    public TravelNextZoneElement[] StartPoints_L = new TravelNextZoneElement[2];
    public TravelNextZoneElement[] StartPoints_R = new TravelNextZoneElement[2];
    public TravelNextZoneElement[] StartPoints_U = new TravelNextZoneElement[2];
    public TravelNextZoneElement[] StartPoints_D = new TravelNextZoneElement[2];
    public Transform EnvironmentParent;

    public List<InteractiveElement> InitialInteractiveElements = new List<InteractiveElement>();
    public Tilemap FieldTileMap;
    public Transform OverlayTileContainer;
    public void AsignIds()
    {
        foreach (InteractiveElement element in InitialInteractiveElements)
        {
            if (string.IsNullOrEmpty(element.ID))
            {
                element.ID = SetID(element);
            }
        }
    }

    private string SetID(InteractiveElement newElement)
    {
        switch (newElement)
        {
            case BlockerElement blocker_e:
                return blocker_e.ID = $"#BLOCKER_{GetAmountOfInteractiveWithID(blocker_e).ToString("00")}";
            case ContainerElement container_e:
                return container_e.ID = $"#CONTAINER_{GetAmountOfInteractiveWithID(container_e).ToString("00")}";
            case CreatureElement creature_e:
                return creature_e.ID = $"#CREATURE_{GetAmountOfInteractiveWithID(creature_e).ToString("00")}";
            case ItemElement item_e:
                return item_e.ID = $"#ITEM_{GetAmountOfInteractiveWithID(item_e).ToString("00")}";
            case ResourceElement resource_e:
                return resource_e.ID = $"#RESOURCE_{GetAmountOfInteractiveWithID(resource_e).ToString("00")}";
            case SwitchElement switch_e:
                return switch_e.ID = $"#SWITCH_{GetAmountOfInteractiveWithID(switch_e).ToString("00")}";
            case TravelNextZoneElement travel_e:
                return travel_e.ID = $"#TRAVEL_{GetAmountOfInteractiveWithID(travel_e).ToString("00")}";
            default:
                return $"#UNID_{Random.Range(0, 10000).ToString("00000")}";
        }
    }
    private int GetAmountOfInteractiveWithID<T>(T element) where T : InteractiveElement
    {
        int amount = 0;
        foreach (var initElement in InitialInteractiveElements)
        {
            if (initElement is T && !string.IsNullOrEmpty(initElement.ID))
            {
                string[] partes = initElement.ID.Split("_");
                string numeroString = partes[1];
                if (int.TryParse(numeroString, out int numero))
                {
                    if (numero >= amount)
                    {
                        amount = numero + 1;
                    }
                }
            }
        }
        return amount;
    }

    public void InitializeInteractiveElements()
    {
        foreach (var element in InitialInteractiveElements)
        {
            element.AddOverlayTileUnderRef();
        }
    }
    public void AddInteractiveElements()
    {
        InteractiveElement[] allInteractiveElements = GetComponentsInChildren<InteractiveElement>();
        InitialInteractiveElements = allInteractiveElements.ToList();
    }
}
