using UnityEngine;

public class ProvinceView : MonoBehaviour
{
    public ProvinceData Data { get; private set; }

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private SpriteRenderer overlayRenderer;

    private void Awake()
    {
        if (overlayRenderer != null)
            overlayRenderer.enabled = false;
    }

    public void Init(ProvinceData province)
    {
        Data = province;
        ProvinceViewRegistry.Register(this);
    }

    private void OnDestroy()
    {
        ProvinceViewRegistry.Unregister(this);
    }

    public void SetSelected(bool selected)
    {
        if (overlayRenderer != null)
            overlayRenderer.enabled = selected;
    }

    public void SetBiome(BiomeData biome)
    {
        if (baseRenderer != null && biome != null)
            baseRenderer.sprite = biome.sprite;
    }

    public void SetBiomeSprite(Sprite sprite)
    {
        if (baseRenderer != null)
            baseRenderer.sprite = sprite;
    }

    public void OnSelected()
    {
        if (Data == null || SelectionManager.Instance == null)
            return;

        SelectionManager.Instance.SelectProvince(Data);
    }
}
