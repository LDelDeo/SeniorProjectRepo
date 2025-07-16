using UnityEngine;
using System.Collections;
using UnityEngine.AI;

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
    public float attackRange = 0.25f;
    public float attackCooldown = 4.1f;
    private bool isAttacking = false;
    private bool canDealDamage = false;

    // Start & Update
    private void Start()
    {
        health = maxHealth;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player");
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        fpShooting = FindFirstObjectByType<FPShooting>();

    }

    private void Update()
    {
        animator.applyRootMotion = false;
        if (playerTransform == null)
            return;

        if (!isKnockedBack && isHostile)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.transform.position);

            if (distance <= attackRange)
            {
                agent.isStopped = true;
                if (!isAttacking)
                    StartCoroutine(AttackPlayer());
            }
            else
            {
                agent.isStopped = false;
                FollowPlayer();
            }
        }
    }

    // Will Attack
    public void BecomeHostile()
    {
        isHostile = true;

        if (alertIconPrefab != null && alertIconInstance == null)
        {
            alertIconInstance = Instantiate(alertIconPrefab, transform.position + Vector3.up * 4f, Quaternion.identity);
            alertIconInstance.transform.SetParent(transform);
        }
    }

    // Dealing & Taking Damage
    public void DealDamage()
    {
        playerHealth.TakeDamage(attackDamage);
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

    // Knockback Coroutine
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

    // Movement
    private void FollowPlayer()
    {
        if (isHostile && playerTransform != null)
            agent.destination = playerTransform.transform.position;
            animator.SetTrigger("isMoving");
    }

    private void StopPursuit()
    {
        agent.ResetPath();
    }

    // Attack Coroutine
    private IEnumerator AttackPlayer()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    // Called via animation event during attack animation
    public void EnableDamageWindow()
    {
        canDealDamage = true;
    }

    // Called by hand collider script
    public void TryDealDamage()
    {
        if (canDealDamage)
        {
            DealDamage();
            canDealDamage = false; // Prevent double hit
        }
    }
}
