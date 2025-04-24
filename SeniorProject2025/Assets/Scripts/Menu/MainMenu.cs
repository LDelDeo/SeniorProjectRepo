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

    private void Start()
    {
        playScreen.SetActive(false);
        options.SetActive(false);
        credits.SetActive(false);
        if (playerData.completedTutorial)
        {
            playTutorial.isOn = false;
        }
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

    public void loadGame()
    {
        if (playTutorial.isOn && !playerData.completedTutorial)
        {
            SceneManager.LoadScene("TutorialScene");
        }
        else
        {
            SceneManager.LoadScene("MainScene");
        }
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
