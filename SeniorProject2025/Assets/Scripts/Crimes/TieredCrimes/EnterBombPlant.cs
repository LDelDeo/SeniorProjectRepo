using UnityEngine;

public class EnterBombPlant : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject exclamationPoint;

    [Header("Script Grabs")]
    private CrimeCompletion crimeCompletion;
    private WireCut wireCut;

    public void Update()
    {
        crimeCompletion = FindObjectOfType<CrimeCompletion>();

        if (wireCut == null)
        wireCut = FindObjectOfType<WireCut>();

        if (wireCut.doneCorrectly)
        {
            Debug.Log("Crime Stopped!");

            // Payout Player Credits
            crimeCompletion.CrimeStopped(crimeCompletion.tierThreeXP, crimeCompletion.tierThreeCredits);

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
                MeleeOrcEnemy melee = child.GetComponent<MeleeOrcEnemy>();
                if (melee != null)
                {
                    melee.BecomeHostile();
                }

                RangedOrcEnemy ranged = child.GetComponent<RangedOrcEnemy>();
                if (ranged != null)
                {
                    ranged.BecomeHostile();
                }
            }
        }
    }

    
}