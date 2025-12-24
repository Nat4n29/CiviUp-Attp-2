using UnityEngine;

public class ProvinceView : MonoBehaviour
{
    public ProvinceData Data { get; private set; }
    public BiomeData biome;

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
        if (Data == null)
        {
            Debug.LogError($"ProvinceView {name} sem ProvinceData");
            return;
        }

        if (SelectionManager.Instance == null)
        {
            Debug.LogError("SelectionManager.Instance é NULL");
            return;
        }

        Debug.Log($"ProvinceView.OnSelected: {Data?.provinceName}");

        SelectionManager.Instance.SelectProvince(Data);
    }
}

