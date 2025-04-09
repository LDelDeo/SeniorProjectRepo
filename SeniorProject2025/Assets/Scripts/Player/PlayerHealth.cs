using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Script Grabs")]
    public PlayerStats playerStats;

    void Start()
    {
        playerStats.health = playerStats.maxHealth;
    }

    public void TakeDamage(float damageToTake)
    {
        playerStats.health -= damageToTake;
        Debug.Log("Updated Player Health: " + playerStats.health);

        if (playerStats.health <= 0)
        {
            //GameOver or Death Screen
        }
    }


}
