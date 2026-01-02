using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Turn Control")]
    public int currentTurn = 1;
    public CountryData currentCountry;

    [Header("Game Data")]
    public List<CountryData> countries = new();

    private int countryIndex = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        countryIndex = 0;
        currentTurn = 1;
        currentCountry = countries[countryIndex];

        Debug.Log($"Game started. Turn {currentTurn}");
        StartCountryTurn();
    }

    public void EndTurn()
    {
        ProcessEndOfCountryTurn();

        countryIndex++;

        if (countryIndex >= countries.Count)
        {
            countryIndex = 0;
            currentTurn++;
            Debug.Log($"New Turn: {currentTurn}");
        }

        currentCountry = countries[countryIndex];
        StartCountryTurn();
    }

    private void StartCountryTurn()
    {
        Debug.Log($"Country turn: {currentCountry.countryName}");
        ProcessCountryTurn(currentCountry);
    }

    private void ProcessCountryTurn(CountryData country)
    {
        foreach (var state in country.states)
        {
            ProcessState(state);
        }
    }

    private void ProcessState(StateData state)
    {
        foreach (var city in state.cities)
        {
            ProcessCity(city);
        }
    }

    private void ProcessCity(CityData city)
    {
        // economia, produção, eventos
    }

    private void ProcessProvince(ProvinceData province)
    {
        
    }

    private void ProcessEndOfCountryTurn()
    {
        // limpeza, upkeep, custos
    }
}
