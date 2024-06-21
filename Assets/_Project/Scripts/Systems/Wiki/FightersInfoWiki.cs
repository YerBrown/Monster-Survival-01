using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightersInfoWiki : MonoBehaviour
{


    public List<CreatureSO> AllFighters = new();
    public Dictionary<string, CreatureSO> FightersDictionary = new();
    public List<ElementInfoUI> AllElementsInfo = new();
    public Dictionary<ElementType, ElementInfoUI> ElementsInfoDictionary = new();
    [Serializable]
    public class ElementInfoUI
    {
        public ElementType Element;
        public Sprite ElementSprite;
        public Color BackgroundColor;
        public Color ElementColor;
    }
    private void Awake()
    {
        SearchCreaturesInFolder();
        AddElementSprites();
    }
    // Reset element sprite info
    private void AddElementSprites()
    {
        for (int i = 0; i < AllElementsInfo.Count; i++)
        {
            ElementsInfoDictionary.Add(AllElementsInfo[i].Element, AllElementsInfo[i]);
        }
    }

    public bool GetElementSprite(ElementType element, out Sprite elementSprite)
    {
        elementSprite = null;
        if (ElementsInfoDictionary.ContainsKey(element))
        {
            elementSprite = ElementsInfoDictionary[element].ElementSprite;
            return true;
        }
        else
        {
            Debug.LogWarning($"Element type {element} not found in fighters wiki");
            return false;
        }
    }
    public bool GetElementInfo(ElementType element, out ElementInfoUI elemementInfo)
    {
        elemementInfo = null;
        if (ElementsInfoDictionary.ContainsKey(element))
        {
            elemementInfo = ElementsInfoDictionary[element];
            return true;
        }
        else
        {
            Debug.LogWarning($"Element type {element} not found in fighters wiki");
            return false;
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
