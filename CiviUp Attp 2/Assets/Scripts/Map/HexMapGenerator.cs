using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    public int width = 80;
    public int height = 45;

    [Header("Prefabs & Data")]
    public GameObject hexPrefab;
    public BiomeDatabase biomeDatabase;

    [Header("Map Output")]
    public Transform mapRoot;

    [Header("Seeds")]
    public int seed = 0;

    [Header("Continents")]
    public float continentScale = 0.03f;
    public float landThreshold = 0.45f;
    public float oceanFalloffStrength = 0.6f;
    public int borderWaterSize = 2;

    [Header("Elevation")]
    public float elevationScale = 0.08f;

    [Header("Mountains")]
    public float mountainScale = 0.02f;
    public float mountainInfluence = 0.6f;

    private float hexWidth;
    private float hexHeight;

    [Header("Temperature")]
    public AnimationCurve temperatureByLatitude;

    private void Start()
    {
        if (seed == 0)
            seed = Random.Range(0, 999999);

        Random.InitState(seed);
        biomeDatabase.Init();

        if (mapRoot == null)
        {
        Debug.LogError("HexMapGenerator: mapRoot não definido");
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
        for (int col = 0; col < width; col++)
        {
            for (int row = 0; row < height; row++)
            {
                Vector2 pos = CalculateHexPosition(col, row);
                GameObject hex = Instantiate(hexPrefab, pos, Quaternion.identity, mapRoot);

                ProvinceView view = hex.GetComponent<ProvinceView>();
                if (view == null) continue;

                BiomeData biome = GenerateBiome(col, row);
                view.SetBiome(biome);
            }
        }
    }

    private BiomeData GenerateBiome(int col, int row)
    {
        if (IsBorder(col, row))
            return biomeDatabase.GetWaterBiome();

        float continent = Mathf.PerlinNoise(
            (col + seed) * continentScale,
            (row + seed) * continentScale
        );

        float falloff = GetOceanFalloff(col, row);
        continent -= falloff * oceanFalloffStrength;

        if (continent < landThreshold)
            return biomeDatabase.GetWaterBiome();

        float baseElevation = Mathf.PerlinNoise(
            (col + seed + 1000) * elevationScale,
            (row + seed + 1000) * elevationScale
        );

        float mountainMask = Mathf.PerlinNoise(
            (col + seed + 3000) * mountainScale,
            (row + seed + 3000) * mountainScale
        );

        float elevation = Mathf.Lerp(
            baseElevation,
            1f,
            mountainMask * mountainInfluence
        );


        float temperature = GetTemperature(row);

        return ChooseLandBiome(elevation, temperature);
    }

    private BiomeData ChooseLandBiome(float elevation, float temperature)
    {
        BiomeData chosen = null;
        int highestPriority = int.MinValue;

        foreach (var biome in biomeDatabase.biomes)
        {
            if (biome.isWater)
                continue;

            if (elevation < biome.minElevation ||
                elevation > biome.maxElevation)
                continue;

            if (temperature < biome.minTemperature ||
                temperature > biome.maxTemperature)
                continue;

            if (biome.priority > highestPriority)
            {
                highestPriority = biome.priority;
                chosen = biome;
            }
        }

        return chosen != null
            ? chosen
            : biomeDatabase.GetRandomLandBiome();
    }

    private float GetOceanFalloff(int col, int row)
    {
        float x = col / (float)(width - 1) * 2f - 1f;
        float y = row / (float)(height - 1) * 2f - 1f;

        float distance = Mathf.Sqrt(x * x + y * y); // 0 centro, ~1.4 cantos
        return Mathf.SmoothStep(0f, 1f, distance);
    }


    private bool IsBorder(int col, int row)
    {
        return col < borderWaterSize ||
               row < borderWaterSize ||
               col >= width - borderWaterSize ||
               row >= height - borderWaterSize;
    }

    private Vector2 CalculateHexPosition(float col, float row)
    {
        float x = col * hexWidth * 0.985f;
        float y = row * (hexHeight * 0.753f);

        if (row % 2 == 1)
            x += hexWidth / 2.04f;

        return new Vector2(x, y);
    }

    private float GetTemperature(int row)
    {
        float latitude01 = row / (float)(height - 1);
        return temperatureByLatitude.Evaluate(latitude01);
    }

}
