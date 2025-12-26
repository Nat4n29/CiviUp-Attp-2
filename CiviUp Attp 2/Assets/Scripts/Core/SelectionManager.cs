using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public ProvinceData SelectedProvince { get; private set; }

    public delegate void ProvinceSelected(ProvinceData province);
    public event ProvinceSelected OnProvinceSelected;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SelectProvince(ProvinceData province)
    {
        ClearSelection();
        SelectedProvince = province;

        foreach (var view in ProvinceViewRegistry.GetViews(province))
            view.SetSelected(true);

        OnProvinceSelected?.Invoke(province);
    }

    public void ClearSelection()
    {
        if (SelectedProvince != null)
        {
            foreach (var view in ProvinceViewRegistry.GetViews(SelectedProvince))
                view.SetSelected(false);
        }

        SelectedProvince = null;
        OnProvinceSelected?.Invoke(null);
    }
}
