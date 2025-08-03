using UnityEngine;

public class DestroyOnClick : MonoBehaviour
{
    private FPShooting fPShooting;
    private FPController fPController;
    private EnterCarScript enterCarScript;
    private GameObject playerHUD;

    void Start()
    {
        fPShooting = FindFirstObjectByType<FPShooting>();
        fPController = FindFirstObjectByType<FPController>();
        enterCarScript = FindFirstObjectByType<EnterCarScript>();
    }

    void Update()
    {
        if (!enterCarScript.isInCar)
        {
            playerHUD = GameObject.FindWithTag("PlayerHUD");
        }
    }

    public void DestroyOnButtonClick()
    {
        GameObject[] crimeCompleteScreens = GameObject.FindGameObjectsWithTag("CrimeCompletionScreen");
        int activeScreenCount = 0;

        foreach (GameObject screen in crimeCompleteScreens)
        {
            if (screen.activeInHierarchy)
            {
                activeScreenCount++;
                if (activeScreenCount > 1)
                    break;
            }
        }

        if (activeScreenCount <= 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            fPShooting.enabled = true;
            fPController.enabled = true;

            playerHUD?.SetActive(true); 
        }

        Destroy(gameObject);
    }
}
