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
        Debug.Log(">>> SelectionManager.SelectProvince() CHAMADO");

        SelectedProvince = province;
        OnProvinceSelected?.Invoke(province);

        Debug.Log($"Selected province ID: {province.id}");

        FindFirstObjectByType<ProvinceInfoPanel>()?.SendMessage("ShowProvince", province);
    }
}

