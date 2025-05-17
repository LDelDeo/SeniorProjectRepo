using UnityEngine;

public class NPCCarRoute : MonoBehaviour
{
    [Header("Values")]
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float arrivalThreshold = 0.1f;
    public bool hasToBeRotated = false;
    public float pullOverDistance = 2f;
    public float pullOverSpeed = 2f;
    public float pullOverTurnAngle = 15f; // Angle to turn right when pulling over

    [Header("Bools and Assignments")]
    private int currentIndex = 0;
    private bool isPulledOver = false;
    private bool isPullingOver = false;
    private Vector3 pullOverTarget;
    private Quaternion originalRotation;
    private Quaternion pullOverRotation;

    [Header("Script Grabs")]
    public EnterCarScript enterCarScript;

    void Update()
    {
        if (waypoints.Length == 0 || isPulledOver) return;

        if (isPullingOver)
        {
            // Move toward pull-over point
            transform.position = Vector3.MoveTowards(transform.position, pullOverTarget, pullOverSpeed * Time.deltaTime);
            
            // Smoothly turn slightly to the right
            transform.rotation = Quaternion.Slerp(transform.rotation, pullOverRotation, rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, pullOverTarget) < 0.1f)
            {
                isPullingOver = false;
                isPulledOver = true;
            }
            return;
        }

        Vector3 targetPos = waypoints[currentIndex].position;
        targetPos.y = transform.position.y;

        Vector3 direction = (targetPos - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            if (hasToBeRotated)
                targetRotation *= Quaternion.Euler(0, 90f, 0);

            Vector3 euler = targetRotation.eulerAngles;
            Quaternion yRotation = Quaternion.Euler(0, euler.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, yRotation, rotationSpeed * Time.deltaTime);
        }

        Vector3 flatPos = transform.position; flatPos.y = 0;
        Vector3 flatTarget = targetPos; flatTarget.y = 0;

        if (Vector3.Distance(flatPos, flatTarget) <= arrivalThreshold)
            currentIndex = (currentIndex + 1) % waypoints.Length;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cop") && !isPulledOver && !isPullingOver && enterCarScript.isInCar)
        {
            // Set pull-over point and rotation
            Vector3 rightOffset = transform.right * pullOverDistance;
            pullOverTarget = transform.position + rightOffset;
            pullOverTarget.y = transform.position.y;

            originalRotation = transform.rotation;
            pullOverRotation = originalRotation * Quaternion.Euler(0, pullOverTurnAngle, 0);

            isPullingOver = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cop"))
        {
            isPulledOver = false;
            isPullingOver = false;
        }
    }
}
