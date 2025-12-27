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

    [Header("Wrap Visual")]
    public MapWrapVisual wrapVisual;

    [Header("Seed")]
    public int seed;

    [Header("Continents")]
    public float continentScale = 0.07f;
    public float landThreshold = 0.35f;
    public int borderWaterSize = 3;
    [Range(0.1f, 8f)]
    public float borderSmoothness = 3f;

    [Header("Elevation")]
    public float elevationScale = 0.08f;
    public float mountainLow = 0.6f;
    public float mountainHigh = 0.8f;

    [Header("Mountains")]
    public float mountainScale = 0.02f;
    public float mountainInfluence = 0.35f;

    [Header("Temperature")]
    public AnimationCurve temperatureByLatitude;
    public float temperatureNoiseScale = 0.05f;
    public float temperatureNoiseStrength = 0.15f;

    private float[,] heightMap;
    private BiomeData[,] baseBiomeMap;
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

        CacheHexMetrics();
        Generate();
    }

    private void CacheHexMetrics()
    {
        SpriteRenderer sr = hexPrefab.GetComponentInChildren<SpriteRenderer>();
        hexWidth = sr.bounds.size.x;
        hexHeight = sr.bounds.size.y;
    }

    private void Generate()
    {
        heightMap = new float[width, height];
        baseBiomeMap = new BiomeData[width, height];
        viewMap = new ProvinceView[width, height];
        provinceMap = new ProvinceData[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = CalculateHexPosition(x, y);
                GameObject hex = Instantiate(hexPrefab, pos, Quaternion.identity, mapRoot);

                ProvinceView view = hex.GetComponent<ProvinceView>();
                ProvinceData province = ScriptableObject.CreateInstance<ProvinceData>();

                province.id = x * height + y;
                province.provinceName = $"Province {x},{y}";
                province.constructions = new List<ConstructionType>();

                view.Init(province);

                provinceMap[x, y] = province;
                viewMap[x, y] = view;

                heightMap[x, y] = CalculateElevation(x, y);
                baseBiomeMap[x, y] = GenerateBaseBiome(x, y);
            }
        }

        ComposeFinalBiomes();

        mapCamera.CacheMapBounds();
        wrapVisual?.BuildVisualWrap();
    }

    private float CalculateElevation(int x, int y)
    {
        float continent = Mathf.PerlinNoise(
            (x + seed) * continentScale,
            (y + seed) * continentScale
        );

        float baseElevation = Mathf.PerlinNoise(
            (x + seed + 1000) * elevationScale,
            (y + seed + 1000) * elevationScale
        );

        float mountainMask = Mathf.PerlinNoise(
            (x + seed + 3000) * mountainScale,
            (y + seed + 3000) * mountainScale
        );

        float mountainFactor = Mathf.SmoothStep(mountainLow, mountainHigh, mountainMask);

        float elevation = Mathf.Lerp(
            baseElevation,
            baseElevation * 0.5f + 0.5f,
            mountainFactor * mountainInfluence
        );

        elevation *= Mathf.InverseLerp(landThreshold, 1f, continent);
        elevation *= GetBorderHeightFactor(x, y);

        return Mathf.Clamp01(elevation);
    }

    private BiomeData GenerateBaseBiome(int x, int y)
    {
        float temperature = GetTemperature(x, y);
        BiomeData best = null;
        int bestPriority = int.MinValue;

        foreach (var biome in biomeDatabase.biomes)
        {
            if (biome.isWater)
                continue;

            if (temperature < biome.minTemperature || temperature > biome.maxTemperature)
                continue;

            if (biome.priority > bestPriority)
            {
                bestPriority = biome.priority;
                best = biome;
            }
        }

        return best;
    }

    private void ComposeFinalBiomes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                ProvinceView view = viewMap[x, y];
                ProvinceData province = view.Data;

                float height = heightMap[x, y];
                float temperature = GetTemperature(x, y);

                ReliefData relief = reliefDatabase.GetRelief(height, temperature);
                province.relief = relief;

                if (relief != null)
                    view.SetBiomeSprite(relief.sprite);

                if (relief == null || relief.allowsBiome)
                {
                    province.biome = baseBiomeMap[x, y];
                    view.SetBiome(baseBiomeMap[x, y]);
                }
            }
        }

        // AGORA que tudo existe, calculamos a costa
        CalculateCoasts();
    }

    private float GetTemperature(int x, int y)
    {
        float latitude = y / (float)(height - 1);
        float baseTemp = temperatureByLatitude.Evaluate(latitude);

        float noise = Mathf.PerlinNoise(
            (x + seed + 5000) * temperatureNoiseScale,
            (y + seed + 5000) * temperatureNoiseScale
        );

        return Mathf.Clamp01(baseTemp + (noise - 0.5f) * temperatureNoiseStrength);
    }

    private float GetBorderHeightFactor(int x, int y)
    {
        int d = Mathf.Min(
            Mathf.Min(x, width - 1 - x),
            Mathf.Min(y, height - 1 - y)
        );

        if (d >= borderWaterSize)
            return 1f;

        float t = d / (float)borderWaterSize;
        return Mathf.SmoothStep(0f, 1f, Mathf.Pow(t, borderSmoothness));
    }

    public void RegenerateMap(bool randomSeed)
    {
        if (mapRoot == null)
            return;


        for (int i = mapRoot.childCount - 1; i >= 0; i--)
            DestroyImmediate(mapRoot.GetChild(i).gameObject);

        if (randomSeed)
            seed = Random.Range(0, 999999);

        Random.InitState(seed);

        Generate();

        mapCamera.CacheMapBounds();
        wrapVisual?.BuildVisualWrap();
    }

    private bool IsCoastal(int x, int y)
    {
        ProvinceData province = provinceMap[x, y];

        // Província de água nunca é costeira
        if (IsWaterProvince(province))
            return false;

        bool oddRow = (y & 1) == 1;

        int[,] evenRowNeighbors =
        {
        { -1,  0 }, { 1,  0 },
        { -1, -1 }, { 0, -1 },
        { -1,  1 }, { 0,  1 }
    };

        int[,] oddRowNeighbors =
        {
        { -1,  0 }, { 1,  0 },
        {  0, -1 }, { 1, -1 },
        {  0,  1 }, { 1,  1 }
    };

        int[,] neighbors = oddRow ? oddRowNeighbors : evenRowNeighbors;

        for (int i = 0; i < neighbors.GetLength(0); i++)
        {
            int nx = WrapX(x + neighbors[i, 0]);
            int ny = y + neighbors[i, 1];

            if (ny < 0 || ny >= height)
                continue;

            ProvinceData neighbor = provinceMap[nx, ny];

            if (IsWaterProvince(neighbor))
                return true;
        }

        return false;
    }

    private void CalculateCoasts()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                provinceMap[x, y].isCoastal = IsCoastal(x, y);
            }
        }
    }

    public ProvinceView GetProvinceByWorldPosition(Vector2 worldPos)
    {
        Vector2 local = worldPos - (Vector2)mapRoot.position;

        float mapWidth = (width - 1) * hexWidth * 0.985f + hexWidth;
        local.x = Mathf.Repeat(local.x, mapWidth);

        int col = Mathf.RoundToInt(local.x / (hexWidth * 0.985f));
        int row = Mathf.RoundToInt(local.y / (hexHeight * 0.753f));

        if (col < 0 || col >= width || row < 0 || row >= height)
            return null;

        return viewMap[col, row];
    }

    private Vector2 CalculateHexPosition(float col, float row)
    {
        float x = col * hexWidth * 0.985f;
        float y = row * (hexHeight * 0.753f);
        if ((int)row % 2 == 1)
            x += hexWidth / 2.027f;

        return new Vector2(x, y);
    }

    private bool IsWaterProvince(ProvinceData province)
    {
        if (province == null)
            return false;

        if (province.relief != null && province.relief.isWater)
            return true;

        if (province.biome != null && province.biome.isWater)
            return true;

        return false;
    }

    private int WrapX(int x)
    {
        if (x < 0)
            return x + width;
        if (x >= width)
            return x - width;
        return x;
    }

    public float HexWidth => hexWidth;
    public float HexHeight => hexHeight;
}
