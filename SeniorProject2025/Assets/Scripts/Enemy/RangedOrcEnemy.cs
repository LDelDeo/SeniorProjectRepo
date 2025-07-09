using UnityEngine;
using UnityEngine.AI;

public class RangedOrcEnemy : MonoBehaviour
{
    [Header("Enemy Values")]
    private float health;
    private float maxHealth = 4.5f;
    private float attackDamage = 35.0f;
    //private float speed = 8.0f;
    public bool isHostile = false;

    [Header("Script & Player Grabs")]
    private PlayerHealth playerHealth;
    private FPShooting fpShooting;
    private GameObject playerTransform;
    private NavMeshAgent agent;
    public GameObject alertIconPrefab;
    private GameObject alertIconInstance;
    public ParticleSystem bloodShed;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab; //Assign this in the Inspector
    public Transform firePoint;         //Empty GameObject used as the spawn point
    public float attackRange = 30f;
    public float projectileSpeed = 25f;
    private float attackCooldown = 2f;
    private float nextAttackTime = 0f;

    // Start & Update
    private void Start()
    {
        //Game Elements
        health = maxHealth;
    }

    private void Update()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player");
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        fpShooting = FindFirstObjectByType<FPShooting>();

        AttackPlayer();
    }

    //Will Attack
    public void BecomeHostile()
    {
        isHostile = true;

        if (alertIconPrefab != null && alertIconInstance == null)
        {
            alertIconInstance = Instantiate(alertIconPrefab, transform.position + Vector3.up * 4f, Quaternion.identity);
            alertIconInstance.transform.SetParent(transform);
        }
    }



    //Dealing & Taking Damage
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
            int orcsDefeated = PlayerPrefs.GetInt("RangedOrcsDefeated", 0);
            PlayerPrefs.SetInt("RangedOrcsDefeated", orcsDefeated + 1);
            
            fpShooting.Deathmarker();
            if (alertIconInstance != null)
            {
                Destroy(alertIconInstance);
            }

            Destroy(gameObject);
        }
        else
        {
            fpShooting.Hitmarker();
        }
    }

    //Movement and Attacking
    private void AttackPlayer()
    {
        if (!isHostile || playerTransform == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.transform.position);

        if (distanceToPlayer > attackRange)
        {
            //Move toward player if out of range
            agent.destination = playerTransform.transform.position;
        }
        else
        {
            //Stop moving and shoot if in range
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
            BoulderProjectile boulder = projectile.GetComponent<BoulderProjectile>();
            if (boulder != null)
            {
                boulder.SetOwner(this); //Pass this enemy as the source of damage
            }
        }
        else
        {
            Debug.LogWarning("Projectile Prefab or FirePoint not assigned!");
        }
    }
}
