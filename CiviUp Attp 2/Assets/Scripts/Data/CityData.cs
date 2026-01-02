using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/City")]
public class CityData : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string cityName;
    public Color cityColor;

    [Header("Political")]
    public StateData state;
    public CountryData country;

    [Header("Geography")]
    public ProvinceData centralProvince;
    public List<ProvinceData> provinces = new();

    [Header("Gameplay")]
    [Range(1, 36)]
    public int maxProvinces = 36;

    public bool IsFull => provinces.Count >= maxProvinces;
}
