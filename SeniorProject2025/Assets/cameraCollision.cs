using Unity.Cinemachine;
using UnityEngine;

[DefaultExecutionOrder(10000)]
public class cameraCollision : MonoBehaviour
{
    public Transform target;          
    public float radius = 0.6f;      
    public LayerMask collideAgainst; 
    public float skin = 0.05f;        

    CinemachineCamera vcam;
    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        if (target == null && vcam != null)
            target = vcam.Target.TrackingTarget;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = transform.position;
        Vector3 origin = target.position;
        Vector3 dir = desired - origin;
        float dist = dir.magnitude;

        if (dist <= Mathf.Epsilon) return;
        dir /= dist;

        if (Physics.SphereCast(origin, radius, dir, out RaycastHit hit, dist, collideAgainst, QueryTriggerInteraction.Ignore))
        {
            // Pull camera in front of the hit point
            Vector3 safePos = hit.point - dir * (radius + skin);
            transform.position = safePos;
        }
    }
}

