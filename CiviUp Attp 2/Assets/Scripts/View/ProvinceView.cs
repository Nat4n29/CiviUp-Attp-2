using UnityEngine;

public class ProvinceView : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private SpriteRenderer overlayRenderer;

    [Header("Overlay Settings")]
    [SerializeField] private Sprite selectionSprite;

    public ProvinceData Data { get; private set; }

    private void Awake()
    {
        if (overlayRenderer != null)
            overlayRenderer.enabled = false;
    }

    public void Init(ProvinceData province)
    {
        Data = province;
    }

    // =========================
    // BASE MAP
    // =========================

    public void SetBiome(BiomeData biome)
    {
        if (biome == null || baseRenderer == null)
            return;

        baseRenderer.sprite = biome.sprite;
    }

    public void SetBaseSprite(Sprite sprite)
    {
        if (baseRenderer == null)
            return;

        baseRenderer.sprite = sprite;
    }

    // =========================
    // OVERLAY API (UNIFICADA)
    // =========================

    public void SetSelected(bool selected)
    {
        if (overlayRenderer == null)
            return;

        if (selected)
        {
            overlayRenderer.sprite = selectionSprite;
            overlayRenderer.enabled = true;
        }
        else
        {
            ClearOverlay();
        }
    }

    public void SetOverlay(Sprite sprite, Color color)
    {
        if (overlayRenderer == null)
            return;

        overlayRenderer.sprite = sprite;
        overlayRenderer.color = color;
        overlayRenderer.enabled = true;
    }

    public void ClearOverlay()
    {
        if (overlayRenderer == null)
            return;

        overlayRenderer.enabled = false;
        overlayRenderer.sprite = null;
    }

    // =========================
    // INPUT CALLBACK
    // =========================

    public void OnSelected()
    {
        if (Data == null || SelectionManager.Instance == null)
            return;

        SelectionManager.Instance.SelectProvince(Data, this);
    }

}
