using UnityEngine;

public class StickToGround : MonoBehaviour
{
    public float raycastDistance = 1.0f;
    public LayerMask groundLayer;
    public float offsetY = 0.02f; 

    void LateUpdate()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayer))
        {
            Vector3 newPos = hit.point + Vector3.up * offsetY;
            transform.position = newPos;
        }
    }
}
