using UnityEngine;

public class ProvinceView : MonoBehaviour
{
    public ProvinceData data;
    public BiomeData biome;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(ProvinceData province, BiomeData biome)
    {
        data = province;
        SetBiome(biome);
    }

    public void SetBiome(BiomeData biome)
    {
        this.biome = biome;
        spriteRenderer.sprite = biome.sprite;
    }

    public void OnSelected()
    {
        SelectionManager.Instance.SelectProvince(data);
    }

    public void SetBiomeSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}