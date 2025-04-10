using UnityEngine;

public class EnterCrime : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject exclamationPoint;


    public void Update()
    {
        int livingEnemies = 0;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
                livingEnemies++;
        }

        if (livingEnemies == 0)
        {
            Debug.Log("Crime Stopped!");

            // Payout Player Credits

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
