using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProvinceView : MonoBehaviour
{
    public ProvinceData Data { get; private set; }
    public BiomeData Biome { get; private set; }

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(ProvinceData province)
    {
        Data = province;
    }

    public void SetBiome(BiomeData biome)
    {
        if (biome == null)
            return;

        Biome = biome;
        spriteRenderer.sprite = biome.sprite;
    }

    public void SetBiomeSprite(Sprite sprite)
    {
        if (sprite == null)
            return;

        spriteRenderer.sprite = sprite;
    }

    public void OnSelected()
    {
        if (Data == null || SelectionManager.Instance == null)
            return;

        SelectionManager.Instance.SelectProvince(Data);
    }
}
