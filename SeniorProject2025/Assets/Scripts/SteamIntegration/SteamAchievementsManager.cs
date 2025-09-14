using UnityEngine;
using Steamworks;

public class SteamAchievementsManager : MonoBehaviour
{
    private static SteamAchievementsManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void UnlockAchievement(string achievementID)
    {
        if (SteamManager.Initialized)
        {
            bool success = SteamUserStats.SetAchievement(achievementID);
            if (success)
            {
                SteamUserStats.StoreStats();
                Debug.Log("Achievement unlocked: " + achievementID);

                CheckForMetaAchievement();
            }
            else
            {
                Debug.LogWarning("Failed to unlock achievement: " + achievementID);
            }
        }
        else
        {
            Debug.LogWarning("Steam not initialized, cannot unlock achievement.");
        }
    }

    private void CheckForMetaAchievement()
    {
        // List all your achievement IDs except the meta one
        string[] allAchievements = new string[]
        {
            "tierOneCrime",
            "hitPerson",
            "level15",
            "roulette",
            "oneMillion",
            "paintWeapon",
            "vick",
            "maxLethal",
            "truck",
            "jackpot",
            "tierTwoCrime",
            "tierThreeCrime"
        };

        foreach (string ach in allAchievements)
        {
            bool achieved;
            SteamUserStats.GetAchievement(ach, out achieved);
            if (!achieved)
            {
                return;
            }
        }

        UnlockAchievement("hundredPercent");
        SteamUserStats.StoreStats();
    }
}
