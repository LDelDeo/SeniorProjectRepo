using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCCarRoute : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float arrivalThreshold = 0.3f;
    public bool hasToBeRotated = false;

    [Header("Pull Over Settings")]
    public float pullOverDistance = 2f;
    public float pullOverSpeed = 2f;
    public float pullOverTurnAngle = 15f;

    [Header("Turn Slowdown")]
    public float turnSlowdownAngleThreshold = 30f;
    public float slowTurnSpeedMultiplier = 0.4f;

    [Header("Random Materials")]
    public List<Material> possibleMaterials = new List<Material>();

    [Header("Script Grabs")]
    public EnterCarScript enterCarScript;

    [Header("Audio")]
    public AudioSource crashSFX;

    [Header("Particles")]
    public GameObject smoke;

    private WaypointNode currentNode;
    private WaypointNode previousNode;
    private bool isPulledOver = false;
    private bool isPullingOver = false;
    private bool hasCrashed = false;
    private Vector3 pullOverTarget;
    private Quaternion originalRotation;
    private Quaternion pullOverRotation;

    void Start()
    {
        smoke.SetActive(false);

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (possibleMaterials.Count > 0)
            {
                Material randomMat = possibleMaterials[Random.Range(0, possibleMaterials.Count)];
                renderer.material = randomMat;
            }
        }

        WaypointNode[] allNodes = FindObjectsOfType<WaypointNode>();
        if (allNodes.Length == 0)
        {
            Debug.LogError("No WaypointNodes found in the scene!");
            return;
        }

        if (enterCarScript == null)
            enterCarScript = FindObjectOfType<EnterCarScript>();

        currentNode = allNodes[Random.Range(0, allNodes.Length)];
        transform.position = currentNode.transform.position;
        PickNextNode();
    }

    void Update()
    {
        if (currentNode == null || isPulledOver || hasCrashed) return;

        if (isPullingOver)
        {
            transform.position = Vector3.MoveTowards(transform.position, pullOverTarget, pullOverSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, pullOverRotation, rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, pullOverTarget) < 0.1f)
            {
                isPullingOver = false;
                isPulledOver = true;
            }
            return;
        }

        Vector3 targetPos = currentNode.transform.position;
        targetPos.y = transform.position.y;

        Vector3 direction = (targetPos - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, direction);

        // Slow down for sharp turns
        float adjustedSpeed = moveSpeed;
        if (angleToTarget > 70f)
            adjustedSpeed = moveSpeed * 0.3f;
        else if (angleToTarget > 30f)
            adjustedSpeed = moveSpeed * 0.5f;

        // Move forward
        transform.position += transform.forward * adjustedSpeed * Time.deltaTime;

        // Rotate smoothly toward the target
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            if (hasToBeRotated)
                targetRotation *= Quaternion.Euler(0, 90f, 0);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 100f);
        }

        // Check for arrival
        Vector3 flatPos = transform.position; flatPos.y = 0;
        Vector3 flatTarget = targetPos; flatTarget.y = 0;

        if (Vector3.Distance(flatPos, flatTarget) <= arrivalThreshold)
            PickNextNode();
    }

    void PickNextNode()
    {
        if (currentNode.neighbors == null || currentNode.neighbors.Length == 0)
        {
            Debug.LogWarning("Current node has no neighbors.");
            return;
        }

        WaypointNode nextNode;
        do
        {
            nextNode = currentNode.neighbors[Random.Range(0, currentNode.neighbors.Length)];
        } while (nextNode == previousNode && currentNode.neighbors.Length > 1);

        previousNode = currentNode;
        currentNode = nextNode;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cop") && !isPulledOver && !isPullingOver && enterCarScript.isInCar && enterCarScript.areLightsOn)
        {
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NPCCar") || collision.gameObject.CompareTag("Cop"))
        {
            moveSpeed = 0;
            hasCrashed = true;

            crashSFX.Play();
            smoke.SetActive(true);

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }
}
