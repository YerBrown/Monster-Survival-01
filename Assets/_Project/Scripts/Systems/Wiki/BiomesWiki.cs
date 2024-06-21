using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomesWiki : MonoBehaviour
{
    public List<BiomeSO> BiomesLibrary = new List<BiomeSO>();
    public Dictionary<string, BiomeSO> BiomesDictionary = new Dictionary<string, BiomeSO>();
    private void Awake()
    {
        SearchBiomesInFolder();
    }

    private void SearchBiomesInFolder()
    {
        BiomeSO[] allBiomesSO = Resources.LoadAll<BiomeSO>("SO/Biomes");
        BiomesLibrary.Clear();
        foreach (var biome in allBiomesSO)
        {
            BiomesLibrary.Add(biome);
        }
        foreach (var biome in BiomesLibrary)
        {
            BiomesDictionary.Add(biome.Name, biome);
        }
    }
    public BiomeSO GetBiomeByID(string id)
    {
        return BiomesDictionary[id];
    }
}
