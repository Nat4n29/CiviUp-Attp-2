using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MapCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform mapRoot;
    public HexMapGenerator mapGenerator;

    [Header("Movement")]
    public float dragSpeed = 1f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float zoomSmoothTime = 0.12f;
    public float minZoom = 3f;
    public float maxZoom = 20f;

    [Header("Wrap")]
    public bool horizontalWrap = true;

    private Camera cam;

    private Vector3 targetPosition;
    private float targetZoom;
    private float zoomVelocity;

    private Vector3 dragAnchorWorld;

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private float mapWidthWorld;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;

        targetPosition = transform.position;
        targetZoom = cam.orthographicSize;
    }

    private void Start()
    {
        CacheMapBounds();
        ClampTarget();
        ApplyImmediate();
    }

    private void Update()
    {
        HandleDrag();
        HandleZoom();
        HandleHorizontalWrap();

        ClampTarget();

        transform.position = targetPosition;

        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize,
            targetZoom,
            ref zoomVelocity,
            zoomSmoothTime
        );
    }

    // =========================
    // INPUT
    // =========================

    private void HandleDrag()
    {
        if (!Input.GetMouseButton(2))
        {
            dragAnchorWorld = Vector3.zero;
            return;
        }

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        if (dragAnchorWorld == Vector3.zero)
        {
            dragAnchorWorld = mouseWorld;
            return;
        }

        Vector3 delta = dragAnchorWorld - mouseWorld;
        targetPosition += delta * dragSpeed;
    }

    private void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) < 0.01f)
            return;

        targetZoom = Mathf.Clamp(
            targetZoom - scroll * zoomSpeed,
            minZoom,
            maxZoom
        );
    }

    // =========================
    // WRAP HORIZONTAL
    // =========================

    private void HandleHorizontalWrap()
    {
        if (!horizontalWrap)
            return;

        if (targetPosition.x < minBounds.x - mapWidthWorld * 0.5f)
        {
            targetPosition.x += mapWidthWorld;
            transform.position += Vector3.right * mapWidthWorld;
            dragAnchorWorld = Vector3.zero; // ← ESSENCIAL
        }
        else if (targetPosition.x > maxBounds.x + mapWidthWorld * 0.5f)
        {
            targetPosition.x -= mapWidthWorld;
            transform.position += Vector3.left * mapWidthWorld;
            dragAnchorWorld = Vector3.zero; // ← ESSENCIAL
        }
    }


    // =========================
    // BOUNDS
    // =========================

    public void CacheMapBounds()
    {
        if (mapGenerator == null || mapRoot == null)
            return;

        float hexWidth = mapGenerator.HexWidth;
        float hexHeight = mapGenerator.HexHeight;

        mapWidthWorld =
            (mapGenerator.width - 1) * hexWidth * 0.985f + hexWidth;

        float mapHeightWorld =
            (mapGenerator.height - 1) * hexHeight * 0.753f + hexHeight;

        minBounds = new Vector2(
            mapRoot.position.x,
            mapRoot.position.y + hexHeight * 0.5f
        );

        maxBounds = new Vector2(
            mapRoot.position.x + mapWidthWorld,
            mapRoot.position.y + mapHeightWorld - hexHeight * 0.5f
        );
    }

    private void ClampTarget()
    {
        float camHalfHeight = targetZoom;
        float camHalfWidth = camHalfHeight * cam.aspect;

        if (!horizontalWrap)
        {
            targetPosition.x = Mathf.Clamp(
                targetPosition.x,
                minBounds.x + camHalfWidth,
                maxBounds.x - camHalfWidth
            );
        }

        targetPosition.y = Mathf.Clamp(
            targetPosition.y,
            minBounds.y + camHalfHeight,
            maxBounds.y - camHalfHeight
        );

        targetPosition.z = transform.position.z;
    }

    private void ApplyImmediate()
    {
        transform.position = targetPosition;
        cam.orthographicSize = targetZoom;
    }
}
