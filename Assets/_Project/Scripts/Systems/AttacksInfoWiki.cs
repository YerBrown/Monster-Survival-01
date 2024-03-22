using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttacksInfoWiki : MonoBehaviour
{
    private static AttacksInfoWiki _instance;
    public static AttacksInfoWiki Instance { get { return _instance; } }

    [SerializeField] private List<AttackInfo> _AllAttacks = new();
    private Dictionary<string, AttackInfo> _AttacksDictionary = new();
    [Serializable]
    public class AttackInfo
    {
        public string ID;
        public AnimationClip AnimClip;
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var attack in _AllAttacks)
            {
                _AttacksDictionary.Add(attack.ID, attack);
            }
        }
    }

    public AnimationClip GetAttackAnimClip(string attackId)
    {
        return _AttacksDictionary[attackId].AnimClip;
    }

}
