using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoWiki : MonoSingleton<ItemInfoWiki>
{
    public List<ItemsSO> ItemsLibrary = new List<ItemsSO>();
    public Dictionary<string, ItemsSO> ItemsDictionary = new Dictionary<string, ItemsSO>();


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
