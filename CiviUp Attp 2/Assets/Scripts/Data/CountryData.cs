using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Country")]
public class CountryData : ScriptableObject
{
    public int id;
    public string countryName;

    public ProvinceData capitalProvince;

    public List<StateData> states = new();
    public List<ProvinceData> provinces = new();
}
