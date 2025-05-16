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
        if (enterCarScript.isInCar == false)
        playerHUD = GameObject.FindWithTag("PlayerHUD");
    }
    public void DestroyOnButtonClick()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        fPShooting.enabled = true;
        fPController.enabled = true;

        playerHUD.SetActive(true);

        Destroy(gameObject);
    }
}
