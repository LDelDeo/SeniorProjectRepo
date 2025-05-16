using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class RangedHumanEnemy : MonoBehaviour
{
    [Header("Enemy Values")]
    private float health;
    private float maxHealth = 3f;
    private float attackDamage = 20.0f;
    //private float speed = 12.0f;
    private bool isHostile = false;

    [Header("Script & Player Grabs")]
    private PlayerHealth playerHealth;
    private FPShooting fpShooting;
    private GameObject playerTransform;
    private NavMeshAgent agent;
    public GameObject alertIconPrefab;
    private GameObject alertIconInstance;
    public ParticleSystem bloodShed;

    [Header("Knockback Settings")]
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab; //Assign this in the Inspector
    public Transform firePoint; //Empty GameObject used as the spawn point
    public float attackRange = 25f;
    public float projectileSpeed = 20f;
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

        if (isHostile)
        {
            FacePlayer();
        }
        if (isKnockedBack) return;


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

    //Rotate to Face Player
    private void FacePlayer()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.transform.position - transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Smooth rotation
            }
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

    //Instant Death from Gun
    public void TakeDamageFromGun()
    {
        bloodShed.Play();
        fpShooting.Deathmarker();

        if (alertIconInstance != null)
        {
            Destroy(alertIconInstance);
        }



        Destroy(gameObject);    
    }

    public void TakeDamageFromBaton(float damageToTake)
    {
        health -= damageToTake;
        bloodShed.Play();
        
        isHostile = true;

        if (health <= 0)
        {
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
            StartCoroutine(ApplyKnockback());
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
            BulletProjectile bullet = projectile.GetComponent<BulletProjectile>();
            if (bullet != null)
            {
                bullet.SetOwner(this); //Pass this enemy as the source of damage
            }
        }
        else
        {
            Debug.LogWarning("Projectile Prefab or FirePoint not assigned!");
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
}
