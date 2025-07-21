using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneHelper
{
    public static void SaveAndLoadScene(string sceneName)
    {
        // Save player and car position
        PlayerData data = GameObject.FindFirstObjectByType<PlayerData>();
        if (data != null)
        {
            data.SavePlayerPosition();
            data.SaveCarPosition();
        }

        // Save time of day
        LightingManager lighting = GameObject.FindFirstObjectByType<LightingManager>();
        if (lighting != null)
        {
            PlayerPrefs.SetFloat("TimeOfDay", lighting.GetCurrentTimeOfDay());
        }

        PlayerPrefs.Save();

        // Load new scene with fallback
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.LoadSceneWithLoadingScreen(sceneName);
        }
        else
        {
            Debug.LogWarning("LoadingScreenManager.Instance is null. Falling back to SceneManager.LoadScene.");
            SceneManager.LoadScene(sceneName);
        }
    }
}


