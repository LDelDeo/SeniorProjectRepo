using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
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
    public ParticleSystem smoke;
    public Transform smokePosition;

    private WaypointNode currentNode;
    private WaypointNode previousNode;
    private bool isPulledOver = false;
    private bool isPullingOver = false;
    private bool hasCrashed = false;
    private Vector3 pullOverTarget;
    private Quaternion originalRotation;
    private Quaternion pullOverRotation;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

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

        // Choose an unoccupied node
        List<WaypointNode> unoccupiedNodes = new List<WaypointNode>();
        foreach (var node in allNodes)
        {
            if (!node.isOccupied)
                unoccupiedNodes.Add(node);
        }

        if (unoccupiedNodes.Count == 0)
        {
            Debug.LogError("No unoccupied nodes available for spawning!");
            return;
        }

        currentNode = unoccupiedNodes[Random.Range(0, unoccupiedNodes.Count)];
        currentNode.isOccupied = true;
        transform.position = currentNode.transform.position;

        PickNextNode();
    }

    void FixedUpdate()
    {
        if (currentNode == null || isPulledOver || hasCrashed) return;

        if (isPullingOver)
        {
            Vector3 newPos = Vector3.MoveTowards(rb.position, pullOverTarget, pullOverSpeed * Time.fixedDeltaTime);
            Quaternion newRot = Quaternion.Slerp(rb.rotation, pullOverRotation, rotationSpeed * Time.fixedDeltaTime);

            rb.MovePosition(newPos);
            rb.MoveRotation(newRot);

            if (Vector3.Distance(rb.position, pullOverTarget) < 0.1f)
            {
                isPullingOver = false;
                isPulledOver = true;
            }
            return;
        }

        Vector3 targetPos = currentNode.transform.position;
        targetPos.y = rb.position.y;

        Vector3 direction = (targetPos - rb.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, direction);

        float adjustedSpeed = moveSpeed;
        if (angleToTarget > 70f)
            adjustedSpeed = moveSpeed * 0.3f;
        else if (angleToTarget > 30f)
            adjustedSpeed = moveSpeed * 0.5f;

        Vector3 move = rb.position + transform.forward * adjustedSpeed * Time.fixedDeltaTime;
        rb.MovePosition(move);

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            if (hasToBeRotated)
                targetRotation *= Quaternion.Euler(0, 90f, 0);

            Quaternion smoothedRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime * 100f);
            rb.MoveRotation(smoothedRotation);
        }

        Vector3 flatPos = rb.position; flatPos.y = 0;
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
        } while ((nextNode == previousNode || nextNode.isOccupied) && currentNode.neighbors.Length > 1);

        // Release the current node
        if (currentNode != null)
            currentNode.isOccupied = false;

        previousNode = currentNode;
        currentNode = nextNode;
        currentNode.isOccupied = true;
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
            Instantiate(smoke, smokePosition.position, Quaternion.Euler(-90f, 0f, 0f));

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            // Free the current node on crash
            if (currentNode != null)
                currentNode.isOccupied = false;
        }
    }
}
