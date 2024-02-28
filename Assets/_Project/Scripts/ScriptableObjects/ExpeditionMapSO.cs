using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ExpeditionMapSO", menuName = "ScriptableObjects/ExpeditionMap")]
public class ExpeditionMapSO : ScriptableObject
{
    public List<XFields> yFields = new List<XFields>();

    public GameObject GetField(Vector2Int coordinates)
    {
        if (yFields.Count > coordinates.y && yFields[coordinates.y].xFields.Count > coordinates.x)
        {
            return yFields[coordinates.y].xFields[coordinates.x];
        }
        return null;
    }
    [Serializable]
    public class XFields
    {
        public List<GameObject> xFields = new List<GameObject>();
    }
}
