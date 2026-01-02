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

        if (MapRenderManager.Instance != null)
            transform.SetParent(MapRenderManager.Instance.baseMapLayer, true);
    }


    private void OnDestroy()
    {
        ProvinceViewRegistry.Unregister(this);
    }

    // =========================
    // BASE MAP
    // =========================

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

    // =========================
    // OVERLAYS
    // =========================

    public void SetSelected(bool selected)
    {
        if (overlayRenderer == null)
            return;

        overlayRenderer.enabled = selected;

        if (selected)
        {
            transform.SetParent(
                MapRenderManager.Instance.selectionOverlayLayer,
                true
            );
        }
        else
        {
            transform.SetParent(
                MapRenderManager.Instance.baseMapLayer,
                true
            );
        }
    }

    public void OnSelected()
    {
        if (Data == null || SelectionManager.Instance == null)
            return;

        SelectionManager.Instance.SelectProvince(Data);
    }
}
