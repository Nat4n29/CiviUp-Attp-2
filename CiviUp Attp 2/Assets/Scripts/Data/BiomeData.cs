using UnityEngine;

[CreateAssetMenu(menuName = "Map/Biome")]
public class BiomeData : ScriptableObject
{
    public int id;
    public string biomeName;

    public Sprite sprite;

    [Header ("Gameplay")]
    public bool isWater;
    public bool isPassable = true;
}
