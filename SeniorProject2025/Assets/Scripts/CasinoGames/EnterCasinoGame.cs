using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EnterCasinoGame : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text gameName;
    public GameObject pressEToPlay;

    [Header("Values")]
    public string nameOfGame;
    public string gameScene;

    [Header("Booleans")]
    public bool isInGameTrigger = false;

    void Start()
    {
        gameName.text = nameOfGame;
        pressEToPlay.SetActive(false);
    }

    void Update()
    {
        if (isInGameTrigger && Input.GetKeyDown(KeyCode.E))
        {
            SceneHelper.SaveAndLoadScene(gameScene);
            //LoadingScreenManager.Instance.LoadSceneWithLoadingScreen(gameScene);
        }
    }

  

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pressEToPlay.SetActive(true);
            isInGameTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pressEToPlay.SetActive(false);
            isInGameTrigger = false;
        }
    }
}

