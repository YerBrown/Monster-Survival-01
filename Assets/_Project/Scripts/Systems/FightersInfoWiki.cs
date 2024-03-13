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

            foreach (var fighter in AllFighters)
            {
                FightersDictionary.Add(fighter.c_Name, fighter);
            }
        }
    }

    public bool GetCreatureInfo(string id, out CreatureSO creature)
    {
        creature = null;
        if (FightersDictionary.Count > 0)
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
        return false;
    }
}
