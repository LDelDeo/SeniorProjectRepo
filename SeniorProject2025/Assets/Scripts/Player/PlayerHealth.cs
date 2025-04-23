using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Script Grabs")]
    public PlayerStats playerStats;
    public FPShooting fpsShooting;

    [Header("Health Effects")]
    public TMP_Text HealthText;
    public Image bloodSplatter;
    private float previousHealth;
    private float flashAlpha = 1f;
    private float flashDuration = 0.2f;
    private float flashTimer = 0f;

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
    private float currentHealthNow;

    [Header("Fade & Respawn")]
    public Image fadeImage;
    public float fadeDuration = 1.5f;
    public GameObject car;
    private GameObject crime;

    private bool wasBlocking = false;

    void Start()
    {
        //Health
        playerStats.health = playerStats.maxHealth;
        HealthText.text = "HP: " + Mathf.CeilToInt(playerStats.health);
        previousHealth = playerStats.health;
        flashAlpha = 0f;

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
            if (isStimReady && playerStats.health < playerStats.maxHealth && playerStats.health > 0)
            {
                StimShot();
            } 
        }

        // Blood Splatter Stuff
        bloodSplatterBorder();
    }

    // Blood Splatter
    private void bloodSplatterBorder()
    {
        float currentHealth = playerStats.health;
        float maxHealth = playerStats.maxHealth;
        float healthPercent = currentHealth / maxHealth;

        if (currentHealth < previousHealth)
        {
            flashTimer = flashDuration;
            flashAlpha = 1f; 
        }
      

        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
        }
        else
        {
            float targetAlpha = Mathf.Clamp01(1f - healthPercent);
            flashAlpha = Mathf.Lerp(flashAlpha, targetAlpha, Time.deltaTime * 3f);
        }

        Color color = bloodSplatter.color;
        color.a = flashAlpha;
        bloodSplatter.color = color;

        previousHealth = currentHealth;
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
            playerDied();
        }
    }

    // Player Death
    public void playerDied()
     {
        StartCoroutine(FadeOutAndRespawn());
        playerStats.isRespawning = true;
    }

    // Stim Stuff
    private void StimShot()
    {
        // Stim Cooldown Stuff
        isStimReady = false;
        currentStimCooldown = stimCooldownTime;
        stimCooldownIcon.fillAmount = 0f;
        currentHealthNow = playerStats.health;

        // Stim Animation & Healing Stuff
        shieldObject.SetActive(false);
        stimObject.SetActive(true);
        stimAnim.SetTrigger("stimShot");
        StartCoroutine(PlayerHeal());
    }

    private IEnumerator PlayerHeal()
    {
        yield return new WaitForSeconds(stimShotAnimTime);
        shieldObject.SetActive(true);
        
        var healthToHeal = playerStats.maxHealth - currentHealthNow;
        var healingHealth = 0;
        while (healingHealth < healthToHeal)
        {
            healingHealth++;
            playerStats.health++;  
            UpdateHealthUI();
            yield return new WaitForSeconds(0.05f); // Rapidly Increases Health, not all at Once
        }
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

    private IEnumerator FadeOutAndRespawn()
    {
        float t = 0f;
        Color c = fadeImage.color;

        // Fade to black
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        // Disable movement & respawn
        CharacterController controller = GetComponent<CharacterController>();
        controller.enabled = false;
        transform.position = Vector3.zero;
        controller.enabled = true;
        isStimReady = true;
        currentStimCooldown = 0;
        stimCooldownIcon.fillAmount = 1f;
        currentHealthNow = playerStats.health;

        // Reset health
        playerStats.health = playerStats.maxHealth;
        UpdateHealthUI();

        // Reset Mag
        fpsShooting.bullets = 16;

        // Reset Shield
        fpsShooting.CancelShieldCooldown();


        // Reset Car Position
        car.transform.position = new Vector3(2.5f, 1, -3);

        // Destroy Current Crimes
        DestroyCrime("crimeOne");
        DestroyCrime("crimeTwo");
        DestroyCrime("crimeThree");
        DestroyCrime("crimeFour");
        DestroyCrime("crimeFive");
        DestroyCrime("crimeSix");
        DestroyCrime("crimeSeven");
        DestroyCrime("crimeEight");
        
        // Fade back in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        playerStats.isRespawning = false;
    }

    public void DestroyCrime(string crimeTag)
    {
        crime = GameObject.FindWithTag(crimeTag);
        if (crime == null)
        {
            // Your Good
        }
        else if (crime != null)
        {
            Destroy(crime);
        }
    }
}
