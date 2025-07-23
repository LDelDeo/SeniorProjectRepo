using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MeleeHumanEnemy : MonoBehaviour
{
    [Header("Enemy Values")]
    private float health;
    private float maxHealth = 4f;
    private float attackDamage = 15.0f;
    private bool isHostile = false;

    [Header("Script & Player Grabs")]
    private PlayerHealth playerHealth;
    private FPShooting fpShooting;
    private GameObject playerTransform;
    private NavMeshAgent agent;
    public GameObject alertIconPrefab;
    private GameObject alertIconInstance;
    public ParticleSystem bloodShed;
    private Animator animator;

    [Header("Knockback Settings")]
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource healthAudioSource;
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Attack Settings")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.8f;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool canDealDamage = false;

    private void Start()
    {
        health = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        playerTransform = GameObject.FindWithTag("Player");
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        fpShooting = FindFirstObjectByType<FPShooting>();

        agent.stoppingDistance = attackRange * 0.85f;
    }

    private void Update()
    {
        if (!isHostile || isKnockedBack || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.transform.position);
        Vector3 lookDir = playerTransform.transform.position - transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }

        attackTimer += Time.deltaTime;

        if (distance <= attackRange)
        {
            if (!isAttacking && attackTimer >= attackCooldown)
                StartAttack();

            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        else
        {
            StopAttack();
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        if (agent.enabled && !agent.pathPending && playerTransform != null)
        {
            agent.SetDestination(playerTransform.transform.position);
            animator.SetTrigger("isMoving");

        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = 0f;
        if (animator != null)
            animator.SetBool("isAttacking", true);
    }

    private void StopAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            if (animator != null)
                animator.SetBool("isAttacking", false);
        }

        if (agent.enabled && playerTransform != null)
        {
            agent.isStopped = false;
        }
    }

    public void BecomeHostile()
    {
        isHostile = true;

        if (alertIconPrefab != null && alertIconInstance == null)
        {
            alertIconInstance = Instantiate(alertIconPrefab, transform.position + Vector3.up * 4f, Quaternion.identity);
            alertIconInstance.transform.SetParent(transform);
        }
    }

    public void DealDamage()
    {
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);
    }

    public void EnableDamageWindow()
    {
        canDealDamage = true;
    }

    public void TryDealDamage()
    {
        if (!canDealDamage) return;

        float distance = Vector3.Distance(transform.position, playerTransform.transform.position);
        if (distance <= attackRange + 0.2f)
        {
            DealDamage();
        }

        canDealDamage = false;
    }

    public void TakeDamageFromGun()
    {
        GetComponent<NPCRagdoll>().Die();
        gameObject.tag = "Untagged";
        if (alertIconInstance != null)
            Destroy(alertIconInstance);

        bloodShed.Play();
        fpShooting.Deathmarker();
        healthAudioSource.PlayOneShot(deathSound, 1.0f);
        
        Destroy(this);
    }

    public void TakeDamageFromBaton(float damageToTake)
    {
        health -= damageToTake;
        bloodShed.Play();
        isHostile = true;

        if (health <= 0)
        {
            GetComponent<NPCRagdoll>().Die();
            gameObject.tag = "Untagged";
            if (alertIconInstance != null)
                Destroy(alertIconInstance);

            fpShooting.Deathmarker();
            healthAudioSource.PlayOneShot(deathSound, 1.0f);
           
            Destroy(this);
        }
        else
        {
            fpShooting.Hitmarker();
            healthAudioSource.PlayOneShot(takeDamageSound, 1.0f);
            StartCoroutine(ApplyKnockback());
        }
    }

    private IEnumerator ApplyKnockback()
    {
        isKnockedBack = true;
        agent.isStopped = true;

        Vector3 knockbackDir = (transform.position - playerTransform.transform.position).normalized;
        float timer = 0f;

        while (timer < knockbackDuration)
        {
            transform.position += knockbackDir * knockbackForce * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        isKnockedBack = false;
    }
}
