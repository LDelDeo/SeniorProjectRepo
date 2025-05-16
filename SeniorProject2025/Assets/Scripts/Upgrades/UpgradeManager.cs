using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    public PlayerData playerData;
    public PlayerStats playerStats;

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

    private readonly float[] gunDamageLevels = { 1f, 1.5f, 2f, 2.5f };
    private readonly float[] batonDamageLevels = { 1f, 2f, 3f, 4f };
    private int maxUpgradeLevel = 3;

    private int gunLevel;
    private int batonLevel;
    private int ammoLevel;

    private int baseCost = 3000;

    void Start()
    {
        LoadUpgrades();
        UpdateUI();
    }

    public void UpgradeGunDamage()
    {
        if (gunLevel >= maxUpgradeLevel) return;

        int cost = GetUpgradeCost(gunLevel);
        if (playerData.credits >= cost)
        {
            playerData.credits -= cost;
            gunLevel++;
            playerStats.playerRangedDamage = gunDamageLevels[gunLevel];
            SaveUpgrades();
            UpdateUI();
        }
    }

    public void UpgradeBatonDamage()
    {
        if (batonLevel >= maxUpgradeLevel) return;

        int cost = GetUpgradeCost(batonLevel);
        if (playerData.credits >= cost)
        {
            playerData.credits -= cost;
            batonLevel++;
            playerStats.playerMeleeDamage = batonDamageLevels[batonLevel];
            SaveUpgrades();
            UpdateUI();
        }
    }

    public void UpgradeGunAmmo()
    {
        int cost = GetUpgradeCost(ammoLevel);
        if (playerData.credits >= cost)
        {
            playerData.credits -= cost;
            ammoLevel++;
            SaveUpgrades();
            UpdateUI();
        }
    }
    public void Exit()
    {
        SceneManager.LoadScene("MainScene");
    }

    private int GetUpgradeCost(int level)
    {
        return baseCost + (level * 1000);
    }

    private void UpdateUI()
    {
        creditsText.text = $"Credits: {playerData.credits}";

        gunLevelText.text = $"Level: {gunLevel}";
        if (gunLevel >= maxUpgradeLevel)
        {
            gunCostText.text = "MAX";
            gunUpgradeButton.interactable = false;
        }
        else
        {
            gunCostText.text = $"Cost: {GetUpgradeCost(gunLevel)}";
            gunUpgradeButton.interactable = playerData.credits >= GetUpgradeCost(gunLevel);
        }

        batonLevelText.text = $"Level: {batonLevel}";
        if (batonLevel >= maxUpgradeLevel)
        {
            batonCostText.text = "MAX";
            batonUpgradeButton.interactable = false;
        }
        else
        {
            batonCostText.text = $"Cost: {GetUpgradeCost(batonLevel)}";
            batonUpgradeButton.interactable = playerData.credits >= GetUpgradeCost(batonLevel);
        }

        ammoLevelText.text = $"Level: {ammoLevel}";
        ammoCostText.text = ammoLevel >= 99 ? "MAX" : $"Cost: {GetUpgradeCost(ammoLevel)}";
        ammoUpgradeButton.interactable = playerData.credits >= GetUpgradeCost(ammoLevel);
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

    // Call this to reset everything (e.g., button on main menu)
    public void ResetUpgrades()
    {
        gunLevel = 0;
        batonLevel = 0;
        ammoLevel = 0;

        playerStats.playerRangedDamage = gunDamageLevels[0];
        playerStats.playerMeleeDamage = batonDamageLevels[0];

        UpdateUI();
    }
}
