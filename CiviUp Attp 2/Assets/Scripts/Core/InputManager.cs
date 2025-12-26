using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private HexMapGenerator mapGenerator;

    private void Awake()
    {
        mainCamera = Camera.main;
        mapGenerator = FindFirstObjectByType<HexMapGenerator>();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            HandleClick();
    }

    private void HandleClick()
    {
        Vector2 mouseWorld =
            mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        ProvinceView view = mapGenerator?.GetProvinceByWorldPosition(mouseWorld);

        if (view != null)
            view.OnSelected();
    }
}
