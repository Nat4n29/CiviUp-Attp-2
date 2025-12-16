using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    public int width = 20;
    public int height = 15;

    public GameObject hexPrefab;

    private float hexWidth;
    private float hexHeight;

    private void Start()
    {
        CacheHexMetrics();
        Generate();
    }

    private void CacheHexMetrics()
    {
        SpriteRenderer sr = hexPrefab.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError("Hex prefab precisa de SpriteRenderer");
            return;
        }

        // tamanho REAL do sprite no mundo
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

                Instantiate(hexPrefab, pos, Quaternion.identity, transform);
            }
        }
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
