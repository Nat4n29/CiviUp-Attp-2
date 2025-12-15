using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

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
        Debug.Log("HandleClick chamado");

        Vector2 mouseWorldPos =
        mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Collider2D col = Physics2D.OverlapPoint(
        mouseWorldPos,
        LayerMask.GetMask("Province")
        );

        if (col == null)
        {
            Debug.Log("OverlapPoint NÃO encontrou collider");
            return;
        }

        Debug.Log("Collider encontrado: " + col.name);

        ProvinceView view = col.GetComponent<ProvinceView>();

        if (view == null)
        {
        Debug.Log("Collider NÃO tem ProvinceView");
        return;
        }

        Debug.Log("ProvinceView encontrada: " + view.name);
        view.OnSelected();
    }

    /*private bool IsPlayerProvince(ProvinceView view)
    {
        return view.data.country == GameManager.Instance.currentCountry;
    }*/
}
