using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveElementsWiki : MonoBehaviour
{
    private static InteractiveElementsWiki _instance;
    public static InteractiveElementsWiki Instance { get { return _instance; } }

    public List<GameObject> Blockers = new List<GameObject>();
    public List<GameObject> Containers = new List<GameObject>();
    public List<GameObject> Creatures = new List<GameObject>();
    public List<GameObject> Items = new List<GameObject>();
    public List<GameObject> Resources = new List<GameObject>();
    public List<GameObject> Switchers = new List<GameObject>();
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public GameObject GetPrefab(string element_ID)
    {
        GameObject prefab = null;
        prefab = GetElementByList(Blockers, element_ID, prefab);
        prefab = GetElementByList(Containers, element_ID, prefab);
        prefab = GetElementByList(Creatures, element_ID, prefab);
        prefab = GetElementByList(Items, element_ID, prefab);
        prefab = GetElementByList(Resources, element_ID, prefab);
        prefab = GetElementByList(Switchers, element_ID, prefab);
        return prefab;
    }

    public GameObject GetElementByList(List<GameObject> list, string element_ID, GameObject currentFind)
    {
        if (currentFind != null) return currentFind;
        foreach (GameObject element in list)
        {
            InteractiveElement newElement = element.GetComponent<InteractiveElement>();
            if (newElement.Interactive_Element_ID == element_ID)
            {
                return element;
            }
        }
        return null;
    }
}
