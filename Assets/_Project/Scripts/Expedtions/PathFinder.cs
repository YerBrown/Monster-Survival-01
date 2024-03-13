using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closedList = new List<OverlayTile>();

        openList.Add(start);

        while (openList.Count > 0)
        {
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if (currentOverlayTile == end)
            {
                //finalize our path
                return GetFinishedList(start, end);
            }

            var neighbourTiles = GetNeighbourTiles(currentOverlayTile);

            foreach (var neighbour in neighbourTiles)
            {
                //1 = jump height
                if (neighbour.isBlocked || closedList.Contains(neighbour) || Mathf.Abs(currentOverlayTile.gridLocation.z - neighbour.gridLocation.z) > 1 || (neighbour.I_Element != null && neighbour != end && neighbour.I_Element.IsBlockingMovement))
                {
                    continue;
                }

                neighbour.G = GetManhattenDistance(start, neighbour);
                neighbour.H = GetManhattenDistance(end, neighbour);

                neighbour.previous = currentOverlayTile;
                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }
        }

        return new List<OverlayTile>();
    }

    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();
        OverlayTile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }
        finishedList.Reverse();
        return finishedList;
    }

    private int GetManhattenDistance(OverlayTile start, OverlayTile neighbour)
    {
        return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
    }

    private List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile)
    {
        var map = new Dictionary<Vector2Int, OverlayTile>();
        if (MapManager.Instance != null)
        {
            map = MapManager.Instance.map;
        }
        else if (CombatManager.Instance != null)
        {
            map = CombatManager.Instance.Map;
        }
        else
        {
            return null;
        }

        List<OverlayTile> neighbours = new List<OverlayTile>();

        //top
        Vector2Int locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y + 1
            );
        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        //bottom
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y - 1
            );
        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        //right
        locationToCheck = new Vector2Int(
           currentOverlayTile.gridLocation.x + 1,
           currentOverlayTile.gridLocation.y
           );
        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        //left
        locationToCheck = new Vector2Int(
           currentOverlayTile.gridLocation.x - 1,
           currentOverlayTile.gridLocation.y
           );
        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        return neighbours;
    }
}
