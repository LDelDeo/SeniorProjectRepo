using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Script Grabs")]
    public PlayerStats playerStats;
    public FPShooting fpsShooting;
    public TMP_Text HealthText;

    [Header("Shield UI")]
    public Image[] shieldIcons;

    private bool wasBlocking = false;

    void Start()
    {
        playerStats.health = playerStats.maxHealth;
        HealthText.text = "HP: " + Mathf.CeilToInt(playerStats.health);
        SetShieldIconsVisible(false);
    }

   void Update()
    {
        if (playerStats.isBlocking)
        {
            if (!wasBlocking)
            {
                SetShieldIconsVisible(true);
                ResetShieldIcons(); 
                playerStats.blockAmt = 0;
            }
        }
        else
        {
            if (wasBlocking)
            {
                SetShieldIconsVisible(false);
            }
        }

        wasBlocking = playerStats.isBlocking;
    }

    public void TakeDamage(float damageToTake)
    {
        if (playerStats.isBlocking)
        {
            playerStats.blockAmt++;
            UpdateShieldIcons();

            if (playerStats.blockAmt >= playerStats.maxBlockAmt)
            {
                if (fpsShooting != null)
                    fpsShooting.Unblock();

                playerStats.blockAmt = 0;
                SetShieldIconsVisible(false);
            }
            return;
        }

        // Not blocking, take damage
        playerStats.blockAmt = 0;
        SetShieldIconsVisible(false);
        playerStats.health -= damageToTake;
        UpdateHealthUI();

        Debug.Log("Updated Player Health: " + playerStats.health);

        if (playerStats.health <= 0)
        {
            // GameOver or Death Screen
        }
    }

    private void UpdateShieldIcons()
    {
        for (int i = 0; i < shieldIcons.Length; i++)
        {
            float alpha = (i >= playerStats.blockAmt) ? 1f : 0.3f;
            Color c = shieldIcons[i].color;
            c.a = alpha;
            shieldIcons[i].color = c;
        }
    }

    private void ResetShieldIcons()
    {
        foreach (Image icon in shieldIcons)
        {
            icon.enabled = true;
            Color c = icon.color;
            c.a = 1f;
            icon.color = c;
        }
    }

    private void SetShieldIconsVisible(bool visible)
    {
        foreach (Image icon in shieldIcons)
        {
            icon.enabled = visible;
        }
    }

    private void UpdateHealthUI()
    {
        if (HealthText != null)
            HealthText.text = "HP: " + Mathf.CeilToInt(playerStats.health);
    }

    private IEnumerator DelayedUnblock()
    {
        yield return null;
        fpsShooting.Unblock();
    }
}
