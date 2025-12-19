using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MapCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform mapRoot;
    public HexMapGenerator mapGenerator;

    [Header("Movement")]
    public float dragSpeed = 1f;
    public float smoothTime = 0.15f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float zoomSmoothTime = 0.12f;
    public float minZoom = 3f;
    public float maxZoom = 20f;

    private Camera cam;

    private Vector3 velocity;
    private Vector3 targetPosition;

    private float targetZoom;
    private float zoomVelocity;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private float hexHalfWidth;
    private float hexHalfHeight;

    private Vector3 lastMouseWorld;

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

        // IMPORTANTE: clamp APÓS atualizar targetZoom e targetPosition
        ClampTarget();

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

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
        if (!Input.GetMouseButton(2)) // botão do meio
            return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        if (Input.GetMouseButtonDown(2))
        {
            lastMouseWorld = mouseWorld;
            return;
        }

        Vector3 delta = lastMouseWorld - mouseWorld;
        targetPosition += delta * dragSpeed;
        lastMouseWorld = mouseWorld;
    }

    private void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) < 0.01f)
            return;

        // Posição do mouse ANTES do zoom
        Vector3 mouseWorldBefore = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldBefore.z = 0f;

        // Atualiza zoom alvo
        targetZoom = Mathf.Clamp(
            targetZoom - scroll * zoomSpeed,
            minZoom,
            maxZoom
        );

        // Simula o novo zoom para calcular offset
        float zoomRatio = targetZoom / cam.orthographicSize;

        Vector3 mouseWorldAfter = transform.position +
            (mouseWorldBefore - transform.position) * zoomRatio;

        // Ajusta posição alvo para manter o ponto sob o mouse
        Vector3 delta = mouseWorldBefore - mouseWorldAfter;
        targetPosition += delta;
    }

    // =========================
    // BOUNDS
    // =========================

    public void CacheMapBounds()
    {
        if (mapGenerator == null || mapRoot == null)
        {
            Debug.LogError("MapCameraController: referências não definidas");
            return;
        }

        float hexWidth = mapGenerator.HexWidth;
        float hexHeight = mapGenerator.HexHeight;

        hexHalfWidth = hexWidth * 0.5f;
        hexHalfHeight = hexHeight * 0.5f;

        float mapWidthWorld =
            (mapGenerator.width - 1) * hexWidth * 0.985f + hexWidth;

        float mapHeightWorld =
            (mapGenerator.height - 1) * hexHeight * 0.753f + hexHeight;

        minBounds = new Vector2(
            mapRoot.position.x + hexHalfWidth,
            mapRoot.position.y + hexHalfHeight
        );

        maxBounds = new Vector2(
            mapRoot.position.x + mapWidthWorld - hexHalfWidth,
            mapRoot.position.y + mapHeightWorld - hexHalfHeight
        );
    }

    private void ClampTarget()
    {
        float camHalfHeight = targetZoom;
        float camHalfWidth = camHalfHeight * cam.aspect;

        targetPosition.x = Mathf.Clamp(
            targetPosition.x,
            minBounds.x + camHalfWidth,
            maxBounds.x - camHalfWidth
        );

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
