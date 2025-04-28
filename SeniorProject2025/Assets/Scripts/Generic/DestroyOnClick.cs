using UnityEngine;

public class DestroyOnClick : MonoBehaviour
{
    private FPShooting fPShooting;
    private FPController fPController;
    private EnterCarScript enterCarScript;
    private GameObject playerHUD;
    void Start()
    {
        fPShooting = FindObjectOfType<FPShooting>();
        fPController = FindObjectOfType<FPController>();
        enterCarScript = FindObjectOfType<EnterCarScript>();
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
