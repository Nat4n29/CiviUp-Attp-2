using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    private HexMapGenerator mapGenerator;

    private void Awake()
    {
        mainCamera = Camera.main;
        mapGenerator = FindFirstObjectByType<HexMapGenerator>();

        if (mainCamera == null)
            Debug.LogError("InputManager: MainCamera NÃO encontrada");
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame) HandleClick();
    }

    private void HandleClick()
    {
        Vector2 mouseWorld =
            mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        ProvinceView view = FindProvinceByWorldPosition(mouseWorld);

        if (view == null)
            return;

        view.OnSelected();
    }

    private ProvinceView FindProvinceByWorldPosition(Vector2 worldPos)
    {
        if (mapGenerator == null)
            return null;

        return mapGenerator.GetProvinceByWorldPosition(worldPos);
    }

    /*private bool IsPlayerProvince(ProvinceView view)
    {
        return view.data.country == GameManager.Instance.currentCountry;
    }*/
}

