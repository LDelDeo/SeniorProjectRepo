using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour
{
    [Header("Player Stats")]
    public int credits;
    public int xp;
    public int level = 1;
    public int xpToNextLevel = 100;

    [Header("UI References")]
    public TMP_Text creditsText;
    public TMP_Text xpText;
    public TMP_Text levelText;

    void Start()
    {
        credits = PlayerPrefs.GetInt("Credits", 0);
        xp = PlayerPrefs.GetInt("XP", 0);
        level = PlayerPrefs.GetInt("Level", 1);

        UpdateUI();
    }

    public void AddCredits(int amount)
    {
        credits += amount;
        PlayerPrefs.SetInt("Credits", credits);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void SpendCredits(int amount)
    {
        if (credits >= amount)
        {
            credits -= amount;
            PlayerPrefs.SetInt("Credits", credits);
            PlayerPrefs.Save();
            UpdateUI();
        }
    }

    public void AddXP(int amount)
    {
        xp += amount;
        CheckLevelUp();
        PlayerPrefs.SetInt("XP", xp);
        PlayerPrefs.Save();
        UpdateUI();
    }

    private void CheckLevelUp()
    {
        while (xp >= xpToNextLevel)
        {
            xp -= xpToNextLevel;
            level++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.25f); //XP required increases per level

            PlayerPrefs.SetInt("Level", level);
            PlayerPrefs.SetInt("XP", xp);
        }
    }

    private void UpdateUI()
    {
        if (creditsText != null)
            creditsText.text = "Credits: " + credits;

        if (xpText != null)
            xpText.text = "XP: " + xp + " / " + xpToNextLevel;

        if (levelText != null)
            levelText.text = "Level: " + level;
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey("Credits");
        PlayerPrefs.DeleteKey("XP");
        PlayerPrefs.DeleteKey("Level");

        credits = 0;
        xp = 0;
        level = 1;
        xpToNextLevel = 100;

        UpdateUI();
    }
}

