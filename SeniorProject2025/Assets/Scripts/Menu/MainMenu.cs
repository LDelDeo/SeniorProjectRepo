using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Grabs")]
    public PlayerData playerData;
    public GameObject playScreen;
    public GameObject mainMenu;
    public GameObject options;
    public GameObject credits;
    public Toggle playTutorial;
    public GameObject newGameButton;
    public GameObject continueButton;

    private void Start()
    {
        playScreen.SetActive(false);
        options.SetActive(false);
        credits.SetActive(false);

        bool hasPlayedBefore = PlayerPrefs.HasKey("XP") && PlayerPrefs.GetInt("XP") > 0;

        if (hasPlayedBefore)
        {
            continueButton.SetActive(true);
            newGameButton.SetActive(true); 
            playTutorial.isOn = false;     
        }
        else
        {
            continueButton.SetActive(false);
            newGameButton.SetActive(true);
            playTutorial.isOn = true;      
        }
        this.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }


    public void PlayGame()
    {
        playScreen.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        playScreen.SetActive(false);
        options.SetActive(false);
        credits.SetActive(false);
    }

    public void NewGame()
    {
        if (playTutorial.isOn)
        {
            //SceneManager.LoadScene("TutorialScene");
            LoadingScreenManager.Instance.LoadSceneWithLoadingScreen("TutorialScene");
            PlayerPrefs.DeleteAll();
        }
        else
        {
            //SceneManager.LoadScene("MainScene");
            LoadingScreenManager.Instance.LoadSceneWithLoadingScreen("MainScene");
            PlayerPrefs.DeleteAll();
        }
    }

    public void ContinueGame()
    {
        //SceneManager.LoadScene("MainScene");
        LoadingScreenManager.Instance.LoadSceneWithLoadingScreen("MainScene");
    }

    public void Options()
    {
        options.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void Credits()
    {
        credits.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
