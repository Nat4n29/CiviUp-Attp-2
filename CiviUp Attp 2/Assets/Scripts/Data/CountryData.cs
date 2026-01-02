using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Country")]
public class CountryData : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string countryName;
    public Color countryColor;

    [Header("Geography")]
    public CityData capitalCity;

    [Header("Political")]
    public List<StateData> states = new();
    public List<CityData> cities = new();
}
