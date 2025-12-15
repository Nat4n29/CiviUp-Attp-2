using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Province")]
public class ProvinceData : ScriptableObject
{
    public int id;
    public string provinceName;

    public StateData state;
    public CountryData country;

    [Range(1, 5)]
    public int maxConstructions = 5;

    public List<ConstructionType> constructions = new();
}
