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
    public int seed = 0;

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

        if (mapCamera == null || mapRoot == null)
        {
            Debug.LogError("HexMapGenerator: referências não definidas");
            return;
        }

        CacheHexMetrics();
        Generate();
    }

    private void CacheHexMetrics()
    {
        if (hexPrefab == null)
        {
            Debug.LogError("HexMapGenerator: hexPrefab não definido");
            return;
        }

        SpriteRenderer sr = hexPrefab.GetComponentInChildren<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError(
                "HexMapGenerator: Nenhum SpriteRenderer encontrado no prefab. " +
                "Verifique se existe um BaseRenderer com SpriteRenderer."
            );
            return;
        }

        hexWidth = sr.bounds.size.x;
        hexHeight = sr.bounds.size.y;
    }

    private void Generate()
    {
        heightMap = new float[width, height];
        baseBiomeMap = new BiomeData[width, height];
        finalBiomeMap = new BiomeData[width, height];
        viewMap = new ProvinceView[width, height];
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

                ProvinceData province = ScriptableObject.CreateInstance<ProvinceData>();
                province.id = x * height + y;
                province.provinceName = $"Province {x},{y}";
                province.constructions = new List<ConstructionType>();

                provinceMap[x, y] = province;
                viewMap[x, y] = view;

                heightMap[x, y] = CalculateElevation(x, y);
                baseBiomeMap[x, y] = GenerateBaseBiome(x, y);

                view.Init(province);
                province.view = view;
            }
        }

        ComposeFinalBiomes();
        mapCamera.CacheMapBounds();

        if (wrapVisual != null)
        {
            wrapVisual.BuildVisualWrap();
        }
    }

    // =========================
    // WORLD PHYSICS
    // =========================

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

        float mountainFactor = Mathf.SmoothStep(
            mountainLow,
            mountainHigh,
            mountainMask
        );

        float elevation = Mathf.Lerp(
            baseElevation,
            baseElevation * 0.5f + 0.5f,
            mountainFactor * mountainInfluence
        );

        float continentMask = Mathf.InverseLerp(landThreshold, 1f, continent);
        elevation *= continentMask;

        elevation *= GetBorderHeightFactor(x, y);

        return Mathf.Clamp01(elevation);
    }

    // =========================
    // BIOME BASE (LOWLAND)
    // =========================

    private BiomeData GenerateBaseBiome(int x, int y)
    {
        float temperature = GetTemperature(x, y);

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
                ProvinceView view = viewMap[x, y];
                ProvinceData province = view.Data;

                float height = heightMap[x, y];
                float temperature = GetTemperature(x, y);

                ReliefData relief = reliefDatabase.GetRelief(height, temperature);
                province.relief = relief;

                if (relief != null)
                    view.SetBaseSprite(relief.sprite);

                if (relief == null || relief.allowsBiome)
                {
                    province.biome = baseBiomeMap[x, y];
                    view.SetBiome(baseBiomeMap[x, y]);
                }

                province.isCoastal = IsCoastal(x, y);
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
        if (viewMap[x, y] == null || biome == null)
            return;

        finalBiomeMap[x, y] = biome;
        viewMap[x, y].SetBiome(biome);
    }

    private float GetBorderHeightFactor(int x, int y)
    {
        int distX = Mathf.Min(x, width - 1 - x);
        int distY = Mathf.Min(y, height - 1 - y);
        int distToBorder = Mathf.Min(distX, distY);

        if (distToBorder >= borderWaterSize)
            return 1f;

        float t = distToBorder / (float)borderWaterSize;
        return Mathf.SmoothStep(0f, 1f, Mathf.Pow(t, borderSmoothness));
    }

    private Vector2 CalculateHexPosition(float col, float row)
    {
        float x = col * hexWidth * 0.985f;
        float y = row * (hexHeight * 0.753f);

        if ((int)row % 2 == 1)
            x += hexWidth / 2.027f;

        return new Vector2(x, y);
    }

    public void RegenerateMap(bool randomSeed)
    {
        for (int i = mapRoot.childCount - 1; i >= 0; i--)
            Destroy(mapRoot.GetChild(i).gameObject);

        if (randomSeed)
            seed = Random.Range(0, 999999);

        Random.InitState(seed);
        Generate();
        mapCamera.CacheMapBounds();

        if (wrapVisual != null)
        {
            wrapVisual.BuildVisualWrap();
        }
    }

    private bool IsCoastal(int x, int y)
    {
        if (heightMap[x, y] <= 0f)
            return false;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                    continue;

                if (heightMap[nx, ny] <= 0f)
                    return true;
            }
        }

        return false;
    }

    public ProvinceView GetProvinceByWorldPosition(Vector2 worldPos)
    {
        // Converte posição do mundo para posição RELATIVA ao mapa principal
        Vector2 local = worldPos - (Vector2)mapRoot.position;

        float mapWidth = (width - 1) * hexWidth * 0.985f + hexWidth;

        // WRAP horizontal
        local.x = Mathf.Repeat(local.x, mapWidth);

        // Converte posição local em grid aproximado
        int col = Mathf.RoundToInt(local.x / (hexWidth * 0.985f));
        int row = Mathf.RoundToInt(local.y / (hexHeight * 0.753f));

        if (col < 0 || col >= width || row < 0 || row >= height)
            return null;

        return viewMap[col, row];
    }

    public ProvinceView GetViewByProvince(ProvinceData province)
    {
        if (province == null)
            return null;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (provinceMap[x, y] == province)
                    return viewMap[x, y];
            }
        }

        return null;
    }


    public float HexWidth => hexWidth;
    public float HexHeight => hexHeight;
}

