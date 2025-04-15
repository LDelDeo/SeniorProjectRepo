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
    public GameObject shieldObject;

    [Header("Stim Shot")]
    public Animator stimAnim;
    public GameObject stimObject;
    public Image stimCooldownIcon;
    public TMP_Text stimStatus;
    private Color originalTextColor;
    private float stimCooldownTime = 30f;
    private float currentStimCooldown = 0f;
    private bool isStimReady = true;
    private float stimShotAnimTime = 0.75f;
    

    private bool wasBlocking = false;

    void Start()
    {
        //Health
        playerStats.health = playerStats.maxHealth;
        HealthText.text = "HP: " + Mathf.CeilToInt(playerStats.health);
        
        //Shield
        SetShieldIconsVisible(false);

        //Stim
        stimObject.SetActive(false);
        originalTextColor = stimStatus.color;
    }

    void Update()
    {
        // Shield Block Stuff
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

        // Stim Shot Stuff

        if (!isStimReady)
        {
            currentStimCooldown -= Time.deltaTime;
            stimCooldownIcon.fillAmount = 1 - (currentStimCooldown / stimCooldownTime);

            if (currentStimCooldown <= 0)
            {
                isStimReady = true;
                stimCooldownIcon.fillAmount = 1f;
            }

            stimStatus.text = "Recharging";
            stimStatus.color = Color.yellow;
        }
        else
        {
            stimStatus.text = "Ready";
            stimStatus.color = originalTextColor;
        }

        if (!playerStats.isBlocking && Input.GetKeyDown(KeyCode.Q))
        {
            if (isStimReady)
            {
                StimShot();
            } 
        }
    }

    // Heal Stuff
    private void UpdateHealthUI()
    {
        if (HealthText != null)
            HealthText.text = "HP: " + Mathf.CeilToInt(playerStats.health);
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

    // Stim Stuff
    private void StimShot()
    {
        // Stim Cooldown Stuff
        isStimReady = false;
        currentStimCooldown = stimCooldownTime;
        stimCooldownIcon.fillAmount = 0f;

        // Stim Animation & Healing Stuff
        shieldObject.SetActive(false);
        stimObject.SetActive(true);
        stimAnim.SetTrigger("stimShot");
        StartCoroutine(PlayerHeal());
    }

    private IEnumerator PlayerHeal()
    {
        yield return new WaitForSeconds(stimShotAnimTime);
        while (playerStats.health < playerStats.maxHealth)
        {
            playerStats.health++;  
            UpdateHealthUI();
            yield return new WaitForSeconds(0.05f); // Rapidly Increases Health, not all at Once
        }
        shieldObject.SetActive(true);
        stimObject.SetActive(false);
    }

    // Shield Stuff
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

    private IEnumerator DelayedUnblock()
    {
        yield return null;
        fpsShooting.Unblock();
    }
}
