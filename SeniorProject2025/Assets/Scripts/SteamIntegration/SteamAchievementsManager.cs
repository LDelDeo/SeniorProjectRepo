using UnityEngine;
using Steamworks;



public class SteamAchievementsManager : MonoBehaviour
{
    public void UnlockAchievement(string achievementID)
    {
        if (SteamManager.Initialized) // from your SteamManager script
        {
            bool success = SteamUserStats.SetAchievement(achievementID);
            if (success)
            {
                SteamUserStats.StoreStats(); // Saves it to Steam
                Debug.Log("Achievement unlocked: " + achievementID);
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
}
