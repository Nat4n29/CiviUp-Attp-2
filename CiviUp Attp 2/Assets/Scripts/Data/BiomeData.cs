using UnityEngine;

[CreateAssetMenu(menuName = "Map/Biome")]
public class BiomeData : ScriptableObject
{
    public int id;
    public string biomeName;
    public Sprite sprite;

    [Header("Gameplay")]
    public bool isWater;
    public bool isPassable = true;

    [Header("Generation Rules")]
    [Range(0f, 1f)]
    public float minElevation;
    [Range(0f, 1f)]
    public float maxElevation = 1f;

    [Range(0f, 1f)]
    public float minTemperature;
    [Range(0f, 1f)]
    public float maxTemperature = 1f;

    public int priority = 0;
}