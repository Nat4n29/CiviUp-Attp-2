using UnityEngine;

public class OverlayManager : MonoBehaviour
{
    public static OverlayManager Instance;

    private ProvinceView current;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void HighlightProvince(ProvinceView view)
    {
        Clear();

        if (view == null)
            return;

        current = view;
        view.SetSelected(true);
    }

    public void Clear()
    {
        if (current != null)
        {
            current.ClearOverlay();
            current = null;
        }
    }
}
