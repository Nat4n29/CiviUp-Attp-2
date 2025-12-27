using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        // BLOQUEIA input se o mouse estiver sobre UI
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            HandleLeftClick();

        if (Mouse.current.rightButton.wasPressedThisFrame)
            HandleRightClick();
    }


    private void HandleLeftClick()
    {
        Vector2 mouseWorld =
            mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);

        if (!hit)
            return;

        ProvinceView view = hit.collider.GetComponent<ProvinceView>();
        if (view == null)
            return;

        view.OnSelected();
    }

    private void HandleRightClick()
    {
        if (SelectionManager.Instance != null)
            SelectionManager.Instance.ClearSelection();
    }
}
