using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Biome Database")]
public class BiomeDatabase : ScriptableObject
{
    public List<BiomeData> biomes = new();

    private Dictionary<int, BiomeData> lookup;
    private List<BiomeData> landBiomes;
    private BiomeData waterBiome;

    public void Init()
    {
        lookup = new Dictionary<int, BiomeData>();
        landBiomes = new List<BiomeData>();

        foreach (var biome in biomes)
        {
            lookup[biome.id] = biome;

            if (biome.isWater)
                waterBiome = biome;
            else
                landBiomes.Add(biome);
        }

        //if (waterBiome == null)Debug.LogError("BiomeDatabase: Nenhum bioma de agua definido");
    }

    public BiomeData GetById(int id)
    {
        return lookup[id];
    }

    public BiomeData GetWaterBiome()
    {
        return waterBiome;
    }

    public BiomeData GetRandomLandBiome()
    {
        return landBiomes[Random.Range(0, landBiomes.Count)];
    }
}
