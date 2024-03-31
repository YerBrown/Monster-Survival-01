using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AddGridDetailsFields : MonoBehaviour
{
    public int Columns = 8;
    public int Rows = 8;
    public GameObject DetailPosPrefab;
    public Grid Grid;
    public Transform DetailsParent;
    public List<GameObject> DetailPos;
    public void OnEnable()
    {
        GenerateDetailsPositions();
    }
    private void OnDisable()
    {
        RemoveDetailPos();
    }
    public void GenerateDetailsPositions()
    {
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                Vector2 newPosition = Vector2.zero;
                newPosition.x = 0 - (Columns / 2 * Grid.cellSize.x - Grid.cellSize.x / 2) + i * (Grid.cellSize.x / 2) + j * Grid.cellSize.x / 2;
                newPosition.y = i * (Grid.cellSize.y / 2) - j * (Grid.cellSize.y / 2);
                GameObject newDetailPos = Instantiate(DetailPosPrefab, DetailsParent);
                newDetailPos.transform.position = newPosition;

            }
        }
    }
    public void RemoveDetailPos()
    {
        foreach (GameObject go in DetailPos)
        {
            DestroyImmediate(go);
        }
        DetailPos.Clear();
    }
}
