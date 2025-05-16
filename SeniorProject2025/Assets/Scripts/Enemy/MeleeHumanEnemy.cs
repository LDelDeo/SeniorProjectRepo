using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MeleeHumanEnemy : MonoBehaviour
{
    [Header("Enemy Values")]
    private float health;
    private float maxHealth = 4f;
    private float attackDamage = 15.0f;
    //private float speed = 18.0f;
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

    //Start & Update
    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (!isKnockedBack)
            FollowPlayer();

        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player");
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        fpShooting = FindFirstObjectByType<FPShooting>();
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
        if (isHostile)
            agent.destination = playerTransform.transform.position;
    }
}
