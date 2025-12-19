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

    public void Init(ProvinceData province)
    {
        data = province;
    }

    public void SetBiome(BiomeData biome)
    {
        if (biome == null)
        {
            Debug.LogError($"SetBiome chamado com NULL em {name}");
            return;
        }

        this.biome = biome;
        spriteRenderer.sprite = biome.sprite;
    }

    public void SetBiomeSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void OnSelected()
    {
        if (data == null)
        {
            Debug.LogError($"ProvinceView {name} sem ProvinceData");
            return;
        }

        if (SelectionManager.Instance == null)
        {
            Debug.LogError("SelectionManager.Instance é NULL");
            return;
        }

        SelectionManager.Instance.SelectProvince(data);
    }
}
