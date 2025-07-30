using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [Header("Fields")]
    public string key = "space";
    public string sceneName;

    private KeyCode parsedKey;

    void Start()
    {
        if (!System.Enum.TryParse(key, true, out parsedKey))
        {
            Debug.LogError($"Invalid key name: '{key}'. Please use a valid KeyCode name.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(parsedKey))
        {
            //SceneManager.LoadScene(sceneName);
            LoadingScreenManager.Instance.LoadSceneWithLoadingScreen("MainScene");
        }
    }
}
