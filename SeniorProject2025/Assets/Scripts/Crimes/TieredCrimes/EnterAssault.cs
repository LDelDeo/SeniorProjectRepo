using UnityEngine;

public class EnterAssault : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject exclamationPoint;
    public bool crimeFoughtCorrectly = true;

    [Header("Script Grabs")]
    private CrimeCompletion crimeCompletion;

    public void Update()
    {
        crimeCompletion = FindFirstObjectByType<CrimeCompletion>();

        int livingEnemies = 0;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
                livingEnemies++;
        }


        if (livingEnemies == 0)
        {
            if (crimeFoughtCorrectly)
            {
                // Payout Player Credits
                crimeCompletion.CrimeStopped(crimeCompletion.tierTwoXP, crimeCompletion.tierTwoCredits);
            }
            else
            {
                // No Payout, Done Wrong
                crimeCompletion.CrimeStopped(crimeCompletion.failedXP, crimeCompletion.failedCredits);
            }
            

            Destroy(exclamationPoint);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {

            foreach (Transform child in transform)
            {
                MeleeHumanEnemy melee = child.GetComponent<MeleeHumanEnemy>();
                if (melee != null)
                {
                    melee.BecomeHostile();
                }
            }
        }
    }
}
