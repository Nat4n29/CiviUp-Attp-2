using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class StateData : ScriptableObject
{
    public int id;
    public string stateName;

    public CountryData country;
    public ProvinceData capital;

    public List<ProvinceData> provinces = new();
}
