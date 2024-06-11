using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BiomesManager : MonoSingleton<BiomesManager>
{
    [Serializable]
    public class CreatureBiome
    {
        public BiomeSO BiomeInfo;
        public List<Creature> CreatureSlots = new();
        public int UnlockedSlots;
        public int Level;
        public bool AddNewCreature(Creature addedCreature)
        {
            if (BiomeInfo.Creatures.Contains(addedCreature.CreatureInfo))
            {
                int slotsFull = CreatureSlots.Count(slot => slot != null);
                if (slotsFull < UnlockedSlots)
                {
                    for (int i = 0; i < UnlockedSlots; i++)
                    {
                        if (CreatureSlots[i] == null || string.IsNullOrEmpty(CreatureSlots[i].ID))
                        {
                            CreatureSlots[i] = addedCreature;
                            SortCreatureList();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool RemoveCreature(Creature selectedCreature)
        {
            if (CreatureSlots.Contains(selectedCreature))
            {
                Creature removedCreature = selectedCreature;
                int removedIndex = CreatureSlots.IndexOf(selectedCreature);
                CreatureSlots[removedIndex] = null;
                SortCreatureList();
                return true;
            }
            return false;
        }
        private void SortCreatureList()
        {
            Comparison<Creature> comparator = (x, y) =>
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                else if (x == null)
                {
                    return 1;
                }
                else if (y == null)
                {
                    return -1;
                }
                else
                {
                    int resultado = x.CreatureInfo.c_Name.CompareTo(y.CreatureInfo.c_Name);
                    return resultado;
                }
            };
            CreatureSlots.Sort(comparator);
        }
        public void UpgradeLevel()
        {
            if (Level < 3)
            {
                Level++;
                UnlockedSlots += 6;
                if (UnlockedSlots > 24)
                {
                    UnlockedSlots = 24;
                }
            }
        }
    }

    public List<CreatureBiome> CurrentBiomes = new();

    public bool AddCreatureToBiome(CreatureBiome biomeTarget, Creature addedCreature)
    {
        if (biomeTarget.BiomeInfo.Creatures.Contains(addedCreature.CreatureInfo))
        {
            int slotsFull = biomeTarget.CreatureSlots.Count(slot => slot != null);
            if (slotsFull < biomeTarget.UnlockedSlots)
            {
                bool success = biomeTarget.AddNewCreature(addedCreature);
                return success;
            }
        }
        return false;
    }
    public bool RemoveCreatureFromBiome(CreatureBiome biomeTarget, Creature removedCreature)
    {
        if (biomeTarget.CreatureSlots.Contains(removedCreature))
        {
            bool success = biomeTarget.RemoveCreature(removedCreature);
            return success;
        }
        return false;
    }
    public void UpgradeBiome(CreatureBiome biomeTarget)
    {
        //TODO: Consume resources
        biomeTarget.UpgradeLevel();
    }
}
