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

    /*private void Awake()
    {
        root.SetActive(false);
    }*/

    private void Start()
    {
        root.SetActive(false);

        selectionManager = FindFirstObjectByType<SelectionManager>();

        if (selectionManager == null)
        {
            Debug.LogError("ProvinceInfoPanel: SelectionManager NÃO encontrado");
            return;
        }

        selectionManager.OnProvinceSelected += ShowProvince;

        Debug.Log("ProvinceInfoPanel: inscrito com sucesso no SelectionManager");
    }

    private void OnDestroy()
    {
        if (selectionManager != null)
            selectionManager.OnProvinceSelected -= ShowProvince;
    }

    private void ShowProvince(ProvinceData province)
    {
        if (province == null)
        {
            root.SetActive(false);
            return;
        }

        root.SetActive(true);

        provinceName.text = province.provinceName;
        provinceId.text = $"ID: {province.id}";
        biomeText.text = $"Bioma: {province.biome?.biomeName ?? "Nenhum"}";
        reliefText.text = $"Relevo: {province.relief?.reliefName ?? "Nenhum"}";
        coastText.text = $"Costa: {(province.isCoastal ? "Sim" : "Não")}";
        stateText.text = $"Estado: {province.state?.stateName ?? "-"}";
        countryText.text = $"País: {province.country?.countryName ?? "-"}";
    }
}

