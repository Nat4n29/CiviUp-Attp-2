using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsDebug : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 worldPos =
            Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Collider2D col = Physics2D.OverlapPoint(worldPos);

        if (col == null)
        {
            Debug.Log("NO COLLIDER HIT");
            return;
        }

        Debug.Log("HIT COLLIDER: " + col.name);
    }
}
