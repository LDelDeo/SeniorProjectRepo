using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenu;
    public Button resumeButton;
    public Button quitButton;
    public FPShooting fPShooting;
    public FPController fPMovement;

    private bool isPaused = false;
    private InputSystemUIInputModule inputModule;

    void Start()
    {
        pauseMenu.SetActive(false);

        resumeButton.onClick.AddListener(Resume);
        quitButton.onClick.AddListener(Quit);

        inputModule = FindFirstObjectByType<InputSystemUIInputModule>();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) Resume();
            else Pause();
        }

        if (isPaused && inputModule != null)
        {
            inputModule.Process();
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        fPShooting.enabled = true;
        fPMovement.enabled = true;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        fPShooting.enabled = false;
        fPMovement.enabled = false;
    }

    public void Quit()
    {
        Time.timeScale = 1f;
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}


