using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttacksInfoWiki : MonoBehaviour
{
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
        UpdateDictionary();
    }
    private void UpdateDictionary()
    {
        foreach (var attack in _AllAttacks)
        {
            _AttacksDictionary.Add(attack.ID, attack);
        }
    }
    public AnimationClip GetAttackAnimClip(string attackId)
    {
        return _AttacksDictionary[attackId].AnimClip;
    }

}