using UnityEngine;
using Steamworks;

public class EnterVandalism : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject exclamationPoint;
    public bool crimeFoughtCorrectly = true;

    [Header("Script Grabs")]
    private CrimeCompletion crimeCompletion;
    private SteamAchievementsManager steamAM;

    private void Start()
    {
        steamAM = FindObjectOfType<SteamAchievementsManager>(); 
    }
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
                crimeCompletion.CrimeStopped(crimeCompletion.tierTwoXP, crimeCompletion.tierTwoCredits, false, 2);
                steamAM.UnlockAchievement("tierTwoCrime");
                SteamUserStats.StoreStats();
            }
            else
            {
                // No Payout, Done Wrong
                crimeCompletion.CrimeStopped(crimeCompletion.failedXP, crimeCompletion.failedCredits, true, 2);
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
