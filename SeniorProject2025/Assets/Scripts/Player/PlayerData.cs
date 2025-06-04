using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    [Header("Player Stats")]
    public PlayerStats playerStats;
    public int credits;
    public int xp;
    public int level = 1;
    public int xpToNextLevel = 100;

    [Header("Player Position")]
    public Transform playerTransform;

    [Header("Player's Car Position")]
    public Transform carTransform;

    [Header("UI References")]
    public TMP_Text creditsText;
    public Slider xpBar;
    public TMP_Text levelText;


    IEnumerator Start()
    {
        credits = PlayerPrefs.GetInt("Credits", 0);
        xp = PlayerPrefs.GetInt("XP", 0);
        level = PlayerPrefs.GetInt("Level", 1);
        xpToNextLevel = PlayerPrefs.GetInt("XPToNextLevel", 100);

        yield return null;

        LoadPlayerPosition();
        LoadCarPosition();
        UpdateUI();
    }

    private void OnApplicationQuit()
    {
        SavePlayerPosition();
        SaveCarPosition();
        PlayerPrefs.Save();
    }


    void Update()
    {
       
        //SaveCarPosition();
        CheckCurrentLvl();
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

        UpdateUI();
    }

    public void CheckLevelUp()
    {
        while (xp >= xpToNextLevel)
        {
   
            xp -= xpToNextLevel;
            level++;
            

            PlayerPrefs.SetInt("Level", level);
            PlayerPrefs.SetInt("XP", xp);
            PlayerPrefs.SetInt("XPToNextLevel", xpToNextLevel);
            PlayerPrefs.Save();

        }
    }
    public void CheckCurrentLvl()
    {
        switch (level)
        {
            case 1:
                xpToNextLevel = 95;
                break;
            case 2:
                xpToNextLevel = 195;
                break;
            case 3:
                xpToNextLevel = 500;
                break;
            case 4:
                xpToNextLevel = 600;
                break;
            case 5:
                xpToNextLevel = 700;
                break;
            case 6:
                xpToNextLevel = 1400;
                break;
            default:
                int linearXP = 50;
                xpToNextLevel = 1400 + (level - 6) * linearXP;
               
                break;
        }
    }
    public void UpdateUI()
    {
        if (creditsText != null)
            creditsText.text = "" + credits;

        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = xp;
        }

        if (levelText != null)
            levelText.text = "Level" + level;
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
        PlayerPrefs.DeleteKey("AmmoLevel");
        PlayerPrefs.DeleteKey("GunLevel");
        PlayerPrefs.DeleteKey("BatonLevel");
        PlayerPrefs.DeleteKey("Bullets");
        PlayerPrefs.DeleteKey("HasStarted");
        PlayerPrefs.DeleteKey("TimeOfDay");
        PlayerPrefs.DeleteKey("SelectedCar");
        PlayerPrefs.DeleteKey("SelectedCamo");
        PlayerPrefs.DeleteKey("HandsPlayedBJ");
        PlayerPrefs.DeleteKey("Tier1CrimesCompleted");

        PlayerPrefs.Save();

        credits = 0;
        xp = 0;
        level = 1;
        xpToNextLevel = 100;

        playerStats.bullets = 16;
        playerStats.playerRangedDamage = 1f;
        playerStats.playerMeleeDamage = 1f;

        PlayerPrefs.SetInt("ResetUpgrades", 1);
        PlayerPrefs.Save();
        UpdateUI();
        
        SceneManager.LoadScene("MainScene");
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
            float posX = PlayerPrefs.GetFloat("PlayerPosX", -0.53f);
            float posY = PlayerPrefs.GetFloat("PlayerPosY", 0.13f);
            float posZ = PlayerPrefs.GetFloat("PlayerPosZ", -11.45f);
            Vector3 loadedPos = new Vector3(posX, posY, posZ);
            playerTransform.position = loadedPos;
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
            Vector3 loadedPosCar = new Vector3(posX1, posY1, posZ1);
            carTransform.position = loadedPosCar;
        }
    }
   

}

