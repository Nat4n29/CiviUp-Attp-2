using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    public int width = 80;
    public int height = 45;

    [Header("Prefabs & Data")]
    public GameObject hexPrefab;
    public BiomeDatabase biomeDatabase;

    [Header("Seeds")]
    public int seed = 0;

    [Header("Continents")]
    public float continentScale = 0.03f;
    public float landThreshold = 0.45f;
    public float oceanFalloffStrength = 0.6f;
    public int borderWaterSize = 2;

    [Header("Elevation")]
    public float elevationScale = 0.08f;

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
                GameObject hex = Instantiate(hexPrefab, pos, Quaternion.identity, transform);

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

        float elevation = Mathf.PerlinNoise(
            (col + seed + 1000) * elevationScale,
            (row + seed + 1000) * elevationScale
        );

        float latitude = Mathf.Abs((row / (float)height) * 2f - 1f);

        return ChooseLandBiome(elevation, latitude);
    }

    private BiomeData ChooseLandBiome(float elevation, float latitude)
    {
        if (elevation > 0.85f)
            return biomeDatabase.GetById(4); // Montanha Alta

        if (elevation > 0.7f)
            return biomeDatabase.GetById(3); // Montanha Baixa

        if (latitude < 0.25f && elevation < 0.5f)
            return biomeDatabase.GetById(2); // Deserto

        if (elevation < 0.45f)
            return biomeDatabase.GetById(0); // Planície

        return biomeDatabase.GetById(1); // Floresta
    }

    private float GetOceanFalloff(int col, int row)
    {
        float x = col / (float)width * 2f - 1f;
        float y = row / (float)height * 2f - 1f;
        return Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
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
}
