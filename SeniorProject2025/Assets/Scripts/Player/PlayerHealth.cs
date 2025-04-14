using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Script Grabs")]
    public PlayerStats playerStats;
    public FPShooting fpsShooting;
    public TMP_Text HealthText;

    void Start()
    {
        playerStats.health = playerStats.maxHealth;
        HealthText.text = "HP: " + Mathf.CeilToInt(playerStats.health);
    }

    public void TakeDamage(float damageToTake)
    {
        if (playerStats.isBlocking)
        {
            playerStats.blockAmt++;
            if (playerStats.blockAmt >= playerStats.maxBlockAmt)
            {
                if (fpsShooting != null)
                    fpsShooting.Unblock(); 

                playerStats.blockAmt = 0;
            }
            return;
        }
        playerStats.blockAmt = 0;
        playerStats.health -= damageToTake;
        UpdateHealthUI();
        Debug.Log("Updated Player Health: " + playerStats.health);

        if (playerStats.health <= 0)
        {
            //GameOver or Death Screen
        }
    }

    private void UpdateHealthUI()
    {
        if (HealthText != null)
            HealthText.text = "HP: " + Mathf.CeilToInt(playerStats.health);
    }
    private IEnumerator DelayedUnblock()
    {
        yield return null; // Wait one frame before dropping shield
        fpsShooting.Unblock();
    }
}
