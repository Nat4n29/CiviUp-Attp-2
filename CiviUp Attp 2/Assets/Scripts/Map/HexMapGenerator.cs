using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    public int width = 160;
    public int height = 90;

    [Header("Camera")]
    public MapCameraController mapCamera;

    [Header("Prefabs & Data")]
    public GameObject hexPrefab;
    public BiomeDatabase biomeDatabase;
    public ReliefDatabase reliefDatabase;

    [Header("Map Output")]
    public Transform mapRoot;

    [Header("Seed")]
    public int seed = 0;

    [Header("Continents")]
    public float continentScale = 0.07f;
    public float landThreshold = 0.35f;
    public float oceanFalloffStrength = 0.4f;
    public int borderWaterSize = 3;

    [Header("Elevation")]
    public float elevationScale = 0.08f;
    public float mountainLow = 0.6f;
    public float mountainHigh = 0.8f;

    [Header("Mountains")]
    public float mountainScale = 0.02f;
    public float mountainInfluence = 0.35f;

    [Header("Biome Base Noise")]
    public float biomeNoiseScale = 0.05f;

    [Header("Temperature")]
    public AnimationCurve temperatureByLatitude;

    [Header("Temperature Noise")]
    public float temperatureNoiseScale = 0.05f;
    public float temperatureNoiseStrength = 0.15f;

    private float[,] heightMap;
    private BiomeData[,] baseBiomeMap;
    private BiomeData[,] finalBiomeMap;
    private ProvinceView[,] viewMap;
    private ProvinceData[,] provinceMap;

    private float hexWidth;
    private float hexHeight;

    private void Start()
    {
        if (seed == 0)
            seed = Random.Range(0, 999999);

        Random.InitState(seed);
        biomeDatabase.Init();

        if (mapCamera == null)
        {
            Debug.LogError("HexMapGenerator: mapCamera não definido");
            return;
        }

        CacheHexMetrics();
        Generate();
    }

    private void CacheHexMetrics()
    {
        SpriteRenderer sr = hexPrefab.GetComponent<SpriteRenderer>();
        hexWidth = sr.bounds.size.x;
        hexHeight = sr.bounds.size.y;
    }

    private void Generate()
    {
        heightMap     = new float[width, height];
        baseBiomeMap  = new BiomeData[width, height];
        finalBiomeMap = new BiomeData[width, height];
        viewMap       = new ProvinceView[width, height];
        provinceMap = new ProvinceData[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = CalculateHexPosition(x, y);
                GameObject hex = Instantiate(hexPrefab, pos, Quaternion.identity, mapRoot);

                ProvinceView view = hex.GetComponent<ProvinceView>();

                if (view == null)
                {
                    Debug.LogError($"Hex prefab sem ProvinceView em {x},{y}");
                    continue;
                }

                // CRIA ProvinceData EM RUNTIME
                ProvinceData province = ScriptableObject.CreateInstance<ProvinceData>();
                province.id = x * height + y;
                province.provinceName = $"Province {x},{y}";
                province.constructions = new List<ConstructionType>();

                provinceMap[x, y] = province;
                viewMap[x, y] = view;

                // CALCULA MAPAS
                heightMap[x, y] = CalculateElevation(x, y);
                baseBiomeMap[x, y] = GenerateBaseBiome(x, y);

                // INICIALIZA O VIEW (IMPORTANTE)
                view.Init(province);
            }
        }

        ComposeFinalBiomes();
        mapCamera.CacheMapBounds();
    }

    // =========================
    // WORLD PHYSICS
    // =========================

    private float CalculateElevation(int x, int y)
    {
        if (IsBorder(x, y))
            return 0f;

        float continent = Mathf.PerlinNoise(
            (x + seed) * continentScale,
            (y + seed) * continentScale
        );

        continent -= GetOceanFalloff(x, y) * oceanFalloffStrength;

        if (continent < landThreshold)
            return 0f;

        float baseElevation = Mathf.PerlinNoise(
            (x + seed + 1000) * elevationScale,
            (y + seed + 1000) * elevationScale
        );

        float mountainMask = Mathf.PerlinNoise(
            (x + seed + 3000) * mountainScale,
            (y + seed + 3000) * mountainScale
        );

        float mountainFactor = Mathf.SmoothStep(0.6f, 0.8f, mountainMask);

        return Mathf.Lerp(
            baseElevation,
            baseElevation * 0.5f + 0.5f,
            mountainFactor * mountainInfluence
        );
    }

    // =========================
    // BIOME BASE (LOWLAND ONLY)
    // =========================

    private BiomeData GenerateBaseBiome(int x, int y)
    {
        float temperature = GetTemperature(x, y);

        float biomeNoise = Mathf.PerlinNoise(
            (x + seed + 7000) * biomeNoiseScale,
            (y + seed + 7000) * biomeNoiseScale
        );

        BiomeData chosen = null;
        int bestPriority = int.MinValue;

        foreach (var biome in biomeDatabase.biomes)
        {
            if (biome.isWater)
                continue;

            if (temperature < biome.minTemperature ||
                temperature > biome.maxTemperature)
                continue;

            if (biome.priority > bestPriority)
            {
                bestPriority = biome.priority;
                chosen = biome;
            }
        }

        return chosen;
    }

    // =========================
    // FINAL COMPOSITION
    // =========================

    private void ComposeFinalBiomes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float height = heightMap[x, y];
                float temperature = GetTemperature(x, y);

                if (height <= 0f)
                {
                    ApplyBiome(x, y, biomeDatabase.GetWaterBiome());
                    continue;
                }

                ReliefData relief = reliefDatabase.GetRelief(height, temperature);

                if (relief != null && relief.overridesBiome)
                {
                    viewMap[x, y].SetBiomeSprite(relief.sprite);
                    continue;
                }

                ApplyBiome(x, y, baseBiomeMap[x, y]);
            }
        }
    }

    // =========================
    // HELPERS
    // =========================

    private float GetTemperature(int x, int y)
    {
        float latitude01 = y / (float)(height - 1);
        float baseTemp = temperatureByLatitude.Evaluate(latitude01);

        float noise = Mathf.PerlinNoise(
            (x + seed + 5000) * temperatureNoiseScale,
            (y + seed + 5000) * temperatureNoiseScale
        );

        return Mathf.Clamp01(
            baseTemp + (noise - 0.5f) * temperatureNoiseStrength
        );
    }

    private void ApplyBiome(int x, int y, BiomeData biome)
    {
        if (viewMap[x, y] == null)
            return;

        finalBiomeMap[x, y] = biome;
        viewMap[x, y].SetBiome(biome);
    }

    private float GetOceanFalloff(int x, int y)
    {
        float nx = x / (float)(width - 1) * 2f - 1f;
        float ny = y / (float)(height - 1) * 2f - 1f;
        return Mathf.SmoothStep(0f, 1f, Mathf.Sqrt(nx * nx + ny * ny));
    }

    private bool IsBorder(int x, int y)
    {
        return x < borderWaterSize || y < borderWaterSize ||
               x >= width - borderWaterSize || y >= height - borderWaterSize;
    }

    private Vector2 CalculateHexPosition(float col, float row)
    {
        float x = col * hexWidth * 0.985f;
        float y = row * (hexHeight * 0.753f);
        if ((int)row % 2 == 1)
            x += hexWidth / 2.04f;
        return new Vector2(x, y);
    }

    public void RegenerateMap(bool isRandomSeed)
    {
        if (mapRoot == null)
        {
            Debug.LogError("HexMapGenerator: mapRoot não definido");
            return;
        }

        // Limpa o mapa atual
        for (int i = mapRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(mapRoot.GetChild(i).gameObject);
        }

        if(isRandomSeed == true)
        {
            seed = Random.Range(0, 999999);
        }
        Random.InitState(seed);

        Generate();

        mapCamera.CacheMapBounds();

        // Suavização final
        /*for (int i = 0; i < 2; i++)
            SmoothBiomes();*/
    }

    public float HexWidth => hexWidth;
    public float HexHeight => hexHeight;
}
