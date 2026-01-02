using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Province")]
public class ProvinceData : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string provinceName;

    [Header("Geography")]
    public BiomeData biome;
    public ReliefData relief;
    public bool isCoastal;

    [Header("Political")]
    public CityData city;
    public StateData state;
    public CountryData country;

    [Header("Building")]
    [Range(1, 5)]
    public int maxConstructions = 5;
    public List<ConstructionType> constructions = new();

    [HideInInspector]
    public ProvinceView view;
}
