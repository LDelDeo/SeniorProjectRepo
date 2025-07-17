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
    private Animator animator;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab; //Assign this in the Inspector
    public Transform firePoint;         //Empty GameObject used as the spawn point
    public float attackRange = 30f;
    public float projectileSpeed = 25f;
    private float attackCooldown = 2f;
    private float nextAttackTime = 0f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource healthAudioSource;
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip deathSound;

    // Start & Update
    private void Start()
    {
        //Game Elements
        health = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player");
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        fpShooting = FindFirstObjectByType<FPShooting>();

        if (isHostile == true)
        {
            animator.SetBool("isHostile", true);
            RotateTowardsPlayer();
        }
        else
        {
            animator.SetBool("isHostile", false);
        }

        AttackPlayer();
    }

    private void RotateTowardsPlayer()
    {
        if (playerTransform == null) return;

        Vector3 direction = playerTransform.transform.position - transform.position;
        direction.y = 0; // keep rotation only on horizontal plane

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Apply a -90 degrees Y offset to fix model facing
            Quaternion rotationOffset = Quaternion.Euler(0, -90f, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * rotationOffset, Time.deltaTime * 5f);
        }
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
            healthAudioSource.PlayOneShot(deathSound, 1.0f);
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
            healthAudioSource.PlayOneShot(takeDamageSound, 1.0f);
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
                //ShootProjectile();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    public void ShootProjectile()
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
