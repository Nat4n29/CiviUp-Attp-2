using UnityEngine;

public class MapRenderManager : MonoBehaviour
{
    public static MapRenderManager Instance;

    [Header("Layers")]
    public Transform baseMapLayer;
    public Transform selectionOverlayLayer;
    public Transform stateOverlayLayer;
    public Transform countryOverlayLayer;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Transform GetOverlayLayer(OverlayMode mode)
    {
        return mode switch
        {
            OverlayMode.Selection => selectionOverlayLayer,
            OverlayMode.State => stateOverlayLayer,
            OverlayMode.Country => countryOverlayLayer,
            _ => null
        };
    }

    public void SetOverlayActive(OverlayMode mode, bool active)
    {
        Transform layer = GetOverlayLayer(mode);
        if (layer != null)
            layer.gameObject.SetActive(active);
    }
}
