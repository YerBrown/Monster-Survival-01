using MonsterSurvival.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public CreatureBiome(BiomesData.CreatureBiomeData biomeData)
        {
            BiomeInfo = MainWikiManager.Instance.GetBiomeByID(biomeData.Biome_ID);
            CreatureSlots.Clear();
            foreach (var creatureData in biomeData.Creatures)
            {
                CreatureSlots.Add(new Creature(creatureData));
            }
            Level = biomeData.Level;
            UnlockedSlots = Level * 6;
        }
        public bool AddNewCreature(Creature addedCreature)
        {
            if (BiomeInfo.Creatures.Contains(addedCreature.CreatureInfo))
            {
                int slotsFull = CreatureSlots.Count(slot => slot != null);
                if (slotsFull < UnlockedSlots)
                {
                    for (int i = 0; i < UnlockedSlots; i++)
                    {
                        if (i == CreatureSlots.Count)
                        {
                            CreatureSlots.Add(addedCreature);
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
                int removedIndex = CreatureSlots.IndexOf(selectedCreature);
                CreatureSlots.RemoveAt(removedIndex);
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
            if (Level < GeneralValues.StaticCombatGeneralValues.Biomes_Max_Level)
            {
                Level++;
                UnlockedSlots += 6;
                if (UnlockedSlots > 24)
                {
                    UnlockedSlots = 24;
                }
            }
        }
        public bool CheckCreatureAddPossibility(CreatureSO creatureInfo)
        {
            return BiomeInfo.Creatures.Contains(creatureInfo);
        }
    }

    public List<CreatureBiome> CurrentBiomes = new();
    public BiomesData CurrentBiomesData;
    public bool SaveData = true;
    private void Start()
    {
        LoadBiomesData();
    }
    public bool AddCreatureToBiome(CreatureBiome biomeTarget, Creature addedCreature)
    {
        if (biomeTarget.BiomeInfo.Creatures.Contains(addedCreature.CreatureInfo))
        {
            int slotsFull = biomeTarget.CreatureSlots.Count(slot => slot != null);
            if (slotsFull < biomeTarget.UnlockedSlots)
            {
                bool success = biomeTarget.AddNewCreature(addedCreature);
                SaveBiomesData(CurrentBiomes);
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
            SaveBiomesData(CurrentBiomes);
            return success;
        }
        return false;
    }
    public void UpgradeBiome(CreatureBiome biomeTarget)
    {
        if (biomeTarget.Level < GeneralValues.StaticCombatGeneralValues.Biomes_Max_Level)
        {
            biomeTarget.UpgradeLevel();
            SaveBiomesData(CurrentBiomes);
        }
    }

    public void SaveBiomesData(List<CreatureBiome> biome)
    {
        if (SaveData)
        {
            CurrentBiomesData = new BiomesData(biome);

            string biomesDataJson = JsonUtility.ToJson(CurrentBiomesData, true);

            string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string fileName = "biomes_data.json";
            string completeRute = Path.Combine(folderPath, fileName);
            File.WriteAllText(completeRute, biomesDataJson);
        }
    }
    public void LoadBiomesData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        string fileName = "biomes_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        if (Directory.Exists(folderPath) && File.Exists(completeRute))
        {
            string json = File.ReadAllText(completeRute);
            CurrentBiomesData = JsonUtility.FromJson<BiomesData>(json);
            AddBiomesData(CurrentBiomesData);
        }
    }
    public void DeleteBiomesData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Datos");
        string fileName = "biomes_data.json";
        string completeRute = Path.Combine(folderPath, fileName);
        if (Directory.Exists(folderPath) && File.Exists(completeRute))
        {
            File.Delete(completeRute);
        }
    }
    private void AddBiomesData(BiomesData biomesData)
    {
        CurrentBiomes.Clear();
        foreach (var biome in biomesData.AllBiomes)
        {
            CurrentBiomes.Add(new CreatureBiome(biome));
        }
    }

}
