using UnityEngine;
using UnityEngine.AI;

public class NPCRagdoll : MonoBehaviour
{
    private Animator animator;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private Collider rootCollider;
    private NavMeshAgent agent;
    public Collider bodyCollider;
    

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rootCollider = GetComponent<Collider>();

        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        DisableRagdoll(); 
    }

    private void DisableRagdoll()
    {
        foreach (var rb in ragdollBodies)
        {
            if (rb != null) rb.isKinematic = true;
        }

        foreach (var col in ragdollColliders)
        {
            if (col != null && col.gameObject != this.gameObject)
                col.enabled = false;
        }

        if (animator != null) animator.enabled = true;
        if (rootCollider != null) rootCollider.enabled = true;
        if (agent != null) agent.enabled = true;
    }

    public void Die()
    {
        
        if (animator != null) animator.enabled = false;
        if (agent != null) agent.enabled = false;
        if (rootCollider != null) rootCollider.enabled = false;
        if (bodyCollider != null) bodyCollider.enabled = false;

        foreach (var rb in ragdollBodies)
        {
            if (rb != null) rb.isKinematic = false;
        }

        foreach (var col in ragdollColliders)
        {
            if (col != null) col.enabled = true;
        }

        Destroy(gameObject, 8f);
    }
}