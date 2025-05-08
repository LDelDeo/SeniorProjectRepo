using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GoblinGraffitiEnemy : MonoBehaviour
{
    [Header("Running Away")]
    private GameObject policeOfficer;
    private NavMeshAgent agent;
    private bool isSpooked = false;
    private float runDistance = 10f;
    private bool hasRunAway = false;
    private TMP_Text pressE;
    private bool canBeCuffed = false;

    [Header("Enemy Values")]
    private float health;
    private float maxHealth = 1f;

    [Header("Script & Player Grabs")]
    private FPShooting fpShooting;
    public GameObject alertIconPrefab;
    private GameObject alertIconInstance;
    public ParticleSystem bloodShed;
    private EnterCarScript enterCarScript;

    [Header("Knockback Settings")]
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enterCarScript = FindObjectOfType<EnterCarScript>();
        
        health = maxHealth;

        if (enterCarScript.isInCar == false && pressE != null)
        {
            pressE.text = "";
        }
    }

    void Update()
    {
        if (enterCarScript.isInCar == false)
        {
            policeOfficer = GameObject.FindGameObjectWithTag("Player");
            fpShooting = FindObjectOfType<FPShooting>();
            GameObject pressEObject = GameObject.FindGameObjectWithTag("handcuffText");
            if (pressEObject != null)
            {
                pressE = pressEObject.GetComponent<TMP_Text>();
            }
        }

        
        

        if (isSpooked && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            RunAway();
        }

        if (canBeCuffed && Input.GetKeyDown(KeyCode.E))
        {
            pressE.text = "";
            Destroy(gameObject);
        }
    }

    public void BecomeSpooked()
    {
        isSpooked = true;

        if (alertIconPrefab != null && alertIconInstance == null)
        {
            alertIconInstance = Instantiate(alertIconPrefab, transform.position + Vector3.up * 4f, Quaternion.identity);
            alertIconInstance.transform.SetParent(transform);
        }
    }

    private void RunAway()
    {
        Vector3 directionAwayFromThreat = (transform.position - policeOfficer.transform.position).normalized;
        Vector3 randomOffset = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        Vector3 runToPosition = transform.position + directionAwayFromThreat * runDistance + randomOffset;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(runToPosition, out hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} couldn't find valid NavMesh position to flee to.");
        }
    }


    // Instant Death from Gun
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

        if (!isSpooked)
            BecomeSpooked();

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

    private IEnumerator ApplyKnockback()
    {
        isKnockedBack = true;
        agent.isStopped = true;

        Vector3 knockbackDir = (transform.position - policeOfficer.transform.position).normalized;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            pressE.text = "Press [E] to Handcuff";
            canBeCuffed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            pressE.text = "";
            canBeCuffed = false;
        }
    }
}
