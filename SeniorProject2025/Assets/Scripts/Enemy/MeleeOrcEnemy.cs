using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MeleeOrcEnemy : MonoBehaviour
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

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    //Start & Update
    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerTransform = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
    }

    private void Update()
    {
        if (!isKnockedBack)
            FollowPlayer();
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
            if (alertIconInstance != null)
            {
                Destroy(alertIconInstance);
            }

            Destroy(gameObject);    
        }
        else
        {
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
