using UnityEngine;

[CreateAssetMenu(menuName = "Map/Relief")]
public class ReliefData : ScriptableObject
{
    [Header("Identity")]
    public string reliefName;

    [Header("Visual")]
    public Sprite sprite;

    [Header("Biome Rules")]
    public bool allowsBiome;
    public bool isWater;

    [Header("Generation Rules")]
    [Range(0f, 1f)]
    public float minHeight;
    [Range(0f, 1f)]
    public float maxHeight = 1f;

    [Range(0f, 1f)]
    public float minTemperature = 0f;
    [Range(0f, 1f)]
    public float maxTemperature = 1f;

    [Header("Behaviour")]
    public bool overridesBiome = true;
    public int priority = 0;
}
