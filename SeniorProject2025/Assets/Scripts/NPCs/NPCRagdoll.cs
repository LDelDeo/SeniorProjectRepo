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
    private bool isDead = false;

    [Header("Death Effects")]
    public AudioSource npcVoiceSource;
    public AudioSource deathSFX;
    public AudioClip[] deathArray;
    public ParticleSystem bloodCloud;
    public ParticleSystem bloodSplat;


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
    public Rigidbody GetMainRagdollBody()
    {
        return ragdollBodies.Length > 0 ? ragdollBodies[0] : null;
    }
    public void Die()
    {
        if (npcVoiceSource != null)
        {
            npcVoiceSource.Stop();
            npcVoiceSource.enabled = false;
        }
        

        if (deathSFX != null && deathArray.Length > 0)
        {
            int rand = Random.Range(0, deathArray.Length);
            deathSFX.PlayOneShot(deathArray[rand], 1.0f);
        }
        
        
        if (bloodCloud != null)
        bloodCloud.Play();

        if(bloodSplat != null)
        bloodSplat.Play();

        gameObject.layer = LayerMask.NameToLayer("DeadNpc");

        if (animator != null) animator.enabled = false;
        if (agent != null) agent.enabled = false;
        if (rootCollider != null) rootCollider.enabled = false;
        if (bodyCollider != null) bodyCollider.enabled = false;

        foreach (var rb in ragdollBodies)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        foreach (var col in ragdollColliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }

        Destroy(gameObject, 8f);
    }
}