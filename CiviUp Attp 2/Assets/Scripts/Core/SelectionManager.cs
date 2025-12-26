using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public ProvinceData SelectedProvince { get; private set; }
    private ProvinceView selectedView;

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

    public void SelectProvince(ProvinceData province, ProvinceView view)
    {
        // Remove seleção anterior
        ClearSelection();

        SelectedProvince = province;
        selectedView = view;

        selectedView.SetSelected(true);

        OnProvinceSelected?.Invoke(province);
    }

    public void ClearSelection()
    {
        if (selectedView != null)
        {
            selectedView.SetSelected(false);
            selectedView = null;
        }

        SelectedProvince = null;
        OnProvinceSelected?.Invoke(null);
    }
}
