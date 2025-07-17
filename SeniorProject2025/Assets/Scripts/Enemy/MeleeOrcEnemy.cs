using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MeleeOrcEnemy : MonoBehaviour
{
    [Header("Enemy Values")]
    private float health;
    private float maxHealth = 6.5f;
    private float attackDamage = 35.0f;
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
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource healthAudioSource;
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Attack Settings")]
    public float attackRange = 1.3f;
    public float attackCooldown = 2.0f;
    private bool canDealDamage = false;
    private float attackTimer = 0f;
    private bool isAttacking = false;

    private void Start()
    {
        health = maxHealth;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player");
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        fpShooting = FindFirstObjectByType<FPShooting>();

        agent.stoppingDistance = attackRange * 0.85f;
    }

    private void Update()
    {
        if (playerTransform == null || isKnockedBack || !isHostile)
            return;

        float distance = Vector3.Distance(transform.position, playerTransform.transform.position);

        // Always face the player
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
            {
                StartAttack();
            }

            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            animator.SetBool("isMoving", false);
        }
        else
        {
            StopAttack();
            FollowPlayer();
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

        if (!agent.enabled || playerTransform == null)
            return;

        agent.isStopped = false;
    }

    private void FollowPlayer()
    {
        if (isHostile && playerTransform != null)
        {
            if (!agent.pathPending && agent.enabled)
            {
                agent.SetDestination(playerTransform.transform.position);
                animator.SetBool("isMoving", true);
            }
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
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;
        bloodShed.Play();
        isHostile = true;

        if (health <= 0)
        {
            healthAudioSource.PlayOneShot(deathSound, 1.0f);
            fpShooting.Deathmarker();
            if (alertIconInstance != null)
                Destroy(alertIconInstance);
            Destroy(gameObject);
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

    // Called via animation event (beginning of punch wind-up)
    public void EnableDamageWindow()
    {
        canDealDamage = true;
    }

    // Called via animation event (at impact frame)
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
}
