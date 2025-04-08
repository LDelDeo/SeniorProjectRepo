using UnityEngine;
[RequireComponent(typeof(Renderer))]
public class OcclusionCulling : MonoBehaviour
{
    [Header("Visibility Settings")]
    public float maxDistance = 20f;

    [Header("Occlusion Settings")]
    public LayerMask occlusionMask;
    [Range(0f, 1f)] public float visibilityThreshold = 0.1f;
    public float sphereRadius = 0.1f;

    [Header("Components To Disable")]
    public Collider[] collidersToToggle;

    private Renderer objectRenderer;
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
        objectRenderer = GetComponentInChildren<Renderer>();

        // Optional: force opaque if needed
        SetMaterialOpaque(objectRenderer.material);
    }

    void Update()
    {
        if (Camera.main != null && camTransform != Camera.main.transform)
            camTransform = Camera.main.transform;

        Vector3[] checkPoints = GetVisibilityCheckPoints();
        int visiblePoints = 0;

        foreach (var point in checkPoints)
        {
            Vector3 dir = camTransform.position - point;
            float distance = dir.magnitude;

            bool blocked = Physics.SphereCast(point, sphereRadius, dir.normalized, out RaycastHit hit, distance, occlusionMask);
            if (!blocked) visiblePoints++;

            Debug.DrawLine(point, camTransform.position, blocked ? Color.red : Color.green);
        }

        float visibleRatio = (float)visiblePoints / checkPoints.Length;
        bool isOccluded = visiblePoints == 0;

        float camDistance = Vector3.Distance(objectRenderer.bounds.center, camTransform.position);
        bool tooFar = camDistance >= maxDistance;

        bool shouldShow = !isOccluded && !tooFar;

        // Toggle renderer
        if (objectRenderer.enabled != shouldShow)
            objectRenderer.enabled = shouldShow;

        // Toggle colliders
        foreach (var col in collidersToToggle)
            if (col != null && col.enabled != shouldShow)
                col.enabled = shouldShow;
    }

    Vector3[] GetVisibilityCheckPoints()
    {
        Bounds bounds = objectRenderer.bounds;
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        return new Vector3[]
        {
            center,
            center + new Vector3(0, extents.y, 0),     // top
            center - new Vector3(0, extents.y, 0),     // bottom
            center + new Vector3(extents.x, 0, 0),     // right
            center - new Vector3(extents.x, 0, 0),     // left
            center + new Vector3(0, 0, extents.z),     // front
        };
    }

    void SetMaterialOpaque(Material mat)
    {
        mat.SetFloat("_Mode", 0); // Opaque
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
    }
}