using UnityEngine;



public class SteamAchievementsManager : MonoBehaviour
{
    public void IsThisAchievementUnlocked(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);

        Debug.Log($"Achievement {id} status: " + ach.State);
    }
    public void UnlockAchievements(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);
        ach.Trigger();

        Debug.Log($"Achievement {id} unlocked");
    }
}
