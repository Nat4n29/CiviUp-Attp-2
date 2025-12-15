using UnityEngine;

public class ProvinceView : MonoBehaviour
{
    public ProvinceData data;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void OnSelected()
    {
        Debug.Log("ProvinceView.OnSelected chamado");

    if (SelectionManager.Instance == null)
    {
        Debug.LogError("SelectionManager.Instance é NULL");
        return;
    }

    if (data == null)
    {
        Debug.LogError("ProvinceView.data é NULL");
        return;
    }

        SelectionManager.Instance.SelectProvince(data);
    }
}
