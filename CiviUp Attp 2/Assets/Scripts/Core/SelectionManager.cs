using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public ProvinceData selectedProvince;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SelectProvince(ProvinceData province)
    {
        selectedProvince = province;
        Debug.Log($"Selected province: {province.provinceName}");
    }
}
