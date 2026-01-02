using UnityEngine;

public class OverlayController : MonoBehaviour
{
    public OverlayMode currentMode = OverlayMode.None;

    public void SetOverlayMode(OverlayMode mode)
    {
        currentMode = mode;

        MapRenderManager.Instance.SetOverlayActive(OverlayMode.State, false);
        MapRenderManager.Instance.SetOverlayActive(OverlayMode.Country, false);

        if (mode != OverlayMode.None)
            MapRenderManager.Instance.SetOverlayActive(mode, true);
    }
}
