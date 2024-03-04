using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightersInfoWiki : MonoBehaviour
{
    private static FightersInfoWiki _instance;
    public static FightersInfoWiki Instance { get { return _instance; } }

    public List<FighterInfo> AllFighters = new();
    [Serializable]
    public class FighterInfo
    {
        public string ID;
        public GameObject Prefab;
    }
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
    public GameObject GetFighterPrefab(string id)
    {
        foreach (var fighter in AllFighters)
        {
            if (fighter.ID == id)
            {
                return fighter.Prefab;
            }
        }
        return null;
    }
}
