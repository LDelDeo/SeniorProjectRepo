using UnityEngine;
using UnityEngine.AI;

public class RangedOrcEnemy : MonoBehaviour
{
    [Header("Enemy Values")]
    private float health;
    private float maxHealth = 125.0f;
    private float attackDamage = 35.0f;
    private float speed = 8.0f;
    private bool isHostile = false;

    [Header("Script & Player Grabs")]
    private PlayerHealth playerHealth;
    private GameObject playerTransform;
    private NavMeshAgent agent;
    public GameObject alertIconPrefab;
    private GameObject alertIconInstance;
    public ParticleSystem bloodShed;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab; // Assign this in the Inspector
    public Transform firePoint;         // Empty GameObject used as the spawn point
    public float attackRange = 10f;
    public float projectileSpeed = 15f;
    private float attackCooldown = 2f;
    private float nextAttackTime = 0f;

    // Start & Update
    private void Start()
    {
        // Private Grabs
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerTransform = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();

        // Game Elements
        health = maxHealth;
    }

    private void Update()
    {
        AttackPlayer();
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
            if (alertIconInstance != null)
            {
                Destroy(alertIconInstance);
            }

            Destroy(gameObject);    
        }
    }

    // Movement and Attacking
    private void AttackPlayer()
    {
        if (!isHostile || playerTransform == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.transform.position);

        if (distanceToPlayer > attackRange)
        {
            // Move toward player if out of range
            agent.destination = playerTransform.transform.position;
        }
        else
        {
            // Stop moving and shoot if in range
            agent.destination = transform.position;

            if (Time.time >= nextAttackTime)
            {
                ShootProjectile();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector3 direction = (playerTransform.transform.position - firePoint.position).normalized;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * projectileSpeed;
            }
        }
        else
        {
            Debug.LogWarning("Projectile Prefab or FirePoint not assigned!");
        }
    }
}
