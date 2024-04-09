using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightersInfoWiki : MonoBehaviour
{
    private static FightersInfoWiki _instance;
    public static FightersInfoWiki Instance { get { return _instance; } }

    public List<CreatureSO> AllFighters = new();
    public Dictionary<string, CreatureSO> FightersDictionary = new();
    public List<Sprite> ElementSprites = new();
    public Dictionary<ElementType, Sprite> ElementSpritesDictionary = new();
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
            SearchCreaturesInFolder();
            AddElementSprites();
        }
    }
    // Reset element sprite info
    private void AddElementSprites()
    {
        for (int i = 0; i < ElementSprites.Count; i++)
        {
            ElementType element = (ElementType)i;
            ElementSpritesDictionary.Add(element, ElementSprites[i]);
        }
    }
    // Reset all creatures SO info
    private void SearchCreaturesInFolder()
    {
        CreatureSO[] allCreaturesSO = Resources.LoadAll<CreatureSO>("SO/Creatures");
        AllFighters.Clear();
        foreach (var creature in allCreaturesSO)
        {
            AllFighters.Add(creature);
        }
        foreach (var creature in AllFighters)
        {
            FightersDictionary.Add(creature.c_Name, creature);
        }
    }
    // Return a creature SO by creature race id
    public bool GetCreatureInfo(string id, out CreatureSO creature)
    {
        creature = null;
        if (FightersDictionary.Count > 0 && FightersDictionary.ContainsKey(id))
        {
            creature = FightersDictionary[id];
            return true;
        }
        else
        {
            foreach (var fighter in AllFighters)
            {
                if (fighter.c_Name == id)
                {
                    creature = fighter;
                    return true;
                }
            }
        }
        Debug.LogWarning($"Fighter type {id} not found in fighters wiki");
        return false;
    }
}
