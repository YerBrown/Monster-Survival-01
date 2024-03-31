using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Mono.Cecil;
using UnityEngine;

public class ItemInfoWiki : MonoBehaviour
{
    private static ItemInfoWiki _instance;
    public static ItemInfoWiki Instance { get { return _instance; } }

    public List<ItemsSO> ItemsLibrary = new List<ItemsSO>();
    public Dictionary<string, ItemsSO> ItemsDictionary = new Dictionary<string, ItemsSO>();

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

    private void Start()
    {
        SearchItemsInFolder();
    }

    private void SearchItemsInFolder()
    {
        ItemsSO[] allItemsSO = Resources.LoadAll<ItemsSO>("SO/Items");
        ItemsLibrary.Clear();
        foreach (var item in allItemsSO)
        {
            ItemsLibrary.Add(item);
        }
        foreach (var item in ItemsLibrary)
        {
            ItemsDictionary.Add(item.i_Name, item);
        }
    }
    public ItemsSO GetItemByID(string id)
    {
        return ItemsDictionary[id];
    }
}
