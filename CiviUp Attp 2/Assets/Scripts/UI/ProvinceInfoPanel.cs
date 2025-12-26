using UnityEngine;
using TMPro;

public class ProvinceInfoPanel : MonoBehaviour
{
    [Header("UI Root")]
    public GameObject root;

    [Header("Texts")]
    public TextMeshProUGUI provinceName;
    public TextMeshProUGUI provinceId;
    public TextMeshProUGUI biomeText;
    public TextMeshProUGUI reliefText;
    public TextMeshProUGUI coastText;
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI countryText;

    private SelectionManager selectionManager;

    private void Start()
    {
        root.SetActive(false);

        selectionManager = FindFirstObjectByType<SelectionManager>();
        if (selectionManager == null)
            return;

        selectionManager.OnProvinceSelected += ShowProvince;
    }

    private void OnDestroy()
    {
        if (selectionManager != null)
            selectionManager.OnProvinceSelected -= ShowProvince;
    }

    private void ShowProvince(ProvinceData province)
    {
        if (province == null)
            return;

        root.SetActive(true);

        provinceName.text = province.provinceName;
        provinceId.text = $"ID: {province.id}";
        biomeText.text = $"Bioma: {province.biome?.biomeName ?? "-"}";
        reliefText.text = $"Relevo: {province.relief?.reliefName ?? "-"}";
        coastText.text = $"Costa: {(province.isCoastal ? "Sim" : "Não")}";
        stateText.text = $"Estado: {province.state?.stateName ?? "-"}";
        countryText.text = $"País: {province.country?.countryName ?? "-"}";
    }
}
