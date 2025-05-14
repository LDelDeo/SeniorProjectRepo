using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [Header("Debug Console Objects")]
    public GameObject debugConsoleObject;
    private bool consoleOpen;

    [Header("Script Grabs")]
    public FPShooting fPShooting;
    public FPController fPController;
    public PlayerData playerData;
    public PlayerHealth playerHealth;

    //Start & Update
    void Start()
    {
        consoleOpen = false;
        debugConsoleObject.SetActive(false);
    }

    void Update()
    {
        //F3 to Trigger Debug Console
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (consoleOpen)
            {
                
                CloseDebugConsole();
            }
            else if (!consoleOpen)
            {
                
                OpenDebugConsole();
            }
        } 

        if (consoleOpen)
        {
            fPShooting.enabled = false;
            fPController.enabled = false;
            debugConsoleObject.SetActive(true);
        }

        if (!consoleOpen)
        {
            fPShooting.enabled = true;
            fPController.enabled = true;
            debugConsoleObject.SetActive(false);
        }
    }

    //Open and Close Console 
    public void OpenDebugConsole()
    {
        //Bool
        consoleOpen = true;

        //Cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseDebugConsole()
    {
        //Bool
        consoleOpen = false;

        //Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Button Functionality
    public void AddCredits()
    {
        playerData.AddCredits(1000);
    }

    public void AddXP()
    {
        playerData.AddXP(100);
        playerData.CheckCurrentLvl();
        playerData.CheckLevelUp();
    }

    public void AddLevel()
    {
        playerData.xp = playerData.xpToNextLevel;
        playerData.CheckCurrentLvl();
        playerData.CheckLevelUp();
        playerData.UpdateUI();
    }

    public void ResetPlayerPos()
    {
        playerData.playerTransform.position = new Vector3(0f, 0f, 0f);

        PlayerPrefs.SetFloat("PlayerPosX", 0f);
        PlayerPrefs.SetFloat("PlayerPosY", 0f); 
        PlayerPrefs.SetFloat("PlayerPosZ", 0f);

        PlayerPrefs.Save();
    }

    public void ResetCarPos()
    {
        playerData.carTransform.position = new Vector3(2.5f, -1f, -3f);

        PlayerPrefs.SetFloat("CarPosX", 2.5f);
        PlayerPrefs.SetFloat("CarPosY", -1);
        PlayerPrefs.SetFloat("CarPosZ", -3);

        PlayerPrefs.Save();
    }

    public void ResetCrimes()
    {
        playerHealth.DestroyCrime("crimeOne");
        playerHealth.DestroyCrime("crimeTwo");
        playerHealth.DestroyCrime("crimeThree");
        playerHealth.DestroyCrime("crimeFour");
        playerHealth.DestroyCrime("crimeFive");
        playerHealth.DestroyCrime("crimeSix");
        playerHealth.DestroyCrime("crimeSeven");
        playerHealth.DestroyCrime("crimeEight");
    }

    public void ResetProgress()
    {
        playerData.ResetData();
    }
}
