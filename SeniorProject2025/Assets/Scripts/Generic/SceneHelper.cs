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

        // Load new scene
        //SceneManager.LoadScene(sceneName);
        LoadingScreenManager.Instance.LoadSceneWithLoadingScreen(sceneName);
    }
}


