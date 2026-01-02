using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/State")]
public class StateData : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string stateName;
    public Color stateColor;

    [Header("Political")]
    public CountryData country;

    [Header("Geography")]
    public CityData capital;
    public List<CityData> cities = new();
}
