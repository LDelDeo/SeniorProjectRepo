using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    [Header("Loading Screen Setup")]
    public GameObject loadingScreenPrefab;
    private GameObject loadingScreenInstance;
    private CanvasGroup canvasGroup;

    [Header("Random Image")]
    public Image splashImage; // Assign via Inspector or find by name
    public List<Sprite> loadingImages; // Assign in Inspector

    public float fadeDuration = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            loadingScreenInstance = Instantiate(loadingScreenPrefab);
            DontDestroyOnLoad(loadingScreenInstance);

            canvasGroup = loadingScreenInstance.GetComponent<CanvasGroup>();
            splashImage = loadingScreenInstance.transform.Find("RandomSplashImage").GetComponent<Image>();

            canvasGroup.alpha = 0f;
            loadingScreenInstance.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSceneWithLoadingScreen(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // Choose a random image
        if (loadingImages != null && loadingImages.Count > 0)
        {
            splashImage.sprite = loadingImages[Random.Range(0, loadingImages.Count)];
        }

        loadingScreenInstance.SetActive(true);

        // Fade in
        yield return StartCoroutine(FadeCanvas(0f, 1f));

        // Load scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade out
        yield return StartCoroutine(FadeCanvas(1f, 0f));

        loadingScreenInstance.SetActive(false);
    }

    private IEnumerator FadeCanvas(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
