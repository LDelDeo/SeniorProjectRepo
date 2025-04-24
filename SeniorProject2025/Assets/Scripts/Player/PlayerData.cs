using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PlayerData : MonoBehaviour
{
    [Header("Player Stats")]
    public int credits;
    public int xp;
    public int level = 1;
    public int xpToNextLevel = 100;
    public bool completedTutorial = true;

    [Header("Player Position")]
    public Transform playerTransform;

    [Header("Player's Car Position")]
    public Transform carTransform;

    [Header("UI References")]
    public TMP_Text creditsText;
    public Slider xpBar;
    public TMP_Text levelText;


    void Start()
    {
        credits = PlayerPrefs.GetInt("Credits", 0);
        xp = PlayerPrefs.GetInt("XP", 0);
        level = PlayerPrefs.GetInt("Level", 1);
        xpToNextLevel = PlayerPrefs.GetInt("XPToNextLevel", 100);

        float posX = PlayerPrefs.GetFloat("PlayerPosX", 0f);
        float posY = PlayerPrefs.GetFloat("PlayerPosY", 0f); 
        float posZ = PlayerPrefs.GetFloat("PlayerPosZ", 0f);

        float posX1 = PlayerPrefs.GetFloat("CarPosX", 0f);
        float posY1 = PlayerPrefs.GetFloat("CarPosY", 0f);
        float posZ1 = PlayerPrefs.GetFloat("CarPosZ", 0f);

        if (playerTransform != null)
            playerTransform.position = new Vector3(posX, posY, posZ);

        if (carTransform != null)
        carTransform.position = new Vector3(posX1, posY1, posZ1); 

        LoadPlayerPosition();
        LoadCarPosition();

        UpdateUI();

      
        
    }

    void Update()
    {
        SavePlayerPosition();
        SaveCarPosition();
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
        PlayerPrefs.SetInt("XPToNextLevel", xpToNextLevel);
        PlayerPrefs.Save();

        Debug.Log("Added XP: " + amount);
        Debug.Log("Current XP: " + xp + " XP to Next Level: " + xpToNextLevel);
        UpdateUI();
    }

    private void CheckLevelUp()
    {
        while (xp >= xpToNextLevel)
        {
            xp -= xpToNextLevel;
            level++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.75f);

            PlayerPrefs.SetInt("Level", level);
            PlayerPrefs.SetInt("XP", xp);
            PlayerPrefs.SetInt("XPToNextLevel", xpToNextLevel);
            PlayerPrefs.Save();

            Debug.Log("Level Up! New Level: " + level + " XP to Next Level: " + xpToNextLevel);


        }
    }

    private void UpdateUI()
    {
        if (creditsText != null)
            creditsText.text = "Credits: " + credits;

        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = xp;
        }

        if (levelText != null)
            levelText.text = "" + level;
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey("Credits");
        PlayerPrefs.DeleteKey("XP");
        PlayerPrefs.DeleteKey("Level");
        PlayerPrefs.DeleteKey("XPToNextLevel");
        PlayerPrefs.DeleteKey("PlayerPosX");
        PlayerPrefs.DeleteKey("PlayerPosY");
        PlayerPrefs.DeleteKey("PlayerPosZ");
        PlayerPrefs.DeleteKey("CarPosX");
        PlayerPrefs.DeleteKey("CarPosY");
        PlayerPrefs.DeleteKey("CarPosZ");

        credits = 0;
        xp = 0;
        level = 1;
        xpToNextLevel = 100;
        completedTutorial = true;

        UpdateUI();
    }

    public void SavePlayerPosition()
    {
        if (playerTransform != null)
        {
            PlayerPrefs.SetFloat("PlayerPosX", playerTransform.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", playerTransform.position.y);
            PlayerPrefs.SetFloat("PlayerPosZ", playerTransform.position.z);

            PlayerPrefs.Save();
        }
    }

    public void LoadPlayerPosition()
    {
        if (playerTransform != null)
        {
            float posX = PlayerPrefs.GetFloat("PlayerPosX", -.53f);
            float posY = PlayerPrefs.GetFloat("PlayerPosY", .13f);
            float posZ = PlayerPrefs.GetFloat("PlayerPosZ", -11.45f);
            playerTransform.position = new Vector3(posX, posY, posZ);
        }
    }

    public void SaveCarPosition()
    {
        if (carTransform != null)
        {
            PlayerPrefs.SetFloat("CarPosX", carTransform.position.x);
            PlayerPrefs.SetFloat("CarPosY", carTransform.position.y);
            PlayerPrefs.SetFloat("CarPosZ", carTransform.position.z);

            PlayerPrefs.Save();
        }
    }

    public void LoadCarPosition()
    {
        if (carTransform != null)
        {
            float posX1 = PlayerPrefs.GetFloat("CarPosX", 2.5f);
            float posY1 = PlayerPrefs.GetFloat("CarPosY", -1f);
            float posZ1 = PlayerPrefs.GetFloat("CarPosZ", -3f);
            carTransform.position = new Vector3(posX1, posY1, posZ1);
        }
    }

    
}

