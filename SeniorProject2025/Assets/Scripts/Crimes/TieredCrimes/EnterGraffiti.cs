using UnityEngine;

public class EnterGraffiti : MonoBehaviour
{
    public GameObject[] artist;
    public GameObject exclamationPoint;
    public bool crimeFoughtCorrectly = true;

    [Header("Script Grabs")]
    private CrimeCompletion crimeCompletion;

    public void Update()
    {
        crimeCompletion = FindFirstObjectByType<CrimeCompletion>();

        int livingEnemies = 0;

        foreach (GameObject enemy in artist)
        {
            if (enemy != null)
                livingEnemies++;
        }


        if (livingEnemies == 0)
        {
            if (crimeFoughtCorrectly)
            {
                // Payout Player Credits
                crimeCompletion.CrimeStopped(crimeCompletion.tierOneXP, crimeCompletion.tierOneCredits);
            }
            else
            {
                // No Payout, Done Wrong
                crimeCompletion.CrimeStopped(crimeCompletion.failedXP, crimeCompletion.failedCredits);
            }
            

            //Spawn the Goblin Handcuffed Animation Here


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
                GoblinGraffitiEnemy art = child.GetComponent<GoblinGraffitiEnemy>();
                if (art != null)
                {
                    art.BecomeSpooked();
                }
            }
        }
    }
}
