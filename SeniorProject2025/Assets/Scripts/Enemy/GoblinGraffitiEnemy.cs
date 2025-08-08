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
    public bool hasBeenCaught = false;
    private float runDistance = 10f;
    public TMP_Text pressE;
    public bool canBeCuffed = false;

    [Header("Enemy Values")]
    private float health;
    private float maxHealth = 1f;

    [Header("Script & Player Grabs")]
    private FPShooting fpShooting;
    public GameObject alertIconPrefab;
    private GameObject alertIconInstance;
    public ParticleSystem bloodShed;
    private EnterCarScript enterCarScript;
    public Animator anim;
    public GoblinGraffitiSprayPaint sprayPaint;

    [Header("Knockback Settings")]
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource healthAudioSource;
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip deathSound;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enterCarScript = FindFirstObjectByType<EnterCarScript>();
        fpShooting = FindFirstObjectByType<FPShooting>();
        policeOfficer = GameObject.FindGameObjectWithTag("Player");

        GameObject pressEObject = GameObject.FindGameObjectWithTag("handcuffText");
        if (pressEObject != null)
        {
            pressE = pressEObject.GetComponent<TMP_Text>();
        }

        health = maxHealth;

        if (enterCarScript.isInCar == false && pressE != null)
        {
            pressE.text = "";
        }
    }

    void Update()
    {
        GameObject pressEObject = GameObject.FindGameObjectWithTag("handcuffText");
        if (pressEObject != null)
        {
            pressE = pressEObject.GetComponent<TMP_Text>();
        }
        
        if (enterCarScript.isInCar || isKnockedBack || hasBeenCaught) return;

        if (isSpooked && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && hasBeenCaught == false)
        {
            RunAway();
            
        }
        if (canBeCuffed)
        {
            if (pressE != null)
                pressE.text = "Press [E] to Handcuff";
        }
        if (canBeCuffed && Input.GetKeyDown(KeyCode.E))
        {
            this.tag = "";
            
            hasBeenCaught = true;
            canBeCuffed = false;
            if (pressE != null)
            {
                pressE.text = "";
            }

            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.isStopped = true;

            anim.SetBool("gotCaught", true);
            anim.applyRootMotion = false;

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            isSpooked = false;

            StartCoroutine(Despawn());
        }

        if (isSpooked)
        {
            sprayPaint.StopPainting();
        }

    }

    public IEnumerator Despawn()
    {
        yield return new WaitForSeconds(8f);
        Destroy(gameObject);
    }

    public void BecomeSpooked()
    {
        if (hasBeenCaught) return;

        isSpooked = true;

        if (alertIconPrefab != null && alertIconInstance == null)
        {
            alertIconInstance = Instantiate(alertIconPrefab, transform.position + Vector3.up * 4f, Quaternion.identity);
            alertIconInstance.transform.SetParent(transform);
        }
    }

    private void RunAway()
    {
        if (hasBeenCaught) return;

        Vector3 directionAwayFromThreat = (transform.position - policeOfficer.transform.position).normalized;
        Vector3 runToPosition = transform.position + directionAwayFromThreat * runDistance;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(runToPosition, out navHit, 10f, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(navHit.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                anim.SetBool("isSpooked", true);
                agent.SetDestination(navHit.position);
            }
            else
            {
                // Could not find valid path, pick a random valid point instead

                if (!anim.GetBool("isSpooked"))
                {
                    anim.SetBool("isSpooked", true);
                }

                TryRandomEscapeDirection();
            }
        }
        else
        {
            // Could not sample a nav point in that direction
            if (!anim.GetBool("isSpooked"))
            {
                anim.SetBool("isSpooked", true);
            }

            TryRandomEscapeDirection();
        }
    }

    private void TryRandomEscapeDirection()
    {
        for (int i = 0; i < 10; i++) // Try 10 times to find a random safe position
        {
            Vector3 randomDirection = Random.insideUnitSphere * runDistance;
            randomDirection.y = 0;
            Vector3 candidatePos = transform.position + randomDirection;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidatePos, out hit, 5f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetDestination(hit.position);
                    return;
                }
            }
        }
    }


    public void TakeDamageFromGun()
    {
        if (hasBeenCaught) return;

        healthAudioSource.PlayOneShot(deathSound, 1.0f);
        bloodShed.Play();
        GetComponent<NPCRagdoll>().Die();
        gameObject.tag = "Untagged";
        fpShooting.Deathmarker();
        if (pressE != null) pressE.text = "";


        if (alertIconInstance != null)
        {
            Destroy(alertIconInstance);
        }

        Destroy(this);
    }

    public void TakeDamageFromBaton(float damageToTake)
    {
        if (hasBeenCaught) return;

        health -= damageToTake;
        bloodShed.Play();

        if (!isSpooked)
            BecomeSpooked();

        if (health <= 0)
        {
            
            GetComponent<NPCRagdoll>().Die();
            gameObject.tag = "Untagged";
            fpShooting.Deathmarker();
            healthAudioSource.PlayOneShot(deathSound, 1.0f);
            if (pressE != null) pressE.text = "";

            if (alertIconInstance != null)
            {
                Destroy(alertIconInstance);
            }

            Destroy(this);
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

  
}
