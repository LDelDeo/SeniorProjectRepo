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

    //Start & Update
    private void Start()
    {
        //Private Grabs
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerTransform = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();

        //Game Elements
        health = maxHealth;
    }

    private void Update()
    {
        FollowPlayer();
    }

    // Dealing & Taking Damage
    public void DealDamage()
    {
        playerHealth.TakeDamage(attackDamage);
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;
        Debug.Log("Orc Health: " + health);

        isHostile = true;

        if (health <= 0)
        {
            Debug.Log("Orc Dead");
            Destroy(gameObject);    
        }
    }

    //Movement
    private void FollowPlayer()
    {
        if (isHostile)
        agent.destination = playerTransform.transform.position;
    }
}
