using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    public PlayerData playerData;
    public PlayerStats playerStats;

    [Header("Progress Bars")]
    public Slider gunDamageSlider;
    public Slider batonDamageSlider;
    public Slider ammoSlider;

    [Header("UI - Credits")]
    public TMP_Text creditsText;

    [Header("UI - Gun Damage")]
    public TMP_Text gunCostText;
    public TMP_Text gunLevelText;
    public Button gunUpgradeButton;

    [Header("UI - Baton Damage")]
    public TMP_Text batonCostText;
    public TMP_Text batonLevelText;
    public Button batonUpgradeButton;

    [Header("UI - Gun Ammo")]
    public TMP_Text ammoCostText;
    public TMP_Text ammoLevelText;
    public Button ammoUpgradeButton;

    [Header("Upgrade Effects")]
    public AudioSource upgradeSoundEffects;
    public AudioClip upgradeSound;
    public Animator animGun;
    public Animator animBaton;
    public Animator animAmmo;

    private readonly float[] gunDamageLevels = { 1f, 1.5f, 2f, 2.5f };
    private readonly float[] batonDamageLevels = { 1f, 2f, 3f, 4f };
    private int maxUpgradeLevel = 3;

    private int gunLevel;
    private int batonLevel;
    private int ammoLevel;

    private int baseCost = 300;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (PlayerPrefs.GetInt("ResetUpgrades", 0) == 1)
        {
            ResetUpgrades();
            PlayerPrefs.SetInt("ResetUpgrades", 0);
            PlayerPrefs.Save();
        }

        LoadUpgrades();
        UpdateUI();
    }

    public void UpgradeGunDamage()
    {
        if (gunLevel >= maxUpgradeLevel)
            return;

        int cost = GetUpgradeCost(gunLevel);
        if (playerData.credits >= cost)
        {
            playerData.credits -= cost;
            gunLevel++;
            playerStats.playerRangedDamage = gunDamageLevels[gunLevel];
            upgradeSoundEffects.PlayOneShot(upgradeSound);
            animGun.SetTrigger("upgrade");
            SaveUpgrades();
            UpdateUI();
        }
    }

    public void UpgradeBatonDamage()
    {
        if (batonLevel >= maxUpgradeLevel)
            return;

        int cost = GetUpgradeCost(batonLevel);
        if (playerData.credits >= cost)
        {
            playerData.credits -= cost;
            batonLevel++;
            playerStats.playerMeleeDamage = batonDamageLevels[batonLevel];
            upgradeSoundEffects.PlayOneShot(upgradeSound);
            animBaton.SetTrigger("upgrade");
            SaveUpgrades();
            UpdateUI();
        }
    }

    public void UpgradeGunAmmo()
    {
        if (ammoLevel >= 99)
            return;

        int cost = GetUpgradeCost(ammoLevel);
        if (playerData.credits >= cost)
        {
            playerData.credits -= cost;
            ammoLevel++;
            upgradeSoundEffects.PlayOneShot(upgradeSound);
            animAmmo.SetTrigger("upgrade");
            SaveUpgrades();
            UpdateUI();
        }
    }

    public void OnExit()
    {
        SceneHelper.SaveAndLoadScene("MainScene");
    }

    private int GetUpgradeCost(int level)
    {
        return baseCost + (level * 100);
    }

    private void UpdateUI()
    {
        if (creditsText != null)
            creditsText.text = $"Credits: {playerData.credits}";

        // Gun
        if (gunLevelText != null)
        {
            if (gunLevel >= maxUpgradeLevel)
            {
                gunLevelText.text = "MAX";
                gunLevelText.color = Color.green; //This color wont change because of the animator forcing it white
            }
            else
            {
                gunLevelText.text = $"Level: {gunLevel}";
                gunLevelText.color = Color.white;
            }
        }


        if (gunLevel >= maxUpgradeLevel)
        {
            if (gunUpgradeButton != null)
            {
                Destroy(gunUpgradeButton.gameObject);
                gunUpgradeButton = null;
            }
            if (gunCostText != null)
            {
                Destroy(gunCostText.gameObject);
                gunCostText = null;
            }
        }
        else
        {
            if (gunCostText != null)
                gunCostText.text = $"Cost: {GetUpgradeCost(gunLevel)}";
            if (gunUpgradeButton != null)
                gunUpgradeButton.interactable = playerData.credits >= GetUpgradeCost(gunLevel);
        }

        // Baton
        if (batonLevelText != null)
        {
            if (batonLevel >= maxUpgradeLevel)
            {
                batonLevelText.text = "MAX";
                batonLevelText.color = Color.green;
            }
            else
            {
                batonLevelText.text = $"Level: {batonLevel}";
                batonLevelText.color = Color.white;
            }
        }


        if (batonLevel >= maxUpgradeLevel)
        {
            if (batonUpgradeButton != null)
            {
                Destroy(batonUpgradeButton.gameObject);
                batonUpgradeButton = null;
            }
            if (batonCostText != null)
            {
                Destroy(batonCostText.gameObject);
                batonCostText = null;
            }
        }
        else
        {
            if (batonCostText != null)
                batonCostText.text = $"Cost: {GetUpgradeCost(batonLevel)}";
            if (batonUpgradeButton != null)
                batonUpgradeButton.interactable = playerData.credits >= GetUpgradeCost(batonLevel);
        }

        // Ammo
        if (ammoLevelText != null)
        {
            if (ammoLevel >= 99)
            {
                ammoLevelText.text = "MAX";
                ammoLevelText.color = Color.green;
            }
            else
            {
                ammoLevelText.text = $"Level: {ammoLevel}";
                ammoLevelText.color = Color.white;
            }
        }


        if (ammoLevel >= 99)
        {
            if (ammoUpgradeButton != null)
            {
                Destroy(ammoUpgradeButton.gameObject);
                ammoUpgradeButton = null;
            }
            if (ammoCostText != null)
            {
                Destroy(ammoCostText.gameObject);
                ammoCostText = null;
            }
        }
        else
        {
            if (ammoCostText != null)
                ammoCostText.text = $"Cost: {GetUpgradeCost(ammoLevel)}";
            if (ammoUpgradeButton != null)
                ammoUpgradeButton.interactable = playerData.credits >= GetUpgradeCost(ammoLevel);
        }

        if (gunDamageSlider != null)
        {
            gunDamageSlider.maxValue = maxUpgradeLevel;
            gunDamageSlider.value = gunLevel;
        }

        if (batonDamageSlider != null)
        {
            batonDamageSlider.maxValue = maxUpgradeLevel;
            batonDamageSlider.value = batonLevel;
        }

        if (ammoSlider != null)
        {
            ammoSlider.maxValue = 99;
            ammoSlider.value = ammoLevel;
        }
    }

    private void SaveUpgrades()
    {
        PlayerPrefs.SetInt("GunLevel", gunLevel);
        PlayerPrefs.SetInt("BatonLevel", batonLevel);
        PlayerPrefs.SetInt("AmmoLevel", ammoLevel);
        PlayerPrefs.SetInt("Credits", playerData.credits);
        PlayerPrefs.Save();
    }

    private void LoadUpgrades()
    {
        gunLevel = PlayerPrefs.GetInt("GunLevel", 0);
        batonLevel = PlayerPrefs.GetInt("BatonLevel", 0);
        ammoLevel = PlayerPrefs.GetInt("AmmoLevel", 0);
        playerData.credits = PlayerPrefs.GetInt("Credits", 0);

        playerStats.playerRangedDamage = gunDamageLevels[Mathf.Clamp(gunLevel, 0, maxUpgradeLevel)];
        playerStats.playerMeleeDamage = batonDamageLevels[Mathf.Clamp(batonLevel, 0, maxUpgradeLevel)];
    }

    public void ResetUpgrades()
    {
        gunLevel = 0;
        batonLevel = 0;
        ammoLevel = 0;

        PlayerPrefs.DeleteKey("GunLevel");
        PlayerPrefs.DeleteKey("BatonLevel");
        PlayerPrefs.DeleteKey("AmmoLevel");
        PlayerPrefs.DeleteKey("Bullets");

        playerStats.playerRangedDamage = 1f;
        playerStats.playerMeleeDamage = 1f;
        playerStats.bullets = 16;

        PlayerPrefs.Save();
        UpdateUI();
    }
}
